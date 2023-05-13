# main.py用来实现api的接口
import os, stat
import shutil
from pathlib import Path
from tempfile import NamedTemporaryFile
from typing import List, Optional

import requests
from fastapi import Cookie, Depends, FastAPI, HTTPException, Response, responses
from fastapi.datastructures import UploadFile
from fastapi.params import File
from sqlalchemy.orm import Session

from . import crud, models, schemas
from .data.data import imgprefix, imgpath, testopenid, wxappid, wxsecret, wxurl
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
    return schemas.OrderDict(
        msg="succeed", data=crud.get_orders_by_user_id(db, user.id)
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





############################
# 以下为商家模块


# @app.post(
#     "/api/seproject/shop/signIn",
#     response_model=schemas.Shop,
#     summary="商家登录",
#     description="商家输入手机号码和密码进行登录",
#     tags=["商家"],
# )
# def signin(shop: schemas.ShopLogin, response: Response, db: Session = Depends(get_db)):
#     db_shop = crud.get_shop_by_phone(db, phone=shop.phone)
#     if db_shop is None:
#         raise HTTPException(status_code=400, detail="Shop not found")
#     if db_shop.password != shop.password:
#         raise HTTPException(status_code=400, detail="Wrong password, try again")
#     if db_shop.img:
#         db_shop.img = imgprefix + db_shop.img
#     response.set_cookie(key="shopid", value=db_shop.id)
#     return db_shop
#
#
# @app.post(
#     "/api/seproject/shop/signUp",
#     response_model=schemas.Shop,
#     summary="商家注册",
#     description="商家输入信息进行注册",
#     tags=["商家"],
# )
# def create_shop(shop: schemas.ShopCreate, db: Session = Depends(get_db)):
#     db_shop = crud.get_shop_by_phone(db, phone=shop.phone)
#     if db_shop:
#         raise HTTPException(status_code=400, detail="Phone already registered")
#     return crud.create_shop(db=db, shop=shop)
#
#
# @app.post(
#     "/api/seproject/shop/changeInfo",
#     response_model=schemas.Shop,
#     summary="商家修改信息",
#     description="商家输入信息进行修改",
#     tags=["商家"],
# )
# def change_shop(
#     shop: schemas.ShopChange,
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#
#     return crud.change_shop_by_id(db, shopid, shop)
#
#
# @app.post(
#     "/api/seproject/shop/createDish",
#     response_model=schemas.Dish,
#     summary="商家添加菜品",
#     description="商家输入菜品信息添加菜品",
#     tags=["商家"],
# )
# def create_dish(
#     dish: schemas.DishCreate,
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#     return crud.create_dish(db, dish, shopid)
#
#
# @app.get(
#     "/api/seproject/shop/getShopDishInfo",
#     response_model=List[schemas.Dish],
#     summary="商家获取所有菜品信息",
#     description="返回所有菜品信息",
#     tags=["商家"],
# )
# def get_shop_dish_info(
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#     l = crud.get_dishs_by_store_id(db, shopid)
#     for i in l:
#         if i.icon:
#             i.icon = imgprefix + i.icon
#     return l
#
#
# @app.post(
#     "/api/seproject/shop/changeShopDishInfo",
#     response_model=schemas.Dish,
#     summary="商家更改菜品信息",
#     description="更改菜品信息",
#     tags=["商家"],
# )
# def change_shop_dish_info(
#     dish: schemas.DishChange,
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     db_dish = crud.get_dish_by_id(db, dish.id)
#     if db_dish is None:
#         raise HTTPException(status_code=404, detail="Dish not found")
#     if db_dish.store_id != shopid:
#         raise HTTPException(status_code=400, detail="You have no permission to do this")
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#
#     return crud.change_dish_by_id(db, db_dish.id, dish)
#
#
# @app.get(
#     "/api/seproject/shop/changeOrderStatus",
#     response_model=schemas.SimpleReply,
#     summary="商家修改订单状态",
#     description="修改订单状态",
#     tags=["商家"],
# )
# def change_order_status(
#     order_id: int,
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#     db_order = crud.get_order_by_id(db, order_id)
#     if db_order is None:
#         raise HTTPException(status_code=404, detail="Order not found")
#     if db_order.store_id != shopid:
#         raise HTTPException(status_code=400, detail="You have no permission to do this")
#     crud.change_order_status(db, order_id)
#     return schemas.SimpleReply(msg="succeed")
#
#
# @app.get(
#     "/api/seproject/shop/getShopOrder",
#     response_model=List[schemas.OrderShop],
#     summary="商家获取所有订单",
#     description="返回该商家的所有订单",
#     tags=["商家"],
# )
# def get_shop_order(
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#     return crud.get_orders_by_store_id(db, shopid)
#
#
# @app.get(
#     "/api/seproject/shop/getShopComment",
#     response_model=List[schemas.Comment],
#     summary="商家获取所有评价",
#     description="返回商家的所有评价",
#     tags=["商家"],
# )
# def get_shop_comment(
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#     return crud.get_comments_by_store_id(db, shopid)
#
#
# @app.post(
#     "/api/seproject/shop/replyComment",
#     response_model=schemas.Comment,
#     summary="商家回复评论",
#     description="回复评论",
#     tags=["商家"],
# )
# def reply_comment(
#     comment: schemas.CommentReply,
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#
#     db_comment = crud.get_comment_by_order_id(db, comment.order_id)
#     if db_comment is None:
#         raise HTTPException(status_code=404, detail="Comment not found")
#
#     db_order = crud.get_order_by_id(db, db_comment.order_id)
#     db_shop = crud.get_shop_by_id(db, shopid)
#     if db_order is None:
#         raise HTTPException(status_code=400, detail="Order not found")
#     if db_order.store_id != db_shop.id:
#         raise HTTPException(status_code=400, detail="You have no permission to do this")
#
#     return crud.change_reply_comment(db, db_order.id, comment)
#
#
# @app.post(
#     "/api/seproject/shop/upLoadImg",
#     response_model=schemas.SimpleReply,
#     summary="商家上传图片",
#     description="上传图片，type为1时上传的是店铺图片，为2时是菜品图片，此时菜品id为必填",
#     tags=["商家"],
# )
# def upload_image(
#     type: int,
#     dish_id: Optional[int] = None,
#     file: UploadFile = File(...),
#     db: Session = Depends(get_db),
#     shopid: Optional[int] = Cookie(None),
# ):
#     if not shopid:
#         raise HTTPException(status_code=401, detail="please login first")
#     if type == 1:
#         add = "storeImg/"
#     elif type != 2:
#         raise HTTPException(status_code=422, detail="Unknown type")
#     else:
#         if dish_id is None:
#             raise HTTPException(status_code=422, detail="Missing parameter")
#         db_dish = crud.get_dish_by_id(db, dish_id)
#         if db_dish.store_id != shopid:
#             raise HTTPException(status_code=400, detail="You have no permission to do this")
#         add = "dishImg/"
#
#     save_dir = imgpath + add
#     if not os.path.exists(save_dir):
#         os.mkdir(save_dir)
#
#     try:
#         suffix = Path(file.filename).suffix
#         with NamedTemporaryFile(delete=False, suffix=suffix, dir=save_dir) as tmp:
#             shutil.copyfileobj(file.file, tmp)
#             os.chmod(tmp.name, stat.S_IRWXO + stat.S_IRWXG + stat.S_IRWXU)
#             tmp_file_name = Path(tmp.name).name
#     finally:
#         file.file.close()
#
#     if type == 1:
#         crud.change_shop_img(db, shopid, add + tmp_file_name)
#     else:
#         crud.change_dish_img(db, dish_id, add + tmp_file_name)
#
#     return schemas.SimpleReply(msg = "succeed")
