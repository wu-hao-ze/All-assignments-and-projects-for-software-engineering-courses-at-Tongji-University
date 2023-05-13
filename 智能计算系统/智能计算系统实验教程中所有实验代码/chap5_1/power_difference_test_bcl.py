# coding: utf-8
import numpy as np
import os
import time
os.environ['MLU_VISIBLE_DEVICES']="0"
import tensorflow as tf
np.set_printoptions(suppress=True)
from power_diff_numpy import *

def power_difference_op(input_x,input_y,input_pow):
    with tf.Session() as sess:
      # 完成TensorFlow接口调用
      placeholder_x = tf.placeholder(tf.float32, shape=input_x.shape, name='placeholder_x')
      placeholder_y = tf.placeholder(tf.float32, shape=input_y.shape, name='placeholder_y')
      placeholder_z = tf.placeholder(tf.float32, name='placeholder_z')
      out = tf.power_difference(placeholder_x, placeholder_y, placeholder_z)
      return sess.run(out, feed_dict = {placeholder_x:input_x, placeholder_y:input_y, placeholder_z:input_pow})

def main():
    value = 256
    input_x = np.random.randn(1,value,value,3)
    input_y = np.random.randn(1,1,1,3)
    input_pow = np.zeros((2))

    start = time.time()
    res = power_difference_op(input_x, input_y, input_pow)
    end = time.time() - start
    print("comput BCL op cost "+ str(end*1000) + "ms" )

    start = time.time()
    output = power_diff_numpy(input_x, input_y,len(input_pow))
    end = time.time()
    res = np.reshape(res,(-1))
    output = np.reshape(output,(-1))
    print("comput op cost "+ str((end-start)*1000) + "ms" )
    err = sum(abs(res - output))/sum(output)
    print("err rate= "+ str(err*100))


if __name__ == '__main__':
    main()
