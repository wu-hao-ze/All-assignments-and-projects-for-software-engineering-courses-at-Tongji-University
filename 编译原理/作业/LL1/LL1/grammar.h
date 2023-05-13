#pragma once
#ifndef _grammar_
#define _grammar_

#include <list>
#include <set>
#include <vector>
#define N 50

using namespace std;

//定义文法类，保存文法个数和记录所有文法
class Grammar {
public:
    //保存所有文法
    list<char> grammarTable[N][N];
    //保存终结字符
    char terminalChar[N];
    //保存终结字符的个数
    int terNum;
    //保存每行的产生式的个数
    int countEachRow[N];
    //定义文法数量
    int count;

    Grammar() {
        terNum = 0;
    }
};
//保存每个非终结符的FIRST集合
class FIRST {
public:
    //保存每个非终结符的FIRST集合
    set<char> First[N];
    //保存非终结符
    char nonTerminal[N];
    //保存是否计算出相应的FIRST集
    bool flag[N] = { 0 };
    //保存已计算FIRST集的个数
    int calCount;

    FIRST() {
        calCount = 0;
    }

};
class Position {
public:
    int x;
    int y;
    Position() {
        x = -1;
        y = -1;
    }
};
//保存每个非终结符的FOLLOW集合
class FOLLOW {
public:
    //保存每个非终结符的FOLLOW集合
    set<char> Follow[N];
    //保存非终结符
    char nonTerminal[N];
    //保存是否计算出相应的FOLLOW集
    bool flag[N] = { 0 };
    //保存已计算Follow集的个数
    int calCount;
    //保存产生式的索引
    vector<Position> position[N];

    FOLLOW() {
        calCount = 0;
    }
};

#endif // _grammar_
