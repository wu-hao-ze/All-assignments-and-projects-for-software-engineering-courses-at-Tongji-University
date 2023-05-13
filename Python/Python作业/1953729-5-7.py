p = []
for i in range(100, 1000):
    s = []

    t = i
    while i > 0:
        s = s + [i % 10]
        i = i // 10
    i = t
    if i == (s[0] ** 3 + s[1] ** 3 + s[2] ** 3):
        p.append(i)

print(p)
