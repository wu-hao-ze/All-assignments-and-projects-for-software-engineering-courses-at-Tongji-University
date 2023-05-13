#pragma once
#include<iostream>
#include<cmath>
using namespace std;
class divideByZero :public exception                      //异常类处理，分母为0
{
public:
	const char* what()const throw()
	{
		return "ERROR! DIVIDE BY ZERO.\n";
	}
};
class wrongExpression :public exception                   //异常类处理，字符串表达式错误
{
public:
	const char* what()const throw()
	{
		return "ERROR! BAD EXPRESSION.\n";
	}
};
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
	virtual T Read_top() const= 0;                //读取栈顶元素
	virtual bool Empty()const = 0;                //判断栈是否为空
	//virtual int Get_size()const = 0;            //计算栈中的元素个数
};
template<class T>
class Linked_stack:public stack<T>
{
public:
	Linked_stack() :top(NULL) {};           //构造函数将栈顶指针置空
	~Linked_stack() { Make_empty();};       //析构函数,将栈中数据都消除
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
class calculator
{
private:
	Linked_stack<char>optr;                                  //运算符栈
	Linked_stack<int>opnd;                                   //运算数栈
public:
	calculator() {};                                         //构造函数
	~calculator() {};                                        //析构函数
	int prior_in_stack(char item);                           //计算栈内运算符优先级
	int prior_out_stack(char ch);                            //计算栈外运算符优先级
	int precede(char item, char ch);                         //比较栈内外运算符优先级
	int operate(int a, int b, char sign);                    //a,b进行sign运算
	int calculator_number();                                 //计算表达式的值
};
int calculator::prior_in_stack(char item)
{
	int prior_in=0;
	switch (item)                           //赋予栈内运算符不同的优先级
	{
	case'=':
	{
		prior_in = 0;
		break;
	}
	case'+':
	case'-':
	{
		prior_in = 3;
		break;
	}
	case'*':
	case'/':
	{
		prior_in = 5;
		break;
	}
	case'%':
	{
		prior_in = 7;
		break;
	}
	case'^':
	{
		prior_in = 9;
		break;
	}
	case'(':
	{
		prior_in = 1;
		break;
	}
	case')':
	{
		prior_in = 10;
		break;
	}
	}
	return prior_in;
}
int calculator::prior_out_stack(char ch)               //赋予栈外运算符不同的优先级
{
	int prior_out=0;
	switch (ch)
	{
		case'=':
		{
			prior_out = 0;
			break;
		}
		case'+':
		case'-':
		{
			prior_out = 2;
			break;
		}
		case'*':
		case'/':
		{
			prior_out = 4;
			break;
		}
		case'%':
		{
			prior_out = 6;
			break;
		}
		case'^':
		{
			prior_out = 8;
			break;
		}
		case'(':
		{
			prior_out = 10;
			break;
		}
		case')':
		{
			prior_out = 1;
			break;
		}
		default:
		{
			throw wrongExpression();
			break;
		}
		}
	return prior_out;
}
int calculator::precede(char item, char ch)
{
	if (prior_in_stack(item) < prior_out_stack(ch))               //栈外运算符优先级高
	{
		return -1;
	}
	else if (prior_in_stack(item) == prior_out_stack(ch))         //栈内外运算符优先级相同
	{
		return 0;
	}
	else                                                          //栈外运算符优先级低
	{
		return 1;
	}
}
int calculator::operate(int a, int b, char sign)
{
	int result=0;                              //按照输入规则进行运算
	switch (sign)
	{
	case'+':
	{
		result = a + b;
		break;
	}
	case'-':
	{
		result = a - b;
		break;
	}
	case'*':
	{
		result = a * b;
		break;
	}
	case'/':
	{
		if (b == 0)
		{
			throw divideByZero();
		}
		result = a / b;
		break;
	}
	case'%':
	{
		result = a % b;
		break;
	}
	case'^':
	{
		result = pow(a,b);
		break;
	}
	}
	return result;
}
int calculator::calculator_number()                                                   //计算表达式的值
{
	int number=0,a=0,b=0,operate_number,look=0;
	char item, ch=NULL,sign,judge_sign=NULL;
	optr.Make_empty();                                                                //将字符栈和数字栈置空
	opnd.Make_empty();
	optr.Push('=');                                                                   //字符栈放入=
	while ((ch = getchar()) != '\n')                                                  //逐字吞取，直到读到回车
	{
		if ((judge_sign == NULL||judge_sign=='(') && (ch == '+' || ch == '-'))        //判断是否为单目运算符
		{
			if (ch == '+')
			{
				look++;                                                               //ch为+时记录下
			}
			else
			{
				look = look + 2;                                                      //ch为-时记录下
			}
		}
		else
		{
			judge_sign = ch;
			switch (ch)
			{
			case'+':case'-':case'*':case'/':case'%':case'^':case'(':case')':case'=': //读取ch为以上字符则进行下列操作
			{
				item=optr.Read_top();                                                //读字符栈顶端字符
				while (precede(item, ch) == 1)                                       //ch与字符串顶端字符比较若优先级ch<item
				{
					sign=optr.Get_top();                                             //从字符串弹出运算字符
					b=opnd.Get_top();                                                //弹出两个操作数
					a=opnd.Get_top();
					opnd.Push(operate(a, b, sign));                                  //进行运算操作
					item = optr.Read_top();                                          //为了让ch与之后的一个运算符优先级比较
				}
				if (precede(item, ch) == -1)                                         //ch与字符串顶端字符比较若优先级ch>item
			    {
				optr.Push(ch);                                                       //将ch压入栈中
			    }
				else if (precede(item, ch) == 0)                                     //将括号或者等号从栈中弹出
				{
				item = optr.Get_top();
			    }
				break;
			}
			default:                                                                 //若字符不为运算字符
			{
				cin.putback(ch);                                                     //将其退回读取流中
				cin >> operate_number;                                               //读取数字
				if (look == 2)                                                       //检查是否需要将进行单目运算
				{
					operate_number = 0 - operate_number;                             //进行单目运算
				}
				look = 0;                                                            //将look置零
				opnd.Push(operate_number);                                           //操作数压入栈中
				judge_sign = 'R';                                                    //将判断是否进行单目运算的前一个字符置换
				break;
			}
			}
		}
	}
	number=opnd.Get_top();                                                           //最后在栈中取结果
	if (optr.Empty()==false)                                                         //如果字符栈中还有元素                                                      
	{
		throw wrongExpression();
	}
	return number;
}



	
