"""
模块 module，是一个python文件，以.py结尾，包含了python对象定义的和python语句
模块能定义函数，类和变量，模块也能包含可执行的代码
"""
'''
导入模块的方式：
import 模块名
from 模块名 import 功能名
from 模块名 import *
as定义别名：
模块定义别名：import 模块名 as 别名
功能定义别名：from 模块名 import 功能名 as 别名
'''
'''
import 模块名1,模块名2,模块名3...   不推荐这种写法
模块名.功能名()
'''
import time

time.sleep(1)
'''
from 模块名 import 功能1,功能2,功能3
功能名()
'''
from math import sin

print(sin(1.57))
'''
from 模块名 import *
功能名()
'''
from math import *

print(sqrt(9))
# 定义别名
import random as tt

print(tt.random())
from random import randint as ss

print(ss(1, 3))
