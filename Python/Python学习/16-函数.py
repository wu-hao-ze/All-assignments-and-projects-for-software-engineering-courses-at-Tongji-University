"""
def 函数名(参数):
先定义再调用
"""


def add_num(a, b):
    """求和函数"""  # 此行就是写函数的说明文档
    return a + b


def multiply(a, b):
    """
    乘法函数，求两个数的乘法
    :param a: 第一个参数的作用
    :param b: 第二个参数的作用
    :return: 返回值
    """
    return a * b


# return函数返回值，且退出当前函数，不执行下方代码
result = add_num(4, 5)
print(result)

# help()函数可以查看函数的说明文档
help(len)
help(add_num)
help(multiply)


def print_line():
    print('-' * 20)


def print_lines(num):
    i = 0
    while i < num:
        print_line()
        i += 1


print_lines(5)


def sum_num(a, b, c):
    return a + b + c


def average(a, b, c):
    result1 = sum_num(a, b, c)
    return result1 / 3


result = average(1, 2, 3)
print(result)
