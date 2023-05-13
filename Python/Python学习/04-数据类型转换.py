"""
input('提示信息')
输入的当成字符串处理
"""
password = input('请输入你的密码：')
print(f'你输入的密码是{password}')
print(type(password))
# 数据类型的转换
'''
int()
float()
complex()  # 转成复数
str()
eval()  # 转成字符串中本身应该的数据类型
tuple()
list()
chr() #将ASCII码转换为对应的字符
ord() #将字符转换为对应的ASCII码
hex()
oct()
'''

print(type(eval('"1234好"')))
print(type(eval('1234.1234')))
print(type(eval('{1234}')))
print(type(eval('[1234]')))
print(type(eval('"1234"')))

print(complex(12.01))
print(complex(-2j))
print(complex(10 - 2j))
print(chr(32))
print(ord('0'))
