<template>
  <div class="system-test">
    <div class="main-header">
      <el-select v-model="value" placeholder="请选择测试方法">
        <el-option
          v-for="item in options"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        />
      </el-select>
      <div class="button-group">
        <el-button
          class="main-button"
          type="success"
          plain
          @click="doTest"
          :loading="loading"
          >进行测试<i class="el-icon-upload el-icon--right"></i
        ></el-button>
        <el-button
          @click="reset(value)"
          class="reset-button"
          type="warning"
          plain
          >重置</el-button
        >
      </div>
    </div>

    <el-divider content-position="left">测试用例</el-divider>

    <div class="main-table">
      <el-table
        :data="tableData"
        :height="tableHeight"
        border
        style="width: 100%"
        v-loading="loading"
        :row-class-name="tableRowClassName"
      >
        <el-table-column
          prop="id"
          label="测试用例编号"
          width="120"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="A"
          label="第一条边的值（a）"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="B"
          label="第二条边的值（b）"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="C"
          label="第三条边的值（c）"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="expectation"
          label="程序预期输出"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="actual"
          label="程序实际输出"
          align="center"
        ></el-table-column>
        <el-table-column
            prop="state"
            label="测试结果"
            align="center">
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import mock_1_json from "@/mock/triangle/triangle_mock_1.json";
import mock_2_json from "@/mock/triangle/triangle_mock_2.json";
import { testtriangle } from "@/api/triangletest.js";
export default {
  name: "SystemTest",
  components: {},
  props: ["parentHeight"],
  data() {
    return {
      options: [
        { value: "1", label: "健壮性边界分析" },
        { value: "2", label: "等价类法" },
      ],
      value: "1",
      tableData: [],
      loading: false,
      classState: [],
      inputData: {
        triangle_test_list: mock_1_json,
      },
    };
  },
  computed: {
    tableHeight() {
      return this.parentHeight - 260 > 700 ? 700 : this.parentHeight - 260;
    },
  },
  watch: {
    value: {
      handler(newVal) {
        this.reset(newVal);
      },
      immediate: false,
    },
  },
  created() {},
  mounted() {
    this.initTableData(mock_1_json);
  },
  methods: {
    initTableData(json) {
      this.classState = [];
      this.tableData = [];
      json.forEach((element) => {
        let newData = {};
        for (let key in element) {
          newData[key] = element[key];
        }
        newData["actual"] = "";
        newData["state"] = null;
        this.tableData.push(newData);
      });
    },
    tableRowClassName({ row, rowIndex }) {
      return this.classState[rowIndex];
    },
    doTest() {
      this.tableData.forEach((item) => {
        let a = parseFloat(item.A)
        let b = parseFloat(item.B)
        let c = parseFloat(item.C)
        let ans = ""
        var nums = [a, b, c]

        function func(a, b) {
          return a - b;
        }

        nums.sort(func)
        if (nums[0] <= 0 || (nums[0] + nums[1]) <= nums[2]){
          ans = "不构成三角形"
        }
        else if(nums[0] == nums[1] && nums[1] == nums[2]){
          ans = "等边三角形"
        }
        else if(nums[0] == nums[1] || nums[1] == nums[2]){
          ans = "等腰三角形"
        }
        else {
          ans = "一般三角形"
        }
        console.log(ans)
        item.actual = ans
        if(item.expectation == item.actual){
          item.state = "测试通过"
        }
        else {
          item.state = "测试未通过"
        }
      })
    },
    reset(value) {
      if (value === "1") {
        this.initTableData(mock_1_json);
        this.inputData = {
          triangle_test_list: mock_1_json,
        };
      } else if (value === "2") {
        this.initTableData(mock_2_json);
        this.inputData = {
          triangle_test_list: mock_2_json,
        };
      }
    },
  },
};
</script>

<style scoped lang="less">
/deep/ .el-table .error-row {
  background: #fff0f0;
}
/deep/ .el-table .success-row {
  background-color: #f7fff9;
}
.main-button {
  width: 500px;
  margin-top: 10px;
}
.reset-button {
  width: 200px;
  margin-top: 10px;
}
.main-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.main-table {
  height: 100%;
  display: flex;
  align-items: center;
}
</style>
