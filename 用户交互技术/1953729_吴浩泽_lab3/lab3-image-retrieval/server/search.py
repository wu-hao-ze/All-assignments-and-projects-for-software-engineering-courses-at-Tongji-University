################################################################################################################################
# This function implements the image search/retrieval .
# inputs: Input location of uploaded image, extracted vectors
# 
################################################################################################################################
import random
import tensorflow.compat.v1 as tf
import numpy as np
import imageio
import os
import scipy.io
import time
from datetime import datetime
from scipy import ndimage

# from scipy.misc import imsave
imsave = imageio.imsave
imread = imageio.imread
from scipy.spatial.distance import cosine
# import matplotlib.pyplot as plt
from sklearn.neighbors import NearestNeighbors
import pickle
from PIL import Image
import gc
import os
from tempfile import TemporaryFile
from tensorflow.python.platform import gfile

BOTTLENECK_TENSOR_NAME = 'pool_3/_reshape:0'
BOTTLENECK_TENSOR_SIZE = 2048
MODEL_INPUT_WIDTH = 299
MODEL_INPUT_HEIGHT = 299
MODEL_INPUT_DEPTH = 3
JPEG_DATA_TENSOR_NAME = 'DecodeJpeg/contents:0'
RESIZED_INPUT_TENSOR_NAME = 'ResizeBilinear:0'
MAX_NUM_IMAGES_PER_CLASS = 2 ** 27 - 1  # ~134M


# show_neighbors(random.randint(0, len(extracted_features)), indices, neighbor_list)

def get_top_k_similar(image_data, pred, pred_final, k):
    print("total data", len(pred))
    print(image_data.shape)
    # for i in pred:
    # print(i.shape)
    # break
    os.mkdir('static/result')

    # cosine calculates the cosine distance, not similiarity. Hence no need to reverse list
    top_k_ind = np.argsort([cosine(image_data, pred_row) \
                            for ith_row, pred_row in enumerate(pred)])[:k]
    print(top_k_ind)

    for i, neighbor in enumerate(top_k_ind):
        image = imread(pred_final[neighbor].replace('\\', '/'))
        # timestr = datetime.now().strftime("%Y%m%d%H%M%S")
        # name= timestr+"."+str(i)
        name = pred_final[neighbor]
        tokens = name.split("\\")
        img_name = tokens[-1]
        print(img_name)
        name = 'static/result/' + img_name
        imsave(name, image)


def create_inception_graph():
    """"Creates a graph from saved GraphDef file and returns a Graph object.

  Returns:
    Graph holding the trained Inception network, and various tensors we'll be
    manipulating.
  """
    with tf.Session() as sess:
        model_filename = os.path.join(
            'imagenet', 'classify_image_graph_def.pb')
        with gfile.FastGFile(model_filename, 'rb') as f:
            graph_def = tf.GraphDef()
            graph_def.ParseFromString(f.read())
            bottleneck_tensor, jpeg_data_tensor, resized_input_tensor = (
                tf.import_graph_def(graph_def, name='', return_elements=[
                    BOTTLENECK_TENSOR_NAME, JPEG_DATA_TENSOR_NAME,
                    RESIZED_INPUT_TENSOR_NAME]))
    return sess.graph, bottleneck_tensor, jpeg_data_tensor, resized_input_tensor


def run_bottleneck_on_image(sess, image_data, image_data_tensor,
                            bottleneck_tensor):
    bottleneck_values = sess.run(
        bottleneck_tensor,
        {image_data_tensor: image_data})
    bottleneck_values = np.squeeze(bottleneck_values)
    return bottleneck_values


def recommend(imagePath, extracted_features):
    tf.reset_default_graph()

    config = tf.ConfigProto(
        device_count={'GPU': 0}
    )

    sess = tf.Session(config=config)
    graph, bottleneck_tensor, jpeg_data_tensor, resized_image_tensor = (create_inception_graph())
    image_data = gfile.FastGFile(imagePath, 'rb').read()
    features = run_bottleneck_on_image(sess, image_data, jpeg_data_tensor, bottleneck_tensor)

    with open('neighbor_list_recom.pickle', 'rb') as f:
        neighbor_list = pickle.load(f)
    print("loaded images")
    get_top_k_similar(features, extracted_features, neighbor_list, k=9)
