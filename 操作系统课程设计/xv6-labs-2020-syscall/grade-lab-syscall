#!/usr/bin/env python

import re
from gradelib import *

r = Runner(save("xv6.out"))

@test(5, "trace 32 grep")
def test_trace_32_grep():
    r.run_qemu(shell_script([
        'trace 32 grep hello README'
    ]))
    r.match('^\\d+: syscall read -> 1023')
    r.match('^\\d+: syscall read -> 966')
    r.match('^\\d+: syscall read -> 0')

@test(5, "trace all grep")
def test_trace_all_grep():
    r.run_qemu(shell_script([
        'trace 2147483647 grep hello README'
    ]))
    r.match('^\\d+: syscall trace -> 0')
    r.match('^\\d+: syscall exec -> 3')
    r.match('^\\d+: syscall open -> 3')
    r.match('^\\d+: syscall read -> 1023')
    r.match('^\\d+: syscall read -> 966')
    r.match('^\\d+: syscall read -> 0')
    r.match('^\\d+: syscall close -> 0')

@test(5, "trace nothing")
def test_trace_nothing():
    r.run_qemu(shell_script([
        'grep hello README'
    ]))
    r.match(no=[".* syscall .*"])

@test(5, "trace children")
def test_trace_children():
    r.run_qemu(shell_script([
        'trace 2 usertests forkforkfork'
    ]))
    r.match('3: syscall fork -> 4')
    r.match('^5: syscall fork -> \\d+')
    r.match('^6: syscall fork -> \\d+')
    r.match('^\\d+: syscall fork -> -1')
    r.match('^ALL TESTS PASSED')

@test(14, "sysinfotest")
def test_sysinfotest():
    r.run_qemu(shell_script([
        'sysinfotest'
    ]))
    r.match('^sysinfotest: OK', no=[".* FAIL .*"])

@test(1, "time")
def test_time():
    check_time()

run_tests()



