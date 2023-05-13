"""
如果要遍历一个文件夹下的所有文件，要使用递归
很多算法也需要递归，比如快速排序
递归的特点：函数内部调用自己，必须有出口
"""


def sum_numbers(num):
    if num == 1:
        return 1
    return num + sum_numbers(num - 1)


print(sum_numbers(3))
