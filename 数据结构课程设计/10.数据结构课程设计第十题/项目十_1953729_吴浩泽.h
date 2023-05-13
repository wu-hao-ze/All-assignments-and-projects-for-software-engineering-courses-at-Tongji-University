#pragma once
#include<iostream>
#include<ctime>
using namespace std;
class Sort 
{
public:
	Sort()                                                                              //构造与析构函数
	{
		Random_number = 0;
	}               
	~Sort(){}
	void Initial_interface();                                                           //初始化界面
	long long int Bubble_sort(int* array1,double& sort_time);                           //声明冒泡排序函数
	long long int Selection_sort(int* array1, double& sort_time);                       //声明选择排序函数
	long long int Direct_insertion_sort(int* array1,double& sort_time);                 //声明直接插入排序函数
	long long int Shell_sort(int* array1, double& sort_time);                           //声明希尔排序函数
	long long int Quick_sort(int* array1, double& sort_time,int l,int r);               //声明快速排序函数
	long long int Merge_sort(int* array1, double& sort_time);                           //声明归并排序函数
	int Radix_sort(int* array1, double& sort_time);                                     //声明基数排序函数
	long long int Heap_sort(int* array1, double& sort_time);                            //声明堆排序函数
	long long int siftDown(int parent, int* data, int number);
	int maxbit(int* array, int n);
	int Random_number;                               //输入随机数的个数
};
void Sort::Initial_interface()
{
	cout << "**                 排序算法比较                    **" << endl;
	cout << "=====================================================" << endl;
	cout << "**                 1.冒泡排序                      **" << endl;
	cout << "**                 2.选择排序                      **" << endl;
	cout << "**                 3.直接插入排序                  **" << endl;
	cout << "**                 4.希尔排序                      **" << endl;
	cout << "**                 5.快速排序                      **" << endl;
	cout << "**                 6.堆排序                        **" << endl;
	cout << "**                 7.归并排序                      **" << endl;
	cout << "**                 8.基数排序                      **" << endl;
	cout << "**                 9.退出程序                      **" << endl;
	cout << "=====================================================" << endl;
	cout << endl;
	cout << "请输入随机数的数量:" << endl;
	cin >> Random_number;
}
long long int Sort::Bubble_sort(int* array1,double&sort_time)
{
	int i, j, temp;                                                     //i,j循环变量，temp存储待排序元素
	long long int  exchange_number = 0;                                 //exchange_number为交换次数
	int* array;
	clock_t start, stop;
	array = new int[Random_number];
	for (i = 0; i < Random_number; i++)
	{
		array[i] = array1[i];
	}
	start = clock();
	for (i = 1; i < Random_number ; i++)                                //进行n-1趟比较
	{
		for (j = Random_number-1; j > i - 1; j--)                       //两两比较依次上浮
		{
			if (array[j] < array[j - 1])                                //进行交换
			{
				temp = array[j];
				array[j] = array[j - 1];
				array[j - 1] = temp;
				exchange_number++;
			}
		}
	}
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::Selection_sort(int* array1, double& sort_time)
{
	int i, j, m, temp;                                            //i,j循环变量，temp存储待排序元素
	long long int exchange_number = 0;                            //exchange_number为交换次数
	int* array;
	clock_t start, stop;
	array = new int[Random_number];
	for (i = 0; i < Random_number; i++)
	{
		array[i] = array1[i];
	}
	start = clock();
	for (i = 0; i < Random_number-1; i++)                         //寻找最小元素依次往前放
	{
		m = i;
		for (j = i + 1; j < Random_number; j++)
		{
			if (array[j] < array[m])
			{
				m = j;
				if (m != i)                                       //有比该位置小的元素，进行交换
				{
					temp = array[i];
					array[i] = array[m];
					array[m] = temp;
					exchange_number++;
				}
			}
		}
	}
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::Direct_insertion_sort(int* array1,double& sort_time)
{
	int i, j, temp;
	long long int exchange_number = 0;           //i,j循环变量，temp存储待排序元素,exchange_number为交换次数
	int* array;
	clock_t start, stop;                         //记录时间
	array = new int[Random_number];
	for (i = 0; i < Random_number; i++)
	{
		array[i] = array1[i];
	}
	start = clock();
	for (i = 1; i < Random_number; i++) 
	{
		j = i;
		temp = array[i];                         //待排序元素赋值给临时变量
		while (j > 0 && temp < array[j - 1]) 
		{                                        //当未达到数组的第一个元素或者待插入元素小于当前元素
			array[j] = array[j - 1];             //就将该元素后移
			j--;                                 //下标减一，继续比较
			exchange_number++;                   //交换次数加一
		}
		array[j] = temp;                         //插入位置已经找到，立即插入
	}
	stop = clock();
	sort_time= ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::Shell_sort(int* array1, double& sort_time)
{
	int i, j, temp;
	long long int exchange_number = 0;                      //i,j循环变量，temp存储待排序元素,exchange_number为交换次数
	int gap=0;
	int* array;
	clock_t start, stop;                                                       //记录时间
	array = new int[Random_number];
	for (i = 0; i < Random_number; i++)
	{
		array[i] = array1[i];
	}
	gap = Random_number;                                                       //分组间隔
	start = clock();
	do {	                                                                   //每次减小增量，直到increment = 1
		gap = gap / 3 + 1;
		for (int i = gap; i < Random_number; i++) 
		{	                                                                   //对每个划分进行直接插入排序
			if (array[i - gap] > array[i]) 
			{
				temp = array[i];
				j = i - gap;
				do {	                                                       //移动元素并寻找位置
					array[j + gap] = array[j];
					j = j-gap;
				} while (j >= 0 && array[j] > temp);
				array[j + gap] = temp;	                                       //插入元素
				exchange_number++;
			}
		}
	} while (gap > 1);
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::Quick_sort(int* array2, double& sort_time,int l,int r)
{
	long long int exchange_number = 0;                     //exchange_number为交换次数
	clock_t start, stop;                         //记录时间
	start = clock();
	if (l < r)
	{
		int i = l, j = r, x = array2[l];
		while (i < j)
		{
			while (i < j && array2[j] >= x)        // 从右向左找第一个小于x的数
			{
				j--;
			}
			if (i < j)
			{
				array2[i++] = array2[j];
				exchange_number++;
			}
			while (i < j && array2[i] < x)         // 从左向右找第一个大于等于x的数
			{
				i++;
			}
			if (i < j)
			{
				array2[j--] = array2[i];
				exchange_number++;
			}
		}
		array2[i] = x;
		exchange_number= exchange_number+Quick_sort(array2, sort_time,l, i - 1); // 递归调用 
		exchange_number= exchange_number+Quick_sort(array2, sort_time, i + 1, r);
	}
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::Heap_sort(int* array1, double& sort_time)
{
	int i;           //i,j循环变量，temp存储待排序元素
	long long int exchange_number = 0;//exchange_number为交换次数
	int* array;
	int temp = Random_number;
	clock_t start, stop;                         //记录时间
	array = new int[Random_number +1];
	for (i = 0; i < Random_number; i++)
	{
		array[i+1] = array1[i];
	}
	start = clock();
	for (int i = Random_number / 2; i > 0; i--)      //建堆
	{
		exchange_number=siftDown(i,array, Random_number);
	}
	for (i = 0; i < Random_number; i++)
	{
		//cout << array[1]<<" ";                                        //保存最小元素
		array[1] = array[temp];                                //队尾元素存入下标为1的位置
		temp--;
		exchange_number= exchange_number+siftDown(1, array, temp);                                //从队首向下调整
	}
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::Merge_sort(int* array1, double& sort_time)
{
	int i;                                        //i,j循环变量，temp存储待排序元素,
	long long int exchange_number = 0;            //exchange_number为交换次数
	int* array;
	clock_t start, stop;                         //记录时间
	array = new int[Random_number];
	for (i = 0; i < Random_number; i++)
	{
		array[i] = array1[i];
	}
	start = clock();
	int* a = array;
	int* b = new int[Random_number];
	for (int seg = 1; seg < Random_number; seg += seg) {
		for (int start = 0; start < Random_number; start += seg + seg) {
			int low = start, mid = (start + seg)< Random_number ? (start + seg): Random_number, high = (start + seg + seg)< Random_number ? (start + seg + seg): Random_number;
			int k = low;
			int start1 = low, end1 = mid;
			int start2 = mid, end2 = high;
			while (start1 < end1 && start2 < end2)
			{
				b[k++] = a[start1] < a[start2] ? a[start1++] : a[start2++];
				exchange_number++;
			}
			while (start1 < end1)
				b[k++] = a[start1++];
			while (start2 < end2)
				b[k++] = a[start2++];
		}
		int* temp = a;
		a = b;
		b = temp;
	}
	if (a != array) {
		for (int i = 0; i < Random_number; i++)
			b[i] = a[i];
		b = a;
	}
	delete[] b;
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
int Sort::Radix_sort(int* array1, double& sort_time)
{
	int i, j,k,exchange_number = 0;            //i,j循环变量，temp存储待排序元素,exchange_number为交换次数
	int* array;
	clock_t start, stop;                         //记录时间
	array = new int[Random_number];
	for (i = 0; i < Random_number; i++)
	{
		array[i] = array1[i];
	}
	start = clock();
	int d = maxbit(array, Random_number);
	int* temp = new int[Random_number];
	int* count = new int[10]; //计数器
	int radix = 1;
	for (i = 1; i <= d; i++) //进行d次排序
	{
		for (j = 0; j < 10; j++)
			count[j] = 0; //每次分配前清空计数器
		for (j = 0; j < Random_number; j++)
		{
			k = (array[j] / radix) % 10; //统计每个桶中的记录数
			count[k]++;
		}
		for (j = 1; j < 10; j++)
			count[j] = count[j - 1] + count[j]; //将tmp中的位置依次分配给每个桶
		for (j = Random_number - 1; j >= 0; j--) //将所有桶中记录依次收集到tmp中
		{
			k = (array[j] / radix) % 10;
			temp[count[k] - 1] = array[j];
			count[k]--;
		}
		for (j = 0; j < Random_number; j++) //将临时数组的内容复制到data中
			array[j] = temp[j];
		radix = radix * 10;
	}
	delete[]temp;
	delete[]count;
	stop = clock();
	sort_time = ((double)(stop - start)) / CLK_TCK;
	return exchange_number;
}
long long int Sort::siftDown(int parent ,int *data,int number)              //从parent开始向下调整
{
	int child;
	long long int exchange_time=0;
	int temp = data[parent];                                //保存parent处结点
	for (; parent * 2 <= number; parent = child)
	{
		child = parent * 2;                               //child用于记录较小的子节点
		if (child != number && data[child + 1] < data[child])
		{
			child++;                                      //右孩子更小
		}
		if (data[child] < temp)                           //左右孩子有一个比父节点小
		{
			data[parent] = data[child];                   //进行交换
			exchange_time++;
		}
		else                                              //若没有则停止
		{
			break;
		}
		data[child] = temp;
	}
	return exchange_time;
}
int Sort::maxbit(int* array, int n) //辅助函数，求数据的最大位数
{
	int maxData = array[0];              ///< 最大数
	/// 先求出最大数，再求其位数，这样有原先依次每个数判断其位数，稍微优化点。
	for (int i = 1; i < n; ++i)
	{
		if (maxData < array[i])
			maxData = array[i];
	}
	int d = 1;
	int p = 10;
	while (maxData >= p)
	{
		maxData /= 10;
		++d;
	}
	return d;
}
