def fib(n):
    i1 = 1
    i2 = 1
    s = [i1, i2]
    if n == 1:
        return [i1]
    elif n == 2:
        return [i1, i2]
    else:
        i = 2
        while i < n:
            t = i1 + i2
            i1 = i2
            i2 = t
            s += [i2]
            i += 1
        return s


n = int(input("请输入n："))
print(fib(n))
