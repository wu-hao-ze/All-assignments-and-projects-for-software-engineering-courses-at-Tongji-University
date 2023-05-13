f = open('T1.txt', 'r')
list1 = f.readlines()
n = len(list1)
f.close()
f1 = open('T1.txt', 'r')
f2 = open('T2.txt', 'w')
while 1:
    item = f1.read(1)
    if len(item) == 0:
        break
    n = ord(item)
    if 65 <= n <= 90:
        n += 32
    elif 97 <= n <= 122:
        n -= 32
    f2.write(chr(n))
f1.close()
f2.close()
