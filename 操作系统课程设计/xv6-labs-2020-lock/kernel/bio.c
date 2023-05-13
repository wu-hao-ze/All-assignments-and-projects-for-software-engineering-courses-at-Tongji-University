// Buffer cache.
//
// The buffer cache is a linked list of buf structures holding
// cached copies of disk block contents.  Caching disk blocks
// in memory reduces the number of disk reads and also provides
// a synchronization point for disk blocks used by multiple processes.
//
// Interface:
// * To get a buffer for a particular disk block, call bread.
// * After changing buffer data, call bwrite to write it to disk.
// * When done with the buffer, call brelse.
// * Do not use the buffer after calling brelse.
// * Only one process at a time can use a buffer,
//     so do not keep them longer than necessary.


#include "types.h"
#include "param.h"
#include "spinlock.h"
#include "sleeplock.h"
#include "riscv.h"
#include "defs.h"
#include "fs.h"
#include "buf.h"

// 哈希表bucket的数目
#define NBUCKET 13
#define HASH(blockno) (blockno % NBUCKET)
extern uint ticks;

struct {
  struct spinlock lock;
  struct buf buf[NBUF];
  // 记录已经分配到哈希表的缓存块 struct buf 的数量
  int size;
  // 哈希表的 bucket 数组
  struct buf buckets[NBUCKET];
  // 作为每个 bucket 对应的锁
  struct spinlock locks[NBUCKET];
  // 哈希表的全局锁, 用于对哈希表整体加锁
  struct spinlock hashlock;

  // Linked list of all buffers, through prev/next.
  // Sorted by how recently the buffer was used.
  // head.next is most recent, head.prev is least.
  // 去除head
  // struct buf head;
} bcache;

void
binit(void)
{
  int i;
  struct buf *b;

  bcache.size = 0;
  initlock(&bcache.lock, "bcache");
  // 初始化哈希锁
  initlock(&bcache.hashlock, "bcache_hash");
  for(i = 0; i < NBUCKET; ++i) {
    initlock(&bcache.locks[i], "bcache_bucket");
  }

  for(b = bcache.buf; b < bcache.buf+NBUF; b++){
    initsleeplock(&b->lock, "buffer");
  }
}

// Look through buffer cache for block on device dev.
// If not found, allocate a buffer.
// In either case, return locked buffer.
static struct buf*
bget(uint dev, uint blockno)
{
  struct buf *b;
  // 定义一些变量
  int idx = HASH(blockno);
  struct buf *pre, *minb = 0, *minpre;
  uint mintimestamp;
  int i;

  // 在buckets[idx]里循环buf
  acquire(&bcache.locks[idx]);
  for(b = bcache.buckets[idx].next; b; b = b->next){
    if(b->dev == dev && b->blockno == blockno){
      b->refcnt++;
      release(&bcache.locks[idx]);
      acquiresleep(&b->lock);
      return b;
    }
  }

  // Not cached.
  // check if there is a buf not used -lab8-2
  acquire(&bcache.lock);
  if(bcache.size < NBUF) {
    b = &bcache.buf[bcache.size++];
    release(&bcache.lock);
    b->dev = dev;
    b->blockno = blockno;
    b->valid = 0;
    b->refcnt = 1;
    b->next = bcache.buckets[idx].next;
    bcache.buckets[idx].next = b;
    release(&bcache.locks[idx]);
    acquiresleep(&b->lock);
    return b;
  }
  release(&bcache.lock);
  release(&bcache.locks[idx]);

  // select the last-recently used block int the bucket
  //based on the timestamp - lab8-2
  acquire(&bcache.hashlock);
  for(i = 0; i < NBUCKET; ++i) {
      mintimestamp = -1;
      acquire(&bcache.locks[idx]);
      for(pre = &bcache.buckets[idx], b = pre->next; b; pre = b, b = b->next) {
          // research the block
          if(idx == HASH(blockno) && b->dev == dev && b->blockno == blockno){
              b->refcnt++;
              release(&bcache.locks[idx]);
              release(&bcache.hashlock);
              acquiresleep(&b->lock);
              return b;
          }
          if(b->refcnt == 0 && b->timestamp < mintimestamp) {
              minb = b;
              minpre = pre;
              mintimestamp = b->timestamp;
          }
      }
      // find an unused block
      if(minb) {
          minb->dev = dev;
          minb->blockno = blockno;
          minb->valid = 0;
          minb->refcnt = 1;
          // if block in another bucket, we should move it to correct bucket
          if(idx != HASH(blockno)) {
              minpre->next = minb->next;    // remove block
              release(&bcache.locks[idx]);
              idx = HASH(blockno);  // the correct bucket index
              acquire(&bcache.locks[idx]);
              minb->next = bcache.buckets[idx].next;    // move block to correct bucket
              bcache.buckets[idx].next = minb;
          }
          release(&bcache.locks[idx]);
          release(&bcache.hashlock);
          acquiresleep(&minb->lock);
          return minb;
      }
      release(&bcache.locks[idx]);
      if(++idx == NBUCKET) {
          idx = 0;
      }
  }
  panic("bget: no buffers");
}

// Return a locked buf with the contents of the indicated block.
struct buf*
bread(uint dev, uint blockno)
{
  struct buf *b;

  b = bget(dev, blockno);
  if(!b->valid) {
    virtio_disk_rw(b, 0);
    b->valid = 1;
  }
  return b;
}

// Write b's contents to disk.  Must be locked.
void
bwrite(struct buf *b)
{
  if(!holdingsleep(&b->lock))
    panic("bwrite");
  virtio_disk_rw(b, 1);
}

// Release a locked buffer.
// Move to the head of the most-recently-used list.
void
brelse(struct buf *b)
{
  int idx;
  if(!holdingsleep(&b->lock))
    panic("brelse");

  releasesleep(&b->lock);

  // 改变锁
  idx = HASH(b->blockno);
  acquire(&bcache.locks[idx]);
  b->refcnt--;
  if (b->refcnt == 0) {
    // 改变时间戳记录的缓存块
    b->timestamp = ticks;
  }
  
  release(&bcache.locks[idx]);
}

void
bpin(struct buf *b) {
  // 改变锁，将原本的全局锁替换为缓存块对应的 bucket 的锁
  int idx = HASH(b->blockno);
  acquire(&bcache.locks[idx]);
  b->refcnt++;
  release(&bcache.locks[idx]);
}

void
bunpin(struct buf *b) {
  // 改变锁，将原本的全局锁替换为缓存块对应的 bucket 的锁
  int idx = HASH(b->blockno);
  acquire(&bcache.locks[idx]);
  b->refcnt--;
  release(&bcache.locks[idx]);
}