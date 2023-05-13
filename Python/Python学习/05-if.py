import random

if True:
    print('whz')
    print('whz')
print(123)
age = int(input('请输入年龄：'))
if (age >= 18) and (age <= 60):
    print(f'你的年龄是{age},可以上网')
elif age > 60:
    print('年龄太大，为%d不能上网' % age)
else:
    print('年龄太小')
print('系统关闭')

num = random.randint(0, 2)  # randint(0, 2)随机数为0，1，2
print(num)
num = random.random()  # 生成一个[0, 1)
print(num)
num = random.uniform(0, 2)  # 生成[0, 2]的实数
print(num)

a = 1
b = 2
c = a if a > b else b
'''
三目运算符
条件成立执行的表达式 if 条件 else 条件不成立执行的表达式
'''
