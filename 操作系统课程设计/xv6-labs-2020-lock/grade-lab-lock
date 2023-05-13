#!/usr/bin/env python3

import re
from gradelib import *

r = Runner(save("xv6.out"))

@test(0, "running kalloctest")
def test_kalloctest():
    r.run_qemu(shell_script([
        'kalloctest'
    ]), timeout=200)
    
@test(10, "kalloctest: test1", parent=test_kalloctest)
def test_kalloctest_test1():
    r.match('^test1 OK$')
    
@test(10, "kalloctest: test2", parent=test_kalloctest)
def test_kalloctest_test2():
    r.match('^test2 OK$')
    
@test(10, "kalloctest: sbrkmuch")
def test_sbrkmuch():
    r.run_qemu(shell_script([
        'usertests sbrkmuch'
    ]), timeout=90)
    r.match('^ALL TESTS PASSED$')

@test(0, "running bcachetest")
def test_bcachetest():
    r.run_qemu(shell_script([
        'bcachetest'
    ]), timeout=90)
    
@test(10, "bcachetest: test0", parent=test_bcachetest)
def test_bcachetest_test0():
    r.match('^test0: OK$')
    
@test(10, "bcachetest: test1", parent=test_bcachetest)
def test_bcachetest_test1():
    r.match('^test1 OK$')

@test(19, "usertests")
def test_usertests():
    r.run_qemu(shell_script([
        'usertests'
    ]), timeout=300)
    r.match('^ALL TESTS PASSED$')

@test(1, "time")
def test_time():
    check_time()

run_tests()
