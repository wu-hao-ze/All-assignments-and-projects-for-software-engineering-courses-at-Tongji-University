"""
用类创建一个对象，对象也是类的实例化
类是对一系列具有相同特征和行为的事物的统称，是一个抽象的概念，特征是属性(变量)，行为是方法(函数)
对象是用类创建出来的真实的事物
python2的版本下经典写法是class 类名:
python3的版本下新式写法是class 类名(object):
括号里的object是基类，如果继承的话，将object改成父类
类名要满足标识符命名规则，遵循大驼峰命名
对象又叫实例
对象名=类名()
"""


class Washer(object):
    # 类里面可以直接添加属性
    length = 700

    def wash(self):
        # self指的是调用该函数的对象
        # 比如下面haier对象调用的wash，那么haier对象传到self中，print(self)就是print(haier)
        print('whz')
        print(self)
        # 类里面可以获取对象属性
        # self.属性名
        # 但是尽量用公用的，因为width并不是类Washer共有的属性，而只是haier有的
        print(self.width)


# 创建对象
haier = Washer()  # haier是对象名
print(haier)  # 打印的是内存地址

# 类外面添加对象属性
# 对象名.属性名=值
haier.width = 400
haier.height = 500
# 但是不是公用的，比如haier1这个对象就没有width这个属性，但是有length

# 类外面可以获取对象属性
print(haier.width)
haier1 = Washer()
print(haier1.length)

# 实例方法/对象方法：对象名.wash()
haier.wash()
# 这个不能放在width400，height500的前面，因为类中的wash函数中出现了width
# 只要是调用wash函数，就得先定义属性，先得有属性才能调用
