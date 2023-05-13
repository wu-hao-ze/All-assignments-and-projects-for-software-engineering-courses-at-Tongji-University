#include "kernel/types.h"
#include "user/user.h"

void transmit_data(int lpipe[2], int rpipe[2], int first)
{
    int data;
    // 从左管道读取数据
    while (read(lpipe[0], &data, sizeof(int)) == sizeof(int)) {
        // 将无法整除的数据传递入右管道
        if (data % first)
            write(rpipe[1], &data, sizeof(int));
    }
    close(lpipe[0]);
    close(rpipe[1]);
}

// 递归寻找素数的函数
void primes(int lpipe[2])
{
    close(lpipe[1]);
    int first;
    if (read(lpipe[0], &first, sizeof(int)) != sizeof(int)) {
        exit(0);
    }
    printf("prime %d\n", first);
    // 当前的管道，接受左管道传过来的数据
    int p[2];
    pipe(p);
    // 读取左邻居的数据，将不能被first整除的写入右邻居
    transmit_data(lpipe, p, first);
    // 递归调用这一过程
    if (fork() == 0) {
        primes(p);
    } else {
        close(p[0]);
        wait(0);
    }
    exit(0);
}

int main(int argc, char const *argv[])
{
    int p[2];
    pipe(p);
    // 写入初始数据，为int的大小
    for (int i = 2; i <= 35; i++)
        write(p[1], &i, sizeof(int));
    // 针对父子进程不同的操作
    if (fork() == 0) {
        primes(p);
    } else {
        close(p[1]);
        close(p[0]);
        wait(0);
    }

    exit(0);
}