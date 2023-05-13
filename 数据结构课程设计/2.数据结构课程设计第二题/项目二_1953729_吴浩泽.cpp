#include"项目二_1953729_吴浩泽.h"
using namespace std;
int main()
{
	cout << "请输入两个正整数链表序列，非降序排列，以-1表示序列结束" << endl;	
	s1.Creat_Link_list();                                                         //调用函数创建s1
	s2.Creat_Link_list();                                                         //调用函数创建s2
	s3.Intersection(s1, s2);                                                      //求s1，s2交集
	return 0;
}
