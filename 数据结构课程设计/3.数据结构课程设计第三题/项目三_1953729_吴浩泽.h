#pragma once
#include<iostream>
using namespace std; 
struct offsets
{
	int a, b;
	char dir;
}direction[4] = {{0,1,'E'},{0,-1,'W'},{1,0,'S'},{-1,0,'N'}};  //前进方向表
class maze                                                    //迷宫类，进行迷宫的建立，寻找路径，打印
{
public:
	maze();
	void Creat_maze();
	int Find_road(int x,int y);
	void Print_maze();
private:
	char** a;                                                 //a，b是一个二维char型数组
	char** b;
	int row;
	int	col;                                                  //迷宫行数以及列数
	int road_number;                                          //路径数
};
maze::maze()                                                  //构造函数赋初值
{
	a = NULL;
	b = NULL;
	row = 0;
	col = 0;
	road_number = 0;
}
void maze::Creat_maze()
{
	cout << "请输入数组行数与列数" << endl;
	cin >> row >> col;                                //输入数组行数列数
	a = new char* [row];                              //动态建立二维数组
	b = new char* [row];
	if (a == NULL||b==NULL)                           //检查内存
	{
		cout << "内存分配错误！" << endl;
		exit(1);
	}
	for (int i = 0; i < row; i++)
	{
		a[i] = new char[col];
		b[i] = new char[col];
		if (a[i] == NULL||b[i]==NULL)                 //检查内存
		{
			cout << "内存分配错误！" << endl;
			exit(1);
		}
	}
	cout << "请输入您的迷宫，以0表示空地，#表示围墙，" << endl;
	cout << "并保证起止点（除围墙外的左上角与右下角）是空地" << endl;
	for (int i = 0; i < row; i++)
	{
		for (int j = 0; j < col; j++)
		{
			cin >> a[i][j];
		}
	}
	for (int i = 0; i < row; i++)                                //输入迷宫不合法的判定
	{
		for (int j = 0; j < col; j++)
		{
			if (a[i][j] != '0' && a[i][j] != '#')
			{
				cout << "迷宫内部有不可识别字符，请重新输入：";
				Creat_maze();
			}
			if (i == 1&& j == 1)
			{
				if (a[i][j] != '0')
				{
					cout << "迷宫起始坐标不合法，请重新输入：";
					Creat_maze();
				}
			}
			if (i == row-2 && j == col-2)
			{
				if (a[i][j] != '0')
				{
					cout << "迷宫起始坐标不合法，请重新输入：";
					Creat_maze();
				}
			}
			if (i == 0||j == 0)
			{
				if (a[i][j] != '#')
				{
					cout<< "迷宫围墙不合法，请重新输入：";
					Creat_maze();
				}
			}
			if (i == row-1 || j == col-1)
			{
				if (a[i][j] != '#')
				{
					cout << "迷宫围墙不合法，请重新输入：";
					Creat_maze();
				}
			}
		}
	}
	cout << "该迷宫建立时为：" << endl;
	for (int i = 0; i < row; i++)
	{
		for (int j = 0; j < col; j++)
		{
			cout << a[i][j] << " ";
		}
		cout << endl;
	}
}
int maze::Find_road(int x, int y)
{
	int g, h;
	char d;
	a[x][y] = '1';                                //将当前位置1
	if ((x == row - 2) &&( y == col - 2))         //递归截止条件
	{		
		return 1;
	}
	for (int i = 0; i < 4; i++)                   //向四个方向进行试探，看是否能走
	{
		g = x + direction[i].a;
		h = y + direction[i].b;
		d = direction[i].dir;
		if (a[g][h] == '0')
		{
			if (Find_road(g, h))
			{
				road_number++;                   //找到回退时计数
				b[g][h] = road_number;           //方便输出走出迷宫路径
				return 1;
			}
		}
	}
	a[x][y] = '0';                               //如果没找到路径则将起点置零
	return 0;
}
void maze::Print_maze()                             //打印行走以后的迷宫
{
	if (a[1][1] == '0')
	{
		cout << "该迷宫无可出路径" << endl;
	}
	else
	{
		cout << "迷宫路径如下：" << endl;           //先输出迷宫，1表示路径
		for (int i = 0; i < row; i++)
		{
			for (int j = 0; j < col; j++)
			{
				cout << a[i][j] << " ";
			}
			cout << endl;
		}
		cout << "(1,1)->";
		for (int k = road_number; k > 0; k--)      //输出行走路径坐标
		{
			for (int i = 0; i < row; i++)
			{
				for (int j = 0; j < col; j++)
				{
					if (b[i][j] == k)
					{
						cout << "(" << i << "," << j << ")->";
					}
				}
			}
		}
	}
}
/*
10 10
##########
#0##0#000#
#00000#0##
#####0####
#00000000#
##0#0#0#0#
#00#00#00#
#0###0#0##
#00#0#000#
##########
5 5
#####
#000#
#000#
#000#
#####
4 4
####
#0##
#00#
####

*/
