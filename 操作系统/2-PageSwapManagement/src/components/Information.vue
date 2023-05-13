<template>
  <div>
    <p>作业指令总数</p>
    <h3>{{ sumInstruction }}</h3>
    <p>每页存放指令数</p>
    <h3>{{ instructionEachPage }}</h3>
    <p>作业占用内存页数</p>
    <h3>{{ memoryPage }}</h3>
    <p>页面置换算法</p>

    <el-select v-model="algorithmValue" placeholder="请选择">
      <el-option
        v-for="item in algorithmOptions"
        :key="item.value"
        :label="item.label"
        :value="item.value"
      >
      </el-option>
    </el-select>

    <p>指令执行顺序</p>
    <el-select v-model="sequenceValue" placeholder="请选择">
      <el-option
        v-for="item in sequence"
        :key="item.value"
        :label="item.label"
        :value="item.value"
      >
      </el-option>
    </el-select>
  </div>
</template>



<script>
export default {
  data: () => ({
    show: true,
    algorithmOptions: [
      {
        value: "FIFO",
        label: "FIFO算法",
      },
      {
        value: "LRU",
        label: "LRU算法",
      },
    ],
    algorithmValue: "FIFO",
    //指令执行顺序
    sequence: [
      {
        value: "skip",
        label: "混合执行",
      },
      {
        value: "sequential",
        label: "顺序执行",
      },
      {
        value: "random",
        label: "随机执行",
      },
    ],
    sequenceValue: "skip",
  }),
  name: "Information",
  props: {
    sumInstruction: Int32Array,
    instructionEachPage: Int32Array,
    memoryPage: Int32Array,
    replaceAlgorithm: String,
    executionSequence: String,
  },
};
</script>

<style scoped>
.transition-box {
  margin-bottom: 10px;
  width: 200px;
  height: 100px;
  border-radius: 4px;
  background-color: #409eff;
  text-align: center;
  color: #fff;
  padding: 40px 20px;
  box-sizing: border-box;
  margin-right: 20px;
}
</style>