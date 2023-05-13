#include <stdarg.h>

#include "types.h"
#include "param.h"
#include "spinlock.h"
#include "sleeplock.h"
#include "fs.h"
#include "file.h"
#include "riscv.h"
#include "defs.h"

#define BUFSZ 4096
static struct {
  struct spinlock lock;
  char buf[BUFSZ];
  int sz;
  int off;
} stats;

int statscopyin(char*, int);
int statslock(char*, int);
  
int
statswrite(int user_src, uint64 src, int n)
{
  return -1;
}

int
statsread(int user_dst, uint64 dst, int n)
{
  int m;

  acquire(&stats.lock);

  if(stats.sz == 0) {
#ifdef LAB_PGTBL
    stats.sz = statscopyin(stats.buf, BUFSZ);
#endif
#ifdef LAB_LOCK
    stats.sz = statslock(stats.buf, BUFSZ);
#endif
  }
  m = stats.sz - stats.off;

  if (m > 0) {
    if(m > n)
      m  = n;
    if(either_copyout(user_dst, dst, stats.buf+stats.off, m) != -1) {
      stats.off += m;
    }
  } else {
    m = -1;
    stats.sz = 0;
    stats.off = 0;
  }
  release(&stats.lock);
  return m;
}

void
statsinit(void)
{
  initlock(&stats.lock, "stats");

  devsw[STATS].read = statsread;
  devsw[STATS].write = statswrite;
}

