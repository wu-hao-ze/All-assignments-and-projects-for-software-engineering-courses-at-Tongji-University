#include"项目一_1953729_吴浩泽.h"
using namespace std;
int main()
{
	int n, number, choice = 0;
	List member;
	cout << "首先请建立考生系统！" << endl;
	cout << "请输入考生人数：" << endl;
	cin >> n;
	member.CreateLink_list(n);                                  //建立考生系统
	while (1)
	{
		cout << '\n' << "请输入你要进行的操作:";
		cin >> choice;
		switch (choice)
		{
		case 1:
		{
			cout << "请依次输入你要插入考生的考号，姓名，性别，年龄以及报考种类" << endl;
			member.Insert();
			break;
		}
		case 2:
		{
			cout << "请输入你要删除的考生考号：";
			cin >> number;
			if (number <= 0)                               //考虑学号不能为空的情况
			{
				cout << "考号不能为此单元" << endl;
			}
			else
			{
				member.Remove(number);                        //调用函数进行删除
			}
			break;
		}
		case 3:
		{
			cout << "请输入要查找考生的考号：";
			cin >> number;
			if (number<=0)                               //考虑学号不能为空的情况
			{
				cout << "考号不能为此单元" << endl;
			}
			else
			{
				member.Research(number);                      //调用函数进行搜索
			}
			break;
		}
		case 4:
		{
			cout << "请输入要修改考生的考号：";
			cin >> number;
			if (number <=0)                               //考虑学号不能为空的情况
			{
				cout << "考号不能为此单元" << endl;
			}
			else
			{
				member.Modify(number);                      //调用函数进行搜索
			}
			break;
		}
		case 0:
		{
			cout << "操作结束，完结撒花！！！" << endl;
			return 0;
		}
		default:
			cout << "无此操作，请重新输入：" << endl;
		}
	}
}