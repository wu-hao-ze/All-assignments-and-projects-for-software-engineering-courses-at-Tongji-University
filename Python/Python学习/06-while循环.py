i = 0
result = 0
while i <= 100:
    if i % 2 == 0:
        result += i
    i += 1
print(result)

n = int(input("请输入n的值："))
i = 0
while i < n:
    print(' ' * (n - i - 1), end='')
    print('* ' * (i + 1))
    i += 1

# 九九乘法表
i = 1
while i <= 9:
    j = 1
    while j <= i:
        print('%d*%d=%d' % (j, i, i * j), end='\t')
        j += 1
    i += 1
    print()

i = 1
while i <= 9:
    j = 1
    print(' ' * (8 * (9 - i)), end='')
    while j <= i:
        print('%d*%d=%d' % (j, i, i * j), end='\t')
        j += 1
    i += 1
    print()


# 循环可以和else配合，else下方缩进的代码是指当循环正常结束之后要执行的代码
else:
    print('这就是九九乘法表')
