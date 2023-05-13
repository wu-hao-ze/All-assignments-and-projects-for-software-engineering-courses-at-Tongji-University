#include "kernel/types.h"
#include "user/user.h"

int main(int argc, char const *argv[]) {
    char buf = 'A'; //用于传送的字节
    // 0代表读，1代表写
    int chiToFar[2]; //子进程->父进程
    int farToChi[2]; //父进程->子进程
    pipe(chiToFar);
    pipe(farToChi);
    // 使用fork创建子进程
    int pid = fork();
    int exit_status = 0;
    // 错误情况
    if (pid < 0) {
        printf("error!\n");
        close(chiToFar[0]);
        close(chiToFar[1]);
        close(farToChi[0]);
        close(farToChi[1]);
        exit(1);
    } else if (pid == 0) { //子进程
        close(farToChi[1]);
        close(chiToFar[0]);
        if (read(farToChi[0], &buf, sizeof(char)) != sizeof(char)) {
            printf("child read() error!\n");
            exit_status = 1; //标记出错
        } else {
            printf("%d: received ping\n", getpid());
        }
        if (write(chiToFar[1], &buf, sizeof(char)) != sizeof(char)) {
            printf("child write() error!\n");
            exit_status = 1; //标记出错
        }
        close(farToChi[0]);
        close(chiToFar[1]);
        exit(exit_status);
    } else { //父进程
        close(farToChi[0]);
        close(chiToFar[1]);
        if (write(farToChi[1], &buf, sizeof(char)) != sizeof(char)) {
            printf("parent write() error!\n");
            exit_status = 1; //标记写出错
        }
        if (read(chiToFar[0], &buf, sizeof(char)) != sizeof(char)) {
            printf("parent read() error!\n");
            exit_status = 1; //标记出错
        } else {
            printf("%d: received pong\n", getpid());
        }
        close(farToChi[1]);
        close(chiToFar[0]);
        exit(exit_status);
    }
}