#pragma once
#include<iostream>
#include<string>
using namespace std;
class List;
class Link_node                            //创建结点类，包含节点中的数据域以及指针域
{
	friend class List;                     //以List类为友元类
private:
	int tnumber;
	string name;
	string gender;
	int age;
    string post;
	Link_node* next;
};
class List                                 //创建链表类，进行单链表插入删除等操作
{
public:
	List();   
	~List() {};
    int CreateLink_list(int n);            //创建最初的考生信息
	void Print(Link_node*head);            //打印考生信息    
	int Research(int a);                   //在表中搜索考生信息
	int Insert();                          //在表中插入考生信息
	int Remove(int a);                     //在表中删除考生信息
	int Modify(int a);                     //在表中修改已有考生信息
private:
	Link_node* head;
	int size;
};
List::List()                               //初始化头指针，为创建一个带头结点的链表铺垫
{
	head = new Link_node;
	head->tnumber = NULL;
	head->name =' ';
	head->gender = ' ';
	head->age = NULL;
	head->post = ' ';
	head->next = NULL;
}
int List::CreateLink_list(int n)           //创建初始考生信息
{
	if (n <= 0)                            //如果链表有效节点数不大于0，则创建失败
	{
		cout << "error";
		return 0;
	}
	else                                   //构建学生信息单链表
	{
		cout <<"请按照考号升序依次输入考生的考号，姓名，性别，年龄，以及报考类别:" << endl;
		size = n;                          //学生人数
		Link_node* pnew;                   //定义两个节点一个为当前节点一个为新建结点
		Link_node* ptemp;                  
		ptemp = head;                      //初始时当前节点在头结点的位置
		for(int i = 1; i <= n;i++ )
		{
			pnew = new Link_node;          //创建新节点
			if (pnew == NULL)
			{
				cout << "存储分配错误！" << endl;
				exit(1);
			}
			                               //为新结点赋值
			cin >> pnew->tnumber >> pnew->name >> pnew->gender >> pnew->age >> pnew->post;
			pnew->next = NULL;             //默认新结点为最后一个节点
			ptemp->next = pnew;            //将新结点链入链表
			ptemp = pnew;                  //将当前节点变为新创建的结点
		}
		pnew = new Link_node;              //以下操作为在链表最后加入一无数据尾结点，方便后续插入和删除操作
		if (pnew == NULL)
		{
			cout << "存储分配错误！" << endl;
			exit(1);
		}
		pnew->tnumber = NULL;
		pnew->name = ' ';
		pnew->gender = ' ';
		pnew->age = NULL;
		pnew->post = ' ';
		pnew->next = NULL;
		ptemp->next = pnew;
		ptemp = pnew;
		Print(head);
		cout << "请选择你要进行的操作（1为插入，2为删除，3为查找，4为修改，0为取消操作）" << endl;
		return 1;
	}

}
void List::Print(Link_node*head)           //创建打印函数，打印考生信息
{
	int N=size;                            
	Link_node* ptemp;                      //定义当前节点
	ptemp = head->next;                    //跳过头结点，从有学生信息的地方开始打印
	cout << '\n' << endl;
	while (ptemp->next!= NULL && N >0)     //历遍链表进行打印（只打印有效数据）
	{
		cout << ptemp->tnumber << "   " << ptemp->name << "    " << ptemp->gender << "       " << ptemp->age << "        " << ptemp->post << endl;
		ptemp = ptemp->next;
		N--;
	}
	cout << "考生人数为：" << size<<endl;
}
int List::Insert()
{
	int N = size;
	Link_node* ptemp1 = head;                     //定义两个新结点一个指向头结点，一个指向有效数据首元，历遍链表
	Link_node* ptemp2 = head->next;
	Link_node* pnew = new Link_node;              //定义新结点存储要插入的值
	if (pnew == NULL)
	{
		cout << "存储分配错误！" << endl;
		exit(1);
	}
	cin >> pnew->tnumber >> pnew->name >> pnew->gender >> pnew->age >> pnew->post;
	if (pnew->tnumber <=0)                    //判定输入学号不能为空
	{
		cout << "考号不能为此单元" << endl;
		cout << "请重新输入你要插入考生的考号，姓名，性别，年龄以及报考种类" << endl;
		Insert();
		return 0;
	}
	else                                          //学号不为空进行寻找位置插入操作
	{
		while (N > 0)
		{
			if (pnew->tnumber == ptemp2->tnumber) //如果考生考号相同重新输入
			{
				cout << "考号不能重复" << endl;
				cout << "请重新输入你要插入考生的考号，姓名，性别，年龄以及报考种类" << endl;
				Insert();
				return 0;
			}
			if (pnew->tnumber < ptemp2->tnumber)  //如果找到该位置比要插入的考号大则进行插入操作
			{
				pnew->next = ptemp2;
				ptemp1->next = pnew;
				size++;
				Print(head);
				return 1;                         //插入完毕
			}
			else
			{
				N--;
				if (N == 0)                           //重新加入者考号最大插到最后
				{
					pnew->next = ptemp2->next;
					ptemp2->next = pnew;
					size++;
					Print(head);
					return 0;
				}
				ptemp1 = ptemp2;                      //若当前位置不满足要求则往后移动
				ptemp2 = ptemp2->next;
			}
			
		}
		return 0;
	}
}
int List::Remove(int a)                                   //删除学生数据函数
{
	int N = size;
	Link_node* ptemp1 = head;                             //定义当前位置和当前位置的前一个位置
	Link_node* ptemp2 = head->next;
	while (N > 0)                                         //历遍单链表寻找学号相符合同学
	{
		if (ptemp2->tnumber == a)                         //若找到相符合的进行操作
		{
			size--;
			cout << "你删除的考生信息为：" << endl;       //输出删除信息
			cout << ptemp2->tnumber << "   " << ptemp2->name << "    " << ptemp2->gender << "       " << ptemp2->age << "        " << ptemp2->post << endl;
			ptemp1->next = ptemp2->next;                  //进行删除操作
			delete ptemp2;
			Print(head);                                  //打印删除后的学生信息
			return 1;
		}
		else                                              //如果当前节点不是相符合的
		{
			ptemp1 = ptemp2;                              //往后移动寻找符合的结点
			ptemp2 = ptemp2->next;
			N--;
		}
	}
	cout << "查无此学号" << endl;                           //如果无此学号则进行输出原学生数据
	Print(head);
	return 0;
}
int List::Research(int a)
{
	int N=size;
	Link_node* ptemp = head->next;               //从第一个有效数据位置开始搜索所需考生
	while (N > 0)
	{
	if (ptemp->tnumber == a)                     //如果搜索到直接进行输出打印
	{
		cout << ptemp->tnumber << "   " << ptemp->name << "    " << ptemp->gender << "       " << ptemp->age << "        " << ptemp->post << endl;
		return 1;
	}
	ptemp=ptemp->next;                           //若未找到移到下一位置继续查找
	N--;
	}
	cout << "未找到该学生" << endl;
	return 0;
}
int List::Modify(int a) 
{
	int N = size;
	Link_node* ptemp = head->next;                     //从第一个有效数据位置开始搜索所需考生
	while (N > 0)
	{
		if (ptemp->tnumber == a)                       //如果搜索到直接进行输出打印
		{
			cout << "请输入修改以后的考生信息：" << endl;
			cin >> ptemp->tnumber >> ptemp->name >> ptemp->gender >> ptemp->age >> ptemp->post;
			Print(head);
			return 1;
		}
		ptemp = ptemp->next;                           //若未找到移到下一位置继续查找
		N--;
	}
	cout << "未找到该学生" << endl;
	return 0;
}

/* 
2 stu1 男 18 软件工程师
3 stu2 男 19 网络架构师
4 stu3 女 21 销售总经理

0 stu4 女 20 软件工程师
1 stu5 男 25 软件架构师
*/
