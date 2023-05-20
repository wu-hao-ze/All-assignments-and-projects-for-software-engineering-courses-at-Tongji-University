# main.py用来实现api的接口
import json
import os
import shutil
import stat
from pathlib import Path
from tempfile import NamedTemporaryFile
from typing import List, Optional
import unittest
import allure
import pytest
from matplotlib.font_manager import json_load

import requests
import uvicorn
from fastapi import (Cookie, Depends, FastAPI, HTTPException, Response,
                     responses)
from fastapi.datastructures import UploadFile
from fastapi.params import File
from fastapi.testclient import TestClient
from sqlalchemy.orm import Session

from . import crud, models, schemas
from .data.data import imgpath, imgprefix, testopenid, wxappid, wxsecret, wxurl
from .database import SessionLocal, engine

# 在数据库中生成表结构
models.Base.metadata.create_all(bind=engine)

app = FastAPI()


def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()


@app.get(
    "/api/seproject/getopenid",
    summary="测试用，设置openid",
    description="直接为cookie设置预设好的openid",
    tags=["测试"],
)
def fun(response: Response):
    response.set_cookie(key="openid", value=testopenid)
    return


@app.get(
    "/api/seproject/getOpenid",
    response_model=schemas.User,
    summary="顾客微信登录",
    description="通过顾客传入的验证码，借助微信提供的api获取到用户id并进行登录",
    tags=["顾客"],
)
def getid(code: str, db: Session = Depends(get_db)):
    url = wxurl
    appid = wxappid
    secret = wxsecret
    params = {
        "appid": appid,
        "secret": secret,
        "js_code": code,
        "grant_type": "authorization_code",
    }

    data = requests.get(url, params=params).json()
    print(data)
    if "openid" not in data:
        resp = responses.JSONResponse(content=data)
        return resp

    openid = data["openid"]
    print(data)
    db_user = crud.get_user_by_openid(db, openid)
    if not db_user:
        db_user = crud.create_user(db=db, openid=openid)

    resp = responses.JSONResponse(content={"id": db_user.id})
    resp.set_cookie(key="openid", value=openid)
    return resp


@app.get(
    "/api/seproject/getStoreInfo",
    response_model=schemas.ShopDict,
    summary="顾客获取店铺信息",
    description="返回所有店铺信息",
    tags=["顾客"],
)
def read_shops(skip: int = 0, limit: int = 100, db: Session = Depends(get_db)):
    shops = crud.get_shops(db, skip=skip, limit=limit)
    l = []
    for i in shops:
        d = {"id": i.id,
             "shopName": i.name,
             "shopDesc": i.describe,
             "shopText": "距离{}m".format(i.distance),
             "shopImg": f"{i.img}"}
        l.append(d)
        pass
    data = schemas.ShopDict(msg="ok", shoplist=l)
    return data


@app.get(
    "/api/seproject/getDishInfo",
    response_model=schemas.DishDict,
    summary="顾客获取菜品信息",
    description="返回所选中店铺的所有菜品信息按照菜品种类分类",
    tags=["顾客"],
)
def get_dish_info(store_id: int, db: Session = Depends(get_db)):
    if not store_id:
        raise HTTPException(status_code=422, detail="Missing parameter")
    else:
        dishs = crud.get_dishs_by_store_id(db, store_id)
        data = schemas.DishDict(msg="ok", goods=[])
        h = {}
        foodlist = []
        for inx, i in enumerate(dishs):
            if not i.flavor in h:
                h[i.flavor] = [inx]
            else:
                h[i.flavor].append(inx)
            d = {"name": i.name,
                 "price": i.price,
                 "id": i.id,
                 "description": i.description,
                 "icon": f"{i.icon}",
                 "Count": 0}
            foodlist.append(d)
        temp = []
        for i in h:
            d = schemas.FlavorList(name=i, foods=[])
            for j in h[i]:
                d.foods.append(foodlist[j])
            temp.append(d)
        data.goods = temp
    return data


@app.post(
    "/api/seproject/createOrder", summary="顾客创建订单", description="生成订单", tags=["顾客"]
)
def create_order(
        order: schemas.OrderCreate,
        db: Session = Depends(get_db),
        openid: Optional[str] = Cookie(None),
):
    if not openid:
        raise HTTPException(status_code=401, detail="please login first")

    user = crud.get_user_by_openid(db, openid)

    # 错误检查
    price = 0.0
    store = crud.get_shop_by_id(db, order.store_id)
    if store is None:
        raise HTTPException(status_code=404, detail="Shop not found")
    for i in order.countArray:
        dish = crud.get_dish_by_id(db, i.id)
        if dish is None:
            raise HTTPException(status_code=404, detail="Dish not found")
        if dish.store_id != store.id:
            raise HTTPException(status_code=400, detail="Not a dish in this shop")
        price += dish.price * i.num

    order.totalPrice = round(price, 2)
    # 订单表内加订单
    order_id = crud.create_order(db, user.id, order)
    # 订单-菜品表加数据
    crud.add_order_dish(db, order_id, order.countArray)
    # 订单状态表加数据
    crud.create_order_status(db, order_id)
    data = {"msg": "succeed", "order_id": order_id}
    resp = responses.JSONResponse(content=data)
    return resp


@app.get(
    "/api/seproject/getAllOrders",
    response_model=schemas.OrderDict,
    summary="顾客获取全部订单",
    description="返回顾客创建的所有订单",
    tags=["顾客"],
)
def get_all_orders(db: Session = Depends(get_db), openid: Optional[str] = Cookie(None)):
    if not openid:
        raise HTTPException(status_code=401, detail="please login first")

    user = crud.get_user_by_openid(db, openid)
    if hasattr(user, "id"):
        return schemas.OrderDict(
            msg="succeed", data=crud.get_orders_by_user_id(db, user.id)
        )
    else:
        return schemas.OrderDict(
            msg="succeed", data=crud.get_orders_by_user_id(db, 0)
        )


@app.get(
    "/api/seproject/getComment",
    response_model=schemas.CommentDict,
    summary="获取评价",
    description="返回所选订单的评价",
    tags=["通用"],
)
def get_comment(order_id: int, db: Session = Depends(get_db)):
    if not order_id:
        raise HTTPException(status_code=422, detail="Missing parameter")

    db_comment = crud.get_comment_by_order_id(db, order_id)
    if not db_comment:
        raise HTTPException(status_code=400, detail="Comment not found")

    return schemas.CommentDict(msg="succeed", data=db_comment)


@app.post(
    "/api/seproject/addComment",
    response_model=schemas.SimpleReply,
    summary="顾客创建评论",
    description="创建评论",
    tags=["顾客"],
)
def create_comment(
        comment: schemas.CommentCreate,
        db: Session = Depends(get_db),
        openid: Optional[str] = Cookie(None),
):
    if not openid:
        raise HTTPException(status_code=401, detail="please login first")

    db_comment = crud.get_comment_by_order_id(db, comment.order_id)
    if db_comment:
        raise HTTPException(status_code=400, detail="Comment already exists")

    db_order = crud.get_order_by_id(db, comment.order_id)
    user = crud.get_user_by_openid(db, openid)
    if db_order is None:
        raise HTTPException(status_code=400, detail="Order not found")
    if db_order.user_id != user.id:
        raise HTTPException(status_code=400, detail="You have no permission to do this")

    crud.create_comment(db, comment)
    return schemas.SimpleReply(msg="succeed")


# 声明一个 TestClient，把 FastAPI() 实例对象传进去
client = TestClient(app)
with open('./seproject_app/data/testsedata.json', 'r', encoding='utf8') as fp:
    _data = json.load(fp)


class Test_1(unittest.TestCase):
    def setUp(self):
        # print("开始")
        pass

    def tearDown(self):
        # print("结束")
        pass

    @allure.epic('单元')
    @allure.feature('登录模块')
    @allure.story('顾客微信登录')
    # @allure.issue('http://127.0.0.1:80/zantaopms/')
    # @allure.testcase('http://www.baidu.com/')
    # 设置测试用例的标签, 可以设置多个
    # @allure.tag("回归测试", "重要")
    # 设置测试用例的级别  blocker > critical > normal > minor > trivial
    @allure.title('测试登录:验证码为空')
    @allure.description('不填写验证码进行登录')
    # @allure.severity("blocker")
    def test010(self):
        print()
        n = 'test1'
        # 接口地址
        # 构造数据
        t = _data[n]
        i = t[0]
        params = i['params']
        code = params['code']
        re = i['return']
        r = json.loads(getid(code).body)
        print(f"测试输入：{params}，期望输出：{re}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('登录模块')
    @allure.story('顾客微信登录')
    @allure.title('测试登录:验证码错误')
    @allure.description('测试使用错误的验证码进行登录')
    def test011(self):
        print()
        n = 'test1'
        # 接口地址
        # 构造数据
        t = _data[n]
        i = t[1]
        params = i['params']
        code = params['code']
        re = i['return']
        r = json.loads(getid(code).body)
        print(f"测试输入：{params}，期望输出：{re}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('登录模块')
    @allure.story('顾客微信登录')
    @allure.title('测试登录:验证码错误')
    @allure.description('测试使用错误的验证码进行登录')
    def test012(self):
        print()
        n = 'test1'
        # 接口地址
        # 构造数据
        t = _data[n]
        i = t[2]
        params = i['params']
        code = params['code']
        re = i['return']
        r = json.loads(getid(code).body)
        print(f"测试输入：{params}，期望输出：{re}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取商家信息')
    @allure.title('测试获取商家信息:不填写参数')
    @allure.description('测试登录:测试用例描述信息')
    def test40(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        func = 'api/seproject/getStoreInfo'
        # 构造数据
        t = _data[n]
        i = t[0]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取商家信息')
    @allure.title('测试获取商家信息:只填写skip参数')
    @allure.description('测试登录:测试用例描述信息')
    def test41(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        func = 'api/seproject/getStoreInfo'
        # 构造数据
        t = _data[n]
        i = t[1]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取商家信息')
    @allure.title('测试获取商家信息:只填写limit参数')
    @allure.description('测试登录:测试用例描述信息')
    def test42(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        func = 'api/seproject/getStoreInfo'
        # 构造数据
        t = _data[n]
        i = t[2]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取商家信息')
    @allure.title('测试获取商家信息:填写非法skip参数')
    @allure.description('测试登录:测试用例描述信息')
    def test43(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        func = 'api/seproject/getStoreInfo'
        # 构造数据
        t = _data[n]
        i = t[3]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取商家信息')
    @allure.title('测试获取商家信息:填写非法limit参数')
    @allure.description('测试登录:测试用例描述信息')
    def test44(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        func = 'api/seproject/getStoreInfo'
        # 构造数据
        t = _data[n]
        i = t[4]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取商家信息')
    @allure.title('测试获取商家信息:填写正确参数')
    @allure.description('测试登录:测试用例描述信息')
    def test45(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        func = 'api/seproject/getStoreInfo'
        # 构造数据
        t = _data[n]
        i = t[5]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取菜品信息')
    @allure.title('测试获取菜品信息:不填写参数')
    @allure.description('测试登录:测试用例描述信息')
    def test50(self):
        '''
        获取菜品信息
        '''
        print()
        n = 'test5'
        # 接口地址
        func = 'api/seproject/getDishInfo'
        # 构造数据
        t = _data[n]
        i = t[0]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('获取菜品信息')
    @allure.title('测试获取菜品信息:填写参数')
    @allure.description('测试登录:测试用例描述信息')
    def test51(self):
        '''
        获取菜品信息
        '''
        print()
        n = 'test5'
        # 接口地址
        func = 'api/seproject/getDishInfo'
        # 构造数据
        t = _data[n]
        i = t[1]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:未携带openid')
    @allure.description('测试登录:测试用例描述信息')
    def test60(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": ""}
        i = t[0]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:不填写商家id')
    @allure.description('测试登录:测试用例描述信息')
    def test61(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[1]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:不填写点餐内容')
    @allure.description('测试登录:测试用例描述信息')
    def test62(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[2]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:点餐内容填写为空')
    @allure.description('测试登录:测试用例描述信息')
    def test63(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[3]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:填写不存在的订单id')
    @allure.description('测试登录:测试用例描述信息')
    def test64(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[4]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:填写不存在的菜品id')
    @allure.description('测试登录:测试用例描述信息')
    def test65(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[5]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:填写的菜品id不是该商家')
    @allure.description('测试登录:测试用例描述信息')
    def test66(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[6]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客点餐模块')
    @allure.story('顾客创建订单')
    @allure.title('测试创建订单:填写正确')
    @allure.description('测试登录:测试用例描述信息')
    def test67(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        func = 'api/seproject/createOrder'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[7]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客获取所有订单')
    @allure.title('测试获取所有订单:未携带openid')
    @allure.description('测试登录:测试用例描述信息')
    def test70(self):
        '''
        获取所有订单
        '''
        print()
        n = 'test7'
        # 接口地址
        func = 'api/seproject/getAllOrders'
        # 构造数据
        t = _data[n]
        headers = {}
        i = t[0]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客获取所有订单')
    @allure.title('测试获取所有订单:携带openid')
    @allure.description('测试登录:测试用例描述信息')
    def test71(self):
        '''
        获取所有订单
        '''
        print()
        n = 'test7'
        # 接口地址
        func = 'api/seproject/getAllOrders'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=oxxHL5GS26iMW6iSMzfoLjXk1luw"}
        i = t[1]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客获取评论')
    @allure.title('测试获取评论:订单id为空')
    @allure.description('测试登录:测试用例描述信息')
    def test80(self):
        '''
        获取评论
        '''
        print()
        n = 'test8'
        # 接口地址
        func = 'api/seproject/getComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": ""}
        i = t[0]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客获取评论')
    @allure.title('测试获取评论:订单id不存在')
    @allure.description('测试登录:测试用例描述信息')
    def test81(self):
        '''
        获取评论
        '''
        print()
        n = 'test8'
        # 接口地址
        func = 'api/seproject/getComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[1]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客获取评论')
    @allure.title('测试获取评论:参数正确')
    @allure.description('测试登录:测试用例描述信息')
    def test82(self):
        '''
        获取评论
        '''
        print()
        n = 'test8'
        # 接口地址
        func = 'api/seproject/getComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[2]
        params = i['params']
        re = i['return']
        r = client.get(func, params=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:未携带openid')
    @allure.description('测试登录:测试用例描述信息')
    def test90(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": ""}
        i = t[0]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:订单id为空')
    @allure.description('测试登录:测试用例描述信息')
    def test91(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[1]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:评分为空')
    @allure.description('测试登录:测试用例描述信息')
    def test92(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[2]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:订单id不存在')
    @allure.description('测试登录:测试用例描述信息')
    def test93(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[3]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:评价不是自己的订单')
    @allure.description('测试登录:测试用例描述信息')
    def test94(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[4]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertIn("detail", r)

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:评分不合法')
    @allure.description('测试登录:测试用例描述信息')
    def test95(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[5]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])

    @allure.epic('单元')
    @allure.feature('顾客评价模块')
    @allure.story('顾客创建评论')
    @allure.title('测试创建评论:参数正确')
    @allure.description('测试登录:测试用例描述信息')
    def test96(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        func = 'api/seproject/addComment'
        # 构造数据
        t = _data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        i = t[6]
        params = i['params']
        re = i['return']
        r = client.post(func, json=params, headers=headers).json()
        if "detail" in r:
            t = r['detail']
            if type(t) == list:
                r = t[0]
        print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
        self.assertEqual(r[re['key']], re['value'])


if __name__ == '__main__':
    uvicorn.run(app="37_pytest:app", reload=True, host="127.0.0.1", port=8080)
# 在cmd终端中输入 pytest ./seproject_app/main.py --alluredir ./result 生成result目录
# allure serve ./result 打开测试报告
