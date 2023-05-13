from random import *

n = randint(5, 10)
i = 1
ch1 = 'a'
for whz in range(1, n + 1):
    print(" " * (n - i), ch1 * (2 * i - 1))
    i += 1
    ch1 = chr(ord(ch1) + 1)
print()
ch2 = 'A'
i = 1
j = 1
for whz in range(1, n + 1):
    if whz <= (n + 1) // 2:
        print(" " * (n - i), ch2 * (2 * i - 1))
        i += 1
        ch2 = chr(ord(ch2) + 1)
        t = i - 1
    else:
        if n % 2 == 0:
            print(" " * (n - t + j - 1), ch2 * (2 * (n - i) + 1))
        else:
            print(" " * (n - t + j), ch2 * (2 * (n - i) + 1))
        i += 1
        j += 1
        ch2 = chr(ord(ch2) + 1)
