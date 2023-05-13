#include "kernel/types.h"
#include "kernel/param.h"
#include "user/user.h"
#define MAX_LEN 512

int main(int argc, char *argv[]) {
    // 参数数目不对
    if (argc - 1 >= MAXARG || argc < 2) {
        printf("error! xargs: please check the number of arguments.\n");
        exit(1);
    }
    // 存储行/定位指针和exec的参数数组
    char line[MAX_LEN];
    char *p = line;
    char *x_argv[MAXARG];
    int i;
    // 存储初始参数
    for (i = 1; i < argc; i++) {
        x_argv[i - 1] = argv[i];
    }
    //记录读取到的数据字节数
    int rsz = sizeof(char);
    while (rsz == sizeof(char)) {
        // 定位参数起始 结束位置和参数次序
        int word_begin = 0;
        int word_end = 0;
        int arg_cnt = i - 1;
        //读取一行
        while (1) {
            rsz = read(0, p, sizeof(char));
            if (++word_end >= MAX_LEN) {
                printf("xargs: arguments too long.\n");
                exit(1);
            }

            if (*p == ' ' || *p == '\n' || rsz != sizeof(char)) {
                //用字符串结束标志替换' '，'\n'和空字符
                *p = 0;
                x_argv[arg_cnt++] = &line[word_begin];
                //表明下一个参数的位置，在当前参数结束符之后
                word_begin = word_end;
                if (arg_cnt >= MAXARG) {
                    printf("xargs: too many arguments.\n");
                    exit(1);
                }
                if (*p == '\n' || rsz != sizeof(char))
                    break;
            }
            ++p;
        }
        if (fork() == 0) {
            exec(argv[1], x_argv);
        }
        wait(0);
    }
    exit(0);
}