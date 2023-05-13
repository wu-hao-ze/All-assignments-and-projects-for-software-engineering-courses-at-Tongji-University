#pragma once
#include<string>
#include<fstream>
#include<iomanip>
#include<iostream>
using namespace std;
void MakeSchedule(string Course_Information[38][5], int term_num_course, int arranged_course)
{
	string* Class_Name = new string[term_num_course];
	int* Class_Time = new int[term_num_course];
	ofstream Outfile;
	Outfile.open("课表(已排好).txt", ios::app);
	for (int i = 0; i < term_num_course; i++)                                            //读取该学期的num_of_course节课程信息
	{	
		Class_Name[i] = Course_Information[arranged_course + i][1];                      //其中某堂课周学时数存入ClassTime*中，课程名称ClassName*中	
		Class_Time[i] = (int)(Course_Information[arranged_course + i][2][0] - '0');
	}
	string Schedule[4][5];                                                             //将课表中每个元素初始化为一个空格利于接下来的排课表行为
	for (int i = 0; i < 4; i++) 
	{
		for (int j = 0; j < 5; j++) 
		{
			Schedule[i][j] = " ";
		}
	}
                                                                     //排课表
	for (int i = 0; i < term_num_course; i++)                        //三学时的课程的排课表方法
	{
		int temp = 0;
		if (Class_Time[i] == 3) 
		{
			for (int j = 1; j < 4; j += 2) 
			{
				for (int k = 0; k < 5; k++) 
				{
					if (Schedule[j][k] == " ") 
					{
						Schedule[j][k] = Class_Name[i];
						j = 4;
						k = 5;
					}
				}
			}
		}
		else if (Class_Time[i] == 4)                                 //四学时的课程的排课表方法
		{
			for (int j = 0; j < 4; j += 2)
			{
				for (int k = 0; k < 5; k++) 
				{
					if (Schedule[j][k] == " ") 
					{
						Schedule[j][k] = Class_Name[i];
						temp = k;
						j = 4;
						k = 5;
					}
				}
			}
			if (temp == 0) 
			{
				for (int j = 0; j < 4; j += 2) 
				{
					for (int k = 2; k < 5; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 1) 
			{
				for (int j = 0; j < 4; j += 2) 
				{
					for (int k = 3; k < 5; k++)
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 2) 
			{
				for (int j = 0; j < 4; j += 2) 
				{
					for (int k = 0; k < 5; k += 4) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 3) 
			{
				for (int j = 0; j < 4; j += 2) 
				{
					for (int k = 0; k < 2; k++)
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 4) 
			{
				for (int j = 0; j < 4; j += 2) 
				{
					for (int k = 0; k < 3; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
		}
		else if (Class_Time[i] == 5)                                        //五学时的课程的排课表方法
        {
			for (int j = 0; j < 4; j += 2) 
			{
				for (int k = 0; k < 5; k++) 
				{
					if (Schedule[j][k] == " ")
					{
						Schedule[j][k] = Class_Name[i];
						temp = k;
						j = 4;
						k = 5;
					}
				}
			}
			if (temp == 0) 
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 2; k < 5; k++) 
					{
						if (Schedule[j][k] == " ")
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 1) 
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 3; k < 5; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 2)
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 0; k < 5; k += 4)
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 3) 
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 0; k < 2; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 4) 
			{
				for (int j = 0; j < 4; j += 2) 
				{
					for (int k = 0; k < 3; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
		}
		else if (Class_Time[i] == 6)                                           //六学时的课程的排课表方法
{
			for (int j = 1; j < 4; j += 2)
			{
				for (int k = 0; k < 5; k++) 
				{
					if (Schedule[j][k] == " ") 
					{
						Schedule[j][k] = Class_Name[i];
						temp = k;
						j = 4;
						k = 5;
					}
				}
			}
			if (temp == 0) 
			{
				for (int j = 1; j < 4; j += 2)
				{
					for (int k = 2; k < 5; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 1) 
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 3; k < 5; k++) 
					{
						if (Schedule[j][k] == " ")
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 2) 
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 0; k < 5; k += 4) 
					{
						if (Schedule[j][k] == " ")
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 3)
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 0; k < 2; k++) 
					
					{
						if (Schedule[j][k] == " ") {
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
			else if (temp == 4) 
			{
				for (int j = 1; j < 4; j += 2) 
				{
					for (int k = 0; k < 3; k++) 
					{
						if (Schedule[j][k] == " ") 
						{
							Schedule[j][k] = Class_Name[i];
							j = 4;
							k = 5;
						}
					}
				}
			}
		}
	}	                                                
	for (int i = 0; i < 4; i++)                                         //向目标文件中打印某一学期的课表
	{
		Outfile << "第" << i + 1 << "大节";
		for (int j = 0; j < 5; j++) 
		{
			Outfile << setw(20) << Schedule[i][j];
		}
		Outfile << endl;
	}
	delete[]Class_Name;
	delete[]Class_Time;
	Outfile.close();
}