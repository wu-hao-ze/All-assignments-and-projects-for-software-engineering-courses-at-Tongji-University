#pragma once
#include<iostream>
using namespace std;
template<class T>
class Queue                                       //队列模板类创建
{
public:
	virtual void clear() = 0;                     //将队列清空
	virtual int size()const = 0;                  //求队列长度
	virtual void In_Queue(T& x) = 0;              //入队列
	virtual T Out_Queue() = 0;                    //出队列
	virtual T Read_Queue()const = 0;              //读取队头元素
	virtual bool Is_empty()const = 0;             //判断队列是否为空
};
template<class T>
class Link_Queue :public Queue<T>
{
public:
	Link_Queue() { front = NULL; rear = NULL; }   //构造函数将队头，队尾指针置空
	~Link_Queue() { clear(); }                    //析构函数清除队列
	void clear();                                 //将队列清空
	int size()const;                              //求队列长度
	void In_Queue(T& x);                          //入队列
	T Out_Queue();                                //出队列
	T Read_Queue()const;                          //读取队头元素
	bool Is_empty()const;                         //判断队列是否为空
private:
	struct Link_node                              //队列结点，包含数据以及指针
	{
		T data;
		Link_node* next;
	};
	Link_node* rear, * front;                     //队尾，队头指针
};
template<class T>
void Link_Queue<T>::clear()                      //清空队列
{
	Link_node* p;
	while (front != NULL)                        //当队列头指针不为空（即队列不为空时）进行头指针后移逐个删除队列元素
	{
		p = front;
		front = front->next;
		delete p;
	}
	rear = NULL;                                //将尾指针置空
}
template<class T>
int Link_Queue<T>::size()const
{
	Link_node* p = front;
	int count = 0;
	while (p != NULL)                           //当头指针不为空时依次后移。count++，进行计数
	{
		count++;
		p = p->next;
	}
	return count;
}
template<class T>
void Link_Queue<T>::In_Queue(T& x)
{
	Link_node* p = new Link_node;
	if (p == NULL)
	{
		cout << "存储分配错误！" << endl;      //检查开辟新结点是否成功
		exit(1);
	}
	if (rear == NULL)                          //如果此队列中无元素则进行插入第一个元素操作
	{
		p->data = x;
		rear = p;
		front = p;
		rear->next = NULL;
	}
	else                                       //如果队列中有元素则进行在队尾插入操作
	{
		p->data = x;
		rear->next = p;
		rear = p;
		rear->next = NULL;
	}
}
template<class T>
T Link_Queue<T>::Out_Queue()
{
	Link_node* p=front;
	T x;
	while (front != NULL)                         //判断队列中是否有元素
	{
		if (p == NULL)
		{
			cout << "存储分配错误！" << endl;    //判断建立结点是否成功
			exit(1);
		}
		else                                     //如果成功返回头指针数据值，并将头指针后移
		{
			p = front;
			x = p->data;
			front = front->next;
			if (front == NULL)                   //原来只有一个元素出队后队列为空
			{
				rear == NULL;                    //修改队尾指针
			}
			delete p;
			return x;
		}
	}
}
template<class T>
T Link_Queue<T>::Read_Queue()const
{
	if (front != NULL)                                //如果队列中有元素，返回队头元素的值
	{
		return front->data;
	}
	else
	{
		cout << "队列中无元素，不可读取队列首元" << endl;
	}
}
template<class T>
bool Link_Queue<T>::Is_empty()const
{
	if (rear == NULL||front==NULL)       //头指针或者尾指针为空则队列为空
	{
		return true;
	}
	else
	{
		return false;
	}
}
class Compare_Queue                            //比较队列类
{
private:
	Link_Queue<int>Queue_A;                    //定义三个int型队列
	Link_Queue<int>Queue_B;
	Link_Queue<int>Queue_C;
public:
	Compare_Queue() {};                        //构造函数
	~Compare_Queue() {};                       //析构函数
	void Handling();                           //办理业务实现函数
};
void Compare_Queue::Handling()
{
	int sum,number=0,data;                     //总的排队人数sum， 不同人的编号number，用于暂时存储数据
	int A_size,B_size,C_size;                  //记录A,B,C队列的长度
	cin >> sum;
	for (int i = 0; i < sum; i++)              //循环输入总人数以及人员编号
	{
		cin >> number;
		if ((number % 2) == 0)                 //人员编号为偶数进入B队列
		{
			Queue_B.In_Queue(number);
		}
		else                                   //人员编号为奇数进入A队列
		{
			Queue_A.In_Queue(number);
		}
	}
	while ((Queue_A.size() >= 2) && (Queue_B.Is_empty() == false))    //当A队列还有至少两个元素，且B队列不为空时
	{
		data = Queue_A.Out_Queue();                                   //输出A队列中的两个元素，B队列中的一个元素
		Queue_C.In_Queue(data);
		data = Queue_A.Out_Queue();
		Queue_C.In_Queue(data);
		data = Queue_B.Out_Queue();
		Queue_C.In_Queue(data);
	}
	if ((Queue_A.size() < 2) && (Queue_B.Is_empty() == false))        //当A队列中元素少于2，B队列不为空时
	{
		B_size = Queue_B.size();
		A_size = Queue_A.size();
		for (int i = 0; i < A_size; i++)                              //先将A队列元素全部输出，再将B队列元素全部输出
		{
			data = Queue_A.Out_Queue();
			Queue_C.In_Queue(data);
		}
		for (int i = 0; i < B_size; i++)
		{
			data = Queue_B.Out_Queue();
			Queue_C.In_Queue(data);
		}
	}
	if (Queue_B.Is_empty() == true)                                   //如果B队列中元素为空
	{
		A_size = Queue_A.size();
		for (int i = 0; i < A_size; i++)                              //将A中元素全部放入C中
		{
			data = Queue_A.Out_Queue();
			Queue_C.In_Queue(data);
		}
	}
	if (Queue_C.Is_empty() == false)                                  //如果C中有元素
	{
		C_size = Queue_C.size();
		for (int i = 0; i < C_size; i++)                              //将C中元素全部输出
		{
			data=Queue_C.Out_Queue();
			cout << data;
			if (C_size - 1 != i)                                      //保证C输出元素最后一个后无括号
			{
				cout << " ";
			}
		}
	}
	else                                                              //C中无元素
	{
		cout << "无人在排队" << endl;
	}
}