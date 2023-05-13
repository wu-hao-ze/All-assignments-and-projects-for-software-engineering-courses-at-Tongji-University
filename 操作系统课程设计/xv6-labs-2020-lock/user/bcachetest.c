#include "kernel/fcntl.h"
#include "kernel/param.h"
#include "kernel/types.h"
#include "kernel/stat.h"
#include "kernel/riscv.h"
#include "kernel/fs.h"
#include "user/user.h"

void test0();
void test1();

#define SZ 4096
char buf[SZ];

int
main(int argc, char *argv[])
{
  test0();
  test1();
  exit(0);
}

void
createfile(char *file, int nblock)
{
  int fd;
  char buf[BSIZE];
  int i;
  
  fd = open(file, O_CREATE | O_RDWR);
  if(fd < 0){
    printf("createfile %s failed\n", file);
    exit(-1);
  }
  for(i = 0; i < nblock; i++) {
    if(write(fd, buf, sizeof(buf)) != sizeof(buf)) {
      printf("write %s failed\n", file);
      exit(-1);
    }
  }
  close(fd);
}

void
readfile(char *file, int nbytes, int inc)
{
  char buf[BSIZE];
  int fd;
  int i;

  if(inc > BSIZE) {
    printf("readfile: inc too large\n");
    exit(-1);
  }
  if ((fd = open(file, O_RDONLY)) < 0) {
    printf("readfile open %s failed\n", file);
    exit(-1);
  }
  for (i = 0; i < nbytes; i += inc) {
    if(read(fd, buf, inc) != inc) {
      printf("read %s failed for block %d (%d)\n", file, i, nbytes);
      exit(-1);
    }
  }
  close(fd);
}

int ntas(int print)
{
  int n;
  char *c;

  if (statistics(buf, SZ) <= 0) {
    fprintf(2, "ntas: no stats\n");
  }
  c = strchr(buf, '=');
  n = atoi(c+2);
  if(print)
    printf("%s", buf);
  return n;
}

void
test0()
{
  char file[2];
  char dir[2];
  enum { N = 10, NCHILD = 3 };
  int m, n;

  dir[0] = '0';
  dir[1] = '\0';
  file[0] = 'F';
  file[1] = '\0';

  printf("start test0\n");
  for(int i = 0; i < NCHILD; i++){
    dir[0] = '0' + i;
    mkdir(dir);
    if (chdir(dir) < 0) {
      printf("chdir failed\n");
      exit(1);
    }
    unlink(file);
    createfile(file, N);
    if (chdir("..") < 0) {
      printf("chdir failed\n");
      exit(1);
    }
  }
  m = ntas(0);
  for(int i = 0; i < NCHILD; i++){
    dir[0] = '0' + i;
    int pid = fork();
    if(pid < 0){
      printf("fork failed");
      exit(-1);
    }
    if(pid == 0){
      if (chdir(dir) < 0) {
        printf("chdir failed\n");
        exit(1);
      }

      readfile(file, N*BSIZE, 1);

      exit(0);
    }
  }

  for(int i = 0; i < NCHILD; i++){
    wait(0);
  }
  printf("test0 results:\n");
  n = ntas(1);
  if (n-m < 500)
    printf("test0: OK\n");
  else
    printf("test0: FAIL\n");
}

void test1()
{
  char file[3];
  enum { N = 100, BIG=100, NCHILD=2 };
  
  printf("start test1\n");
  file[0] = 'B';
  file[2] = '\0';
  for(int i = 0; i < NCHILD; i++){
    file[1] = '0' + i;
    unlink(file);
    if (i == 0) {
      createfile(file, BIG);
    } else {
      createfile(file, 1);
    }
  }
  for(int i = 0; i < NCHILD; i++){
    file[1] = '0' + i;
    int pid = fork();
    if(pid < 0){
      printf("fork failed");
      exit(-1);
    }
    if(pid == 0){
      if (i==0) {
        for (i = 0; i < N; i++) {
          readfile(file, BIG*BSIZE, BSIZE);
        }
        unlink(file);
        exit(0);
      } else {
        for (i = 0; i < N; i++) {
          readfile(file, 1, BSIZE);
        }
        unlink(file);
      }
      exit(0);
    }
  }

  for(int i = 0; i < NCHILD; i++){
    wait(0);
  }
  printf("test1 OK\n");
}
