#include <iostream>
#include <Cstring>
#include <algorithm>
#include <string>
#include <set>
#include <iomanip>
#include <stack>
#include "grammar.h"

using namespace std;

//定义一个文法类对象
Grammar grammar;
//定义FIRST集
FIRST first;
//定义·FOLLOW集
FOLLOW follow;
//定义预测分析表
string analyseTable[N][N];
//检测一个字符是否为非终结字符
bool isNonTerminal(char var);
//检测一个字符是否为空字
bool isEmpty(char var);
//检测一个字符是否为终结字符
bool isTerminal(char var);
//从控制台读取文法并保存
void readGrammar();
//判断一个产生式是否能求出FIRST集，能返回true，否则false
bool canCalFIRST(int i);
//计算能够计算FIRST集的产生式
void calFIRST();
//获取其非终结字符所在的索引
int getNonTerminalIndex(char var);
//检测第i个FIRST集是否有空字
bool hasEmpty(int i);
//判断是否能计算FIRST集(首字符含非终结符)
bool adjustFIRST(int i);
//计算两个集合的并集，即set(i) = set(i) ∪ set(j)
void calSetUnion(int i, int j);
//更新calCount
int reloadCalCount();
//计算FIRST集
void calFIRSTSet();
//输出first集
void printFIRST();
//获取索引（每一个非终结符在产生式的索引，索引保存在容器中）
void getPosition();
//将FIRST集去空加入FOLLOW集，i代表FOLLOW,i代表FIRST集
void calFollowAndFirstUnion(int i, int j);
//计算两个FOLLOW集的并集,即set(i) = set(i) ∪ set(j)
void calFollowAndFollowUnion(int i, int j);
//更新FOLLOW集的calCount
int reloadFOLLOWCalCount();
//计算FOLLOW集
void calFOLLOWSet();
//获取每一个非终结符的FOLLOW集
void getFollowSet();
//打印FOLLOW集
void printFOLLOW();
//获取终结符在Grammar.terminal[]中的索引
int getTerminalIndex(char var);
//构建单个产生式的First集,i,j为相应产生式的索引
set<char> buildFirstForOne(int i, int j);
//将产生式字符转为字符串,i,j为相应产生式的索引
string charToString(int i, int j);
//构建预测分析表
void bulidAnalyseTable();
//打印预测分析表
void printAnalyseTable();
//将vector中的字符转化为字符串
string veToString(vector<char>& vec);
//将字符数组有选择的转化为字符串
string toString(char buf[], int start, int end);
//核心函数，对语法进行分析
void analyseGrammar();


/*
E->TG
G->+TG|@
T->FH
H->*FH|@
F->(E)|i
*/
/*
E->TG
G->+TG|-TG|@
T->FSj
S->*FS|/FS|@
F->(E)|i|@
*/
int main()
{
    readGrammar();
    calFIRSTSet();
    printFIRST();

    getFollowSet();
    printFOLLOW();
    bulidAnalyseTable();
    printAnalyseTable();
    analyseGrammar();
    return 0;
}
//从控制台读取文法并保存
void readGrammar() {
    //保存输入的第i行文法
    string str;
    //把第i行文法转换为字符数组
    char buf[100] = { 0 };
    int i = 0;
    int index = 0;
    int count = 0;
    //临时保存非终结字符
    set<char> ter;
    cout << "请输入文法（单行输入#回车结束，空字用@代替）：" << endl;
    cin >> str;
    strcpy_s(buf, str.c_str());
    while (str != "#") {
        i = 0;
        count = 0;
        grammar.grammarTable[index][count].push_back(buf[i]);
        //略去"->"
        i += 3;
        //检测是否到边界
        while ((int)buf[i] != 0) {
            //如果检测到"|"
            if ((int)buf[i] == 124) {
                count++;
                i++;
                //保存起始字符
                grammar.grammarTable[index][count].push_back(buf[0]);
                //保存产生式的每个字符
                grammar.grammarTable[index][count].push_back(buf[i]);
                //如果是终结字符则保存
                if (isTerminal(buf[i])) {
                    ter.insert(buf[i]);
                }
                i++;
            }
            else {
                //保存产生式的每个字符
                grammar.grammarTable[index][count].push_back(buf[i]);
                //如果是终结字符则保存
                if (isTerminal(buf[i])) {
                    ter.insert(buf[i]);
                }
                i++;
            }
        }
        grammar.countEachRow[index] = count + 1;
        index++;
        cin >> str;
        strcpy_s(buf, str.c_str());
    }
    //保留文法个数
    grammar.count = index;
    //保存终结字符
    set<char>::iterator it = ter.begin();
    for (it; it != ter.end(); it++) {
        grammar.terminalChar[grammar.terNum] = *it;
        grammar.terNum++;
    }
    //注意需要把特殊符号"#"，加入
    grammar.terminalChar[grammar.terNum] = '#';
    grammar.terNum++;
}
//检测一个字符是否为终结字符,注意空字@也不算终结字符
bool isTerminal(char var) {
    if ((!isNonTerminal(var)) && (!isEmpty(var))) {
        return true;
    }
    else {
        return false;
    }
}
//检测一个字符是否为非终结字符
bool isNonTerminal(char var) {
    if (((int)var > 64) && ((int)var < 91)) {
        return true;
    }
    else {
        return false;
    }
}
//检测一个字符是否为空字
bool isEmpty(char var) {
    if ((int)var == 64) {
        return true;
    }
    else {
        return false;
    }
}
//获取其非终结字符所在的索引
int getNonTerminalIndex(char var) {
    int index = 0;
    //获取其终结字符所在的索引
    for (index; index < grammar.count; index++) {
        if ((int)var == (int)grammar.grammarTable[index][0].front()) {
            break;
        }
    }
    return index;

}
//检测第i个FIRST集是否有空字
bool hasEmpty(int i) {
    set<char>::iterator it = first.First[i].begin();
    for (it; it != first.First[i].end(); it++) {
        if ((int)*it == 64) {
            return true;
        }
    }
    return false;
}
//计算两个集合的并集，即set(i) = set(i) ∪ set(j)，其中set(j)中去除空字
void calSetUnion(int i, int j) {
    set<char>::iterator it = first.First[j].begin();
    //如果有空字，则去空字
    if (hasEmpty(j)) {
        for (it; it != first.First[j].end(); it++) {
            if (!isEmpty(*it)) {
                first.First[i].insert(*it);
            }
        }
    }
    else {
        for (it; it != first.First[j].end(); it++) {
            first.First[i].insert(*it);
        }
    }
}
//更新calCount
int reloadCalCount() {
    int count = 0;
    for (int i = 0; i < grammar.count; i++) {
        if (first.flag[i] == true) {
            count++;
        }
    }
    first.calCount = count;
    return count;
}
//计算FIRST集
void calFIRSTSet() {
    while (reloadCalCount() != grammar.count) {
        //扫描每一个产生式
        for (int i = 0; i < grammar.count; i++) {
            //如果没有计算FIRST集
            if (!first.flag[i]) {
                for (int j = 0; j < grammar.countEachRow[i]; j++) {
                    list<char>::iterator it = grammar.grammarTable[i][j].begin();
                    //获取产生式的首字符
                    it++;
                    //如果it没有到边界并且是非终结字符并且并且已经计算FIRST集并且FIRST含有空字
                    while (it != grammar.grammarTable[i][j].end() && isNonTerminal(*it) && first.flag[getNonTerminalIndex(*it)] && hasEmpty(getNonTerminalIndex(*it))) {
                        first.nonTerminal[i] = grammar.grammarTable[i][0].front();
                        // first.flag[i] = true;
                        calSetUnion(i, getNonTerminalIndex(*it));
                        it++;
                    }
                    //如果it到边界，说明每个非终结符的FIRST集都已经计算出来，并且都含有空字
                    if (it == grammar.grammarTable[i][j].end()) {
                        //把空字加入
                        first.First[i].insert('@');
                        first.flag[i] = true;
                        continue;
                    }
                    //否则，it没有到边界
                    else {
                        //如果*it为终结符
                        if (isTerminal(*it)) {
                            first.nonTerminal[i] = grammar.grammarTable[i][0].front();
                            first.flag[i] = true;
                            //把终结字符保存到FIRST集
                            first.First[i].insert(*it);
                        }
                        //如果是非终结符
                        else if (isNonTerminal(*it)) {
                            //如果已经计算过FIRST集，则把FIrst集加入
                            if (first.flag[getNonTerminalIndex(*it)]) {
                                first.nonTerminal[i] = grammar.grammarTable[i][0].front();
                                first.flag[i] = true;
                                calSetUnion(i, getNonTerminalIndex(*it));
                            }
                            //没有计算过
                            else {
                                first.flag[i] = false;
                            }
                        }
                        //如果是空字
                        else {
                            first.nonTerminal[i] = grammar.grammarTable[i][0].front();
                            first.flag[i] = true;
                            //把终结字符保存到FIRST集
                            first.First[i].insert(*it);
                        }
                    }
                }
            }
            //如果计算FIRST集
            else {
                continue;
            }
        }
    }
}
//输出first集
void printFIRST() {
    cout << "FIRST集如下：" << endl;
    for (int i = 0; i < grammar.count; i++) {
        cout << "FIRST" << "(" << first.nonTerminal[i] << ")" << "=";
        set<char>::iterator it;
        for (it = first.First[i].begin(); it != first.First[i].end(); it++) {
            cout << *it << " ";
        }
        cout << endl;
    }
    cout << endl;
}
//获取索引（每一个非终结符在产生式的索引，索引保存在容器中）
void getPosition() {
    for (int i = 0; i < grammar.count; i++) {
        list<char>::iterator it = grammar.grammarTable[i][0].begin();
        for (int j = 0; j < grammar.count; j++) {
            for (int k = 0; k < grammar.countEachRow[j]; k++) {
                list<char>::iterator itp = grammar.grammarTable[j][k].begin();
                itp++;
                for (itp; itp != grammar.grammarTable[j][k].end(); itp++) {
                    if ((int)*it == (int)*itp) {
                        Position pos;
                        pos.x = j;
                        pos.y = k;
                        //记下其位置
                        follow.position[i].push_back(pos);
                    }
                }
            }
        }
    }

}
//将FIRST集去空加入FOLLOW集，i代表FOLLOW,i代表FIRST集
void calFollowAndFirstUnion(int i, int j) {
    set<char>::iterator it = first.First[j].begin();
    //如果有空字，则去空字
    if (hasEmpty(j)) 
{
        for (it; it != first.First[j].end(); it++) {
            if (!isEmpty(*it)) {
                follow.Follow[i].insert(*it);
            }
        }
    }
    else 
    {
        for (it; it != first.First[j].end(); it++) {
            follow.Follow[i].insert(*it);
        }
    }
}
//更新FOLLOW集的calCount
int reloadFOLLOWCalCount() {
    int count = 0;
    for (int i = 0; i < grammar.count; i++) {
        if (follow.flag[i] == true) {
            count++;
        }
    }
    follow.calCount = count;
    return count;
}
//计算两个FOLLOW集的并集,即set(i) = set(i) ∪ set(j)
void calFollowAndFollowUnion(int i, int j) {
    set<char>::iterator it = follow.Follow[j].begin();
    for (it; it != follow.Follow[j].end(); it++) {
        follow.Follow[i].insert(*it);
    }
}
//计算FOLLOW集
void calFOLLOWSet() {
    //对于开始符号S，需将"#"加入其FOLLOW集
    follow.Follow[0].insert('#');
    while (reloadFOLLOWCalCount() != grammar.count) {
        for (int i = 0; i < grammar.count; i++) {
            //如果没有计算FOLLOW集，则计算
            if (!follow.flag[i]) {
                vector<Position>::iterator it = follow.position[i].begin();
                for (it; it != follow.position[i].end(); it++) {
                    int m = (*it).x;
                    int n = (*it).y;
                    list<char>::iterator  itp = grammar.grammarTable[m][n].begin();
                    //使其指向首字符
                    itp++;
                    for (itp; itp != grammar.grammarTable[m][n].end(); itp++) {
                        if ((int)(*itp) == (int)grammar.grammarTable[i][0].front()) {
                            itp++;
                            break;
                        }
                    }
                    //itp不指向结尾，并且是非终结符并FIRST集含有空字，则继续检测
                    while (itp != grammar.grammarTable[m][n].end() && isNonTerminal(*itp) && hasEmpty(getNonTerminalIndex(*itp))) {
                        int index = getNonTerminalIndex(*itp);
                        follow.nonTerminal[i] = grammar.grammarTable[i][0].front();
                        //将非终结符去空字的FIRST集加入FOLLOW集
                        calFollowAndFirstUnion(i, index);
                        itp++;
                    }
                    //如果itp没有指向end指针，说明该字符为终结字符或非终结字符或空字
                    if (itp != grammar.grammarTable[m][n].end()) {
                        if (isTerminal(*itp)) {
                            follow.nonTerminal[i] = grammar.grammarTable[i][0].front();
                            //将非终结字符加入FOLLOW集
                            follow.Follow[i].insert(*itp);
                            //标记已经计算该非终结符的FOLLOW集
                            follow.flag[i] = true;
                        }
                        else if (isNonTerminal(*itp)) {
                            int index = getNonTerminalIndex(*itp);
                            follow.nonTerminal[i] = grammar.grammarTable[i][0].front();
                            //将非终结符去空字的FIRST集加入FOLLOW集
                            calFollowAndFirstUnion(i, index);
                            //标记已经计算该非终结符的FOLLOW集
                            follow.flag[i] = true;
                        }
                        //空字什么也不做
                        else {

                        }
                    }
                    //itp指向end指针
                    else {
                        if (!follow.flag[m]) {
                            //如果没有计算则标记false
                            follow.flag[i] = false;
                        }
                        else {
                            follow.nonTerminal[i] = grammar.grammarTable[i][0].front();
                            calFollowAndFollowUnion(i, m);
                            //标记已经计算该非终结符的FOLLOW集
                            follow.flag[i] = true;
                        }
                    }
                }
            }
        }
    }
}
//获取每一个非终结符的FOLLOW集
void getFollowSet() {
    getPosition();
    calFOLLOWSet();
}
//打印FOLLOW集
void printFOLLOW() {
    cout << "FOLLOW集如下：" << endl;
    for (int i = 0; i < grammar.count; i++) {
        cout << "FOLLOW" << "(" << follow.nonTerminal[i] << ")" << "=";
        set<char>::iterator it;
        for (it = follow.Follow[i].begin(); it != follow.Follow[i].end(); it++) {
            cout << *it << " ";
        }
        cout << endl;
    }
    cout << endl;
}
//获取终结符在Grammar.terminal[]中的索引
int getTerminalIndex(char var)
{
    for (int i = 0; i < grammar.terNum; i++) {
        if ((int)grammar.terminalChar[i] == (int)var) {
            return i;
        }
    }
    //不存在返回-1
    return -1;
}
//构建单个产生式的First集,i,j为相应产生式的索引
set<char> buildFirstForOne(int i, int j) {
    //定义集合
    set<char> temp;
    list<char>::iterator it = grammar.grammarTable[i][j].begin();
    it++;
    for (it; it != grammar.grammarTable[i][j].end(); it++) {
        //如果没有出界，并且是非终结字符，并且FIRST集含有空字
        while (it != grammar.grammarTable[i][j].end() && isNonTerminal(*it) && hasEmpty(getNonTerminalIndex(*it))) {
            int index = getNonTerminalIndex(*it);
            set<char>::iterator itp = first.First[index].begin();
            for (itp; itp != first.First[index].end(); itp++) {
                //如果不是空字则添加temp集合
                if (!isEmpty(*itp)) {
                    temp.insert(*itp);
                }
            }
            it++;
        }
        //没有出界
        if (it != grammar.grammarTable[i][j].end()) {
            //如果是终结字符或空字，则把终结字符填到FIRST集
            if (isTerminal(*it) || isEmpty(*it)) {
                temp.insert(*it);
                return temp;
            }
            //否则为非终结符
            else {
                int index = getNonTerminalIndex(*it);
                set<char>::iterator itpt = first.First[index].begin();
                for (itpt; itpt != first.First[index].end(); itpt++) {
                    temp.insert(*itpt);
                }
                return temp;
            }
        }
        //如果出界，则退出
        else {
            //说明都是非终结字符，且都含有空字
            temp.insert('@');
            return temp;
        }
    }
}
//将产生式字符转为字符串,i,j为相应产生式的索引
string charToString(int i, int j) {
    char buf[100] = { 0 };
    int count = 0;
    list<char>::iterator it = grammar.grammarTable[i][j].begin();
    it++;
    for (it; it != grammar.grammarTable[i][j].end(); it++) {
        buf[count] = *it;
        count++;
    }
    buf[count] = '\0';
    string str(buf);
    return str;
}
//构建预测分析表
void bulidAnalyseTable() {
    bool flag = false;
    //遍历每个非终结符
    for (int i = 0; i < grammar.count; i++) {
        //遍历每个非终结字符的产生式
        for (int j = 0; j < grammar.countEachRow[i]; j++) {
            flag = false;
            set<char> firstSet = buildFirstForOne(i, j);
            set<char>::iterator it = firstSet.begin();
            for (it; it != firstSet.end(); it++) {
                //如果FIRST集存在空字，记上标记
                if (isEmpty(*it)) {
                    flag = true;
                }
                //否则将相应的产生式加入预测分析表
                else {
                    //将文法字符转为字符串
                    string str = charToString(i, j);
                    analyseTable[i][getTerminalIndex(*it)] = str;
                }
            }
            //产生式的FIRST集中含有空字
            if (flag) {
                //获取i为索引的非终结字符的FOLLOW集
                set<char>::iterator it = follow.Follow[i].begin();
                for (it; it != follow.Follow[i].end(); it++) {
                    if (isTerminal(*it)) {
                        analyseTable[i][getTerminalIndex(*it)] = (string)"@";
                    }
                }
            }
            //产生式的FIRST集中不含有空字
            else {
                //获取i为索引的非终结字符的FOLLOW集
                set<char>::iterator it = follow.Follow[i].begin();
                for (it; it != follow.Follow[i].end(); it++) {
                    analyseTable[i][getTerminalIndex(*it)] = (string)"synch";
                }
            }
        }
    }
}
//打印预测分析表
void printAnalyseTable() {
    cout << "预测分析表如下：" << endl;
    //占位符
    cout << setw(10) << " ";
    //循环输出每位终结符
    for (int i = 0; i < grammar.terNum; i++) {
        cout << setw(10) << grammar.terminalChar[i];
    }
    cout << endl;
    //输出每行
    for (int i = 0; i < grammar.count; i++) {
        //输出非终结字符
        cout << setw(10) << grammar.grammarTable[i][0].front();
        //输出相应的产生式
        for (int j = 0; j < grammar.terNum; j++) {
            cout << setw(10) << analyseTable[i][j];
        }
        cout << endl;
    }
    cout << endl;
}
//将vector中的字符转化为字符串
string veToString(vector<char>& vec) {
    char buf[N] = { 0 };
    int index = 0;
    vector<char>::iterator it = vec.begin();
    for (it; it != vec.end(); it++) {
        buf[index] = *it;
        index++;
    }
    buf[index] = '\0';
    string str(buf);
    return str;
}
//将字符数组有选择的转化为字符串
string toString(char buf[], int start, int end) {
    char temp[N];
    int index = 0;
    for (start; start <= end; start++) {
        temp[index] = buf[start];
        index++;
    }
    temp[index] = '\0';
    string str(temp);
    return str;
}
//核心函数，对语法进行分析
void analyseGrammar() {
    cout << "请输入待分析的字符串：";
    string str;
    cin >> str;
    //将输入的字符串转化为字符数组
    char buf[N] = { 0 };
    strcpy_s(buf, str.c_str());
    //计算字符的数目
    int count = 0;
    for (int i = 0; buf[i] != 0; i++) {
        count++;
    }
    buf[count++] = '#';
    cout << setw(15) << "步骤" << setw(15) << "符号栈" << setw(15) << "输入串" << setw(15) << "所用产生式" << setw(15) << "动作" << setw(15) << "附注" << endl;
    //定义一个分析栈
    stack<char> analyseStack;
    //把'#'和文法开始符号入栈
    analyseStack.push('#');
    analyseStack.push(grammar.grammarTable[0][0].front());
    vector<char> vec;
    vec.push_back('#');
    vec.push_back(grammar.grammarTable[0][0].front());
    //把第一个字符读入a中
    char a = buf[0];
    //记录步骤
    int step = 0;
    cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, 0, count - 1) << setw(15) << " " << setw(15) << "初始化" << setw(15) << " " << endl;
    //buf[]的索引
    int index = 0;
    bool flag = true;
    while (flag) {
        char ch;
        if (!analyseStack.empty()) {
            ch = analyseStack.top();
            analyseStack.pop();
            vec.pop_back();
        }
        if (isTerminal(ch) && ch != '#') {
            if (ch == a) {

                index++;
                a = buf[index];
                step++;
                cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << " " << setw(15) << (string)"GETNEXT(" + ch + (string)")" << setw(15) << " " << endl;

            }
            else {
                //出错
                step++;
                cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << " " << setw(15) << (string)"pop" << setw(15) << "错误，栈顶终结符与输入符号不匹配 " << endl;
            }
        }
        else if (ch == '#') {
            if (ch == a) {
                flag = false;
            }
            else {
                //出错
                cout << "出错";
                return;
            }
        }
        else if (isNonTerminal(ch)) {
            string str = analyseTable[getNonTerminalIndex(ch)][getTerminalIndex(a)];
            //如果产生式不为空,且不为空字
            if (str != "@" && !str.empty() && str != "synch") {

                int strSize = str.size();
                char temp[N];
                strcpy_s(temp, str.c_str());
                for (int i = strSize - 1; i >= 0; i--) {
                    analyseStack.push(temp[i]);
                    vec.push_back(temp[i]);
                }
                step++;
                cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << str << setw(15) << (string)"pop,push(" + str + (string)")" << setw(15) << " " << endl;
            }
            //如果[M,a]为空,则跳过输入符号a
            else if (str.empty()) {
                //出错
                index++;
                a = buf[index];
                step++;
                cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << " " << setw(15) << " " << setw(15) << "错，跳过" << endl;
            }
            else if (str == "synch") {
                //如果栈顶为文法开始符号,跳过输入符号
                if (ch == grammar.grammarTable[0][0].front()) {
                    index++;
                    a = buf[index];
                    //文法开始符号入栈
                    analyseStack.push(ch);
                    vec.push_back(ch);
                    step++;
                    cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << " " << setw(15) << " " << setw(15) << "错，跳过" << endl;
                }
                else {
                    step++;
                    cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << " " << setw(15) << " " << setw(15) << (string)"错,M[" + ch + (string)"," + a + (string)"]=synch" + "," + ch + (string)"已弹出栈" << endl;
                }
            }
            //若为空字，什么也不做
            else {
                step++;
                cout << setw(15) << step << setw(15) << veToString(vec) << setw(15) << toString(buf, index, count - 1) << setw(15) << ch + (string)"->" + (string)"@" << setw(15) << " " << setw(15) << "" << endl;
            }
        }
        else {
            //出错
            cout << "出错";
            return;
        }
    }
}