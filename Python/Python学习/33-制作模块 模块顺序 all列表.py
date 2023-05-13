"""
在python中每个python文件都可以作为一个模块，模块的名字就是文件的名字，也就是说自定义模块名要符合标识名命名规则
"""
import my_module1

my_module1.testA(2, 3)

'''
当导入一个模块，python解释器对模块位置的搜索位置：
1.当前目录
2.如果不在当前目录，搜索在shell变量PYTHONPATH下的每个目录
3.如果都不在，则会查看默认路径
模块搜索路径存储在system模块的sys.path变量中，变量中包含当前目录，PYTHONPATH和由安装过程决定的默认路径
'''
'''
注意：
自己的文件名不要和已有模块名重复
使用from 模块名 import 功能 或者from 模块名 import *的时候，如果功能名字重复，调用到的是最后定义或导入的功能
'''
'''
针对第一点，假如自己在当前目录下，创建一个random.py文件，那么import random的话就会导致random模块无法使用，不能同名
针对第二点，假如在当前的py文件下创建了一个sleep函数，那么调用time里的sleep函数就得看谁在前谁在后
比如下面的例子先调用time模块，而后又定义了sleep函数，那么直接用sleep的时候就默认调用下面的自定义的，所以不能添加参数
'''

from time import *


def sleep():
    print(123)


sleep()

# 一定不要重名，比如下面的例子，就很可怕
import math

print(math)
math = 1
print(math)
'''
__all__列表是一个简称，是将来模块的一个变量名，只不过这个变量取值是一个列表数据
__all__列表的作用是控制模块导入功能的导入行为
如果一个模块文件中有__all__变量，当使用from 模块名 import *导入时，只能导入这个列表中的元素
'''
from my_module1 import *

testA(3, 3)
testB()
