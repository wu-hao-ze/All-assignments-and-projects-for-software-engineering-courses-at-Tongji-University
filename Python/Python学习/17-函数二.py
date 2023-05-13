# 变量作用域：变量生效的范围，局部变量和全局变量
"""
局部变量：定义在函数体内部的变量，只在函数体内部生效
全局变量：函数体内外都生效的变量
"""


def testA():
    print(a)


def testB():
    # global关键字声明a是全局变量
    global a
    a = 200


a = 100
testB()
print(a)


# 一个函数多个返回值
def return_num():
    return 1, 2


'''
return a,b...的写法返回多个数据，默认是元组类型
return后面可以连接列表，元组，字典等，返回多个值
'''
print(return_num())

# 函数的参数
'''
位置参数：调用函数时根据函数定义的参数位置来传递参数，传递和定义参数的顺序及个数必须一致
关键字参数：函数调用，通过键=值的形式指定，不存在先后顺序，如果有位置参数，必须写在关键字参数的前面
缺省参数：也叫默认参数，在定义函数时使用，为参数提供默认值，所有位置参数必须出现在默认参数前，包括定义和调用
不定长参数：也叫可变参数，不确定调用时会传递多少个参数时使用，可以包裹位置参数，也可以包裹关键字参数
包裹位置传递：传进的所有参数都会被args变量收集，合并成一个元组
包裹关键字传递：传进的所有参数都会被kwargs变量收集，返回一个字典
无论是包裹位置传递还是包裹关键字传递，都是一个组包的过程
'''


def user_info(name, age, gender='男'):
    print(f'您的名字是{name}，年龄是{age}，性别是{gender}')


def test(*args):
    print(args)


def test1(**kwargs):
    print(kwargs)


user_info('Tom', 30, '男')
user_info('小明', gender='女', age=16)
user_info('Tom', 20)
test('Tom')
test('Tom', 18, '男')
test1(name='whz', age=16, gender='男')
a, b = 1, 2
a, b = b, a
print(a, b)

# 引用：python中，值是靠引用来传递的
'''
我们可以用id()来判断两个变量是否为同一个值的引用，将id值理解为那块内存的地址标识
'''
a = 1
b = a
print(id(a))
print(id(b))
print()
# 说明a和b此时所占用的内存是同一块

a = 2
print(id(a))
print(id(b))
print()
# b还是之前的地址，但是a重新赋值后，对应的内存地址也改变了，此时a和b占用不同的内存地址

aa = [10, 20]
bb = aa
print(id(aa))
print(id(bb))
print()
# 说明aa和bb此时所占用的内存是同一块

aa.append(30)
print(aa)
print(bb)
print(id(aa))
print(id(bb))
print()
# int是不可变数据类型，列表是可变数据类型，改变aa的列表，也会影响到bb

aa = [10, 20, 30]
print(aa)
print(bb)
print(id(aa))
print(id(bb))
# 就算赋值一个同样的列表，但这个列表还是重新赋值的，所以还是不一样的内存地址
print()


def test2(d):
    print(id(d))
    d += d
    print(d)
    print(id(d))


b = 100
print(id(b))
test2(b)
print(b)
# 可以看到int的实参是把地址传了进去，但是对形参的值的改变并不会影响到实参的改变
print()
c = [10, 20]
test2(c)
print(c)
print(id(c))
# 可以看到列表的实参是把地址传了进去，对形参的修改会影响到实参

'''
不可变类型：数据不能够直接进行修改，而需要额外赋新值，体现在形参的改变会影响实参
可变类型：数据能够直接进行修改，而不需要额外赋新值，体现在形参的改变不会影响实参

不可变类型有：int整型，float浮点型，str字符串，tuple元组
可变类型有：列表，字典，集合
'''
