%{
#include<stdio.h>
void print_out(char type);
%}


digit [0-9]
letter [a-zA-Z]
identifier {letter}({letter}|{digit})*
integer {digit}{digit}*
whitespace [ |\n|\t]*
illegal ([^a-zA-Z0-9\t\n>=<:\'\(\)\;\[\]\.\!\+\-\*\/\"\,]+)

%%
PROGRAME|BEGIN|END|CONST|VAR|WHILE|DO|IF|THEN {print_out('k');}

{identifier} {
  /*检查长度是否超出范围*/
  if(yyleng > 20){
    printf("Error! The length of this identifier is too long!\n");
    exit(0);
  }
  print_out('i');
}

{integer} {
  print_out('d');
}

{whitespace} {}
"+"|"-"|"*"|"/"|":="|"="|"<>"|">"|">="|"<"|"<="|"("|")"|";"|"," {print_out('c');};

{illegal} {
  printf("<%s,error>\n",yytext);
}
%%

void print_out(char type)
{
  printf("<%s,%c>\n",yytext,type);
}

int yywrap()
{
	return 1;
}

int main(int argc, char **argv)
{
  yylex();
  yywrap();
}

