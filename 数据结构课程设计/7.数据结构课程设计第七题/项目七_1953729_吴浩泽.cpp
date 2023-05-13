#include"项目七_1953729_吴浩泽.h"
using namespace std;
int main()
{
	int x, y,a,sum=0;
	Priority_Queue<int>minnumber;            //int型的最小堆
	minnumber.buildheap();                   //建立最小堆
	a=minnumber.size();                      //元素的个数
	for (int i = 1; i < a-1; i++)            //进行循环
	{
		x = minnumber.Out_Queue();
		y = minnumber.Out_Queue();
		x = x + y;
		sum = sum + x;                       //前a-2次循环循环到最小堆中只有两个元素
		minnumber.In_Queue(x);
	}
	x = minnumber.Out_Queue();               //最后一次进行最后两个元素的相加
	y = minnumber.Out_Queue();
	sum = sum + x + y;
	cout << sum;                             //输出最后结果
	return 0;
}