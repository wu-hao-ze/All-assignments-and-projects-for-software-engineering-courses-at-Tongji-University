from student import *


class StudentManager(object):
    def __init__(self):
        self.student_list = []

    # 程序入口函数，自动程序后执行的函数
    def run(self):
        # 加载学员信息
        self.load_student()
        while 1:
            self.show_menu()
            menu_num = input('请输入功能序号：')
            if menu_num == '1':
                self.add_student()
            elif menu_num == '2':
                self.del_student()
            elif menu_num == '3':
                self.modify_student()
            elif menu_num == '4':
                self.search_student()
            elif menu_num == '5':
                self.show_student()
            elif menu_num == '6':
                self.save_student()
            elif menu_num == '7':
                break
            else:
                print('输入错误，请重新输入')

    def load_student(self):
        # 尝试以只读方式打开，如果不行，就新建文件
        try:
            f = open('student.data', 'r')
        except:
            f = open('student.data', 'w')
        else:
            data = f.read()

            # 文件中读取的数据都是字符串且字符串内部都是字典类型数据，则转换成应该的数据类型之后再转换成对象存储到列表
            new_list = eval(data)
            self.student_list = [Student(i['name'], i['gender'], i['tel']) for i in new_list]
        finally:
            f.close()

    @staticmethod
    def show_menu():
        print('请选择如下功能：')
        print('1.添加学员')
        print('2.删除学员')
        print('3.修改学员信息')
        print('4.查询学员信息')
        print('5.显示学员信息')
        print('6.保存学员信息')
        print('7.退出系统')

    def add_student(self):
        name = input('请输入姓名：')
        gender = input('请输入性别：')
        tel = input('请输入手机号：')
        student = Student(name, gender, tel)
        self.student_list.append(student)

    def del_student(self):
        del_name = input('请输入要删除的学员姓名：')
        for i in self.student_list:
            if i.name == del_name:
                self.student_list.remove(i)
                break
        else:
            print('查无此人')

    def modify_student(self):
        modify_name = input('请输入要修改的学员姓名：')
        for i in self.student_list:
            if i.name == modify_name:
                i.name = input('请输入姓名：')
                i.gender = input('请输入性别：')
                i.tel = input('请输入手机号：')
                break
        else:
            print('查无此人')

    def search_student(self):
        search_name = input('请输入要查询的学员姓名：')
        for i in self.student_list:
            if i.name == search_name:
                print(f'姓名{i.name},性别{i.gender},手机号{i.tel}')
                break
        else:
            print('查无此人')

    def show_student(self):
        print('姓名\t性别\t手机号')
        for i in self.student_list:
            # print(f'{i.name}\t{i.gender}\t{i.tel}')
            print('%-4s\t%-4s\t%s' % (i.name, i.gender, i.tel))

    def save_student(self):
        f = open('student.data', 'w')
        # student_list列表中的元素是一个个的对象，先转换成列表中都是字典，把字典写入文件中
        # 利用dict属性直接获取的是对象属性，也就是name，gender，tel，推导式写法更加快捷
        new_list = [i.__dict__ for i in self.student_list]

        # 注意文件写入的时候写入的是字符串类型，所以要进行转换
        f.write(str(new_list))
        f.close()
