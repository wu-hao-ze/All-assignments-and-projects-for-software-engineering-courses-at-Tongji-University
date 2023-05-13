"""
当检测到一个错误，解释器无法继续执行，反而出现一些错误提示，这就是异常
书写时觉得可能出现错误的语句写异常，就可以执行另一条语句继续向下执行，避免整个程序停滞
try:
    可能发生错误的代码
except:
    如果出现异常执行的代码
一般try下方只放一行尝试执行的代码
"""
try:
    f = open('yqh.txt', 'r')
except:
    f = open('yqh.txt', 'w')
'''
try:
    可能发生错误的代码
except 异常类型:
    如果捕获到该异常类型执行的代码
如果尝试执行的代码的异常类型和要捕获的异常类型不一致，则无法捕获异常
'''
try:
    print(num)
except NameError:
    print('wrong')

'''
异常类型：
NameError:      print(n)
ZeroDivisionError:    print(1 / 0)
如果捕获多个异常，可以把要捕获的异常类型的名字放到except后，用元组的方式书写
'''
try:
    print(num)
except (NameError, ZeroDivisionError):
    print('wrong')
'''
捕获异常信息，用as后的变量捕捉，打印变量就是异常信息
'''
try:
    print(num)
except (NameError, ZeroDivisionError) as t:
    print(t)
'''
Exception是所有程序异常类的父类，包括所有异常类型
'''
try:
    f = open('123.txt', 'r')
except Exception as result:
    print(result)
'''
else表示如果没有异常要执行的代码，没有异常的话try和else都执行
'''
try:
    print(1)
except Exception as result:
    print(result)
else:
    print('没有异常')
'''
finally表示无论是否异常都要执行的代码，例如关闭文件
'''
try:
    f = open('123.txt', 'r')
except Exception as result:
    print(result)
else:
    print('没有异常')
finally:
    f.close()
