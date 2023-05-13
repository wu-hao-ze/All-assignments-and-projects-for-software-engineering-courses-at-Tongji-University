#!/usr/bin/env python3

import re
import subprocess
from gradelib import *

r = Runner(save("xv6.out"))

@test(0, "running nettests")
def test_nettest():
    server = subprocess.Popen(["make", "server"], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    r.run_qemu(shell_script([
        'nettests'
    ]), timeout=30)
    server.terminate()
    server.communicate()

@test(40, "nettest: ping", parent=test_nettest)
def test_nettest_():
    r.match('^testing ping: OK$')

@test(20, "nettest: single process", parent=test_nettest)
def test_nettest_():
    r.match('^testing single-process pings: OK$')

@test(20, "nettest: multi-process", parent=test_nettest)
def test_nettest_fork_test():
    r.match('^testing multi-process pings: OK$')

@test(19, "nettest: DNS", parent=test_nettest)
def test_nettest_dns_test():
    r.match('^DNS OK$')

#@test(10, "answers-net.txt")
#def test_answers():
#    # just a simple sanity check, will be graded manually
#    check_answers("answers-net.txt")

@test(1, "time")
def test_time():
    check_time()

run_tests()
