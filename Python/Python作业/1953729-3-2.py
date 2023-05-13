x = float(input("原来所花的钱为："))
if x < 1000:
    y = x
elif 1000 <= x < 2000:
    y = 0.9 * x
elif 2000 <= x < 3000:
    y = 0.8 * x
else:
    y = 0.7 * x
print("实际所付的钱为：%f元" % y)
