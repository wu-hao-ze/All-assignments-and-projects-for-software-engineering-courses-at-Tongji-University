"""
__xx__()的函数叫做魔法方法，是具有特殊功能的函数
注意是两个下划线
"""
'''
__init__():初始化对象
在创建一个对象时被默认调用，不需要手动调用
self参数不需要开发者传递，python解释器会自动把当前的对象引用传递过去
可以设置多个参数，默认参数等

__str__():当使用print输出对象的时候，默认打印对象的内存地址
如果定义了__str__方法，那么就会打印这个函数中return的值，一般放一些解释说明的文字

__del__():默认调用，删除对象，所以自己也不用写

__dict__:收集类或者实例对象的属性和方法以及对应的值，从而返回一个字典，是类或者实例对象都拥有的一个属性
'''


class Washer(object):
    def __init__(self, width=500):
        self.width = width
        self.height = 800

    def print_info(self):
        print(self.width)

    def __str__(self):
        return 'whz'

    def __del__(self):
        print('对象已删除')


haier = Washer(100)
haier1 = Washer()
haier.print_info()
haier1.print_info()
print(haier)


class A(object):
    # a是类属性
    a = 0

    def __init__(self):
        # b是实例属性
        self.b = 1


aa = A()
print(A.__dict__)  # 返回类内部所有属性和方法对应的字典
print(aa.__dict__)  # 返回实例属性和值组成的字典
