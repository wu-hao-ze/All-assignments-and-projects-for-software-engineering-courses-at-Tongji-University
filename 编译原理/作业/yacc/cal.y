%{
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
int yylex(void);
void yyerror(char *);
%}


%union {int num;}

%token <num> NUM 
%token  ADD SUB MUL DIV LP RP CR
%type <num> line exp single

%start  line
%left ADD SUB
%left MUL DIV
%left LP RP

%%

    line    : exp CR    {printf("result: %d", $1);}
            ;
    exp     : exp MUL exp    {$$ = $1*$3;}
            | exp DIV exp    {$$ = $1/$3;}
            | exp ADD exp    {$$ = $1+$3;}
            | exp SUB exp    {$$ = $1-$3;}
            | LP exp RP      {$$ = $2;}
            | single         {$$ = $1;}
            ;
    single  : NUM            {$$ = $1;}
            ;

%%
void yyerror(char *str){
    fprintf(stderr,"error:%s\n",str);
}

int yywrap(){
    return 1;
}
int main()
{
    yyparse();
}