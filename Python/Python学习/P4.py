def f(n):
    if n == 1:
        return 1
    if n == 2:
        return 2
    t = 3 * f(n - 1) - f(n - 2)
    print(t)
    return t


f(5)
