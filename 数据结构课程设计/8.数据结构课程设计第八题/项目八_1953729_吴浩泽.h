#pragma once
#include<iostream>
using namespace std;
/************************************************************
                       电网顶点类
*************************************************************/
class Node
{
	friend class Linked_list;
	friend class Electric_Frence;
public:
	Node();
private:
	bool flag;                                               //访问标志
	string start_name;                                       //出发顶点名称
	string present_name;                                     //当前接点名称
	double weight;                                           //边的权值
	Node* next;                                              //下一个相连顶点的指针
};
Node::Node()                                                 //结点赋初值
{
	flag = 0;                                                
	present_name='?';
	weight = 0;
	next = NULL;
}
/************************************************************
					   电网线路类
*************************************************************/
class Linked_list
{
	friend class Electric_Frence;
public:
	Linked_list()                                             //构造函数赋初值
	{
		start_name = '?';
		next_name = NULL;
		next_link = NULL;
	}
	void Print(Node* N);                                      //输出Prim最小生成树
	static void Insert(Linked_list* p, string name, double path);    //两顶点之间插入边，p---初始顶点，name目的接电，两者之间的路径权值
	static bool Is_side(Linked_list* p, string name);                //两点之间是否存在边
	static int Statistic(Linked_list& L);                            //统计某顶点可到达其他顶点个数
	string start_name;                                        //本顶点名称
	Node* next_name;                                          //可到达的接点的指针
	Linked_list* next_link;                                   //顶点链表指针
};
void Linked_list::Print(Node* N)
{
	if (N->next)
	{
		Print(N->next);
		cout << N->start_name << "-(" << N->weight << ")->" << N->present_name << " ";
	}
}
void Linked_list::Insert(Linked_list* p, string name, double path)
{
	if (Is_side(p, name))                                     //若两点之间存在边
	{
		cout << "两顶点之间已经有边，无法插入新边";
	} 
	else                                                      //若不存在边，加入新边
	{
		Node* N = new Node;
		N->present_name = name;
		N->weight = path;
		N->next = p->next_name;  
		p->next_name = N;
	}
}                                       
bool Linked_list::Is_side(Linked_list* p, string name)
{
	Node* N = p->next_name;                                   
	while (N)
	{
		if (N->present_name == name)                          //如果顶点之后连接的有name，证明两点之间已经有边
		{
			return true;
		}
		N = N->next;                                          //继续往后找寻为name的顶点
	}
	delete N;
	return false;
}
int Linked_list::Statistic(Linked_list& L)
{
	int i = 0;
	Node* N = L.next_name;
	while (N)
	{
		N = N->next;
		i++;
	}
	return i;
}

/************************************************************
					     电网类
*************************************************************/
class Electric_Frence
{
	friend class Linked_list;
public:
	void Creat_Pinnacle(Electric_Frence&L);                    //初始化顶点
	void Insert_path(Electric_Frence& L);                      //在顶点之间插入边
	bool Is_point(Electric_Frence& L, string name);            //判断是否存在顶点
	void Change_flag(Electric_Frence& L, string name);         //更改访问标志
	int Statistic(Electric_Frence& L);                         //统计网中的顶点数
	Node* Min_path(Electric_Frence L,Linked_list&edge,string name);//统计最小路径
	void Min_Tree(Electric_Frence L, Linked_list& edge, string name);//Prim最小生成树
private:
	Linked_list* head;
};
void Electric_Frence::Creat_Pinnacle(Electric_Frence& L)
{
	cout << "请输入顶点的个数：" << endl;
	int n;
	cin >> n;
	cout << "请依次输入各顶点的名称" << endl;
	Linked_list* s = new Linked_list;                          //新建一个Linked_list结点
	for (int i = 1; i <= n; i++)                               //建立含有n个顶点的链表存储顶点
	{
		Linked_list* p = new Linked_list;
		string name;
		cin >> name;
		p->start_name = name;
		p->next_link = NULL;
		if (i == 1)                                            //如果是第一个顶点则放入L的头顶点
		{
			L.head = p;
			s = L.head;
		}
		else                                                   //如果不是第一个顶点依次往后存放
		{
			s->next_link = p;
			s = p;
		}
	}
}
void Electric_Frence::Insert_path(Electric_Frence& L)
{
	cout << "请输入两个顶点以及边";
	string name1, name2;
	double path;
	cin >> name1 >> name2 >> path;
	if (name1 != "?" && name2 != "?" && path != 0)                                   //输入该情况边输入结束
	{
		if (Is_point(L, name1) && Is_point(L, name2))                                //如果两个顶点均存在
		{
			Linked_list* p = L.head;
			while (p)
			{
				if (p->start_name == name1)                                          //建立边
				{
					Linked_list::Insert(p, name2, path);
				}
				else if (p->start_name == name2)
				{
					Linked_list::Insert(p, name1, path);
				}
				p = p->next_link;
			}
			delete p;
		}
		else
		{
			cout << "错误的顶点，请重新输入：" << endl;
		}
		Insert_path(L);
	}
}
bool Electric_Frence::Is_point(Electric_Frence& L, string name)
{
	Linked_list* p = L.head;                             //从链表的顶点头开始遍历
	while (p)
	{
		if (p->start_name == name)                       //如果p可以找到相同的顶点，返回正确
		{
			return true;
		}
		p = p->next_link;                                //该顶点不是相同的开始寻找下一个
	}
	delete p;                                            //如果全部顶点都不相同，返回错误
	return false;
}
void Electric_Frence::Change_flag(Electric_Frence& L, string name)
{
	Linked_list* p = L.head;                                   //遍历整个图，发现name则置falg为1，weight为-1
	while (p)
	{
		Node* N = p->next_name;
		while (N)
		{
			if (N->present_name == name)
			{
				N->flag = 1;
				N->weight = -1;
				break;
			}
			N = N->next;
		}
		p = p->next_link;
	}
}
int Electric_Frence::Statistic(Electric_Frence& L)
{
	int i = 0;
	Linked_list* p = L.head;
	while (p)
	{
		i++;
		p = p->next_link;
	}
	delete p;
	return i;
}
Node* Electric_Frence::Min_path(Electric_Frence L, Linked_list& edge, string name)
{
	Linked_list* p = L.head;
	while (p->start_name != name)
	{
		p = p->next_link;
	}
	Node* N = p->next_name;
	Node* s = N;
	while (N->flag && N->next)
	{
		N = N->next;
	}
	if (N)
	{
		s = N;
		while (N)
		{
			if (s->weight > N->weight && !N->flag)
			{
				s = N;
			}
			N = N->next;
		}
	}
	return s;
}
void Electric_Frence::Min_Tree(Electric_Frence L, Linked_list& edge, string name)
{
	if (Is_point(L, name))                             //判断name是否存在
	{
		Linked_list* p = L.head;
		while (p->start_name != name)                         
		{
			p = p->next_link;
		}
		Node* start = new Node;                        //建立第一一个新节点
		start->present_name = name;
		start->start_name = name;
		start->weight = 0;
		start->next = edge.next_name;
		edge.next_name = start;
		Change_flag(L, name);
		while (edge.Statistic(edge) < L.Statistic(L))
		{
			Node* t = edge.next_name;
			Node* s = t;
			while(t)
			{
				if ((Min_path(L, edge, s->present_name)->weight > Min_path(L, edge, t->present_name)->weight) && !Min_path(L, edge, t->present_name)->flag)
				{
					s = t;
				}
				t = t->next;
			}
			Node* N = new Node;
			N->start_name = s->present_name;
			N->present_name = Min_path(L, edge, s->present_name)->present_name;
			N->weight = Min_path(L, edge, s->present_name)->weight;
			N->next = edge.next_name;
			edge.next_name = N;
			Change_flag(L, N->present_name);
		}
	}
	else
	{
		cout << "错误的输入，请重新输入：" << endl;
		Min_Tree(L, edge, name);
	}
}
