"""
将有联系的模块组织在一起，放到同一个文件夹下，这个文件夹就称为包
这个文件夹将会默认创建一个名为__init__.py文件
"""
'''
制作包：新建不再是python文件，而是python软件包
'''
'''
导入包方法一：
import 包名.模块名
包名.模块名.函数名
'''

import my_package.whz1

my_package.whz1.info_print1()
'''
导入包方法二：
from 包名 import *
模块名.函数名
但是必须要在__init__.py文件中添加__all__=[],控制允许导入的模块列表
'''
from my_package import *

whz2.info_print2()
