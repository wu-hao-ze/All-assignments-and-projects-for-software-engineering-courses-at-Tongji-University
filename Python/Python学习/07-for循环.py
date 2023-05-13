"""
for 临时变量 in 序列:
    代码
"""
str = 'whz123'
for i in str:
    if i == '1':
        break
    print(i, end="")
else:
    print('能不能拿下啊')
# continue的中止当前循环，并不会影响循环正常结束，所以还是会输出
'''
for N in [2,3,5,7]

for i in range(10)
0,1,2,3,4,5,6,7,8,9

for i in list(range(5,10))
[5,6,7,8,9]

for i in list(range(0,10,2))
[0,2,4,6,8]
'''
