struct buf {
  int valid;   // has data been read from disk?
  int disk;    // does disk "own" buf?
  uint dev;
  uint blockno;
  struct sleeplock lock;
  uint refcnt;
  // 不再使用双向链表，删去
  // struct buf *prev; // LRU cache list
  // 使用这个来当哈希表
  struct buf *next;
  uchar data[BSIZE];
  // 记录最后使用缓存块的时间
  uint timestamp;
};