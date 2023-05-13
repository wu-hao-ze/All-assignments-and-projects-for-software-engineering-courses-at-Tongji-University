#include"项目十_1953729_吴浩泽.h"
int main()
{
	int number = 0, choice = 0;
	long long int exchange_time = 0;
	double sort_time = 4;
	int* array1;                                          //动态数组
	int* array2;
	Sort sort;                                           //定义类成员sort
	srand((unsigned int)time(NULL));                                   //种随机种子
	sort.Initial_interface();                            //调用函数打印初始界面      
	array1 = new int[sort.Random_number];
	array2 = new int[sort.Random_number];
	for (int i = 0; i < sort.Random_number; i++)
	{
		array1[i] = rand();
		array2[i] = array1[i];
	}
	while (choice != 9)
	{
		cout << "请选择排序算法：";
		cin >> choice;
		switch (choice)
		{
			case 1:
			{
				cout << endl;
				exchange_time = sort.Bubble_sort(array1,sort_time);
				cout << "冒泡排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "冒泡排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 2:
			{
				cout << endl;
				exchange_time = sort.Selection_sort(array1, sort_time);
				cout << "选择排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "选择排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 3:
			{
				cout << endl;
				exchange_time = sort.Direct_insertion_sort(array1,sort_time);
				cout << "直接插入排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "直接插入排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 4:
			{
				cout << endl;
				exchange_time = sort.Shell_sort(array1, sort_time);
				cout << "希尔排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "希尔排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 5:
			{
				cout << endl;
				exchange_time = sort.Quick_sort(array2, sort_time,0, sort.Random_number-1);
				cout << "快速排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "快速排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 6:
			{
				cout << endl;
				exchange_time = sort.Heap_sort(array1, sort_time);
				cout << "堆排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "堆排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 7:
			{
				cout << endl;
				exchange_time = sort.Merge_sort(array1, sort_time);
				cout << "归并排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "归并排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 8:
			{
				cout << endl;
				exchange_time = sort.Radix_sort(array1, sort_time);
				cout << "基数排序所用时间：" << endl;
				cout << sort_time << endl;
				cout << "基数排序交换次数：" << endl;
				cout << exchange_time << endl;
				cout << endl;
				break;
			}
			case 9:
			{
				system("pause");
				return 0;
			}
			default:
			{
				cout << "输入数据不合法,请重新输入：" << endl;
			}
		}
	}
}

