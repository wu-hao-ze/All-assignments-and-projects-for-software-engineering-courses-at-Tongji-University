#include "kernel/types.h"
#include "kernel/stat.h"
#include "user/user.h"

#define N 5
char buf[N];

void
pong(int *parent_to_child, int *child_to_parent) {
  if (read(parent_to_child[0], buf, N) < 0) {
    printf("read failed\n");
  }
  printf("%d: received %s\n", getpid(), buf);
  if (write(child_to_parent[1], "pong", 4) != 4) {
    printf("write failed\n");
  }
}

void
ping(int *parent_to_child, int *child_to_parent) {
  
  if (write(parent_to_child[1], "ping", 4) != 4) {
    printf("write failed\n");
  }
  if (read(child_to_parent[0], buf, N) < 0) {
    printf("read failed\n");
  }
  printf("%d: received %s\n", getpid(), buf);
}

int
main(int argc, char *argv[])
{
  int parent_to_child[2];
  int child_to_parent[2];

  int pid;

  if (pipe(parent_to_child) < 0 || pipe(child_to_parent) < 0) {
    printf("pipe failed\n");
  }
  if ((pid = fork()) < 0) {
    printf("fork failed\n");
  }
  if (pid == 0) {
    pong(parent_to_child, child_to_parent);
  } else {
    ping(parent_to_child, child_to_parent);
  }
  
  exit(0);
}
