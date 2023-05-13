"""
创建集合set使用{}或set(),但是如果要创建空集合只能使用set(),因为{}用来创建空字典
集合里的数据不可重复，如果有需求不要重复元素的时候，就可以往集合里存储
"""
s1 = {10, 20, 30, 40, 50}
print(s1)
'''
集合数据直接自动消除重复元素
集合没有顺序，不支持下标操作
'''
s2 = {10, 10, 10, 20}
print(s2)
s3 = set('abcdefg')
print(s3)
s4 = set()
print(s4)

# 增加
# 集合有去重的特点，所以若是增加已经存在的数据的话，则不进行任何操作
'''
update():追加的数据是序列，不能是单个数据，而且追加序列的话也是把序列拆掉，将元素添加进去
add():追加的数据是单个数据，不能是像列表这种序列
'''
s1.add(100)
print(s1)
s1.add(100)
print(s1)
# s1.add([10])
s1.update(['whz', 'yqh'])
print(s1)

# 删除
'''
remove():删除集合中指定的数据，如果数据不存在则报错
discard():不报错
pop():随机删除某个数据，并返回这个数据
'''
s2 = {10, 20}
s2.remove(10)
print(s2)
s2.discard(5)
print(s2)
del_num = s1.pop()
print(del_num)
print(s1)

# 查找
'''
in:判断数据在集合中
not in:判断数据不在集合中
'''
print(10 in s1)
print(10 not in s1)
