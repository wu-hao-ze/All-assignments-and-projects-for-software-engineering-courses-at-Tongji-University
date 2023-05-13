#include"项目六_1953729_吴浩泽.h"
using namespace std;
int main()
{
	cout << endl;
	cout << "**                      家谱管理系统                        **" << endl;
	cout << "==============================================================" << endl;
	cout << "**                   请选择要执行的操作:                    **" << endl;
	cout << "**                      A---完善家谱                        **" << endl;
	cout << "**                      B---添加家庭成员                    **" << endl;
	cout << "**                      C---解散局部家庭                    **" << endl;
	cout << "**                      D---更改家庭成员姓名                **" << endl;
	cout << "**                      E---退出程序                        **" << endl;
	cout << "==============================================================" << endl;
	cout << "首先建立一个家谱！" << endl;
	Family_Tree L;
	L.Creat(L);                                       //创建家谱
	char ch=NULL;
	while (ch != 'E')
	{
		cout << '\n' << "请选择要执行的操作：";
		cin >> ch;
		switch (ch)
		{
		case'A':
		{
			L.Add(L);
			break;
		}
		case'B':
		{
			L.Insert(L);
			break;
		}
		case'C':
		{
			L.Delete(L);
			break;
		}
		case'D':
		{
			L.Change_name(L);
			break;
		}
		case'E':
		{
			break;
		}
		default:
			cout << "请输入正确的操作！" << endl;
		}
	}
	return 0;
}