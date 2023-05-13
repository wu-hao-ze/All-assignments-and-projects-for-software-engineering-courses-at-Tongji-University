def s(x):
    i = 1
    a = x
    sum1 = 0
    while abs(a) >= pow(10, -6):
        sum1 += a
        i = i * (i + 1) * (i + 2)
        a = (-1) * a * x ** 2 / i
    return sum1


x = float(input("请输入x的值："))
print("级数为", s(x))
