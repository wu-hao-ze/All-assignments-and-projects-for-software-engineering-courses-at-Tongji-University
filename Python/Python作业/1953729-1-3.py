import math

c, b = eval(input("输入斜边和直角边："))
a = math.sqrt(c ** 2 - b ** 2)
print("另外一条直角边为：", a)
print("周长为：%f,面积为：%f" % (a + b + c, 1 / 2 * a * b))
