n = float(input("请输入一个三位数"))
a = n % 10
b = n // 10 % 10
c = n // 100 % 10
t = c + 10 * b + 100 * a
print(int(t))
