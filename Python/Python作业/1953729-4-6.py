from math import *

n = 1
pai = 2
while n <= 1000:
    pai = pai * ((2 * n) ** 2 / ((2 * n - 1) * (2 * n + 1)))
    n += 1
print(pai)
print(pi)
