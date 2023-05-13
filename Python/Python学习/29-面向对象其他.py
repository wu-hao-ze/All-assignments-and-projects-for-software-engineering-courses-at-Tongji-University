"""
面向对象三大特性：封装，继承，多态
封装：将属性和方法书写到类里，可以为属性方法添加私有权限
继承：子类默认继承父类的所有属性方法，可以重写父类的属性方法
多态：传入不同的对象，产生不同的结果
类属性：类对象所拥有的属性，被该类的所有实例对象所共有，可以使用类对象或实例对象访问
类属性只能通过类对象修改，不能通过实例对象修改
如果通过实例对象修改类属性，则是创建了一个实例属性，与类属性无关
类方法：需要用装饰器@classmethod标识为类方法，第一个参数必须是类对象，一般以cls作为第一个参数
当方法中需要使用类对象(如访问私有类属性)时，定义类方法，类方法一般和类属性配合使用
静态方法：需要用装饰器@staticmethod修饰，既不需要传递类对象也不需要传递实例对象(形参没有self/cls)
静态方法能够通过实例对象和类对象访问
当方法中既不需要实例对象，也不需要类对象时，定义静态方法，取消不需要的参数传递
"""
'''
多态：是一种使用对象的方式，子类重写父类方法，调用不同的子类对象的相同父类方法，可以产生不同的执行结果
多态的好处是：调用灵活，更容易编写更通用的代码，多态最好依赖于继承
多态实现步骤：1.定义父类，提供公共方法 2.定义子类，重写父类方法 3.传递子类对象调用，得到不同结果
'''


class A(object):
    __tooth = 10

    @classmethod
    def get__tooth(cls):
        return cls.__tooth

    @staticmethod
    def print_info():
        print('123')


class B(A):
    def work(self):
        print('b')


class C(A):
    def work(self):
        print('c')


class D(object):
    def workwith(self, alphabet):
        alphabet.work()


t1 = B()
t2 = C()
ly = D()
ly.workwith(t1)
ly.workwith(t2)

whz = A()
result = whz.get__tooth()
print(result)
A.print_info()
whz.print_info()
