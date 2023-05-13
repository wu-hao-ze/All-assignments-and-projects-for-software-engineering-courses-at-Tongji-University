"""
// 为整除
%  为取余
** 为指数
2**4 为16
()优先级高于**高于* / % //高于+-
整数和浮点数一起参与运算，最后得到的结果是浮点数
"""

num1, float1, str1 = 10, 0.5, 'hello world'
a = b = 100
'''
+=  -=  *=  /=  //=  %=  **=
'''
print(8 / 5)
print(8 // 5)

print(4 / 2)
print(int(4 / 2))
d = 10
d *= 1 + 2
print(d)

# and or not
print(not False)

# and运算符只要有一个值为0，结果就是0，否则结果为最后一个非0数字
# or运算符，只有所有值为0结果才为0，否则结果为第一个非0数字
t=0
print('' and 23 and '123')
print(0 and 2)
print(0 or 2)
print(1 or 0)
print(2 or 3)
