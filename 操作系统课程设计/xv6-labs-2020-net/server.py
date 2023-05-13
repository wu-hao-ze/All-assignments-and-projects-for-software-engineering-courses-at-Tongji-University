import socket
import sys

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
addr = ('localhost', int(sys.argv[1]))
print('listening on %s port %s' % addr, file=sys.stderr)
sock.bind(addr)

while True:
    buf, raddr = sock.recvfrom(4096)
    print(buf.decode("utf-8"), file=sys.stderr)
    if buf:
        sent = sock.sendto(b'this is the host!', raddr)
