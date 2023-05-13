#pragma once
#include<iostream>
using namespace std;
template<class T>
class Queue                                       //队列模板类创建
{
public:
	virtual ~Queue() {}                           //虚析构函数
	virtual int size()const = 0;                  //求队列长度
	virtual void In_Queue(T& x) = 0;              //入队列
	virtual T Out_Queue() = 0;                    //出队列
	virtual bool Is_empty()const = 0;             //判断队列是否为空
};
class outOfRange :public exception                //自定义异常处理类
{
public:
	const char* what()const throw()
	{
		return "ERROR!OUT OF RANGE.\n";
	}
};
template<class T>
class Priority_Queue :public Queue<T>
{
private:
	int Queue_size;                               //队列中元素个数
	T* data;                                      //指向存放元素数组的指针
	int maxsize;                                  //队列数组的容量
	void siftDown(int parent);                    //从parent位置向下调整优先级队列
	void siftUp(int position);                    //从position的位置向上调整优先级队列
public:
	Priority_Queue()                              //构造函数
	{
		Queue_size=NULL;                               
		data=NULL;                                     
		maxsize = NULL;
	}
	~Priority_Queue() { delete[]data;}
	bool Is_empty()const { return Queue_size==0; }//队列判空
	int size()const { return Queue_size; }        //求队列长度
	void buildheap();                             //建堆
	void In_Queue(T& x);                          //入队列
	T Out_Queue();                                //出队列
};
template<class T>
void Priority_Queue<T>::buildheap()
{
	cin >> maxsize;                               //确定最大的队列
	Queue_size = maxsize;
	data = new T[maxsize+1];                      //建立数组
	for (int i = 0; i < Queue_size; i++)          //输入队列元素
	{
		cin >> data[i+1];
	}
	for (int i = Queue_size / 2; i > 0; i--)      //建堆
	{
		siftDown(i);
	}
}
template<class T>
void Priority_Queue<T>::In_Queue(T& x)            //入队列操作
{
	Queue_size++;                                 //队列内元素++
	data[Queue_size] = x;                         //新元素入队尾
	siftUp(Queue_size);                           //向上调整新元素位置
}
template<class T>
void Priority_Queue<T>::siftUp(int position)
{
	T temp = data[position];                      //保存position位置元素
	for (; position > 1 && temp < data[position / 2]; position = position / 2)  //如果该元素比其父节点值小则向上进行交换
	{
		data[position] = data[position / 2];                                    //交换两元素位置
		data[position] = temp;
	}
}
template<class T>
T Priority_Queue<T>::Out_Queue()
{
	if (Is_empty())                                       //若队列为空，出队列异常
	{
		throw outOfRange();
	}
	T min;
	min = data[1];                                        //保存最小元素
	data[1] = data[Queue_size];                           //队尾元素存入下标为1的位置
	Queue_size--;                                         
	siftDown(1);                                          //从队首向下调整
	return min;                                           //返回队首元素
}
template<class T>
void Priority_Queue<T>::siftDown(int parent)              //从parent开始向下调整
{
	int child;
	T temp = data[parent];                                //保存parent处结点
	for (; parent * 2 <= Queue_size; parent = child)
	{
		child = parent * 2;                               //child用于记录较小的子节点
		if (child != Queue_size && data[child + 1] < data[child])
		{
			child++;                                      //右孩子更小
		}
		if (data[child] < temp)                           //左右孩子有一个比父节点小
		{
			data[parent] = data[child];                   //进行交换
		}
		else                                              //若没有则停止
		{
			break;
		}
		data[child] = temp;
	}
}