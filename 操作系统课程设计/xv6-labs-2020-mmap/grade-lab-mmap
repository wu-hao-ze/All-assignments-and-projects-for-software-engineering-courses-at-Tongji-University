#!/usr/bin/env python3

import re
from gradelib import *

r = Runner(save("xv6.out"))

@test(0, "running mmaptest")
def test_mmaptest():
    r.run_qemu(shell_script([
        'mmaptest'
    ]), timeout=180)

@test(20, "mmaptest: mmap f", parent=test_mmaptest)
def test_mmaptest_mmap_f():
    r.match('^test mmap f: OK$')

@test(10, "mmaptest: mmap private", parent=test_mmaptest)
def test_mmaptest_mmap_private():
    r.match('^test mmap private: OK$')

@test(10, "mmaptest: mmap read-only", parent=test_mmaptest)
def test_mmaptest_mmap_readonly():
    r.match('^test mmap read-only: OK$')

@test(10, "mmaptest: mmap read/write", parent=test_mmaptest)
def test_mmaptest_mmap_readwrite():
    r.match('^test mmap read/write: OK$')

@test(10, "mmaptest: mmap dirty", parent=test_mmaptest)
def test_mmaptest_mmap_dirty():
    r.match('^test mmap dirty: OK$')

@test(10, "mmaptest: not-mapped unmap", parent=test_mmaptest)
def test_mmaptest_mmap_unmap():
    r.match('^test not-mapped unmap: OK$')

@test(10, "mmaptest: two files", parent=test_mmaptest)
def test_mmaptest_mmap_two():
    r.match('^test mmap two files: OK$')

@test(40, "mmaptest: fork_test", parent=test_mmaptest)
def test_mmaptest_fork_test():
    r.match('^fork_test OK$')

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
