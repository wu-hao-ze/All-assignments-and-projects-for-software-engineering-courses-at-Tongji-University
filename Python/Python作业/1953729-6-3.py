s = input("输入一个英文句子")
d = {}
s = s.upper()
for i in s:
    if i not in d:
        d[i] = 1
    else:
        d[i] += 1
print(d)
