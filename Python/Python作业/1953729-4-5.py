i = 1
s = 0
n = 1
while 1 / i >= pow(10, -4):
    s = s + 1 / i
    i = i + n
    n += 1
print(s)
