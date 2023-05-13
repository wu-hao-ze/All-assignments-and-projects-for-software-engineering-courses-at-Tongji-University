#pragma once
#include<iostream>
using namespace std;
class Link_list;
class Link_node                                                  //定义节点类，类中包含数据成员data以及指针域成员next
{
	friend class Link_list;
private:
	int data;
	Link_node* next;
};
class Link_list                                                 //定义链表类，包含链表的创立、比较和输出操作
{
public:
	Link_list();                                                //构造函数
	~Link_list() {};							                //析构函数
	int Creat_Link_list();                                      //建立链表
	void Intersection(Link_list s1,Link_list s2);               //求交集函数
	void Display_Link_list(Link_node* head);                    //输出函数
private: 
	int size;                                                   //记录链表的大小
	Link_node* head;                                            //链表的头结点
}s1,s2,s3;
Link_list::Link_list()                                          //构造函数定义
{
	head = new Link_node;                                       //初始化头结点
	size = 0;
	head->data = 0;
	head->next = NULL;
}                                 
int Link_list::Creat_Link_list()                                //建立构造链表函数
{
	Link_node* ptemp;                           //定义当前结点
	ptemp = head;                                               //默认当前结点为头结点
	while (1)
	{
		Link_node* pnew = new Link_node;                        //新定义一个新节点，储存数值
		if (pnew == NULL)
		{
			cout << "存储分配错误！" << endl;
			exit(1);
		}
		cin >> pnew->data;
		if (pnew->data > 0)                                     //当输入数据有效时将其储存在链表中
		{
			pnew->next = NULL;
			ptemp->next = pnew;
			ptemp = pnew;
			size++;                                             //链表数据加一
		}
		else                                                    //如果输入数据无效则清除新结点
		{
			delete pnew;                                        
			return 1;
		}
	}
}
void Link_list::Intersection(Link_list s1,Link_list s2)         //求s1以及s2链表序列交集
{
	if (s1.size == 0 || s2.size == 0)                           //如果s1或s2序列中有空则输出NULL
	{
		cout << "NULL" << endl;
	}
	else                                                        //两序列均不为空时生成交集s3
	{
		Link_node* ptemp1 ;                                     //建立历遍s1的新结点ptemp1
		Link_node* ptemp2 ;                                     //建立历遍s2的新结点ptemp2
		Link_node* ptemp3 ;                                     //s3的当前结点
		ptemp1 = s1.head->next;
		ptemp2 = s2.head->next;
		ptemp3 = s3.head;
		while (s1.size!=0 && s2.size != 0)                      //当s1，s2中数据还未完时继续
		{
			if (ptemp1->data == ptemp2->data)                   //如果数据相等则扔进s3中，并且将ptemp2向后移动一次
			{
				s3.size++;
				Link_node* pnew = new Link_node;
				if (pnew == NULL)
				{
					cout << "存储分配错误！" << endl;
					exit(1);
				}
				pnew->data = ptemp1->data;
				pnew->next = ptemp3->next;
				ptemp3->next = pnew;
				ptemp3 = pnew;
				ptemp2 = ptemp2->next;
				s2.size--;
			}
			else
			{
				if (ptemp1->data < ptemp2->data)                //如果s1中当前数据小则移动ptemp1
				{
					ptemp1 = ptemp1->next;
					s1.size--;
				}
				else
				{
					ptemp2 = ptemp2->next;                      //如果s2中当前数据小则移动ptemp2
					s2.size--;
				}
			}
		}
		if (s3.head->next == NULL)                              //如果s3中无数据输出NULL
		{
			cout << "NULL";
		}
		else
		{
			Display_Link_list(s3.head);                         //有数据则调用输出函数
		}
	}

}
void Link_list::Display_Link_list(Link_node* head)              //进行链表的输出
{
	Link_node* ptemp;                           
	ptemp=head->next;                                           //从头开始进行输出
	while (s3.size>0)
	{
		cout << ptemp->data<<" ";
		ptemp = ptemp->next;
		s3.size--;
	}
}
