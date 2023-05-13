__all__ = ['testA']


def testA(a, b):
    print(a + b)


def testB():
    print('whz')


'''
测试信息不要删除，但是也会影响其他文件调用此模块
__name__是系统变量，是模块的标识符，也就是每个python文件的标识符
如果__name__的使用位置是在自身模块，那么它的值就是'__main__'
如果不是在自身模块使用，那么它的值就是对应的文件的名字
'''
# 只在当前文件中调用该函数，其他导入的文件内不符合该条件，则不执行testA函数调用
if __name__ == '__main__':
    testA(1, 2)
# 33-制作模块中调用my_module1模块，则执行到了print(__name__)，那么此时的__name__的值就是my_module1，而不是'__main__'
print(__name__)
