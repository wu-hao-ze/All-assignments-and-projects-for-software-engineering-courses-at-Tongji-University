#include <stdlib.h>
#include "cnrt.h"
#include "cnrt_data.h"
#include "stdio.h"
#ifdef __cplusplus
extern "C" {
#endif
// 补充PowerDifferenceKernel参数
void PowerDifferenceKernel(half* input1, half* input2, int pow, half* output, int len);
#ifdef __cplusplus
}
#endif
void PowerDifferenceKernel(half* input1, half* input2, int pow, half* output, int len);

int MLUPowerDifferenceOp(float* input1,float* input2, int pow, float*output, int dims_a) {
  
  cnrtInit(0);
  cnrtDev_t dev;
  cnrtGetDeviceHandle(&dev, 0);
  cnrtSetCurrentDevice(dev);
  cnrtQueue_t pQueue;
  cnrtCreateQueue(&pQueue);
  cnrtDim3_t dim;
  dim.x = 1;
  dim.y = 1;
  dim.z = 1;
  float hardware_time = 0.0;
  cnrtNotifier_t event_start;
  cnrtNotifier_t event_end;
  cnrtCreateNotifier(&event_start);
  cnrtCreateNotifier(&event_end);
  cnrtFunctionType_t c = CNRT_FUNC_TYPE_BLOCK;

  //prepare data
  half* input1_half = (half*)malloc(dims_a * sizeof(half));
  half* input2_half = (half*)malloc(dims_a * sizeof(half));
  half* output_half = (half*)malloc(dims_a * sizeof(half));

  cnrtConvertFloatToHalfArray(input1_half, input1, dims_a);
  cnrtConvertFloatToHalfArray(input2_half, input2, dims_a);
  cnrtConvertFloatToHalfArray(output_half, output,dims_a);
 
  half *mlu_input1,*mlu_input2, *mlu_output;
  if (CNRT_RET_SUCCESS != cnrtMalloc((void**)&mlu_input1, dims_a * sizeof(half))) {
    printf("cnrtMalloc Failed!\n");
    exit(-1);
  }
  if (CNRT_RET_SUCCESS != cnrtMalloc((void**)&mlu_input2, dims_a * sizeof(half))) {
    printf("cnrtMalloc Failed!\n");
    exit(-1);
  }
  if (CNRT_RET_SUCCESS != cnrtMalloc((void**)&mlu_output, dims_a * sizeof(half))) {
    printf("cnrtMalloc output Failed!\n");
    exit(-1);
  }
  // 完成cnrtMemcpy拷入函数
  cnrtMemcpy(mlu_input1, input1_half, dims_a * sizeof(half), CNRT_MEM_TRANS_DIR_HOST2DEV);
  cnrtMemcpy(mlu_input2, input2_half, dims_a * sizeof(half), CNRT_MEM_TRANS_DIR_HOST2DEV);
 
  //kernel parameters
  cnrtKernelParamsBuffer_t params;
  cnrtGetKernelParamsBuffer(&params);
  cnrtKernelParamsBufferAddParam(params, &mlu_input1, sizeof(half*)); 
  cnrtKernelParamsBufferAddParam(params, &mlu_input2, sizeof(half*)); 
  cnrtKernelParamsBufferAddParam(params, &pow, sizeof(int));
  cnrtKernelParamsBufferAddParam(params, &mlu_output, sizeof(half*)); 
  cnrtKernelParamsBufferAddParam(params, &dims_a, sizeof(int)); 
  cnrtPlaceNotifier(event_start, pQueue);

  // 完成cnrtInvokeKernel函数
  cnrtInvokeKernel_V2((void*)&PowerDifferenceKernel, dim, params, c, pQueue);
  

  if (CNRT_RET_SUCCESS != cnrtSyncQueue(pQueue))
  {
    printf("syncQueue Failed!\n");
    exit(-1);
  }
  cnrtPlaceNotifier(event_end, pQueue);
  
  //get output data
  // 完成cnrtMemcpy拷出函数
  cnrtMemcpy(output_half, mlu_output, dims_a * sizeof(half), CNRT_MEM_TRANS_DIR_DEV2HOST);

  cnrtConvertHalfToFloatArray(output, output_half,dims_a );

  //free data
  if (CNRT_RET_SUCCESS != cnrtFree(mlu_input1)) {
    printf("cnrtFree Failed!\n");
    exit(-1);
  }
  if (CNRT_RET_SUCCESS != cnrtFree(mlu_input2)) {
    printf("cnrtFree Failed!\n");
    exit(-1);
  }
  if (CNRT_RET_SUCCESS != cnrtFree(mlu_output)) {
    printf("cnrtFree output Failed!\n");
    exit(-1);
  }
  if (CNRT_RET_SUCCESS != cnrtDestroyQueue(pQueue)) {
    printf("cnrtDestroyQueue Failed!\n");
    exit(-1);
  }
  if (CNRT_RET_SUCCESS != cnrtDestroyKernelParamsBuffer(params)) {
    printf("cnrtDestroyKernelParamsBuffer Failed!\n");
    return -1;
  }
  cnrtDestroy();
  free(input1_half);
  free(input2_half);
  free(output_half);
  return 0;
}
