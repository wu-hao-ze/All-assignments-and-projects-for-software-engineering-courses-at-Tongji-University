import os
import tensorflow as tf
from tensorflow.python.platform import gfile
import argparse
import numpy as np
import cv2 as cv
import time
from power_diff_numpy import *

os.putenv('MLU_VISIBLE_DEVICES', '')


def parse_arg():
    parser = argparse.ArgumentParser()
    parser.add_argument('image')
    parser.add_argument('ori_pb')
    parser.add_argument('ori_power_diff_pb')
    parser.add_argument('numpy_pb')
    args = parser.parse_args()
    return args


def run_ori_pb():
    args = parse_arg()
    config = tf.ConfigProto(allow_soft_placement=True,
                            inter_op_parallelism_threads=1,
                            intra_op_parallelism_threads=1)
    model_name = os.path.basename(args.ori_pb).split(".")[0]
    image_name = os.path.basename(args.image).split(".")[0]

    g = tf.Graph()
    with g.as_default():
        with tf.gfile.FastGFile(args.ori_pb, 'rb') as f:
            graph_def = tf.GraphDef()
            graph_def.ParseFromString(f.read())
            tf.import_graph_def(graph_def, name='')
        img = cv.imread(args.image)
        X = cv.resize(img, (256, 256))
        print("X size : ", X.shape)
        print("X type : ", X.dtype)
        with tf.Session(config=config) as sess:
            sess.graph.as_default()
            sess.run(tf.global_variables_initializer())

            input_tensor = sess.graph.get_tensor_by_name('X_content:0')
            output_tensor = sess.graph.get_tensor_by_name('add_37:0')

            start_time = time.time()
            ret = sess.run(output_tensor, feed_dict={input_tensor: [X]})
            end_time = time.time()
            print("C++ inference(CPU) origin pb time is: ", end_time - start_time)
            img1 = tf.reshape(ret, [256, 256, 3])
            img_numpy = img1.eval(session=sess)
            cv.imwrite(image_name + '_' + model_name + '_cpu.jpg', img_numpy)


def run_ori_power_diff_pb():
    args = parse_arg()
    config = tf.ConfigProto(allow_soft_placement=True,
                            inter_op_parallelism_threads=1,
                            intra_op_parallelism_threads=1)
    model_name = os.path.basename(args.ori_power_diff_pb).split(".")[0]
    image_name = os.path.basename(args.image).split(".")[0]

    g = tf.Graph()
    with g.as_default():
        with tf.gfile.FastGFile(args.ori_power_diff_pb, 'rb') as f:
            graph_def = tf.GraphDef()
            graph_def.ParseFromString(f.read())
            tf.import_graph_def(graph_def, name='')
        img = cv.imread(args.image)
        X = cv.resize(img, (256, 256))
        with tf.Session(config=config) as sess:
            sess.graph.as_default()
            sess.run(tf.global_variables_initializer())

            input_differ = sess.graph.get_tensor_by_name('moments_15/PowerDifference_z:0')
            input_tensor = sess.graph.get_tensor_by_name('X_content:0')
            output_tensor = sess.graph.get_tensor_by_name('add_37:0')

            start_time = time.time()
            # ret =sess.run(...)
            ret = sess.run(output_tensor, feed_dict={input_differ: 2, input_tensor: [X]})
            end_time = time.time()
            print("C++ inference(CPU) time is: ", end_time - start_time)
            img1 = tf.reshape(ret, [256, 256, 3])
            img_numpy = img1.eval(session=sess)
            cv.imwrite(image_name + '_' + model_name + '_cpu.jpg', img_numpy)


def run_numpy_pb():
    args = parse_arg()
    config = tf.ConfigProto(allow_soft_placement=True,
                            inter_op_parallelism_threads=1,
                            intra_op_parallelism_threads=1)
    model_name = os.path.basename(args.numpy_pb).split(".")[0]
    image_name = os.path.basename(args.image).split(".")[0]

    g = tf.Graph()
    with g.as_default():
        with tf.gfile.FastGFile(args.numpy_pb, 'rb') as f:
            graph_def = tf.GraphDef()
            graph_def.ParseFromString(f.read())
            tf.import_graph_def(graph_def, name='')
        img = cv.imread(args.image)
        X = cv.resize(img, (256, 256))
        with tf.Session(config=config) as sess:
            sess.graph.as_default()
            sess.run(tf.global_variables_initializer())

            input_tensor = sess.graph.get_tensor_by_name('X_content:0')
            input_differ = sess.graph.get_tensor_by_name('moments_15/PowerDifference:0')
            output_tensor = sess.graph.get_tensor_by_name('add_37:0')
            conv_tensor = sess.graph.get_tensor_by_name('Conv2D_13:0')
            grad_tensor = sess.graph.get_tensor_by_name('moments_15/StopGradient:0')
            temp_feed = {input_differ: [X], input_tensor: [X]}
            conv = sess.run(conv_tensor, feed_dict=temp_feed)
            grad = sess.run(grad_tensor, feed_dict=temp_feed)
            # conv = conv.eval(session=sess)
            # grad = grad.eval(session=sess)
            differ = power_diff_numpy(conv, grad, 2)
            start_time = time.time()
            # ret =sess.run(...)
            ret = sess.run(output_tensor, feed_dict={input_differ: differ, input_tensor: [X]})
            # start_time = time.time()
            # ret = sess.run(...)
            end_time = time.time()
            print("Numpy inference(CPU) time is: ", end_time - start_time)
            img1 = tf.reshape(ret, [256, 256, 3])
            img_numpy = img1.eval(session=sess)
            cv.imwrite(image_name + '_' + model_name + '_cpu.jpg', img_numpy)


if __name__ == '__main__':
    run_ori_pb()
    run_ori_power_diff_pb()
    run_numpy_pb()
