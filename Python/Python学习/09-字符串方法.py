"""
字符串常用操作方法：查找，修改，判断
"""
# 查找    查找子串在字符串中的位置或出现的次数
'''
find():检测某个子串是否包含在这个字符串中，如果在，返回这个子串开始的位置下标，否则返回-1
index():否则报异常
count():返回某个子串在字符串中出现的次数
rfind():查找方向为右侧开始
rindex():查找方向为右侧开始
字符串序列.find(子串,开始位置下标,结束位置下标)
开始和结束下标可以省略，表示在整个字符串序列中查找
'''
mystr = 'hello world and it and ith and Python'
print(mystr.find('and'))
print(mystr.find('and', 15, 30))
print(mystr.find('ands'))
# print(mystr.index('ands'))
print(mystr.count('and'))
print(mystr.count('ands'))
print(mystr.rfind('and'))
print(mystr.rindex('and', 15))
print('-----------')

# 修改     通过函数的形式修改字符串中的数据
'''
replace():替换
字符串序列.replace(旧子串,新子串,替换次数)
替换次数如果不写，则是全部替换
split():按照指定字符分割字符串
字符串序列.split(分割字符,分割字符出现的次数)
返回值为一个列表，丢失分割符
join():将列表中的多个字符串合并成一个新的字符串
字符或子串.join(多字符串组成的序列)
返回值为新的字符串
'''
mystr = 'hello world and it and ith and Python'
new_mystr = mystr.replace('and', 'he')
print(new_mystr)
new_mystr = mystr.replace('and', 'he', 2)
print(new_mystr)
new_mystr = mystr.replace('and', 'he', 5)
print(new_mystr)
new_mystr = '.'.join(new_mystr)
print(new_mystr)
print('-----------')

list1 = mystr.split()
print(list1)
list1 = mystr.split('and', 2)
print(list1)
list2 = 'whz'.join(list1)
print(list2)
print('-----------')

mylist = ['aa', 'bb', 'cc']
new_mylist = ','.join(mylist)
print(new_mylist)
print('-----------')

'''
capitalize():将字符串第一个字符转换成大写
title():将字符串每个单词首字母都转换成大写
lower():将字符串中所有字母大写转小写
upper():将字符串中所有字母小写转大写
'''
print(mystr.capitalize())
print(mystr.title())
print(mystr.lower())
print(mystr.upper())
print('-----------')

'''
lstrip():删除字符串左侧空白字符
rstrip():删除字符串右侧空白字符
strip(): 删除字符串两侧空白字符
'''
mystr1 = "       hello world and it and ith and Python          "
print(mystr1.lstrip())
print(mystr1.rstrip())
print(mystr1.strip())
print('-----------')

'''
ljust():返回一个原字符串左对齐，并使用指定字符填充至对应长度，默认用空格
rjust():右对齐
center():居中对齐
'''
mystr2 = 'hello'
print(mystr2.ljust(10, 'w'))
print(mystr2.rjust(10, 'w'))
print(mystr2.center(11, 'w'))
print('-----------')

'''
startswith():检查字符串是否以指定子串开头，是则返回True，否则返回False，如果设置下标，则是在指定范围寻找
字符串序列.startswith(子串,开始位置下标,结束位置下标)
endswith():结尾判断
'''
print(mystr.startswith('hel'))
print(mystr.startswith('hello  '))
print(mystr.endswith('Python'))

print('........')
'''
isalpha():如果一个字符串至少有一个字符或所有字符都是字母则返回True，否则为False
isdigit():如果一个字符串至少有一个字符或所有字符都是数字则返回True，否则为False
isalnum():如果一个字符串至少有一个字符或所有字符都是字母或数字则返回True，否则为False
isspace():如果一个字符串至少有一个字符或所有字符都是空格则返回True，否则为False
'''
print(''.isalpha())
print(mystr.isalpha())  # mystr中有空格，所以返回False
print(''.isspace())

