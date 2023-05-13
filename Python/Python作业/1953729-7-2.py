f = open('学生成绩文件.txt', 'rb')
list1 = f.readlines()
print(list1)
list3 = []
for i in list1:
    i = str(i)
    list2 = i.split(' ')
    str1 = list2[len(list2) - 1]
    i2 = 0
    for i1 in str1:
        if i1 < '0' or i1 > '9':
            break
        i2 += 1
    str1 = str1[:i2]
    n = float(str1)
    list3.append(n)
list3.sort()
print(f'最小值为{list3[0]}')
print(f'最大值为{list3[len(list3) - 1]}')
print(f'平均值为{sum(list3) / len(list3)}')
f.close()
