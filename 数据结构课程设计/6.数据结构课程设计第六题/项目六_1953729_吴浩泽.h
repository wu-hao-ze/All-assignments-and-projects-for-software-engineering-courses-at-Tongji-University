#pragma once
#include<iostream>
using namespace std;
/**********************************************************
                        栈的实现
***********************************************************/
class outOfRange :public exception                        //栈为空取元素报错
{
public:
	const char* what()const throw()
	{
		return "ERROR! OUT OF RANGE.\n";
	}
};
template<class T>
class stack
{
public:
	stack() {};                                   //构造函数
	virtual bool Push(const T& x) = 0;            //新元素进栈
	virtual T Get_top() = 0;                      //栈顶元素出栈
	virtual T Read_top() const = 0;                //读取栈顶元素
	virtual bool Empty()const = 0;                //判断栈是否为空
	//virtual int Get_size()const = 0;            //计算栈中的元素个数
};
template<class T>
class Linked_stack :public stack<T>
{
public:
	Linked_stack() :top(NULL) {};           //构造函数将栈顶指针置空
	~Linked_stack() { Make_empty(); };      //析构函数,将栈中数据都消除
	bool Push(const T& x);                  //进栈
	T Get_top();                            //退栈
	T Read_top()const;                      //读取栈顶元素
	bool Empty()const;                      //判断栈是否空
	void Make_empty();                      //将栈置空
private:
	struct Link_node                        //结点类中包括一个数据以及下一结点的地址
	{
		T data;
		Link_node* link;
	};
	Link_node* top;                         //栈顶指针
};
template<class T>
bool Linked_stack<T>::Push(const T& x)            //进栈函数
{
	Link_node* p;
	p = new Link_node;
	if (p == NULL)                                //创建失败
	{
		exit(1);
		cout << "创建失败" << endl;
		return false;
	}
	else                                          //创建新的含x的结点
	{
		p->data = x;
		p->link = top;
		top = p;
		return true;
	}
}
template<class T>
T Linked_stack<T>::Get_top()                   //退栈函数
{
	T x;
	if (Empty() == true)                       //判断栈是否为空
	{
		throw outOfRange();
	}
	else                                       //栈不为空
	{
		Link_node* p = top;                    //p暂存栈顶元素
		top = top->link;                       //改变top指针
		x = p->data;                           //将p（栈顶）的值输出
		delete p;                              //清除栈顶元素
		return x;
	}
}
template<class T>
T Linked_stack<T>::Read_top()const             //读取栈顶函数，不改变类内值
{
	T x;
	if (Empty() == true)                       //判断栈是否为空
	{
		throw outOfRange();
	}
	else                                       //不为空读取栈顶元素
	{
		x = top->data;
		return x;
	}
}
template<class T>
bool Linked_stack<T>::Empty()const         //判断栈是否为空
{
	if (top == NULL)                       //判断栈是否为空
	{
		return true;
	}
	else
	{
		return false;
	}
}
template<class T>
void Linked_stack<T>::Make_empty()         //将栈置空
{
	Link_node* p;
	while (top != NULL)                    //头指针不为空时依次下移，删除栈中元素 
	{
		p = top;
		top = top->link;
		delete p;
	}
}
/************************************************************
                        树的节点类
************************************************************/
class Node
{
	friend class Family_Tree;                         //person为其友元类
public:
	Node();                                           //构造函数
private:
	string name;                                      //该节点名称
	Node* child;                                      //该节点的孩子结点指针
	Node* brother;                                    //该节点兄弟结点指针
};
Node::Node()                                          //构造函数赋初值
{
	name = '?';
	child = NULL;
	brother = NULL;
}
/**********************************************************
                      家谱成员类
***********************************************************/
class Family_Tree
{
public:
	void Creat(Family_Tree& L);                       //创建家谱
	Node* Lookup(Node* p, string name);               //查找某人
	void Add(Family_Tree& L);                         //添加部分家庭成员
	void Print(Node* p);                              //显示部分家庭成员
	void Insert(Family_Tree& L);                      //添加单个家庭成员
	void Change_name(Family_Tree& L);                 //家谱成员改名
	void Delete(Family_Tree& L);                      //删除家庭成员
private:
	Node* root;                                       //家谱根节点
};
void Family_Tree::Creat(Family_Tree& L)               //创建家谱实现
{
	cout << "请输入祖先的姓名：";
	string rootname;                                  //存储祖先名称
	cin >> rootname;
	Node* p = new Node;                               //建立新结点p暂存祖先信息 
	if (p == NULL)                                    //健壮性保护
	{
		exit(1);
		cout << "创建失败" << endl;
	}
	p->name = rootname;                               
	L.root = p;                                       //p结点值赋给家谱的根节点root
	cout << "此家谱的祖先是：" << p->name;
}
Node* Family_Tree::Lookup(Node* p, string name)       //查找某人
{
	Node* t = NULL;   
	Linked_stack<Node*>store;
	while (p || store.Empty() != true)                  //当p不为空，或者栈不为空时开始循环，或者向右遍历
	{
		while (p)                                       //如果p不为空，先向左一左到底
		{
			if (p->name == name)                        //碰到相同名字则找到将p结点值赋值给t
			{
				t = p;
			}
			store.Push(p);                              //将p压入栈中
			p = p->child;
		}
		p = store.Get_top();                            //p出栈
		p = p->brother;
	}
		return t;
}
void Family_Tree::Add(Family_Tree& L)
{
	cout << "请输入要建立家庭的人的姓名：";
	string parent_name;                                       //输入需建立家庭的成员
	int m=0, n=0;
	cin >> parent_name;
	Node* s = Lookup(L.root, parent_name);                    //在已有的树中查找该成员
	if (s)
	{
		Node* r = s;
		cout << "请输入" << s->name << "的儿女人数：";
		cin >> n;
		m = n;
		cout << "请依次输入" << s->name << "的儿女姓名：" << endl;
		while (m)                                             //m为计数的一个变量
		{
			string child_name;
			Node* q = new Node;
			if (q == NULL)                                    //健壮性保护
			{
				exit(1);
				cout << "创建失败" << endl;
			}
			cin >> child_name;
			q->name = child_name;                             //将child_name变为新结点的数据
			if (m == n)                                       //如果为建立的第一个子女，则放在该父节点的左边
			{
				s->child = q;
				s = s->child;
			}
			else                                              //如果不是建立第一个子女则放在第一个子女的右边
			{
				s->brother = q;
				s = s->brother;
			}
			m--;
		}
		Print(r);                                             //显示部分家庭成员
	}
	else
	{
		cout << "查无此人，请重新输入：" << endl;             //若未查到此人，则重新输入
		Add(L);
	}
}
void Family_Tree::Print(Node* p)
{
	cout << p->name << "的第一代子孙是" << p->child->name<<" ";      //打印第一个子女成员
	p = p->child;
	while (p->brother)                                                 //打印该子女的兄弟成员
	{
		cout << p->brother->name << " ";
		p = p->brother;
	}
	cout << '\n';
}
void Family_Tree::Insert(Family_Tree& L)
{
	cout << "请输入要添加儿子或女儿的人的姓名：";
	string rootname;
	cin >> rootname;
	Node* s = Lookup(L.root, rootname);
	if (s)
	{
		Node* r = s;
		cout << "请输入" << s->name << "新添加儿子或女儿的姓名：";
		Node* p = new Node;                                             //建立新结点
		if (p == NULL)                                                  //健壮性保护
		{
			exit(1);
			cout << "创建失败" << endl;
		}
		string child_name;
		cin >> child_name;
		p->name = child_name;
		if (!s->child)                                                  //如果父节点的子女结点为空则输入的这个点直接放在父节点的右节点
		{
			s->child = p;
		}
		else                                                            //如果父节点的左节点不为空则放在左节点的兄弟结点上
		{
			s = s->child;
			while (s->brother)
			{
				s = s->brother;
			}
			s->brother = p;
		}
		Print(r);
	}
	else
	{
		cout << "查无此人，请重新输入！" << endl;
		Insert(L);
	}
}
void Family_Tree::Change_name(Family_Tree& L)
{
	cout << "请输入要更改姓名的人的目前姓名：";
	string rootname;
	cin >> rootname;
	Node* s = Lookup(L.root, rootname);
	if (s)
	{
		cout << "请输入更改后的姓名：";
		string name;
		cin >> name;
		s->name = name;
		cout << rootname << "已经改名为" << s->name << endl;
	}
	else
	{
		cout << "查无此人，请重新输入！" << endl;
		Change_name(L);
	}
}
void Family_Tree::Delete(Family_Tree& L)
{
	cout << "请输入要解散家庭的人的姓名：";
	string rootname;
	cin >> rootname;
	Node* s = Lookup(L.root, rootname);
	if (s)
	{
		if (s->child)                                             //如果rootname有左子女，解散他！
		{
			cout << "要解散家庭的人是：" << s->name << endl;
			Print(s);
			s->child = NULL;
		}
		else                                                      //如果没有子女就还未有家庭
		{
			cout << s->name << "尚未有家庭";
		}
	}
	else
	{
		cout << "查无此人，请重新输入！" << endl;
		Delete(L);
	}
}
