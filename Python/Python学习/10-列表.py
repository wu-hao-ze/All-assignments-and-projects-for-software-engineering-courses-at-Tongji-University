"""
列表list的操作：增删改查
列表可以一次性存储多个数据，且可以是不同数据类型
"""
# 查找
name_list = ['Tom', 'Lily', 'Rose']
print(name_list[0])
'''
index():返回指定数据所在位置的下标
count():统计指定数据在当前列表出现的次数
len():访问列表长度，即列表中数据的个数
in/not in:判断指定数据是否在某个列表序列，如果在返回True/False，不在返回False/True
'''
print(name_list.index('Lily'))
print(name_list.index('Rose', 0))
print(name_list.count('Lily'))
print(len(name_list))
print('Lily' in name_list)
print('Tom' not in name_list)
# name = input('输入名字：')
# if name in name_list:
#     print('已输入过')


# 增加
'''
append():列表结尾追加数据
列表序列.append(数据):
extend():列表结尾追加数据
extend若追加字符串，是把字符串拆开追加，若追加列表，是把列表拆开追加里面的元素
insert():指定位置新增数据
列表序列.insert(位置下标,数据)
'''
name_list.append('whz')
print(name_list)
name_list.append(123)
print(name_list)
name_list.append([11, 22])
print(name_list)
name_list.extend('123')
print(name_list)
name_list.extend(['aa', 'bb', 'cc'])
print(name_list)
name_list.insert(2, 'yqh')
print(name_list)

# 删除
'''
del 目标
del删除后直接就没了，如下面的删除列表，列表整个都不复存在了
pop():删除指定下标的数据，如果不指定下标，则默认删除最后一个数据，返回被删除的数据
remove():删除某个数据的第一个匹配项，比如列表中有两个相同的数据，remove只删除第一个数据，返回被删除的数据
clear():清空列表
'''
# del name_list
# print(name_list)
del name_list[1]
print(name_list)
name_list = ['Tom', 'Lily', 'Lily', 'Lily', 'Rose']
del_name = name_list.pop()
print(name_list)
del_name = name_list.pop(0)
print(name_list)
del_name = name_list.remove('Lily')
print(name_list)
name_list = ['Tom', 'Lily', 'Lily', 'Lily', 'Rose']
i = 0
while i < len(name_list):
    if name_list[i] == 'Lily':
        name_list.remove('Lily')
    else:
        i += 1
print(name_list)
name_list.clear()
print(name_list)

# 修改
'''
直接对应下标赋值就可以修改
reverse():将列表元素逆序排列
sort():将列表升序或者降序排序，默认是升序
sort中有两个参数，一个是key，一个是reverse
key是指列表中有字典，按照字典的key值排序
reverse=False是升序,reverse=True是降序
排序按照ASCII码值排序
'''

name_list = ['Tom', 'Lily', 'Lily', 'whz', 'Yqh', 'Rose']
name_list[1] = '1'
print(name_list)
name_list.reverse()
print(name_list)
name_list.sort(reverse=True)
print(name_list)
name_list.sort(reverse=False)
print(name_list)
name_list = [3, 1, 4, 2, 6, 5, 8]
name_list.sort(reverse=True)
print(name_list)
name_list.sort(reverse=False)
print(name_list)
students = [
    {'name': 'whz', 'age': 22},
    {'name': 'Tom', 'age': 18},
    {'name': 'Rose', 'age': 25}
]
students.sort(key=lambda x: x['name'])
print(students)
students.sort(key=lambda x: x['age'])
print(students)

# 复制
'''
copy():列表的复制
'''
name_list = ['Tom', 'Lily', 'Rose']
name_list2 = name_list.copy()
print(name_list2)

# 列表的遍历
'''
可以用while，也可以用for
'''
for i in name_list:
    print(i)

# 一个小案例:8个老师随机分配到3个办公室，要求每个办公室都不为空
import random

teachers = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H']
offices = [[], [], []]
teachers2 = teachers.copy()
j = 0
for i in offices:
    num = random.randint(0, 7 - j)
    # i.append(teachers2[num])
    offices[j] = [teachers2[num]]
    # 上面这两种写法都行，但是推荐第一种
    teachers2.pop(num)
    j += 1
for name in teachers2:
    num = random.randint(0, 2)
    offices[num].append(name)
print(offices)
officei = 1
for office in offices:
    print(f'办公室{officei}的人数是{len(office)},老师分别是')
    for name in office:
        print(name, end='')
    print()
    officei += 1
