d = {}
s = input("请输入学生姓名和手机号：")
list1 = s.split(" ")
name_list = list1[::2]
tel_list = list1[1::2]
d = dict(zip(name_list, tel_list))
print(d)
name = input("请输入姓名:")
list2 = name.split(" ")
tel_list2 = []
i = 0
while list2[i] != "xxx":
    tel_list2.append(d.get(list2[i]))
    i = i + 1
print(tel_list2)
