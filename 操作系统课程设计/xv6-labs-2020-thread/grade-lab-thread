#!/usr/bin/env python3

import re
import subprocess
from gradelib import *


r = Runner(save("xv6.out"))

@test(20, "uthread")
def test_uthread():
    r.run_qemu(shell_script([
        'uthread'
    ]))
    expected = ['thread_a started', 'thread_b started', 'thread_c started']
    expected.extend(['thread_%s %d' % (tid, n) for n in range(100) for tid in ('c', 'a', 'b')])
    expected.extend(['thread_c: exit after 100', 'thread_a: exit after 100', 'thread_b: exit after 100'])
    expected.append('thread_schedule: no runnable threads')
    if not re.findall('\n'.join(expected), r.qemu.output, re.M):
        raise AssertionError('Output does not match expected output')

@test(5, "answers-thread.txt")
def test_answers():
    # just a simple sanity check, will be graded manually
    check_answers("answers-thread.txt")
    
# test the first ph task: add locks to eliminate the missing keys.
@test(10, "ph_safe")
def test_ph_safe():
    subprocess.run(['make', 'ph'], check=True)
    result = subprocess.run(['./ph', '2'], stdout=subprocess.PIPE, check=True)
    out = result.stdout.decode("utf-8")
    matches = re.findall(r'^\d+: (\d+) keys missing$', out, re.MULTILINE)
    assert_equal(len(matches), 2)
    assert_equal(int(matches[0]), 0)
    assert_equal(int(matches[1]), 0)

# test the second ph task: locking that allows put() parallelism
@test(10, "ph_fast")
def test_ph_fast():
    subprocess.run(['make', 'ph'], check=True)
    result = subprocess.run(['./ph', '2'], stdout=subprocess.PIPE, check=True)
    out = result.stdout.decode("utf-8")
    rate2 = re.findall(r' (\d+) puts.second$', out, re.MULTILINE)
    assert_equal(len(rate2), 1)
    result = subprocess.run(['./ph', '1'], stdout=subprocess.PIPE)
    out = result.stdout.decode("utf-8")
    rate1 = re.findall(r' (\d+) puts.second$', out, re.MULTILINE)
    assert_equal(len(rate1), 1)
    rate1 = float(rate1[0])
    rate2 = float(rate2[0])
    # demand that 2 threads yield at least 1.25x the
    # throughput of a single thread.
    if rate2 < 1.25 * rate1:
        raise AssertionError('Parallel put() speedup is less than 1.25x')

@test(14, "barrier")
def test_barrier():
    subprocess.run(['make', 'barrier'])
    result = subprocess.run(['./barrier', '2'], stdout=subprocess.PIPE)
    out = result.stdout.decode("utf-8")
    if not re.match(r'^OK; passed$', out):
        raise AssertionError('Barrier failed')

@test(1, "time")
def test_time():
    check_time()

run_tests()

