#include"项目四_1953729_吴浩泽.h"
using namespace std;
int main()
{
	int number;
	char choice='y';
	char eatendl;
	calculator s;
	while (choice == 'y')
	{
		cout << "是否开始计算(y,n)?";
		choice=getchar();                                 //吞取y/n
		eatendl = getchar();                              //吞取y后的回车
		if (choice == 'y')
		{
			cout << "请输入表达式" << endl;
			number = s.calculator_number();
			cout << "最终结果为：" << number << endl;
		}
		else
		{
			return 0;

		}
	}
}