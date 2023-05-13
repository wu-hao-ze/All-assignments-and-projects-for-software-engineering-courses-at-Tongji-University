"""
在python中文件和文件夹的操作要借助os模块里面的相关功能
import os
os.函数名()
"""

'''
文件重命名：os.rename(目标文件名,新文件名)
删除文件：os.remove(目标文件名)
'''
import os

f = open('1.txt', 'w')
f.close()
os.rename('1.txt', 'whz.txt')
os.remove('whz.txt')

'''
os.mkdir(文件夹名字):创建文件夹
os.rmdir(文件夹名字):删除文件夹
os.getcwd():获取当前目录路径
os.chdir(文件夹名字):改变当前目录到对应的文件夹下目录
os.listdir(文件夹名字):获取对应文件夹下目录内容，组成一个列表，如果不写文件夹名字，则默认是当前目录
os.rename(目标文件名,新文件名):文件夹重命名
'''
os.mkdir('aa')
os.rmdir('aa')
print(os.getcwd())
# 在aa文件夹下创建bb文件夹
os.mkdir('aa')
# 此时aa文件夹是当前文件夹的下级，所以可以直接写aa，实际上是E:\Python学习\aa的简略写法
os.chdir("E:\Python\Python学习\\aa")
os.mkdir('bb')
# 必须先删除bb文件夹，才能再删除aa文件夹，必须保证要删除的文件夹是空的
os.rmdir('bb')
os.chdir('E:\Python\Python学习')
os.rmdir('aa')
print(os.listdir())
print(os.listdir('E:\\C++'))

'''
批量重命名文件添加字符串：
1.获取指定目录的所有文件，用listdir得到一个列表
2.for循环new_name与指定字符串拼接，rename重命名new_name
'''
# flag如果为1，添加指定字符串，如果为2，删除指定字符串
# 这段代码不要轻易执行，注意点
# flag = 2
# file_list = os.listdir()
# for name in file_list:
#     if flag == 1:
#         new_name = 'Python-' + name
#     elif flag == 2:
#         num = len('Python-')
#         new_name = name[num:]
#     os.rename(name, new_name)
