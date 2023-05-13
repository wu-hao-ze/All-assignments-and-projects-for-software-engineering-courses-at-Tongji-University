#include "inference.h"
#include "cnrt.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "stdlib.h"
#include <sys/time.h>
#include <time.h>

namespace StyleTransfer{

typedef unsigned short half;

void cnrtConvertFloatToHalfArray(uint16_t* x, const float* y, int len) {
    for (int i = 0; i < len; i++){
        cnrtConvertFloatToHalf(x+i,y[i]);
    }
}

void cnrtConvertHalfToFloatArray(float* x, const uint16_t* y, int len) {
    for (int i = 0; i < len; i++){
        cnrtConvertHalfToFloat(x+i,y[i]);
    }
}

Inference :: Inference(std::string offline_model){
    offline_model_ = offline_model;
}

void Inference :: run(DataTransfer* DataT){
    // load model
    cnrtInit(0);
    cnrtModel_t model;
    cnrtLoadModel(&model, offline_model_.c_str());

    // set current device
    cnrtDev_t dev;
    cnrtGetDeviceHandle(&dev, 0);
    cnrtSetCurrentDevice(dev);
    int number = 0;
    cnrtGetFunctionNumber(model, &number);
    printf("%d function\n", number);  

    // load extract function
    cnrtFunction_t function;
    if (CNRT_RET_SUCCESS != cnrtCreateFunction(&function)) {
      printf("cnrtCreateFunction Failed!\n");
      exit(-1);
    }
    if (CNRT_RET_SUCCESS != cnrtExtractFunction(&function, model, "subnet0")) {
      printf("cnrtExtractFunction Failed!\n");
      exit(-1);
    }

    // prepare data on cpu
    int inputNum, outputNum;
    int64_t *inputSizeS, *outputSizeS;
    cnrtGetInputDataSize(&inputSizeS, &inputNum, function);
    cnrtGetOutputDataSize(&outputSizeS, &outputNum, function);
    printf("the input num :%d , the output num : %d\n",inputNum,outputNum);
    printf("the input size: %lld, the output size : %lld\n",inputSizeS[0],outputSizeS[0]);

    // allocate I/O data memory on MLU
    void *mlu_input, *mlu_output;

    // prepare input buffer
    float* input_data = reinterpret_cast<float*>(malloc(256 * 256 * 3 * sizeof(float)));
    int t = 256 * 256;
    for(int i=0;i<t;i++)
        for(int j=0;j<3;j++)
            input_data[i * 3 + j] = DataT->input_data[t * j + i];  
    printf("the number of the second: %f\n",input_data[1]);    

    // malloc cpu memory
    half* input_half = (half*)malloc(256 * 256 * 3 * sizeof(half));
    cnrtConvertFloatToHalfArray(input_half, input_data, 256 * 256 * 3);

    // malloc mlu memory
    if (CNRT_RET_SUCCESS != cnrtMalloc(&(mlu_input), inputSizeS[0])) {
      printf("cnrtMalloc Failed!\n");
      exit(-1);
    }
    if (CNRT_RET_SUCCESS != cnrtMemcpy(mlu_input, input_half, 256 * 256 * 3 * sizeof(half), CNRT_MEM_TRANS_DIR_HOST2DEV)) {
      printf("cnrtMemcpy Failed!\n");
      exit(-1);
    }

    // prepare output buffer
    float* output_data = reinterpret_cast<float*>(malloc(256 * 256 * 3 * sizeof(float)));
    DataT->output_data = reinterpret_cast<float*>(malloc(256 * 256 * 3 * sizeof(float)));

    // malloc cpu memory
    half* output_half = (half*)malloc(256 * 256 * 3 * sizeof(half));
    cnrtConvertFloatToHalfArray(output_half, DataT->output_data, 256 * 256 * 3);

    // malloc mlu memory
    if (CNRT_RET_SUCCESS != cnrtMalloc(&(mlu_output), outputSizeS[0])) {
      printf("cnrtMalloc output Failed!\n");
      exit(-1);
    }

    // setup runtime ctx
    cnrtRuntimeContext_t ctx;
    cnrtCreateRuntimeContext(&ctx, function, NULL);

    // bind device
    cnrtSetRuntimeContextDeviceId(ctx, 0);
    cnrtInitRuntimeContext(ctx, NULL);
    void *param[2];
    param[0] = mlu_input;
    param[1] = mlu_output;

    // compute offline
    cnrtQueue_t queue;
    cnrtRuntimeContextCreateQueue(ctx, &queue);
    cnrtInvokeRuntimeContext(ctx, (void**)param, queue, nullptr);
    cnrtSyncQueue(queue);
    
    printf("run success\n");
    
    if (CNRT_RET_SUCCESS != cnrtMemcpy(output_half, mlu_output, 256 * 256 * 3 * sizeof(half), CNRT_MEM_TRANS_DIR_DEV2HOST)) {
      printf("cnrtMemcpy output Failed!\n");
      exit(-1);
    }
    cnrtConvertHalfToFloatArray(output_data, output_half, 256 * 256 * 3);
    printf("memcpy output success\n");
    for(int i=0;i<t;i++)
        for(int j=0;j<3;j++)
            DataT->output_data[t*j+i] = output_data[i*3+j];

    // free memory spac
    if (CNRT_RET_SUCCESS != cnrtFree(mlu_input)) {
      printf("cnrtFree Failed!\n");
      exit(-1);
    }
    if (CNRT_RET_SUCCESS != cnrtFree(mlu_output)) {
      printf("cnrtFree output Failed!\n");
      exit(-1);
    }
    printf("free mlu success\n");
    if (CNRT_RET_SUCCESS != cnrtDestroyQueue(queue)) {
      printf("cnrtDestroyQueue Failed!\n");
      exit(-1);
    }
    printf("free queue success\n");
    cnrtDestroy();
    //free(param);
    free(input_half);
    free(output_half);
}

} // namespace StyleTransfer
