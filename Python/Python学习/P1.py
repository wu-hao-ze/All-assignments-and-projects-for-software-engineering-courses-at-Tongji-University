i1 = 1
i2 = 2
total = 0
for i in range(20):
    a = i2 / i1
    total += a
    print(f'{i2}/{i1}', end=' ')
    if (i + 1) % 5 == 0:
        print()
    t = i2
    i2 = i1 + i2
    i1 = t
print(f'20项之和为{float(total)}')
