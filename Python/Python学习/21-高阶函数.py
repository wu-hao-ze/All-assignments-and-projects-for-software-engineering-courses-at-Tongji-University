"""
把函数作为参数传入另一个函数，另一个函数就是高阶函数
高阶函数是函数式编程的体现，函数式编程是指这种高度抽象的编程范式
"""

'''
python内置的两个函数
abs()完成对数字的绝对值计算
round()完成对数字的四舍五入计算
'''
print(abs(-1))
print(round(1.4))
print(round(1.5))


# 高阶函数
def sum_num(a, b, f):
    return f(a) + f(b)


result1 = sum_num(-1, 2, abs)
result2 = sum_num(-1.5, 2.4, round)
print(result1)
print(result2)

'''
python中内置高阶函数
map(func,lst):将传入的func函数作用到lst变量的每个元素中，并将结果组成新的迭代器，如果要转换为列表，可以用list()
reduce(func,lst):其中func函数必须有两个参数，每次func计算的结果继续和序列中的下一个元素做累积计算
filter(func,lst):用于过滤序列，过滤掉不符合条件的元素，返回一个filter对象，如果要转换为列表，可以用list()
'''
# 计算list1序列中各个数字的2次方
list1 = [1, 2, 3, 4, 5]


def func(x):
    return x ** 2


t = map(func, list1)
print(t)
print(list(t))

# 计算list1序列中各个数字的累加和
import functools

list1 = [1, 2, 3, 4, 5]


def func(a, b):
    return a + b


t = functools.reduce(func, list1)
print(t)

# 选偶数
list1 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]


def func(x):
    return x if x % 2 == 0 else ''


result = filter(func, list1)
print(result)
print(list(result))
