%{
#include <stdio.h>
#include "y.tab.h"
void yyerror(char *);
%}

%%

[1-9]+[0-9]*|0                  {yylval.num = atoi(yytext);return NUM;}
"+"							    return ADD;
"-"							    return SUB;
"*"							    return MUL;
"/"							    return DIV;
"("                             return LP;
")"                             return RP;
\n                              return CR;
[ \t]+                          ;
.                               {yyerror("illegal string");}
%%

