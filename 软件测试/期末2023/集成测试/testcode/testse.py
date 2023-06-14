import json
import unittest

from BeautifulReport import BeautifulReport as bf
'''from BeautifulReport import BeautifulReport as bf  比较美观'''

import requests

with open('./testsedata.json', 'r', encoding='utf-8') as fp:
    data = json.loads(fp.read())
_url = "http://127.0.0.1:8000/api/seproject/"


class Test_Tq(unittest.TestCase):
    def setUp(self):
        # print("开始")
        pass

    def tearDown(self):
        # print("结束")
        pass

    def test1(self):
        '''
        顾客微信登录
        '''
        print()
        n = 'test1'
        # 接口地址
        url = _url + 'getOpenid'
        # 构造数据
        t = data[n]
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.get(url, params=params).json()
            print(f"测试输入：{params}，期望输出：{re}")
            self.assertEqual(r[re['key']], re['value'])
            # self.assertEqual(r["reason"],"failed!")

    def test4(self):
        '''
        获取商家信息
        '''
        print()
        n = 'test4'
        # 接口地址
        url = _url + 'getStoreInfo'
        # 构造数据
        t = data[n]
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.get(url, params=params).json()
            print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
            self.assertEqual(r[re['key']], re['value'])
            # self.assertEqual(r["reason"],"failed!")

    def test5(self):
        '''
        获取菜品信息
        '''
        print()
        n = 'test5'
        # 接口地址
        url = _url + 'getDishInfo'
        # 构造数据
        t = data[n]
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.get(url, params=params).json()
            if "detail" in r:
                t = r['detail']
                if type(t) == list:
                    r = t[0]
            print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
            self.assertEqual(r[re['key']], re['value'])
            # self.assertEqual(r["reason"],"failed!")

    def test6(self):
        '''
        创建订单
        '''
        print()
        n = 'test6'
        # 接口地址
        url = _url + 'createOrder'
        # 构造数据
        t = data[n]
        headers = {"cookie": ""}
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.post(url, json=params, headers=headers).json()
            if "detail" in r:
                t = r['detail']
                if type(t) == list:
                    r = t[0]
            print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
            self.assertEqual(r[re['key']], re['value'])
            headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
            # self.assertEqual(r["reason"],"failed!")

    def test7(self):
        '''
        获取所有订单
        '''
        print()
        n = 'test7'
        # 接口地址
        url = _url + 'getAllOrders'
        # 构造数据
        t = data[n]
        headers = {}
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.get(url, params=params, headers=headers).json()
            if "detail" in r:
                t = r['detail']
                if type(t) == list:
                    r = t[0]
            print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
            self.assertEqual(r[re['key']], re['value'])
            headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}

    def test8(self):
        '''
        获取评论
        '''
        print()
        n = 'test8'
        # 接口地址
        url = _url + 'getComment'
        # 构造数据
        t = data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.get(url, params=params, headers=headers).json()
            if "detail" in r:
                t = r['detail']
                if type(t) == list:
                    r = t[0]
            print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
            self.assertEqual(r[re['key']], re['value'])
            headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}

    def test9(self):
        '''
        顾客进行评论
        '''
        print()
        n = 'test9'
        # 接口地址
        url = _url + 'addComment'
        # 构造数据
        t = data[n]
        headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}
        for i in t:
            params = i['params']
            re = i['return']
            r = requests.post(url, json=params, headers=headers).json()
            if "detail" in r:
                t = r['detail']
                if type(t) == list:
                    r = t[0]
            print(f"测试输入：{params}，期望输出：{re['key']}: {re['value']}")
            self.assertEqual(r[re['key']], re['value'])
            headers = {"cookie": "openid=odYyE5JaZoSAQ7GF_vkOlQTpBupU"}


if __name__ == '__main__':
    suite = unittest.TestSuite()

    # for i in range(1, 10):
    suite.addTest(Test_Tq('test1'))  # 将所有的测试用例加载进去
    suite.addTest(Test_Tq('test4'))  # 将所有的测试用例加载进去
    suite.addTest(Test_Tq('test5'))  # 将所有的测试用例加载进去
    suite.addTest(Test_Tq('test6'))  # 将所有的测试用例加载进去
    suite.addTest(Test_Tq('test7'))  # 将所有的测试用例加载进去
    suite.addTest(Test_Tq('test8'))  # 将所有的测试用例加载进去
    suite.addTest(Test_Tq('test9'))  # 将所有的测试用例加载进去

    run = bf(suite)  # 实例化BeautifulReport模块
    run.report(filename='测试报告.html', description='集成测试', report_dir='./report',
               theme='theme_default')  # description用例名称
