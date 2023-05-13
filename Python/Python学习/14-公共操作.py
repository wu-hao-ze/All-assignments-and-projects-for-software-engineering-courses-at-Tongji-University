str1 = 'aa'
str2 = 'bb'
list1 = [1, 2]
list2 = [10, 20]
tuple1 = (1, 2)
tuple2 = (10, 20)
dict1 = {'name': 'whz', 'age': 18}
dict2 = {'age': 30}
'''
公共操作包括：运算符，公共方法，容器类型转换
'''
# 运算符
'''
+:合并                    字符串，列表，元组
*:复制                    字符串，列表，元组
in:元素是否存在            字符串，列表，元组，字典
not in:元素是否不存在      字符串，列表，元组，字典
'''
print(str1 + str2)
print(list1 + list2)
print(tuple1 + tuple2)
print(str1 * 5)
print(list1 * 3)
print(tuple1 * 3)
print('a' in str1)
print('a' not in list1)
print('a' in tuple1)
print('name' in dict1)
print('whz' in dict1)
print('name' in dict1.keys())
print('whz' in dict1.values())

# 公共方法
'''
len():计算容器中元素的个数
del:删除
max():返回容器中最大值
min():返回容器中最小值
range(start,end,step):生成从start到end的数字，步长为step，供for循环使用，[start, end)
enumerate(可遍历对象,start=0):用于将一个可遍历的数据对象(如列表，元组，字符串等)组合为一个索引序列，
同时列出数据和数据下标，供for循环使用，start参数用来设置遍历数据的起始值，默认为0
'''
print(len(str1))
print(len(list1))
print(len(dict1))  # 字典中有两个键值对

# del str1
# print(str1)
del list1[0]
print(list1)
str1 = 'abcdefg'
print(max(str1))
list1 = [10, 20, 30, 40]
print(min(list1))

for i in range(1, 10, 2):
    print(i, end=' ')
print()
for i in range(1, 10):
    print(i, end=' ')
print()
for i in range(10):
    print(i, end=' ')
print()
# range生成的序列不包括end，切片也不包括end，randint包括end

list1 = ['a', 'b', 'c', 'd', 'e']
for i in enumerate(list1):
    print(i, end=' ')
print()
for i in enumerate(list1, start=1):
    print(i, end=' ')
print()

# 容器类型转换
'''
tuple():将某个序列转换成元组
list():将某个序列转换成列表
set():将某个序列转换成集合
'''
list1 = [10, 20, 30, 40, 50]
t1 = (100, 200, 300, 400)
s1 = {'a', 'b', 'c', 'd', 'e'}
print(tuple(list1))
print(list(s1))
print(set(t1))
