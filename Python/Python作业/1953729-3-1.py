import math

a, b, c = eval(input("请输入一元二次方程的三个系数a,b,c："))
delta = b ** 2 - 4 * a * c
if delta > 0:
    x1 = (-b + math.sqrt(delta)) / 2 * a
    x2 = (-b - math.sqrt(delta)) / 2 * a
    print("有两个不同的实根为：x1=%f,x2=%f" % (x1, x2))
elif delta == 0:
    x1 = (-b + math.sqrt(delta)) / 2 * a
    x2 = (-b - math.sqrt(delta)) / 2 * a
    print("有两个相等的实根为：x1=x2=%f" % x1)
else:
    print("无实根")
