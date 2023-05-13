#include "kernel/types.h"
#include "kernel/net.h"
#include "kernel/stat.h"
#include "user/user.h"

//
// send a UDP packet to the localhost (outside of qemu),
// and receive a response.
//
static void
ping(uint16 sport, uint16 dport, int attempts)
{
  int fd;
  char *obuf = "a message from xv6!";
  uint32 dst;

  // 10.0.2.2, which qemu remaps to the external host,
  // i.e. the machine you're running qemu on.
  dst = (10 << 24) | (0 << 16) | (2 << 8) | (2 << 0);

  // you can send a UDP packet to any Internet address
  // by using a different dst.
  
  if((fd = connect(dst, sport, dport)) < 0){
    fprintf(2, "ping: connect() failed\n");
    exit(1);
  }

  for(int i = 0; i < attempts; i++) {
    if(write(fd, obuf, strlen(obuf)) < 0){
      fprintf(2, "ping: send() failed\n");
      exit(1);
    }
  }

  char ibuf[128];
  int cc = read(fd, ibuf, sizeof(ibuf)-1);
  if(cc < 0){
    fprintf(2, "ping: recv() failed\n");
    exit(1);
  }

  close(fd);
  ibuf[cc] = '\0';
  if(strcmp(ibuf, "this is the host!") != 0){
    fprintf(2, "ping didn't receive correct payload\n");
    exit(1);
  }
}

// Encode a DNS name
static void
encode_qname(char *qn, char *host)
{
  char *l = host; 
  
  for(char *c = host; c < host+strlen(host)+1; c++) {
    if(*c == '.') {
      *qn++ = (char) (c-l);
      for(char *d = l; d < c; d++) {
        *qn++ = *d;
      }
      l = c+1; // skip .
    }
  }
  *qn = '\0';
}

// Decode a DNS name
static void
decode_qname(char *qn)
{
  while(*qn != '\0') {
    int l = *qn;
    if(l == 0)
      break;
    for(int i = 0; i < l; i++) {
      *qn = *(qn+1);
      qn++;
    }
    *qn++ = '.';
  }
}

// Make a DNS request
static int
dns_req(uint8 *obuf)
{
  int len = 0;
  
  struct dns *hdr = (struct dns *) obuf;
  hdr->id = htons(6828);
  hdr->rd = 1;
  hdr->qdcount = htons(1);
  
  len += sizeof(struct dns);
  
  // qname part of question
  char *qname = (char *) (obuf + sizeof(struct dns));
  char *s = "pdos.csail.mit.edu.";
  encode_qname(qname, s);
  len += strlen(qname) + 1;

  // constants part of question
  struct dns_question *h = (struct dns_question *) (qname+strlen(qname)+1);
  h->qtype = htons(0x1);
  h->qclass = htons(0x1);

  len += sizeof(struct dns_question);
  return len;
}

// Process DNS response
static void
dns_rep(uint8 *ibuf, int cc)
{
  struct dns *hdr = (struct dns *) ibuf;
  int len;
  char *qname = 0;
  int record = 0;

  if(!hdr->qr) {
    exit(1);
    printf("Not a DNS response for %d\n", ntohs(hdr->id));
  }

  if(hdr->id != htons(6828))
    printf("DNS wrong id: %d\n", ntohs(hdr->id));
  
  if(hdr->rcode != 0) {
    printf("DNS rcode error: %x\n", hdr->rcode);
    exit(1);
  }
  
  //printf("qdcount: %x\n", ntohs(hdr->qdcount));
  //printf("ancount: %x\n", ntohs(hdr->ancount));
  //printf("nscount: %x\n", ntohs(hdr->nscount));
  //printf("arcount: %x\n", ntohs(hdr->arcount));
  
  len = sizeof(struct dns);

  for(int i =0; i < ntohs(hdr->qdcount); i++) {
    char *qn = (char *) (ibuf+len);
    qname = qn;
    decode_qname(qn);
    len += strlen(qn)+1;
    len += sizeof(struct dns_question);
  }

  for(int i = 0; i < ntohs(hdr->ancount); i++) {
    char *qn = (char *) (ibuf+len);
    
    if((int) qn[0] > 63) {  // compression?
      qn = (char *)(ibuf+qn[1]);
      len += 2;
    } else {
      decode_qname(qn);
      len += strlen(qn)+1;
    }
      
    struct dns_data *d = (struct dns_data *) (ibuf+len);
    len += sizeof(struct dns_data);
    // printf("type %d ttl %d len %d\n", ntohs(d->type), ntohl(d->ttl), ntohs(d->len));
    if(ntohs(d->type) == ARECORD && ntohs(d->len) == 4) {
      record = 1;
      printf("DNS arecord for %s is ", qname ? qname : "" );
      uint8 *ip = (ibuf+len);
      printf("%d.%d.%d.%d\n", ip[0], ip[1], ip[2], ip[3]);
      if(ip[0] != 128 || ip[1] != 52 || ip[2] != 129 || ip[3] != 126) {
        printf("wrong ip address");
        exit(1);
      }
      len += 4;
    }
  }

  if(len != cc) {
    printf("Processed %d data bytes but received %d\n", len, cc);
    exit(1);
  }
  if(!record) {
    printf("Didn't receive an arecord\n");
    exit(1);
  }
}

static void
dns()
{
  #define N 1000
  uint8 obuf[N];
  uint8 ibuf[N];
  uint32 dst;
  int fd;
  int len;

  memset(obuf, 0, N);
  memset(ibuf, 0, N);
  
  // 8.8.8.8: google's name server
  dst = (8 << 24) | (8 << 16) | (8 << 8) | (8 << 0);

  if((fd = connect(dst, 10000, 53)) < 0){
    fprintf(2, "ping: connect() failed\n");
    exit(1);
  }

  len = dns_req(obuf);
  
  if(write(fd, obuf, len) < 0){
    fprintf(2, "dns: send() failed\n");
    exit(1);
  }
  int cc = read(fd, ibuf, sizeof(ibuf));
  if(cc < 0){
    fprintf(2, "dns: recv() failed\n");
    exit(1);
  }
  dns_rep(ibuf, cc);

  close(fd);
}  

int
main(int argc, char *argv[])
{
  int i, ret;
  uint16 dport = NET_TESTS_PORT;

  printf("nettests running on port %d\n", dport);

  printf("testing ping: ");
  ping(2000, dport, 1);
  printf("OK\n");

  printf("testing single-process pings: ");
  for (i = 0; i < 100; i++)
    ping(2000, dport, 1);
  printf("OK\n");

  printf("testing multi-process pings: ");
  for (i = 0; i < 10; i++){
    int pid = fork();
    if (pid == 0){
      ping(2000 + i + 1, dport, 1);
      exit(0);
    }
  }
  for (i = 0; i < 10; i++){
    wait(&ret);
    if (ret != 0)
      exit(1);
  }
  printf("OK\n");
  
  printf("testing DNS\n");
  dns();
  printf("DNS OK\n");
  
  printf("all tests passed.\n");
  exit(0);
}
