def fun1(x):
    return 2 * x ** 3 - 4 * x ** 2 + 3 * x - 6


def fun2(a, b):
    while abs(b - a) > pow(10, -15):
        i = (a + b) / 2
        if fun1(a) * fun1(i) < 0:
            b = i
        else:
            a = i
        if fun1(a) == 0:
            return a
        if fun1(b) == 0:
            return b
    return a


print(fun2(-10, 10))
