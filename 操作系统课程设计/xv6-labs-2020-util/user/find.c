#include "kernel/types.h"
#include "kernel/fs.h"
#include "kernel/stat.h"
#include "user/user.h"

void find(char *path, const char *filename)
{
    char buf[512], *p;
    int fd;
    struct dirent de;
    struct stat st;
    // 打开失败
    if ((fd = open(path, 0)) < 0) {
        printf("find: cannot open %s\n", path);
        return;
    }
    // 描述失败
    if (fstat(fd, &st) < 0) {
        printf("find: cannot fstat %s\n", path);
        close(fd);
        return;
    }
    //参数错误，find的第一个参数必须是目录
    if (st.type != T_DIR) {
        printf("usage: find <DIRECTORY> <filename>\n");
        return;
    }
    // 路径太长
    if (strlen(path) + 1 + DIRSIZ + 1 > sizeof buf) {
        printf("find: path too long\n");
        return;
    }
    strcpy(buf, path);
    //p指向最后一个'/'之后
    p = buf + strlen(buf);
    *p++ = '/';
    while (read(fd, &de, sizeof de) == sizeof de) {
        if (de.inum == 0)
            continue;
        //添加路径名称
        memmove(p, de.name, DIRSIZ);
        //设置字符串结束标志
        p[DIRSIZ] = 0;
        if (stat(buf, &st) < 0) {
            printf("find: cannot stat %s\n", buf);
            continue;
        }
        //防止在.和..目录中递归
        if (st.type == T_DIR && strcmp(p, ".") != 0 && strcmp(p, "..") != 0) {
            find(buf, filename);
        } else if (strcmp(filename, p) == 0)
            printf("%s\n", buf);
    }
    close(fd);
}

int main(int argc, char *argv[])
{
    // 输入参数错误
    if (argc != 3) {
        printf("error! usage: find <directory> <filename>\n");
        exit(1);
    }
    find(argv[1], argv[2]);
    exit(0);
}