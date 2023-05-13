M, N = eval(input("请输入鸡和兔的总数以及鸡和兔的总脚数："))
if N % 2 != 0:
    print("脚数不是偶数，错误")
elif N < 2 * M or N > 4 * M:
    print("所求鸡或兔的总数有负数，错误")
else:
    x = 2 * M - N / 2
    y = N / 2 - M
    print("鸡的数量为：%d,兔的数量为：%d" % (x, y))
