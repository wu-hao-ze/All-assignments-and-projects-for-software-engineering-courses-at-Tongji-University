#include <stdarg.h>

#include "types.h"
#include "param.h"
#include "spinlock.h"
#include "sleeplock.h"
#include "fs.h"
#include "file.h"
#include "riscv.h"
#include "defs.h"

static char digits[] = "0123456789abcdef";

static int
sputc(char *s, char c)
{
  *s = c;
  return 1;
}

static int
sprintint(char *s, int xx, int base, int sign)
{
  char buf[16];
  int i, n;
  uint x;

  if(sign && (sign = xx < 0))
    x = -xx;
  else
    x = xx;

  i = 0;
  do {
    buf[i++] = digits[x % base];
  } while((x /= base) != 0);

  if(sign)
    buf[i++] = '-';

  n = 0;
  while(--i >= 0)
    n += sputc(s+n, buf[i]);
  return n;
}

int
snprintf(char *buf, int sz, char *fmt, ...)
{
  va_list ap;
  int i, c;
  int off = 0;
  char *s;

  if (fmt == 0)
    panic("null fmt");

  va_start(ap, fmt);
  for(i = 0; off < sz && (c = fmt[i] & 0xff) != 0; i++){
    if(c != '%'){
      off += sputc(buf+off, c);
      continue;
    }
    c = fmt[++i] & 0xff;
    if(c == 0)
      break;
    switch(c){
    case 'd':
      off += sprintint(buf+off, va_arg(ap, int), 10, 1);
      break;
    case 'x':
      off += sprintint(buf+off, va_arg(ap, int), 16, 1);
      break;
    case 's':
      if((s = va_arg(ap, char*)) == 0)
        s = "(null)";
      for(; *s && off < sz; s++)
        off += sputc(buf+off, *s);
      break;
    case '%':
      off += sputc(buf+off, '%');
      break;
    default:
      // Print unknown % sequence to draw attention.
      off += sputc(buf+off, '%');
      off += sputc(buf+off, c);
      break;
    }
  }
  return off;
}
