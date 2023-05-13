/*************************************************************************
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *************************************************************************/
 
#include "cnml.h"
#include "cnrt.h"
#include "stdlib.h"
#include <stdio.h>
#include <sys/time.h>
#include <string.h>
#include <assert.h>
#include <vector>
#include <iostream>
#include <fstream>
#include <memory.h>
#include <algorithm>
#include <cmath>

#ifndef CNPLUGIN_H_
#define CNPLUGIN_H_

#define CNPLUGIN_MAJOR_VERSION 0
#define CNPLUGIN_MINOR_VERSION 0
#define CNPLUGIN_PATCH_VERSION 0

using std::vector;
typedef uint16_t half;

#define CNPLUGIN_VERSION (CNPLUGIN_MAJOR_VERSION * 10000 + CNPLUGIN_MINOR_VERSION * 100 + CNPLUGIN_PATCH_VERSION)

/* ====================== */
/* enum definitions start */
/* ====================== */
/*!
 *  @enum cnmlPluginSsdCodeType_t
 *  @breif An enum.
 *
 *  ``cnmlPluginSsdCodeType_t`` is an enum holding the description of CodeType
 *  used in PluginSsdDetectionOutoutOp, including:
 *    CodeType_CORNER:      (x1, y1) + (x2, y2)
 *    CodeType_CENTER_SIZE: (xc, yc) + (w , h )
 *    CodeType_CORNER_SIZE: (x1, y1) + (w , h )
 *    where (x1, y1) represents the top-left corner,
 *          (x2, y2) represents the bottom-right corner, and
 *          (w , h ) represents the (w)idth and (h)eight.
 */
typedef enum
{
  CodeType_CORNER = 0,
  CodeType_CENTER_SIZE = 1,
  CodeType_CORNER_SIZE = 2,
} cnmlPluginSsdCodeType_t;

/*!
 *  @enum cnmlPluginColorCvt_t
 *  @brief An enum.
 *
 *  ``cnmlPluginColorCvt_t`` is an num holding the description of color
 *  conversion mode used in ``ResizeAndColorCvt`` kind of operations, including:
 *  Resize, ResizeYuvToRgba, CropAndResize, YuvToRgba. More will come.
 */
typedef enum
{
  RGBA_TO_RGBA = 0,
  YUV_TO_RGBA_NV12 = 1,
  YUV_TO_RGBA_NV21 = 2,
  YUV_TO_BGRA_NV12 = 3,
  YUV_TO_BGRA_NV21 = 4,
  YUV_TO_ARGB_NV12 = 5,
  YUV_TO_ARGB_NV21 = 6,
  YUV_TO_ABGR_NV12 = 7,
  YUV_TO_ABGR_NV21 = 8,
  GRAY_TO_GRAY = 9
} cnmlPluginColorCvt_t;

/*!
 *  @enum cnmlPluginDataType_t
 *  @brief An enum.
 *
 *  ``cnmlPluginDataType_t`` is an num holding the description of datatype
 *  conversion mode used in ``ResizeAndColorCvt`` kind of operations, including:
 *  Resize, ResizeYuvToRgba, CropAndResize, YuvToRgba. More will come.
 */
typedef enum
{
  FP16_TO_FP16 = 0,
  FP16_TO_UINT8 = 1,
  UINT8_TO_FP16 = 2,
  UINT8_TO_UINT8 = 3
} cnmlPluginDataType_t;
/* -------------------- */
/* enum definitions end */
/* -------------------- */

/* ======================== */
/* struct definitions start */
/* ======================== */
/*!
 *  @struct roiParams
 *  @brief A struct.
 *
 *  ``roiParams`` is a struct holding the description of bounding box info.
 *  CORNER_SIZE mode is used here, i.e., all bounding boxes are discribed in
 *  terms of (x1, y1) + (w , h ).
 */
typedef struct roiParams
{
  int roi_x;
  int roi_y;
  int roi_w;
  int roi_h;
} roiParams;

/*!
 *  @struct ioParams
 *  @brief A struct
 *
 *  ``ioParams`` is a struct holding the descroption of color and datatype
 *  conversion mode used in ``ResizeAndColorCvt`` kind of operations, including:
 *  Resize, ResizeYuvToRgba, CropAndResize, YuvToRgba. More will come.
 */
typedef struct ioParams {
  cnmlPluginColorCvt_t color;
  cnmlPluginDataType_t datatype;
} ioParams;

/*!
 *  @struct cnmlPluginResizeAndColorCvtParam
 *  @brief
 *
 *  ``cnmlPluginResizeAndColorCvtParam`` is a struct holding the parameters used
 *  in ``ResizeAndColotCvt`` kind of operations. In this struct, users only need
 *  to provide "user params". Others will be parsed through the ioParams chosen
 *  by users.
 */
struct cnmlPluginResizeAndColorCvtParam {
  // user params
  int s_row;
  int s_col;
  int d_row;
  int d_col;
  int roi_x;
  int roi_y;
  int roi_w;
  int roi_h;
  ioParams mode;
  int batchNum;
  cnmlCoreVersion_t core_version;

  // operation params
  int inputType;
  int outputType;
  int channelIn;
  int channelOut;
  int layerIn;
  int layerOut;
  int reverseChannel;
  int input2half;
  int output2uint;
  cnmlDataType_t inputDT_MLU;
  cnmlDataType_t inputDT_CPU;
  cnmlDataType_t outputDT_MLU;
  cnmlDataType_t outputDT_CPU;

  int input_num;
  int output_num;
  int static_num;

  // for cropfeatureandresize
  int depth;
  int box_number;
  int pad_size;

  cnmlTensor_t *cnml_static_ptr;
  cnmlCpuTensor_t *cpu_static_ptr;
  void **static_data_ptr;
};
/*! ``cnmlPluginResizeAndColorCvtParam_t`` is a pointer to a
    structure (cnmlPluginResizeAndColorCvtParam) holding the description of CV operations param.
*/
typedef cnmlPluginResizeAndColorCvtParam *cnmlPluginResizeAndColorCvtParam_t;
/* ---------------------- */
/* struct definitions end */
/* ---------------------- */

/* =============================================== */
/* cnmlPluginYolov3DetectionOutout operation start */
/* =============================================== */
/*!
 *  @struct cnmlPluginYolov3DetectionOutputOpParam
 *  @brief A struct.
 *
 *  cnmlPluginYolov3DetectionOutputOpParam is a structure describing the "param"
 *  parameter of Yolov3DetectionOutput operation.
 *  cnmlCreatePluginYolov3DetectionOutputOpParam() is used to create
 *  an instance of cnmlPluginYolov3DetectionOutputOpParam_t.
 *  cnmlDestroyPluginYolov3DetectionOutputOpParam() is used to destroy
 *  an instance of cnmlPluginYolov3DetectionOutputOpParam_t.
 */
struct cnmlPluginYolov3DetectionOutputOpParam
{
    cnmlTensor_t *cnml_static_tensors;
    cnmlCpuTensor_t *cpu_static_tensors;
    int batchNum;
    int inputNum;
    int classNum;
    int maskGroupNum;
    int maxBoxNum;
    int netw;
    int neth;
    float confidence_thresh;
    float nms_thresh;
    cnmlCoreVersion_t core_version;
    int *inputWs;
    int *inputHs;
    float *biases;
    vector<void*> cast_data;
};
/*! ``cnmlPluginYolov3DetectionOutputOpParam_t`` is a pointer to a
    structure (cnmlPluginYolov3DetectionOutputOpParam) holding the description of a Yolov3DetectionOutput operation param.
*/
typedef cnmlPluginYolov3DetectionOutputOpParam
*cnmlPluginYolov3DetectionOutputOpParam_t;

/*!
 *  @brief A function.
 *
 *  This function creates a PluginYolov3DetectionOutputOp param object with
 *  the pointer and parameters provided by user.
 *
 *  **Supports MLU220/MLU270**
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[in] batchNum
 *    Input. The number of input batches.
 *           No default value, a valid batchNum must be in the range of [1, inf).
 *  @param[in] inputNum
 *    Input. The number of input tensors.
 *           No default value, a valid inputNum must be in the range of [1, 7].
 *  @param[in] classNum
 *    Input. The number of input classes.
 *           No default value, a valid classNum must be in the range of [1, 4096].
 *  @param[in] maskGroupNum
 *    Input. The number of anchors used by every input tensors.
 *           No default value, a valid maskGroupNum must be in the range of [1, inf].
 *  @param[in] maxBoxNum
 *    Input. The largest possible number of output boxes.
 *           Default value is 1024, a valid maxBoxNum must be in the range of [1, inf].
 *  @param[in] netw
 *    Input. Width of input image of backbone network.
 *           No default value, a valid netw must be in the range of [1, inf).
 *  @param[in] neth
 *    Input. Height of input image of backbone network.
 *           No default value, a valid neth must be in the range of [1, inf).
 *  @param[in] confidence_thresh
 *    Input. Confidence threshold.
 *           No default value, a valid confidence_thresh must be in the range of [0, 1].
 *  @param[in] nms_thresh.
 *    Input. IOU threshold used in NMS function.
 *           No default value, a valid nms_thresh must be in the range of [0, 1].
 *  @param[in] core_version
 *    Input. Supported core version.
 *           No default value, a valid core_version must be either MLU220 or MLU270.
 *  @param[in] inputWs
 *    Input. Width of every input tensor. Must have the same order as inputHs
 *           No default value, the number of valid elements must be equal with inputNum.
 *  @param[in] inputHs
 *    Input. Height of every input tensor. Must have the same order as inputWs
 *           No default value, the number of valid elements must be equal with inputNum.
 *  @param[in] biases
 *    Input. Anchors of every input tensor.
 *           No default value. The number of valid elements must be equal with 2 x inputNum x maskGroupNum.
 *           The order of data from high to low, is [N(1) H(inputNum) W(maskGroupNum) C(2)]. For example:
 *
 *           Width of anchor for mask0 input0, Height of anchor for mask0 input0,
 *
 *           Width of anchor for mask1 input0, Height of anchor for mask1 input0,
 *
 *           ...
 *
 *           Width of anchor for maskN input0, Height of anchor for maskN input0,
 *
 *           Width of anchor for mask0 input1, Height of anchor for mask0 input1,
 *
 *           ......
 *
 *  @retval CNML_STATUS_SUCCESS
 *    The object was set successfully.
 *  @retval CNML_STATUS_INVALIDPARAM
 *    The inputH/Ws ptr is nullptr or input param is invalid.
 */
cnmlStatus_t cnmlCreatePluginYolov3DetectionOutputOpParam(
    cnmlPluginYolov3DetectionOutputOpParam_t *param,
    int batchNum,
    int inputNum,
    int classNum,
    int maskGroupNum,
    int maxBoxNum,
    int netw,
    int neth,
    float confidence_thresh,
    float nms_thresh,
    cnmlCoreVersion_t core_version,
    int *inputWs,
    int *inputHs,
    float *biases);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginYolov3DetectionOutputOpParam struct, pointed
 *  by the pointer provided by user.
 *
 *  **Supports MLU220/MLU270**
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginYolov3DetectionOutput operator.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginYolov3DetectionOutputOpParam(
    cnmlPluginYolov3DetectionOutputOpParam_t *param);

/*!
 *  @brief A function.
 *
 *  This function creates PluginYolov3DetectionOutputOp with proper param,
 *  input, and output tensors.
 *
 *  PluginYolov3DetectionOutputOp takes in feature maps and network
 *  parameters and computes valid bounding boxes based on two thresholds
 *  you have chosen.
 *
 *  **Reference:**
 *    This implementation is based on the project on ``github/pjreddie/darknet`` .
 *
 *  **Formula:** This op contains two steps:
 *
 *    1. DecodeAllBBoxes.
 *
 *       Convert input feature maps into real ojectness score and coordinates.
 *    for inputIdx in (0, inputNum - 1)
 *
 *       obj = sigmoid(obj_feature);
 *
 *       x   = (x_offset + sigmoid(x_feature)) / inputWs[inputIdx]
 *
 *       y   = (y_offset + sigmoid(y_feature)) / inputHs[inputIdx]
 *
 *       w   = (w_biases * exp(w_feature)) / netw
 *
 *       h   = (h_biases * exp(h_feature)) / neth
 *
 *       Obj, x_feature, y_feature, w_feature, h_feature are data from input feature maps.
 *
 *       x_offset, y_offset are the coordinates of the grid cell in the feature map.
 *
 *       w_offset, h_biases are the shape of the anchor box.
 *
 *    2. Non-maximum Suppression
 *       For each class of data, compute IOU score for every pair of bounding boxes.
 *
 *       If IOU score exceeds the IOU threshold, keep the box with larger score.
 *
 *       x1 = x - w / 2
 *
 *       y1 = y - y / 2
 *
 *       x2 = x + w / 2
 *
 *       y2 = y + y / 2
 *
 *       for classIdx in (0, classNum - 1)
 *
 *        conf = obj * probability[classIdx]
 *
 *        max, maxIdx = findMaxValueAndIndex(conf)
 *
 *        if (max >= confidence_thresh)
 *
 *          for boxIdx in (0, boxNum - 1)
 *
 *            iou = computeIOU(coord_maxIdx, coord_boxIdx)  // where "coords" means x1,y1,x2,y2
 *
 *            if (iou < nms_thresh)
 *
 *              keep coords and conf for boxIdx
 *
 *  **DataType:**
 *
 *    Support only half(float16) type for both input and output tensors.
 *
 *  **Performance Optimization:**
 *
 *    The performance of detection layer depends on both the data size and the value.
 *    However, this op achieves relatively better performance when
 *    all of the following conditions are met:
 *
 *    - inputH/Ws are 64-aligned(unit in number of data).
 *
 *    - (5 + classNum) is 64-aligned(unit in number of data).
 *
 *    The bigger the remainder of the value of param divided by 64, the better performance the op will achieve.
 *
 *  Supports both MLU220 and MLU270.
 *
 *  @param[out]  op
 *    Output. A pointer to the base operator address.
 *  @param[in]  param
 *    Input. A PluginYolov3DetectionOutput parameter struct pointer.
 *  @param[in]  yolov3_input_tensors
 *    Input. An array of four-demensional cnmlTensors with a shape of
 *           [batchNum, (5 + classNum) * numMaskGroup, inputH, inputW](NCHW).
 *           Support only FLOAT16 dataType currently.
 *  @param[in]  outputs
 *    Input. An array of four-demensional cnmlTensors with a shape of
 *           [batchNum, 64 + 7 * numMaxBox, 1, 1](NCHW).
 *           Support only FLOAT16 dataType currently.
 *           The first two numbers of each batch store the number of
 *           detected boxes. The data for each box starts from the 65th number,
 *           with an order of [batchId, classId, score, x1, y1, x2, y2], where
 *           (x1, y1) and (x2, y2) are the coordinates of top-left and bottom-
 *           -right points accordingly.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op pointer is nullptr
 *    - Param is nullptr or not initialized
 *    - Input / output tensor desps is nullptr or inconsistent with param.
 */
cnmlStatus_t cnmlCreatePluginYolov3DetectionOutputOp(
    cnmlBaseOp_t *op,
    cnmlPluginYolov3DetectionOutputOpParam_t param,
    cnmlTensor_t *yolov3_input_tensors,
    cnmlTensor_t *yolov3_output_tensors);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginYolov3DetectionOutputOp on MLU.
 *
 *  **Supports MLU100/MLU270**
 *
 *  @param[out]  op
 *    Input. A pointer to the base operator address.
 *  @param[in]  inputs
 *    Input. An array stores the address of all input tensors
 *  @param[in]  num_inputs
 *    Input. Number of input tensors
 *  @param[out]  outputs
 *    Output. An array stores the address of all output tensors
 *  @param[in]  num_outputs
 *    Input. Number of output tensors
 *  @param[in]  compute_forw_param
 *    Input. A pointer to the struct address, which records runtime degree of
 *    data parallelism and equipment affinity.
 *  @param[in]  queue
 *    Input. A computation queue pointer.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Input / output nums are inconsistent.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginYolov3DetectionOutputOpForward(
    cnmlBaseOp_t op,
    void *input[],
    int num_inputs,
    void *output[],
    int num_outputs,
    cnrtInvokeFuncParam_t *compute_forw_param,
    cnrtQueue_t queue);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginYolov3DetectionOutputOp on CPU.
 *
 *  @param[in]  param
 *    Input. A PluginYolov3DetectionOutput parameter struct pointer.
 *  @param[in]  inputs
 *    Input. An array stores the address of all cpu input data
 *  @param[out]  outputs
 *    Output. An array stores the address of all cpu output data
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is nullptr or inconsistent.
 *    - Input / output addrs is nullptr ot malloced with wrong sizes.
 */
cnmlStatus_t cnmlCpuComputePluginYolov3DetectionOutputOpForward(
    cnmlPluginYolov3DetectionOutputOpParam_t param,
    void *input[],
    void *output);
/* --------------------------------------------- */
/* cnmlPluginYolov3DetectionOutout operation end */
/* --------------------------------------------- */

/* =============================================== */
/* cnmlPluginOneHot operation start */
/* =============================================== */
/*!
 *  @struct cnmlPluginOneHotOpParam
 *  @brief A struct.
 *
 *  cnmlPluginOneHotOpParam is a structure describing the "param"
 *  parameter of OneHot operation.
 *  cnmlPluginOneHotOpParam() is used to create
 *  an instance of cnmlPluginOneHotOpParam_t.
 *  cnmlPluginOneHotOpParam() is used to destroy
 *  an instance of cnmlPluginOneHotOpParam_t.
 */
struct cnmlPluginOneHotOpParam
{
    int N;
    int H;
    int W;
    int C;
    int depth;
    float onvalue;
    float offvalue;
	int axis;
    cnmlCoreVersion_t core_version;
};
/*! ``cnmlPluginOneHotOpParam_t`` is a pointer to a
    structure (cnmlPluginOneHotOpParam) holding the description of a OneHot operation param.
*/
typedef cnmlPluginOneHotOpParam *cnmlPluginOneHotOpParam_t;

/*!
 *  @brief A function.
 *
 *  This function creates a PluginOneHotOp param object with
 *  the pointer and parameters provided by user.
 *
 *  **Supports MLU270**
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[in] core_version
 *    Input. Supported core version, including MLU270.
 *  @param[in] N
 *    Input. The number of batches.
 *  @param[in] H
 *    Input. The Height of input tensors.
 *  @param[in] W
 *    Input. The number of classes.
 *  @param[in] C
 *    Input. The number of anchors for every input tensors.
 *  @param[in] depth
 *    Input. The number of classes.
 *  @param[in] onvalue
 *    Input. The locations represented by indices take value onvalue.
 *  @param[in] offvalue
 *    Input. All other locations take value offvalue.
 *  @param[in] axis
 *    Input. The new axis is created at dimension axis.
 *  @retval CNML_STATUS_SUCCESS
 *    The object was set successfully.
 */
cnmlStatus_t cnmlCreatePluginOneHotOpParam(
    cnmlPluginOneHotOpParam_t *param,
    cnmlCoreVersion_t core_version,
    int N,
    int H,
    int W,
    int C,
    int depth,
    float onvalue,
    float offvalue,
	int axis);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginYolov3DetectionOutputOpParam struct, pointed
 *  by the pointer provided by user.
 *
 *  **Supports MLU270**
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginYolov3DetectionOutput operator.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginOneHotOpParam(
    cnmlPluginOneHotOpParam_t *param);

/*!
 *  @brief A function.
 *
 *  This function creates PluginYolov3DetectionOutputOp with proper param,
 *  input, and output tensors. The current implementation is based on the
 *  official caffe website of weiliu86.
 *
 *  **Supports Caffe/Pytorch on MLU100/MLU270**
 *
 *  @param[out]  op
 *    Output. A pointer to the base operator address.
 *  @param[in]  param
 *    Input. A PluginOneHot parameter struct pointer.
 *  @param[in]  yolov3_input_tensors
 *    Input. An array of four-demensional cnmlTensors with a shape of
 *           [batchNum, (5 + classNum) * numMaskGroup, inputH, inputW](NCHW).
 *           Support only FLOAT16 dataType currently.
 *  @param[in]  outputs
 *    Input. An array of four-demensional cnmlTensors with a shape of
 *           [batchNum, 64 + 7 * numMaxBox, 1, 1](NCHW).
 *           Support only FLOAT16 dataType currently.
 *           The first two numbers of each batch store the number of
 *           detected boxes. The data for each box starts from the 65th number,
 *           with an order of [batchId, classId, score, x1, y1, x2, y2], where
 *           (x1, y1) and (x2, y2) are the coordinates of top-left and bottom-
 *           -right points accordingly.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op pointer is nullptr
 *    - Param is nullptr or not initialized
 *    - Input / output tensor desps is nullptr or inconsistent with param.
 */
cnmlStatus_t cnmlCreatePluginOneHotOp(
    cnmlBaseOp_t *op,
    cnmlPluginOneHotOpParam_t param,
    cnmlTensor_t *input_tensors,
    cnmlTensor_t *output_tensors);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginOneHotOp on MLU.
 *
 *  **Supports MLU270**
 *
 *  @param[out]  op
 *    Input. A pointer to the base operator address.
 *  @param[in]  inputs
 *    Input. An array stores the address of all input tensors
 *  @param[in]  num_inputs
 *    Input. Number of input tensors
 *  @param[out]  outputs
 *    Output. An array stores the address of all output tensors
 *  @param[in]  num_outputs
 *    Input. Number of output tensors
 *  @param[in]  queue
 *    Input. A computation queue pointer.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Input / output nums are inconsistent.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginOneHotOpForward(
    cnmlBaseOp_t op,
    void *input[],
    int num_inputs,
    void *output[],
    int num_outputs,
    cnrtQueue_t queue);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginYolov3DetectionOutputOp on CPU.
 *
 *  @param[in]  param
 *    Input. A PluginYolov3DetectionOutput parameter struct pointer.
 *  @param[in]  inputs
 *    Input. An array stores the address of all cpu input data
 *  @param[out]  outputs
 *    Output. An array stores the address of all cpu output data
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is nullptr or inconsistent.
 *    - Input / output addrs is nullptr ot malloced with wrong sizes.
 */
cnmlStatus_t cnmlCpuComputePluginOneHotOpForward(
   cnmlPluginOneHotOpParam_t param,
   int* indeces,
   float *dst);
/* --------------------------------------------- */
/* cnmlPluginOneHot operation end */
/* --------------------------------------------- */

/* =============================================== */
/* cnmlPluginRange operation start */
/* =============================================== */
/*!
 *  @struct cnmlPluginRangeOpParam
 *  @brief A struct.
 *
 *  cnmlPluginRangeOpParam is a structure describing the "param"
 *  parameter of Range operation.
 *  cnmlCreatePluginRangeOpParam() is used to create
 *  an instance of cnmlPluginRangeOpParam_t.
 *  cnmlDestroyPluginRangeOpParam() is used to destroy
 *  an instance of cnmlPluginOneHotOpParam_t.
 */
struct cnmlPluginRangeOpParam
{
    int size;
    cnmlCoreVersion_t core_version;
};
/*! ``cnmlPluginRangeOpParam_t`` is a pointer to a
    structure (cnmlPluginRangeOpParam) holding the description of a Range operation param.
*/
typedef cnmlPluginRangeOpParam *cnmlPluginRangeOpParam_t;

/*!
 *  @brief A function.
 *
 *  This function creates a PluginRangeOp param object with
 *  the pointer and parameters provided by user.
 *
 *  **Supports MLU220/270**
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[in] core_version
 *    Input. Supported core version, including MLU220/270.
 *  @retval CNML_STATUS_SUCCESS
 *    The object was set successfully.
 *  @warning
 *    The sum of input tensor HW values should be less than 32768.
 */
cnmlStatus_t cnmlCreatePluginRangeOpParam(
    cnmlPluginRangeOpParam_t *param,
    cnmlCoreVersion_t core_version);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginRangeOpParam struct, pointed
 *  by the pointer provided by user.
 *
 *  **Supports MLU270**
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginYolov3DetectionOutput operator.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginRangeOpParam(
    cnmlPluginRangeOpParam_t *param);

/*!
 *  @brief A function.
 *
 *  This function creates PluginRangeOp with proper param,
 *  input, and output tensors. The current implementation is based on the
 *  official caffe website of weiliu86.
 *
 *  **Supports TensorFlow on MLU270**
 *
 *  @param[out]  op
 *    Output. A pointer to the base operator address.
 *  @param[in]  param
 *    Input. A PluginYolov3DetectionOutput parameter struct pointer.
 *  @param[in]  input_tensors
 *    Input. An array of four-demensional cnmlTensors with a shape of
 *           [1, 1, 1, 1](NCHW).The size of array is three, with an order of
 *           [start, limit, delta].
 *           Support only FLOAT32 dataType currently.
 *  @param[in]  outputs
 *    Output. An array of four-demensional cnmlTensors with a shape of
 *           [size, 1, 1, 1](NCHW).
 *           Support only FLOAT32 dataType currently.
 *           The size is the length of result.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op pointer is nullptr
 *    - Param is nullptr or not initialized
 *    - Input / output tensor desps is nullptr or inconsistent with param.
 */
cnmlStatus_t cnmlCreatePluginRangeOp(
    cnmlBaseOp_t *op,
    cnmlPluginRangeOpParam_t param,
    cnmlTensor_t *input_tensors,
    cnmlTensor_t *output_tensors);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginRangeOp on MLU.
 *
 *  **Supports MLU270**
 *
 *  @param[out]  op
 *    Input. A pointer to the base operator address.
 *  @param[in]  inputs
 *    Input. An array stores the address of all input tensors
 *  @param[in]  num_inputs
 *    Input. Number of input tensors
 *  @param[out]  outputs
 *    Output. An array stores the address of all output tensors
 *  @param[in]  num_outputs
 *    Input. Number of output tensors
 *  @param[in]  queue
 *    Input. A computation queue pointer.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Input / output nums are inconsistent.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginRangeOpForward(
    cnmlBaseOp_t op,
    void *input[],
    int num_inputs,
    void *output[],
    int num_outputs,
    cnrtQueue_t queue);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginRangeOp on CPU.
 *
 *  @param[in]  start
 *    Input.
 *  @param[in]  limit
 *    Input.
 *  @param[in]  delta
 *    Input.
 *  @param[out]  output
 *    Output. An address of all cpu output data
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is nullptr or inconsistent.
 *    - Input / output addrs is nullptr ot malloced with wrong sizes.
 */
cnmlStatus_t cnmlCpuComputePluginRangeOpForward(
   float start, float limit, float delta, float *output);
/* --------------------------------------------- */
/* cnmlPluginRange operation end */
/* --------------------------------------------- */


/* ======================================= */
/* cnmlPluginCropFeatureAndResize operation start */
/* ======================================= */
/*!
 *  @brief A function.
 *
 *  This function create a PluginCropFeatureAndResizeOp param onject with a pointer
 *  and "user params" provided.
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[in] s_row
 *    Input. The row number of src image.
 *  @param[in] s_col
 *    Input. The col number of src image.
 *  @param[in] d_row
 *    Input. The row number of dst image.
 *  @param[in] d_col
 *    Input. The col number of dst image.
 *  @param[in] batchNum
 *    Input. The number of batch of input images. This op regards one image as
 *           one batch.
 *  @param[in] depth
 *    Input. The depth/channel of src image.
 *  @param[in] box_number
 *    Input. detect number of bbox.
 *  @param[in] pad_size
 *    Input. pad_size.
 *  @param[in] core_version
 *    Input[in]. The hardware core_version.
 *  @retval CNML_STATUS_SUCCESS
 *    The object was set successfully.
 */
cnmlStatus_t cnmlCreatePluginCropFeatureAndResizeOpParam(
  cnmlPluginResizeAndColorCvtParam_t* param,
  int s_row,
  int s_col,
  int d_row,
  int d_col,
  int batchNum,
  int depth,
  int box_number,
  int pad_size,
  cnmlCoreVersion_t core_version);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginCropFeatureAndResizeOpParam struct, pointed by
 *  the pointer provided by user.
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginCropFeatureAndResize operator.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginCropFeatureAndResizeOpParam(
    cnmlPluginResizeAndColorCvtParam_t* param);

/*!
 *  @brief A function.
 *
 *  This function creates PluginCropFeatureAndResizeOp with proper param,
 *  input, and output tensors. The current implementation is based on the
 *  traditional bi-linear interpolation method on OpenCV.
 *
 *  **Supports Caffe/Pytorch on MLU100/MLU270**
 *
 *  @param[out] op
 *    Output. A pointer to the base operator address.
 *  @param[in] param
 *    Input. A PluginResizeAndColorCvt parameter struct pointer.
 *  @param[in] input_cnml_tensors
 *    Input. A four-dimensional tensor for dst image
 *  @param[in] output_cnml_tensors
 *    Input. A four-dimensional tensor for src image
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - param is not consistant with tensors
 *    - shapes of cropParams and roiNums are not consistent
 */
cnmlStatus_t cnmlCreatePluginCropFeatureAndResizeOp(
    cnmlBaseOp_t* op,
    cnmlPluginResizeAndColorCvtParam_t* param,
    cnmlTensor_t* input_cnml_tensors, // src
    cnmlTensor_t* output_cnml_tensors);  // dst

/*!
 *  @brief A function.
 *
 *  This function forwards PluginResizeYuvToRgbaOp on MLU.
 *
 *  **Supports MLU100/MLU270**
 *
 *  @param[in] op
 *    Output. A pointer to the base operator address.
 *  @param[in] input_addr
 *    Input. Address of input tensor
 *  @param[Out] output_addr
 *    Output. Address of output tensor
 *  @param[in] compute_forw_param
 *    Input. A pointer to the struct address, which records runtime degree of
 *    data parallelism and equipment affinity.
 *  @param[in] queue
 *    Input. A computation queue pointer.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginCropFeatureAndResizeOpForward(
    cnmlBaseOp_t op,
    void* input_addr[],
    void* output_addr[],
    cnrtInvokeFuncParam_t compute_forw_param,
    cnrtQueue_t queue);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginCropFeatureAndResizeOp on CPU.
 *
 *  @param[out] dst
 *    Output. The pointer of dst image
 *  @param[in] src
 *    Input. The pointer of src image
 *  @param[in] boxes
 *    Input. The pointer to detect bbox.
 *  @param[in] box_index
 *    Input. The pointer to index of bbox.
 *  @param[in] new_box
 *    Input. The pointer to output.
 *  @param[in] batchNum
 *    Input. batch size.
 *  @param[in] depth
 *    Input. The channel of input feature.
 *  @param[in] image_height
 *    Input. The height of input feature.
 *  @param[in] image_width
 *    Input. The width of input feature.
 *  @param[in] crop_height
 *    Input. The height of resize output.
 *  @param[in] crop_width
 *    Input. The width of resize output.
 *  @param[in] box_number
 *    Input. The number of detect bbox.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Input/output pointer is nullptr.
 *    - Param is not consistent with input and output.
 */
cnmlStatus_t cnmlCpuComputePluginCropFeatureAndResizeOpForward(
    float* src,
    float* boxes,
    float* box_index,
    float* new_box,
    int batchNum,
    int depth,
    int image_height,
    int image_width,
    int crop_height,
    int crop_width,
    int box_number);
/* ------------------------------------- */
/* cnmlPluginCropFeatureAndResize operation end */
/* ------------------------------------- */


/* ======================================= */
/* cnmlPluginNonMaxSuppression operation start */
/* ======================================= */
/*!
 * @struct cnmlPluginNonMaxSuppressionOpParam
 * @brief A struct.
 *
 * cnmlPluginNonMaxSuppressionOpParam is a structure describing thr "param"
 * parameter of NonMaxSuppression operation.
 * cnmlCreatePluginNonMaxSuppressionOpParam() is used to create
 * an instance of cnmlPluginNonMaxSuppressionOpParam_t.
 * cnmlDestoryPluginNonMaxSuppressionOpParam() is used to destory
 * an instance of cnmlPluginNonMaxSuppressionOpParam_t.
 */
struct cnmlPluginNonMaxSuppressionOpParam
{
  cnmlTensor_t *cnml_static_tensors;
  void* *cpu_static_init;
  int len;
  int max_num;
  float iou_threshold;
  float score_threshold;
  cnmlCoreVersion_t core_version;
  float *input;
};
/*! ``cnmlPluginNonMaxSuppressionOpParam_t`` is a pointer to a
    structure (cnmlPluginNonMaxSuppressionOpParam) holding the description of a NonMaxSuppression operation param.
*/
typedef cnmlPluginNonMaxSuppressionOpParam
*cnmlPluginNonMaxSuppressionOpParam_t;

/*!
 *  @brief A function.
 *
 *  This function create a PluginNonMaxSuppressionOp param onject with a pointer
 *  and "user params" provided.
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[in] len
 *    Input. The number of input boxes.
 *  @param[in] max_num
 *    Input. The max number of output boxes.
 *  @param[in] iou_threshold
 *    Input. The threshold of iou to do nms.
 *  @param[in] score_threshold
 *    Input. The threshold of score to do nms.
 *  @param[in] core_version
 *    Input[in]. The hardware core_version.
 *  @retval CNML_STATUS_SUCCESS
 *    The object was set successfully.
 */
cnmlStatus_t cnmlCreatePluginNonMaxSuppressionOpParam(
  cnmlPluginNonMaxSuppressionOpParam_t *param,
  int len,
  int max_num,
  float iou_threshold,
  float score_threshold,
  cnmlCoreVersion_t core_version=CNML_MLU270);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginNonMaxSuppressionOpParam struct, pointed by
 *  the pointer provided by user.
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginNonMaxSuppression operator.
 *  @param[in]  static_num
 *    Input. Number of static tensors
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginNonMaxSuppressionOpParam(
    cnmlPluginNonMaxSuppressionOpParam_t *param,
    int static_num);

/*!
 *  @brief A function.
 *
 *  This function creates PluginNonMaxSuppressionOp with proper param,
 *  input, and output tensors.
 *
 *  **Supports Tensorflow on MLU270**
 *
 *  @param[out] op
 *    Output. A pointer to the base operator address.
 *  @param[in] param
 *    Input. A PluginNonMaxSuppression parameter struct pointer.
 *  @param[in] nms_input_tensors
 *    Input. This pointer contains two array of four-demensional cnmlTensors,
 *           first tensor's shape is [4, len, 1, 1], second tensor's shape is [1, len, 1, 1].
 *  @param[in] input_num
 *    Input. Number of input tensors
 *  @param[out] nms_output_tensors
 *    Output. This pointer contains an array of four-demensional cnmlTensor,
 *           the tensor's shape is [1, max_num, 1, 1].
 *  @param[in] output_num
 *    Input. Number of output tensors
 *  @param[in] static_num
 *    Input. Number of static tensors
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - param is not consistant with tensors
 *    - shapes of cropParams and roiNums are not consistent
 */
cnmlStatus_t cnmlCreatePluginNonMaxSuppressionOp(
    cnmlBaseOp_t *op,
    cnmlPluginNonMaxSuppressionOpParam_t param,
    cnmlTensor_t *nms_input_tensors,
    int input_num,
    cnmlTensor_t *nms_output_tensors,
    int output_num,
    int static_num);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginNonMaxSuppressionOp on MLU.
 *
 *  **Supports MLU270**
 *
 *  @param[in] op
 *    Output. A pointer to the base operator address.
 *  @param[in] input_tensors
 *    Input. Void
 *  @param[in] inputs
 *    Input. An array stores the address of all input tensors
 *  @param[in] num_inputs
 *    Input. Number of input tensors
 *  @param[out] output_tensors
 *    Output. Void
 *  @param[in] outputs
 *    Input. A array stores the address of all output tensors
 *  @param[in] num_outputs
 *    Input. Number of output tensors
 *  @param[in] queue
 *    Input. A computation queue pointer.
 *  @param[in] extra
 *    Input. A pointer contains other input params
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginNonMaxSuppressionOpForward(
    cnmlBaseOp_t op,
    cnmlTensor_t input_tensors[],
    void *inputs[],
    int num_inputs,
    cnmlTensor_t output_tensors[],
    void *outputs[],
    int num_outputs,
    cnrtQueue_t queue,
    void *extra);

/* ------------------------------------- */
/* cnmlPluginNonMaxSuppression operation end */
/* ------------------------------------- */

/* ================================= */
/*  cnmlPluginArange operation start    */
/* ================================= */
/* cnmlPluginArangeOpParam
 *  @brief A struct.
 *
 *  cnmlPluginArangeOpParam is a structure describing the "param"
 *  parameter of cnmlPluginArangeOpParam operation.
 *  cnmlCreatePlugincnmlPluginArangeOpParam() is used to create an instance of
 *  cnmlPluginArangeParam_t.
 *  cnmlDestroyPlugincnmlPluginArangeOpParam() is used to destroy an instance
 *  of cnmlPluginArangeParam_t.
 */
struct cnmlPluginArangeOpParam
{
  float start;
  float stop;
  float step;
  int repeat;
  int size;
  int dtype_flag;
  cnmlCoreVersion_t coreVersion;
};
/*! ``cnmlPluginArangeParam_t`` is a pointer to a
    structure (cnmlPluginArangeParam) holding the description of a ArangeOp operation param.
*/
typedef cnmlPluginArangeOpParam *cnmlPluginArangeParam_t;

/*!
 *  @brief A function.
 *
 *  This function creates a PluginArangeOp param object with
 *  the pointer and parameters provided by user. This implementation is based
 *  on the official MXNet website.
 *
 *  **Supports MXNet on MLU270**
 * cnmlPluginArangeParam_t *param,
  float start,
  float stop,
  float step,
  int repeat,
  int size,
  int dtype_flag,
  cnmlCoreVersion_t coreVersion
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[float] start
 *    Input. Start of interval.
 *  @param[float] stop
 *    Input. End of interval.
 *  @param[float] step
 *    Input. Spacing between values.
 *  @param[int] repeat
 *    Input. The repeating time of all elements.
 *  @param[int] size
 *    Input. intput shape size .
 *  @param[int] dtype_flag
 *    Input. The data type of input. only support float16 so far
 *  @param[cnmlCoreVersion_t] coreVersion
 *    Input. The core version of MLU.
 *  @retval CNML_STATUS_SUCCESS
 *    The object was set successfully.
 */
cnmlStatus_t cnmlCreatePluginArangeOpParam(
  cnmlPluginArangeParam_t *param,
  float start,
  float stop,
  float step,
  int repeat,
  int size,
  int dtype_flag,
  cnmlCoreVersion_t coreVersion);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginArangeOpParam struct, pointed by
 *  the pointer provided by user.
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginNms operator.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginArangeOpParam(
    cnmlPluginArangeParam_t *param);
/*!
 *  @brief A function.
 *
 *  This function creates PluginArangeOp with proper param,
 *  input, and output tensors. The current implementation is based on the
 *  official MXNet NMS op.
 *
 *  **Supports MXNet on MLU270**
 *
 *  @param[out]  op
 *    Output. A pointer to the base operator address.
 *  @param[in]  param
 *    Input. A PluginArangeOp parameter struct pointer.
 *  @param[in]  arange_input_tensors
 *    Input. Support only FLOAT16 dataType currently.
 *  @param[in]  outputs
 *    Output.Support only FLOAT16 dataType currently.
 *           The size is the length of result.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op pointer is nullptr
 *    - Param is nullptr or not initialized
 *    - Input / output tensor desps is nullptr or inconsistent with param.
 */
cnmlStatus_t cnmlCreatePluginArangeOp(
  cnmlBaseOp_t *op,
  cnmlPluginArangeParam_t param,
  cnmlTensor_t *arange_input_tensors,
  cnmlTensor_t *arange_output_tensors
);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginArangeOp on MLU.
 *
 *  **Supports MLU270**
 *
 *  @param[in] op
 *    Output. A pointer to the base operator address.
 *  @param[in] input_tensors
 *    Input. Void
 *  @param[in] inputs
 *    Input. An array stores the address of all input tensors
 *  @param[in] num_inputs
 *    Input. Number of input tensors
 *  @param[out] output_tensors
 *    Output. Void
 *  @param[in] outputs
 *    Input. A array stores the address of all output tensors
 *  @param[in] num_outputs
 *    Input. Number of output tensors
 *  @param[in] queue
 *    Input. A computation queue pointer.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginArangeOpForward(
  cnmlBaseOp_t op,
  void **inputs,
  int input_num,
  void **outputs,
  int output_num,
  cnrtQueue_t queue
);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginArangeOp on CPU.
 *
 *  @param[in] param
 *    Input. A pointer of cnmlPluginArangeParam, which
 *    support params needed by this operator.
 *  @param[in] output
 *    Input. An address of output tensor
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 */
cnmlStatus_t cnmlCpuComputePluginArangeOpForward(
  cnmlPluginArangeParam_t param,
  float* output
);

/* ======================================= */
/* cnmlPluginArange operation end */
/* ======================================= */

/* ================================= */
/*  cnmlPluginInitOp operation start    */
/* ================================= */
/*!
 *  @struct cnmlPluginInitOpParam
 *  @brief A struct.
 *
 *  cnmlPluginInitOpParam is a structure describing the "param"
 *  parameter of cnmlPluginInitOpParam operation.
 *  cnmlCreatePlugincnmlPluginInitOpParam() is used to create an instance of
 *  cnmlPluginInitParam_t.
 *  cnmlDestroyPlugincnmlPluginInitOpParam() is used to destroy an instance
 *  of cnmlPluginInitParam_t.
 */
struct cnmlPluginInitOpParam
{
  int size;
  float value;
  int dtype_flag;
  cnmlCoreVersion_t coreVersion;
};
/*! ``cnmlPluginInitOpParam_t`` is a pointer to a
    structure (cnmlPluginInitOpParam) holding the description of a InitOp operation param.
*/
typedef cnmlPluginInitOpParam *cnmlPluginInitOpParam_t;

/*!
 *  @brief A function.
 *
 *  This function creates a PluginInitOp param object with
 *  the pointer and parameters provided by user. This implementation is based
 *  on the official MXNet website.
 *
 *  **Supports MXNet on MLU270**
 *
 *  @param[out] param
 *    Output. The returning param descriptor.
 *  @param[in] size
 *    Input. The size of need initialized.
 *  @param[float] value
 *    Input. The value of should be initialized.
 *  @param[in] dtype_flag
 *    Input. The data type of input. 0:float32, 1:float64, 2:float16
 *  @param[cnmlCoreVersion_t] coreVersion
 *    Input. The core version of MLU.
 */
cnmlStatus_t cnmlCreatePluginInitOpParam(
  cnmlPluginInitOpParam_t *param,
  int size,
  float value,
  int dtype_flag=2,
  cnmlCoreVersion_t coreVersion=CNML_MLU270);

/*!
 *  @brief A function.
 *
 *  This function frees the PluginInitOpParam struct, pointed by
 *  the pointer provided by user.
 *
 *  @param[in]  param
 *    Input. A pointer to the address of the struct of computation parameters
 *    for PluginInit operator.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Param is a null pointer.
 *    - The pointer content pointed by param is already freed.
 */
cnmlStatus_t cnmlDestroyPluginInitOpParam(
    cnmlPluginInitOpParam_t *param);

/*!
 *  @brief A function.
 *
 *  This function creates PluginInitOp with proper param,
 *  input, and output tensors. The current implementation is based on the
 *  official MXNet Init op.
 *
 *  **Supports MXNet on MLU270**
 *
 *  @param[out]  op
 *    Output. A pointer to the base operator address.
 *  @param[in]  param
 *    Input. A PluginInitOp parameter struct pointer.
 *  @param[in]  Init_input_tensors
 *    Input. An array of multi-demensional cnmlTensors .
 *           Support only FLOAT16 dataType currently.
 *  @param[in]  outputs
 *    Output. An array of multi-demensional cnmlTensors.
 *           Support only FLOAT16 dataType currently.
 *           The size is the length of result.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op pointer is nullptr
 *    - Param is nullptr or not initialized
 *    - Input / output tensor desps is nullptr or inconsistent with param.
 */
cnmlStatus_t cnmlCreatePluginInitOp(
  cnmlBaseOp_t *op,
  cnmlPluginInitOpParam_t param,
  cnmlTensor_t *Init_input_tensors,
  cnmlTensor_t *Init_output_tensors
);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginInitOp on MLU.
 *
 *  **Supports MLU270**
 *
 *  @param[in] op
 *    Output. A pointer to the base operator address.
 *  @param[in] input_tensors
 *    Input. Void
 *  @param[in] inputs
 *    Input. An array stores the address of all input tensors
 *  @param[in] num_inputs
 *    Input. Number of input tensors
 *  @param[out] output_tensors
 *    Output. Void
 *  @param[in] outputs
 *    Input. A array stores the address of all output tensors
 *  @param[in] num_outputs
 *    Input. Number of output tensors
 *  @param[in] queue
 *    Input. A computation queue pointer.
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 *  @retval CNML_STATUS_INVALIDPARAM
 *    At least one of the following conditions is not met:
 *    - Base op is nullptr.
 *    - Input / output addrs is nullptr.
 *    - Task type is invalid at runtime.
 */
cnmlStatus_t cnmlComputePluginInitOpForward(
  cnmlBaseOp_t op,
  void **inputs,
  int input_num,
  void **outputs,
  int output_num,
  cnrtQueue_t queue
);

/*!
 *  @brief A function.
 *
 *  This function forwards PluginInitOp on CPU.
 *
 *  @param[in] param
 *    Input. Param of Init operator, cnmlPluginInitOpParam
 *  @param[in] output
 *    Input. An address of output tensor
 *  @retval CNML_STATUS_SUCCESS
 *    The function ends normally
 */
cnmlStatus_t cnmlCpuComputePluginInitOpForward(
  cnmlPluginInitOpParam_t param,
  float *output
);

/* ================================= */
/*  cnmlPluginInit operation end      */
/* ================================= */

/* ======================================= */
/* cnmlPluginBertSquad operation start */
/* ======================================= */
cnmlStatus_t cnmlCreatePluginBertSquadOp(
    cnmlBaseOp_t *op,
    cnmlTensor_t *input_tensors,
    cnmlTensor_t *output_tensors,
    cnmlTensor_t *cnml_static_tensors,
    int static_tensors_num,
    int batch_size,
    int seq_num);

cnmlStatus_t cnmlComputePluginBertSquadOpForward(
    cnmlBaseOp_t op,
    cnmlTensor_t* input_tensors, // default as nullptr
    void** inputs,
    cnmlTensor_t* output_tensors, // default as nullptr
    void** outputs,
    cnrtQueue_t queue,
    void *extra);


/* ------------------------------------- */
/* cnmlPluginBertSquad operation end */
/* ------------------------------------- */
struct cnmlPluginPowerDifferenceOpParam{
  int pow;
  int dims_a;
  cnmlCoreVersion_t core_version;
};

typedef cnmlPluginPowerDifferenceOpParam *cnmlPluginPowerDifferenceOpParam_t;

cnmlStatus_t cnmlCreatePluginPowerDifferenceOpParam(
  cnmlPluginPowerDifferenceOpParam_t *params,
  int pow,
  int dims_a,
  cnmlCoreVersion_t core_version
);

cnmlStatus_t cnmlDestroyPluginPowerDifferenceOpParam(
  cnmlPluginPowerDifferenceOpParam_t *params
);

// cnmlStatus_t cnmlCreatePluginPowerDifferenceOp(
//   cnmlBaseOp_t *op,
//   cnmlPluginPowerDifferenceOpParam_t param,
//   cnmlTensor_t *pd_input_tensors,
//   int input_num,
//   cnmlTensor_t *pd_output_tensors,
//   int output_num
// );

cnmlStatus_t cnmlCreatePluginPowerDifferenceOp(
  cnmlBaseOp_t *op,
  cnmlTensor_t *pd_input_tensors,
  int pow,
  cnmlTensor_t *pd_output_tensors,
  int len
);

// cnmlStatus_t cnmlComputePluginPowerDifferenceOpForward(
//   cnmlBaseOp_t op,
//   cnmlTensor_t input_tensors[],
//   void *inputs[],
//   int num_inputs,
//   cnmlTensor_t output_tensors[],
//   void *outputs[],
//   int num_outputs,
//   cnrtQueue_t queue,
//   void *extra
// );
cnmlStatus_t cnmlComputePluginPowerDifferenceOpForward(
  cnmlBaseOp_t op,
  void *inputs[],
  void *outputs[],
  cnrtQueue_t queue
);
#endif

