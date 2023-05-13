f = open('JJ.txt', 'rb')
list1 = f.readlines()
print(list1)
list3 = []
for i in list1:
    i = str(i)
    list2 = i.split(',')
    str1 = list2[len(list2) - 1]
    i2 = 0
    for i1 in str1:
        if '0' <= i1 <= '9' or i1 == '.':
            i2 += 1
        else:
            break
    str1 = str1[:i2]
    n = float(str1)
    list3.append(n)
list3.sort()
f1 = open('NewJJ.txt', 'w')
f1.write(list3)
f.close()
f1.close()
