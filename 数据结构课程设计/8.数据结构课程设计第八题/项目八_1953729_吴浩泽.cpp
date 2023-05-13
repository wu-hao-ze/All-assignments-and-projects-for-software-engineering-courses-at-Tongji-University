#include"项目八_1953729_吴浩泽.h"
using namespace std;
int main()
{
	cout << endl;
	cout << "**                      电网造价模拟系统                    **" << endl;
	cout << "==============================================================" << endl;
	cout << "**                   请选择要执行的操作:                    **" << endl;
	cout << "**                      A---创建电网顶点                    **" << endl;
	cout << "**                      B---添加电网的边                    **" << endl;
	cout << "**                      C---构造最小生成树                  **" << endl;
	cout << "**                      D---显示最小生成树                  **" << endl;
	cout << "**                      E---退出程序                        **" << endl;
	cout << "==============================================================" << endl;
	Electric_Frence L;
	Linked_list edge;
	char ch = ' ';
	while (ch != 'E')
	{
		cout << '\n' << "请选择操作：";
		cin >> ch;
		switch (ch)
		{
		case'A':
		{
			L.Creat_Pinnacle(L);
			break;
		}
		case'B':
		{
			L.Insert_path(L);
			break;
		}
		case'C':
		{
			cout << "请输入起始顶点：";
			string name;
			cin >> name;
			L.Min_Tree(L, edge, name);
			cout << "生成Prim最小生成树！\n";
			break;
		}
		case'D':
		{
			cout << "最小生成树的顶点及边为：" << endl;
			cout << endl;
			edge.Print(edge.next_name);
			cout << endl;
			break;
		}
		case'E':break;
		default:
		{
			cout << "输入错误，请输入正确的操作：" << endl;
			break; 
	 	}
		}
	}
	system("pause");
	return 0;
}