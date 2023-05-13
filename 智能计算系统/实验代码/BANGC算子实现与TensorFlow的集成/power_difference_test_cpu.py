import numpy as np
import os
import time

os.environ['MLU_VISIBLE_DEVICES'] = ""
import tensorflow as tf

np.set_printoptions(suppress=True)
from power_diff_numpy import *


def power_difference_op(input_x, input_y, input_pow):
    with tf.Session() as sess:
        x = tf.placeholder(tf.float32, name='x')
        y = tf.placeholder(tf.float32, name='y')
        pow_ = tf.placeholder(tf.float32, name='pow')
        z = tf.power_difference(x, y, pow_)
        return sess.run(z, feed_dict={x: input_x, y: input_y, pow_: input_pow})


def main():
    value = 256
    input_x = np.random.randn(1, value, value, 3)
    input_y = np.random.randn(1, 1, 1, 3)
    input_pow = 2

    for i in range(1):
        start = time.time()
        res = power_difference_op(input_x, input_y, input_pow)
        end = time.time() - start
        print("comput C++ op cost " + str(end * 1000) + "ms")

    start = time.time()
    output = power_diff_numpy(input_x, input_y, input_pow)
    end = time.time()
    res = np.reshape(res, (-1))
    output = np.reshape(output, (-1))
    print("comput numpy op cost " + str((end - start) * 1000) + "ms")
    err = sum(abs(res - output)) / sum(output)
    print("err rate= " + str(err * 100))


if __name__ == '__main__':
    main()
