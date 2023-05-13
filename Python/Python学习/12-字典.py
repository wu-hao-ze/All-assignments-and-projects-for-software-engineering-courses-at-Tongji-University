"""
字典dict里的数据都是以键值对的形式出现，字典数据和数据顺序没有关系
字典不支持下标，只需要按照对应的键的名字查找数据
"""
'''
数据为键值对形式出现
各个键值对数据之间用逗号隔开
冒号前面的为键key,冒号后面的为值value
字典的key一定是不可变类型，字符串，int，float，元组
'''
# 有数据的字典
dict1 = {'name': 'Tom', 'age': 20, 'gender': '男'}
print(dict1)
dict1 = {1: 'whz', 2: 'yqh', (1, 2, 3): 'Tom'}
print(dict1)
dict1 = dict([('name', 'Tom'), ('age', 20), ('gender', '男')])
print(dict1)

# 空字典
dict2 = {}
dict3 = dict()  # dict函数创建空字典
print(dict3)

# 增加和修改
'''
字典是可变类型，可以新增数据
字典序列[key]=value
如果key存在则修改这个key对应的值，如果key不存在则新增此键值对
'''
dict1['name'] = 'whz'
print(dict1)
dict1['ID'] = 110
print(dict1)

# 删除
'''
del:删除字典或删除字典中指定键值对，删除字典的话，字典不再存在
clear():清空字典，字典仍然存在为{}
'''
del dict1['ID']
print(dict1)
del dict1  # 删除整个字典，字典已不复存在
dict1 = {'name': 'Tom', 'age': 20, 'gender': '男'}
dict1.clear()
print(dict1)

# 查找
'''
key值查找，如果当前要查找的key存在，则返回对应的值，若不存在则报错
字典序列.get(key,默认值)
如果当前查找的key不存在则返回第二个参数即对应的默认值，如果不写第二个参数，则返回None
字典序列.keys():查找该字典序列中所有的key
字典序列.values():查找该字典序列中所有的value
字典序列.items():查找该字典序列中所有的键值对，里面的数据是元组
'''
dict1 = {'name': 'Tom', 'age': 20, 'gender': '男'}
print(dict1['name'])
# print(dict1['ID'])
print(dict1.get('name'))
print(dict1.get('ID', '不存在'))
print(dict1.get('ID'))
print(dict1.keys())
print(dict1.values())
print(dict1.items())

# 循环遍历
for key in dict1.keys():
    print(key, end=' ')

print()

for value in dict1.values():
    print(value, end=' ')

print()

for item in dict1.items():
    print(item, end=' ')

print()

for key, value in dict1.items():
    print(f'{key}={value}', end=' ')
print()

# 字典拆包
dict1 = {'name': 'Tom', 'age': 20, 'gender': '男'}
a, b, c = dict1
print(a, end=' ')
print(b, end=' ')
print(c)
# 对字典拆包，取出来的是key
print(dict1[a], end=' ')
print(dict1[b], end=' ')
print(dict1[c])
