a = 'whz 123'
a = 'whz ' \
    '123'
b = "whz " \
    "123"
print(a)
c = '''whz 123'''
print(c)
c = '''whz 
    123'''
print(c)
d = 'I\'m whz'
# 或者 d = "I'm whz"
print(d)

# 下标又叫索引
str = "abcdefg"
print(str[1])

# 切片是指对操作的对象截取其中一部分的操作，字符串，列表，元组都支持切片操作
# 序列[开始位置下标:结束位置下标:步长]
'不包含结束位置下标对应的数据，正负整数均可'
'步长也是正负整数均可，默认步长为1'

str1 = '0123456789'
print(str1[2:4])
print(str1[2:6:2])
print('-----------')

print(str1[:4])
print(str1[:4:2])
print('-----------')

print(str1[3:])
print(str1[3::2])
print('-----------')

print(str1[:])
print(str1[::2])
print('-----------')

print(str1[4:1:-1])
print(str1[4:1:-2])
print('-----------')

print(str1[::-1])
print(str1[3::-1])
print(str1[-3::-1])
print('-----------')

# 开始位置下标-1表示最后一个数9
print(str1[:-2])
print(str1[:-2:-1])
print('-----------')

# 步长为负数时，默认从-1开始，也就是最后一个数
print(str1[-5:-2:2])
print(str1[-5:-2:-2])  # 不能选取出数据，下标开始到结束的方向和步长的方向冲突，无法选取数据
print('-----------')
