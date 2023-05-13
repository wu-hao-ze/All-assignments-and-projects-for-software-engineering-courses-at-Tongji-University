"""
推导式又叫生成式，适用范围：列表，字典，集合
作用：化简代码，让代码更少
"""
# 列表推导式：用一个表达式创建一个有规律的列表或控制一个有规律的列表
list1 = [i for i in range(10)]
print(list1)
'for左侧是返回值'
list1 = [i for i in range(10) if i % 2 == 0]
print(list1)
list1 = [(i, j) for i in range(1, 3) for j in range(3)]
print(list1)

# 字典推导式：快速合并列表为字典或提取字典中目标数据
list1 = ['name', 'age', 'gender']
list2 = ['Tom', 20, 'man']
dict1 = {list1[i]: list2[i] for i in range(len(list1))}
print(dict1)
# 快速合并列表为字典适用于两个列表数据个数相同，或者key列表数据少于value列表的数据个数
dict1 = {i: i ** 2 for i in range(1, 6)}
print(dict1)

counts = {'MBP': 268, 'HP': 125, 'DELL': 201, 'Lenovo': 199, 'acer': 99}
count1 = {key: value for key, value in counts.items() if value > 200}
print(count1)

count2 = {}
for key, value in counts.items():
    if value > 200:
        count2[key] = value
print(count2)

# 集合推导式：有去重功能
list1 = [1, 1, 2]
set1 = {i ** 2 for i in list1}
print(set1)
