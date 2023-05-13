x = int(input("输入正整数："))
s = []
i = 0
n = 0
while x != 0:
    s = s + [x % 10]
    x = x // 10
    i += 1
t = i
while i >= 1:
    n = n + s[i - 1] * pow(10, t - i)
    i -= 1
print(int(n))
