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

#include "cnplugin.h"
#include "plugin_power_difference_kernel.h"

typedef uint16_t half;
#if (FLOAT_MODE == 1)
typedef float DType;
#elif (FLOAT_MODE == 0)     // NOLINT
typedef half DType;
#endif

cnmlStatus_t cnmlCreatePluginPowerDifferenceOpParam(
  cnmlPluginPowerDifferenceOpParam_t *param,
  int pow,
  int dims_a,
  cnmlCoreVersion_t core_version
  // TODO：添加变量
) {
  *param = new cnmlPluginPowerDifferenceOpParam();

  int static_num = 0;//无常量数据

  (*param)->pow = pow;
  (*param)->dims_a = dims_a;
  // TODO：配置变量

  return CNML_STATUS_SUCCESS;
}

cnmlStatus_t cnmlDestroyPluginPowerDifferenceOpParam(
  cnmlPluginPowerDifferenceOpParam_t *param
) {
  delete (*param);
  *param = nullptr;

  return CNML_STATUS_SUCCESS;
}

cnmlStatus_t cnmlCreatePluginPowerDifferenceOp(
  cnmlBaseOp_t *op,
  cnmlTensor_t *pd_input_tensors,
  int pow,
  cnmlTensor_t *pd_output_tensors,
  int len
  // TODO：添加变量
) {
  cnrtKernelParamsBuffer_t params;
  cnrtGetKernelParamsBuffer(&params);
  // TODO：配置变量

  cnrtKernelParamsBufferMarkInput(params);
  cnrtKernelParamsBufferMarkInput(params);
  cnrtKernelParamsBufferAddParam(params,&pow,sizeof(int));
  cnrtKernelParamsBufferMarkOutput(params);
  cnrtKernelParamsBufferAddParam(params,&len,sizeof(int));
  void **InterfacePtr = reinterpret_cast<void**>(&PowerDifferenceKernel);
  cnmlCreatePluginOp(op,
                     "PowerDifference",
                     InterfacePtr,
                     params,
                     pd_input_tensors,
                     2,
                     pd_output_tensors,
                     1,
                     nullptr,
                     0);
  cnrtDestroyKernelParamsBuffer(params);
  return CNML_STATUS_SUCCESS;
}

cnmlStatus_t cnmlComputePluginPowerDifferenceOpForward(
  cnmlBaseOp_t op,
  void *inputs[],
  void *outputs[],
  // TODO：添加变量
  cnrtQueue_t queue
) {
  // TODO：完成Compute函数
  cnmlComputePluginOpForward_V4(op,
                                nullptr,
                                inputs,
                                2,
                                nullptr,
                                outputs,
                                1,
                                queue,
                                nullptr);
  return CNML_STATUS_SUCCESS;
}


