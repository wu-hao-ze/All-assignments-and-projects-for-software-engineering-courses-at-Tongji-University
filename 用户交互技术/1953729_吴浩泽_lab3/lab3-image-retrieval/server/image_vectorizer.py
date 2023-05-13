################################################################################################################################
# This file is used to extract features from dataset and save it on disc
# inputs: 
# outputs: 
################################################################################################################################


import random
import tensorflow.compat.v1 as tf
#tf.disable_v2_behavior()
import numpy as np
import os
#from scipy import ndimage
#from scipy.spatial.distance import cosine
#import matplotlib.pyplot as plt
#from sklearn.neighbors import NearestNeighbors
import pickle
from tensorflow.python.platform import gfile

BOTTLENECK_TENSOR_NAME = 'pool_3/_reshape:0'
BOTTLENECK_TENSOR_SIZE = 2048
MODEL_INPUT_WIDTH = 299
MODEL_INPUT_HEIGHT = 299
MODEL_INPUT_DEPTH = 3
JPEG_DATA_TENSOR_NAME = 'DecodeJpeg/contents:0'
RESIZED_INPUT_TENSOR_NAME = 'ResizeBilinear:0'
MAX_NUM_IMAGES_PER_CLASS = 2 ** 27 - 1  # ~134M

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

def iter_files(rootDir):
    all_files = []
    for root, dirs, files in os.walk(rootDir):
        for file in files:
            file_name = os.path.join(root, file)
            all_files.append(file_name)
        for dirname in dirs:
            iter_files(dirname)
    return all_files

# Get outputs from second-to-last layer in pre-built model

img_files = iter_files('database/dataset')
#sandals_files = iter_files('uploads/dogs_and_cats/Sandals')
#shoes_files = iter_files('uploads/dogs_and_cats/Shoes')
#slippers_files = iter_files('uploads/dogs_and_cats/Slippers')
#apparel_files = iter_files('uploads/dogs_and_cats/apparel')

all_files = img_files#boots_files + shoes_files + slippers_files + sandals_files + apparel_files

random.shuffle(all_files)

num_images = 10000
neighbor_list = all_files[:num_images]
with open('neighbor_list_recom.pickle','wb') as f:
        pickle.dump(neighbor_list,f)
print("saved neighbour list")

extracted_features = np.ndarray((num_images, 2048))
sess = tf.Session()
graph, bottleneck_tensor, jpeg_data_tensor, resized_image_tensor = (create_inception_graph())	


for i, filename in enumerate(neighbor_list):

    image_data = gfile.FastGFile(filename, 'rb').read()
    features = run_bottleneck_on_image(sess, image_data, jpeg_data_tensor, bottleneck_tensor)

    extracted_features[i:i+1] = features

    if i % 250 == 0:
        print(i)


np.savetxt("saved_features_recom.txt", extracted_features)
print("saved exttracted features")






