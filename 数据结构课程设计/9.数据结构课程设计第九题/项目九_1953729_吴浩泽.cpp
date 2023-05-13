#include"项目九_1953729_吴浩泽.h"
using namespace std;
int main() {
	int sum = 0;
	int num_every_term[8] = { 0 };                              //最开始将八个学期科目数均置为0                    
	while (sum != 38)                                         //8个学期课程总数为38科，所以先检测8个学期课程数是否符合要求
	{
		cout << "请依次输入每个学期所开的课程数:";
		for (int i = 0; i < 8; i++)
		{
			cin >> num_every_term[i];
			sum += num_every_term[i];
		}
		if (sum != 38)                                        	//课程总数如果不为38科，要求重新输入
		{
			cerr << "8个学期课程数总和应为38！请重新输入！" << endl;
			sum = 0;
		}
		else 
		{
			cout << "输入成功，请等待！" << endl;               //课程总数为38科，进入排课模式
		}
	}
	sum = 0;
	ifstream Infile;
	Infile.open("课表.txt");
	if (!Infile)
	{
		cout << "error";
	}
	ofstream Outfile;
	Outfile.open("课表(已排好).txt", ios::app);
	int readed_times = 0;                                       //读取课程信息次数
	int recorded_course = 0;                                    //记录已读取课程数
	string Course_Information[38][5];
	int arranged_course = 0;
	for (int i = 0; i < 8; i++) {
		while (readed_times != num_every_term[i] * 5) {
			Infile >> Course_Information[recorded_course][readed_times % 5];   //从文件中读取课程信息至38*5的数组中
			if (readed_times % 5 == 4) {
				recorded_course++;
			}
			readed_times++;
		}
		readed_times = 0;
	}
	for (int i = 0; i < 8; i++)                                 //打印课表
	{
		Outfile << setw(50) << "第" << i + 1 << "学期" << endl;
		Outfile << setw(25) << "Mon" << setw(20) << "Tues" << setw(20) << "Wen" << setw(20) << "Thur" << setw(20) << "Fri" << endl;
		MakeSchedule(Course_Information, num_every_term[i], arranged_course);
		arranged_course = arranged_course + num_every_term[i];
		Outfile << endl;
		Outfile << endl;
	}
	cout << "排课成功！" << endl;                              //排课已完成
	Infile.close();
	Outfile.close();
	return 0;
}