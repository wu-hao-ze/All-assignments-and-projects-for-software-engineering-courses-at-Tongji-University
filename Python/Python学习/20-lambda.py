"""
如果一个函数有一个返回值，并且只有一行代码，可以使用lambda简化
"""
'''
lambda又叫匿名函数
lambda 参数列表: 表达式
lambda的参数列表可有可无，函数的参数在lambda表达式中完全适用
lambda能接收任何数量的参数但只能返回一个表达式的值
'''
# 无参数
f = lambda: 200
print(f)  # lambda的内存地址
print(f())

# 有一个参数
f1 = lambda a: a
print(f1('whz'))

# 有两个参数/同理可得多个参数
f1 = lambda a, b: a + b
print(f1(1, 2))

# 默认参数
f1 = lambda a, b, c=100: a + b + c
print(f1(1, 2))

# 可变参数args
f1 = lambda *args: args
print(f1(10, 'whz'))

# 可变参数**kwargs
f1 = lambda **kwargs: kwargs
print(f1(name='Tom', age=18))

# 带判断的
f1 = lambda a, b: a if a > b else b
print(f1(2, 3))

# 与sort的连用
students = [
    {'name': 'whz', 'age': 22},
    {'name': 'Tom', 'age': 18},
    {'name': 'Rose', 'age': 25},
    {'name': 'Yqh', 'age': 25}
]
students.sort(key=lambda x: x['name'])
print(students)
students.sort(key=lambda x: x['age'])
print(students)
# 这个排序是按照字符串的第二个字母排序的，即h，o，o，q的顺序
students.sort(key=lambda x: x['name'][1])
print(students)

'''
其中的key=后面也可以用函数来标识
'''


def fun(li):
    return li['name'][1]


students.sort(key=fun)
