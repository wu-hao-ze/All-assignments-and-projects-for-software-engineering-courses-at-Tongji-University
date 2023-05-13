#include<iostream>
#include<fstream>
#include<string>
#include<vector>
using namespace std;
vector<char> vt;//终结符储存数组
vector<char> vn;//非终结符储存数组
string ST="#";//符号栈,预先压入#
string inputs;//用户输入的符号串
string subinput;//剩余子串
int input_flag = 1;//输入子串非法时，变为0
vector<vector<string>> firstvt;//firstvt储存数组。例如，若firstvt[i][0]="E".firstvt[i][1]="abc",表示firstvt(E)={a,b,c}
vector<vector<string>> lastvt;//lastvt储存数组，同理。
vector<vector<char>> table;//储存算符优先关系表,table[i][j]表示vt[i]与vt[j]的关系
//用户将文法一行一行输入到同目录下的suanfuyouxianwenfa.txt文件中
void input_file()
{
	string a;
	ofstream ofs;
	ofs.open("suanfuyouxianwenfa.txt", ios::out);
	cout << "请输入文法：(若结束，请在下一行只输入#)" << endl;
	while (1) {
		cin >> a;
		if (a == "#")
		{
			break;
		}
		ofs << a << endl;
	}


	ofs.close();
}
//读取同目录下的suanfuyouxianwenfa.txt文件，拆分文法，将拆分好的文法写入到split.txt文件中
//返回值为初始化的
void split()
{

	string a;//用于读取每行字符串
	string b;//用于存放拆分后的字符串
	int c;//表示|出现的位置
	ifstream ifs;
	ofstream ofs;
	ifs.open("suanfuyouxianwenfa.txt", ios::in);
	ofs.open("split.txt", ios::out);
	while (getline(ifs, a))
	{
		while (1)
		{
			c = a.find("|");
			if (c == -1)
			{
				ofs << a << endl;
				break;
			}
			else {
				b = a.substr(0, c);
				ofs << b << endl;;
				a.erase(3, c - 3+1);
			}
		}
	}

	ifs.close();
	ofs.close();


}
//判断数组中是否存储某个非终结符,有返回索引，无返回-1
int find_bigchar(vector<vector<string>> S, char a)
{
	string c;
	c = a;
	for (int i = 0; i < S.size(); i++)
	{
		if (S[i][0] == c)
		{
			return i;
		}
	}
	return -1;
}
//判断数组中是否存储某个终结符,有返回1，无返回0
int find_smallchar(vector<vector<string>>  S, char a,int indx)
{
	
	for (int i = 0; i < S[indx][1].size(); i++)
	{
		if (S[indx][1][i]== a)
		{
			return 1;
		}
	}
	return 0;
}
//读取文法中的非终结符存储到vn中，终结符存储到vt。
void read_char()
{
	string a;
	ifstream ifs;
	ifs.open("split.txt", ios::in);
	while (getline(ifs, a))
	{
		for (int i = 0; i < a.size(); i++)
		{
			if (a[i] >= 'A' && a[i] <= 'Z')
			{
				if (find(vn.begin(), vn.end(),a[i])==vn.end() )//即没有重复的
				{
					vn.push_back(a[i]);                      
				}
			}
			else if (i!=1 && i!=2&&a[i]!=' ')
			{
				if (find(vt.begin(), vt.end(), a[i]) == vt.end())
				{
					vt.push_back(a[i]);
				}
			}
		}
	}
	//控制台输出两个栈
	cout << "该文法的非终结符为：";
	for (int i = 0; i < vn.size(); i++) {
		cout << vn[i] << "  ";
	}
	cout << endl;
	cout << "该文法的终结符为：";
	for (int i = 0; i < vt.size(); i++) {
		cout << vt[i] << "  ";
	}
	cout << endl;
	ifs.close();
}
//在终结符数组中查找某个终结符，若找到，返回index，否则返回-1
int find_vt(vector<char> vt1, char a)
{
	for (int i = 0; i < vt1.size(); i++)
	{
		if (vt1[i] == a)
		{
			return i;
		}
	}
	return -1;
}
//比较两个字符的优先级关系，返回比较字符
char com_twochars(char a, char b)
{
	return table[find_vt(vt, a)][find_vt(vt, b)];
}
//动作移进
void movein()
{
	cout << "移进" << "   ";
	ST.insert(ST.begin(), inputs[0]);
	inputs.erase(inputs.begin());
	cout << ST << "         " << inputs << endl;
}
//动作规约
void reduce(int k)
{
	int k1 = k;//暂存
	k++;
	int flag = 0;//是否找到合适的规约式
	cout << "规约" << "   ";
	string a;
	ifstream ifs;
	while (1)
	{
		if ((ST[k] <= 'Z' && ST[k] >= 'A') )
		{
			k = k + 1;
		}
		else if (com_twochars(ST[k1], ST[k]) == '=')
		{
			k1 = k;
			k = k + 1;
		}
		else if (com_twochars(ST[k1], ST[k]) == '>')
		{
			//找到合适的规约候选式
			
			ifs.open("split.txt", ios::in);
			while (getline(ifs, a))
			{
				if ((a.size() - 3) ==k )
				{
					int i = 0;
					for (; i < k ; i++)
					{
						if (a[i + 3] != ST[k - i-1]&& !(ST[k-i-1] <= 'Z' && ST[k-i-1] >= 'A'&& a[i + 3] <= 'Z' && a[i + 3] >= 'A'))
						{
							break;
						}
					}
					if (i == k )
					{
						flag = 1;
						ST.erase(ST.begin(), ST.begin()+k);
						ST.insert(ST.begin(), a[0]);
						break;
					}
				}
			}
			ifs.close();
			if (flag == 0)
			{
				cout << "未找到合适的规约式,ERROR" << endl;
				input_flag = 0;
				return;
			}
			break;
		}
		else {
			cout << "规约时，符号站内比较出错，ERROR" << endl;
			input_flag = 0;
			return;
		}
	}
	
	cout << ST << "         " << inputs << endl;
}
//检查产生式右侧是否出现两个连续的非终结符，若有返回1.否则返回0
int check_two_bigchar()
{
	int i = 3;
	string a;
	ifstream ifs;
	ifs.open("split.txt", ios::in);
	while (getline(ifs, a))
	{
		for (; i< a.size() - 1; i++)
		{
			if (a[i] <= 'Z' && a[i] >= 'A' && a[i + 1] <= 'Z' && a[i + 1] >= 'A')
			{
				return 1;
			}
			
		}
		
	}
	ifs.close();
	return 0;
}
//计算每一个非终结符的FirstVT集合，并打印出来
void Get_firstvt()                    
{
	//扫描文法
	int index;
	int firstsize = 0;
	int flag = 0;//判断集合是否不再变化
	string c;
	vector<string> fv;
	string a;//按行读文件
	ifstream ifs;
	ifs.open("split.txt", ios::in);
	while (getline(ifs, a))
	{
		if (find_bigchar(firstvt, a[0])==-1)//没有此非终结符
		{
			for (int i = 3; i < a.size(); i++)//找到第一个终结符
			{
				if (!(a[i] <= 'Z' && a[i] >= 'A')&&a[i]!=' ')
				{
					c = a[0];
					fv.push_back(c);
					c = a[i];
					fv.push_back(c);
					firstvt.push_back(fv);
					fv.clear();
					break;
				}
			}
		}
		else {
			 index= find_bigchar(firstvt, a[0]);
			for (int i = 3; i < a.size(); i++)//找到第一个终结符
			{
				if (!(a[i] <= 'Z' && a[i] >= 'A') && a[i] != ' ')
				{
					if (!(find_smallchar(firstvt, a[i], index)))//没有找到
					{
						firstvt[index][1]+=a[i];
					}
					break;
				}
			}
		}
	}
	//初始化完成
	while (1)
	{
		flag = 0;
		ifs.clear();//清除当前流状态
		ifs.seekg(0, ios::beg);//文件读指针回到开头
		while (getline(ifs, a))
		{
			
			if (a[3] != a[0] && a[3] <= 'Z' && a[3] >= 'A')
			{
				index = find_bigchar(firstvt, a[0]);
				firstsize = find_bigchar(firstvt, a[3]);
				for (int i = 0; i < firstvt[firstsize][1].size(); i++)
				{
					if (!find_smallchar(firstvt,firstvt[firstsize][1][i],index))//没找到
					{
						firstvt[index][1] += firstvt[firstsize][1][i];
						flag = 1;//表示有添加，继续循环
					}
				}
		   }
		}
		if (flag == 0)
		{
			break;
		}
	}
	ifs.close();
	cout << "firstvt集合打印输出：" << endl;
	for (int i = 0; i < firstvt.size(); i++)
	{
		cout << "firstvt(" << firstvt[i][0] << ")= {"; 
		for (int j = 0; j < firstvt[i][1].size(); j++)
		{
			cout << firstvt[i][1][j] << "  ";
		}
		cout<< "}" << endl;
		
	}

}
//计算每一个非终结符的lastVT集合，并打印出来
void Get_lastvt()
{
	//扫描文法
	int index;
	int firstsize = 0;
	int flag = 0;//判断集合是否不再变化
	string c;
	vector<string> fv;
	string a;//按行读文件
	ifstream ifs;
	ifs.open("split.txt", ios::in);
	while (getline(ifs, a))
	{
		if (find_bigchar(lastvt, a[0])==-1)
		{
			for (int i = a.size()-1; i>=3; i--)//找到最后一个终结符
			{
				if (!(a[i] <= 'Z' && a[i] >= 'A') && a[i] != ' ')
				{
					c = a[0];
					fv.push_back(c);
					c = a[i];
					fv.push_back(c);
					lastvt.push_back(fv);
					fv.clear();
					break;
				}
			}
		}
		else {
			index = find_bigchar(lastvt, a[0]);
			for (int i = a.size() - 1; i >= 3; i--)//找到最后一个终结符
			{
				if (!(a[i] <= 'Z' && a[i] >= 'A') && a[i] != ' ')
				{
					if (!(find_smallchar(lastvt, a[i], index)))
					{
						lastvt[index][1] += a[i];
					}
					break;
				}
			}
		}
	}
	//初始化完成
	while (1)
	{
		flag = 0;
		ifs.clear();
		ifs.seekg(0, ios::beg);//文件读指针回到开头
		while (getline(ifs, a))
		{
			if (a[a.size()-1] != a[0] && a[a.size() - 1] <= 'Z' && a[a.size() - 1] >= 'A')
			{
				index = find_bigchar(lastvt, a[0]);
				firstsize = find_bigchar(lastvt, a[a.size() - 1]);
				for (int i = 0; i < lastvt[firstsize][1].size(); i++)
				{
					if (!find_smallchar(lastvt, lastvt[firstsize][1][i], index))
					{
						lastvt[index][1] += lastvt[firstsize][1][i];
						flag = 1;//表示有添加，继续循环
					}
				}
			}
		}
		if (flag == 0)
		{
			break;
		}
	}
	ifs.close();
	cout << "lastvt集合打印输出：" << endl;
	for (int i = 0; i < lastvt.size(); i++)
	{
		cout << "lastvt(" << lastvt[i][0] << ")= {";
		for (int j = 0; j < lastvt[i][1].size(); j++)
		{
			cout << lastvt[i][1][j] << "  ";
		}
		cout << "}" << endl;

	}

}
//生成算符优先关系表,并打印出来
void Get_table()
{

    //初始化表，即先确定相等关系
	int k1,k2,index;
	vector<char> hang(vt.size() + 1,' ');//+1指加上#,初始化为空格

	for (int i = 0; i < vt.size() + 1; i++)
	{
		table.push_back(hang);
	}
	
	string a;
	ifstream ifs;
	ifs.open("split.txt", ios::in);
	while (getline(ifs, a))
	{
		if (a.size() >= 5)
		{
			for (int i = 3; i < a.size() - 1; i++)
			{
				if (!(a[i] <= 'Z' && a[i] >= 'A')&&!(a[i+1] <= 'Z' && a[i+1] >= 'A')&&a[i]!=' '&&a[i+1]!=' ')
				{
					k1 = find_vt(vt, a[i]);
					k2 = find_vt(vt, a[i+1]);
					if (k1 == -1 || k2 == -1)
					{
						cout << "出现不应该存在的终结符，EORROR" << endl;
						return;
					}
					table[k1][k2] = '=';

				}
				if (!(a[i] <= 'Z' && a[i] >= 'A') && (a[i + 1] <= 'Z' && a[i + 1] >= 'A') && a[i] != ' ')
				{
					//a[i]<firstvt(a[i+1])中的所有元素
					index = find_bigchar(firstvt, a[i + 1]);
					for (int j = 0; j < firstvt[index][1].size(); j++)
					{
						k1 = find_vt(vt, a[i]);
						k2 = find_vt(vt, firstvt[index][1][j]);
						table[k1][k2] = '<';
						
					}
				}
				if (!(a[i+1] <= 'Z' && a[i+1] >= 'A') && (a[i] <= 'Z' && a[i] >= 'A') && a[i+1] != ' ')
				{
					//lastvt(a[i])中的所有元素>a[i+1]
					index = find_bigchar(lastvt, a[i]);
					for (int j = 0; j < lastvt[index][1].size(); j++)
					{
						k2 = find_vt(vt, a[i+1]);
						k1 = find_vt(vt, lastvt[index][1][j]);
						table[k1][k2] = '>';

					}
				}
			}
			if (a.size() >= 6)
			{
				for (int i = 3; i < a.size() - 2; i++)
				{
					if (!(a[i] <= 'Z' && a[i] >= 'A') && !(a[i + 2] <= 'Z' && a[i + 2] >= 'A') && a[i] != ' ' && a[i + 2] != ' ')
					{
						k1 = find_vt(vt, a[i]);
						k2 = find_vt(vt, a[i + 2]);
						if (k1 == -1 || k2 == -1)
						{
							cout << "出现不应该存在的终结符，EORROR" << endl;
							return;
						}
						table[k1][k2] = '=';

					}
				}
			}
		}
		//处理#
		//左#右E
	for (int j = 0; j < firstvt[0][1].size(); j++)
		{
			k1 = vt.size();
			k2 = find_vt(vt, firstvt[0][1][j]);
			table[k1][k2] = '<';

		}
		
			
	for (int j = 0; j < lastvt[0][1].size(); j++)
		{
			k2 = vt.size();
			k1 = find_vt(vt, lastvt[0][1][j]);
			table[k1][k2] = '>';

		}
		
	}
	//打印table表
	cout << "    ";
	for (int i = 0; i < vt.size(); i++)
	{
		cout << vt[i] << "   ";
	}
	cout << "#" << endl;
	for (int i = 0; i < vt.size()+1; i++)
	{
		if (i == vt.size())
		{
			cout << '#' << "   ";
			for (int j = 0; j < vt.size() ; j++)
			{
				cout << table[i][j] << "   ";
			}
			cout << '=';
			table[i][i] = '=';
			cout << endl;
			break;
		}
		cout << vt[i] << "   ";
		for (int j = 0; j < vt.size()+1; j++)
		{
			cout << table[i][j] << "   ";
		}
		cout << endl;
	}

	ifs.close();
}
//算符优先分析器执行，打印执行过程
void analysis()
{

	vt.push_back('#');
	int i=0;//步骤计数
	int k = 0;//符号栈栈顶指针
	cout << "请输入需要处理的语句(以#结尾)：" << endl;
	cin >> inputs;
	cout << "步骤" << "   " << "动作" << "   " << "符号栈" << "   " << "剩余子串" << endl;
	while (1)
	{
		k = 0;
		if (inputs == "")
		{
			break;
		}
		cout << " " << i << "     ";
		if (inputs[0] <= 'Z' && inputs[0] >= 'A')
		{
			movein();
		}
		else {
			if (find_vt(vt, inputs[0]) == -1)
			{
				cout << "输入子串中有非法字符，ERROR" << endl;
				input_flag = 0;
				if (input_flag == 0)
				{
					cout << "输入子串非法" << endl;
					return;
				}
			}
			while (1)
			{
				if (ST[k] <= 'Z' && ST[k] >= 'A')
				{
					k += 1;
				}
				else break;
			}
			if (com_twochars(inputs[0],ST[k] ) == '=' || com_twochars(inputs[0], ST[k]) == '>')
			{
				movein();
			}
			else if (com_twochars(inputs[0], ST[k]) == '<' )
			{
				reduce(k);
				if (input_flag == 0)
				{
					cout << "输入子串非法" << endl;
					return;
				}
			}
			else if (com_twochars(inputs[0], ST[k]) == ' ')
			{
				input_flag = 0;
				if (input_flag == 0)
				{
					cout << "算符优先关系表中没有这种比较关系，ERROR,输入子串非法" << endl;
					return;
				}
			}
		}
		i++;
	}
}

//主程序
void main()
{
	input_file();//用户输入文法
	split();//拆分文法
	if (check_two_bigchar())
	{
		cout << "该文法不是算符文法，程序结束" << endl;
		return;
	}
	read_char();//打印非终结符和终结符
	Get_firstvt();
	Get_lastvt();
	Get_table();
	analysis();
}
