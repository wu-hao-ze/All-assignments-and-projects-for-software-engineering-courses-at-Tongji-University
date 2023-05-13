from random import *

np = [[] for i in range(10)]
for i in range(10):
    for j in range(5):
        np[i].append(randint(-1, 1))
print(np)
list1 = [[] for i in range(5)]
for i in range(5):
    for j in range(10):
        list1[i].append(np[j][i])
print(list1)
