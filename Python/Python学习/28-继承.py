"""
继承是指多个类之间的所属关系，子类默认继承父类所有属性和方法
所有类默认继承object类，object是基类，其他子类是派生类
单继承：一个父类对一个子类，单一的继承关系
多继承：一个子类继承了多个父类，如果父类的属性和方法同名，则默认使用第一个父类的
如果子类和父类拥有同名属性和方法，默认先调用子类里面的属性和方法
如果想调用父类的同名属性和方法，则在子类中另写函数，在该函数中调用
多层继承：比如父子孙这样多层继承
super是子类调用父类方法(函数)，使用super自动查找父类，顺序遵循mro，比较适合单继承使用
可以为实例属性和方法设置私有权限，设置某个实例属性或方法不继承给子类
设置私有权限的方法：在属性名和方法名前面加上两个下划线__
私有属性和私有方法只能在类内访问和修改
一般定义函数名get__xx来获取私有属性，定义set__xx来修改私有属性值
"""


class A(object):
    def __init__(self):
        self.num = 1

    def print_info(self):
        print(self.num)


class C(object):
    def __init__(self):
        self.num = 2

    def print_info(self):
        print(self.num)


class B(C, A):
    def __init__(self):
        self.num = 3
        # 钱设置为私有属性
        self.__money = 100

    def get__money(self):
        return self.__money

    def set__money(self):
        self.__money = 500

    def print_info(self):
        """
        这个self.__init__一定要写！！！！！！
        因为在调用其他函数也有初始化之后，如果不重新初始化，会保留上次的值
        """
        self.__init__()
        print(self.num)

    # 这个函数设置为私有
    def __private(self):
        self.__money += 100

    def callA(self):
        """方法一是写对应的父类名，然后调用函数"""
        A.__init__(self)
        A.print_info(self)

    def callC(self):
        """方法二是super(当前类名,self).函数()"""
        super(B, self).__init__()
        super(B, self).print_info()


class D(B):
    """方法三是super().函数()"""

    def callB(self):
        super().__init__()
        super().print_info()


result = B()
result.print_info()
result.callA()
result.callC()
result.print_info()
print('----------')
# result.__private()
print(result.get__money())
# 如果想要查看某个类的继承关系，使用mro
print(B.mro())
print(B.__mro__)
result1 = D()
result1.print_info()
result1.callA()
result1.callC()
result1.callB()
