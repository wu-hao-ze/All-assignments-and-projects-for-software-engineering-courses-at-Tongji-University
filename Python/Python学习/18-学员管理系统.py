def print_info():
    print()
    print('请选择功能--------------------')
    print('1.添加学员')
    print('2.删除学员')
    print('3.修改学员')
    print('4.查询学员')
    print('5.显示学员')
    print('6.退出系统')
    print('-----------------------------')


def add_info():
    """添加学员"""
    new_id = input('请输入学号：')
    new_name = input('请输入姓名：')
    new_tel = input('请输入手机号：')
    global info
    for i in info:
        if new_name == i['name']:
            print('该用户已经存在')
            return
    info_dict = {'id': new_id, 'name': new_name, 'tel': new_tel}
    info.append(info_dict)
    print(info)


def del_info():
    """删除学员"""
    del_name = input('请输入删除学员的姓名')
    global info
    for i in info:
        if del_name == i['name']:
            info.remove(i)
            break
    print('该学员不存在')
    print(info)


def modify_info():
    """修改学员"""
    modify_name = input('请输入要修改的学员姓名：')
    global info
    for i in info:
        if modify_name == i['name']:
            i['tel'] = input('请输入新的手机号：')
            break
    print('该学员不存在')
    print(info)


def search_info():
    """查找学员"""
    search_name = input('请输入要查找的学员姓名：')
    global info
    for i in info:
        if search_name == i['name']:
            print('查找到的学员信息如下：')
            print(f"该学员的学号是{i['id']},姓名是{i['name']},手机号是{i['tel']}")
            break
    print('该学员不存在')


def print_all():
    """显示学员"""
    print('学号\t姓名\t手机号')
    for i in info:
        print(f"{i['id']}\t{i['name']}\t{i['tel']}")


info = []
while 1:
    print_info()
    user_num = int(input('请输入功能序号：'))
    if user_num == 1:
        add_info()
    elif user_num == 2:
        del_info()
    elif user_num == 3:
        modify_info()
    elif user_num == 4:
        search_info()
    elif user_num == 5:
        # pass pass可以用来占空行，来跳过还没完成的部分
        print_all()
    elif user_num == 6:
        exit_flag = input('确定要退出吗？yes or no\n')
        if exit_flag == 'yes':
            break
    else:
        print('输入错误，请重新输入')
