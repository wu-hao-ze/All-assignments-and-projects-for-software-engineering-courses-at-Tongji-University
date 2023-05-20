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
      <div class="data-text">测试中日期取2021年5月（共31天）</div>
      <div class="button-group">
        <el-button
        class="main-button"
        type="success"
        plain
        @click="doTest"
        :loading="loading"
        >进行测试<i class="el-icon-upload el-icon--right"></i
      ></el-button>
      <el-button @click="reset(value)" class="reset-button" type="warning" plain>重置</el-button>
      </div>
    </div>

    <el-divider content-position="left">测试用例</el-divider>

    <div class="main-table">
      <el-table
        :data="tableData"
        :height="tableHeight"
        border
        style="width: 100%;"
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
          prop="X"
          label="本月的通话分钟数X（分钟）"
          width="240"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="Y"
          label="本年度至本月的累计未按时缴费的次数Y（次）"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="expectation"
          label="每月的电话总费用预期输出"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="actual"
          label="每月的电话总费用实际输出"
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
import mock_1_json from "@/mock/cash/cash_mock_1.json";
import mock_2_json from "@/mock/cash/cash_mock_2.json";
import mock_3_json from "@/mock/cash/cash_mock_3.json";
import mock_4_json from "@/mock/cash/cash_mock_4.json";
import { testcash } from "@/api/cashtest.js";
export default {
  name: "SystemTest",
  components: {},
  props: ["parentHeight"],
  data() {
    return {
      options: [
        {value: "1",label: "健壮性边界值法",},
        {value: "2",label: "强健壮等价类测试",},
        {value: "3",label: "决策表法",},
        {value: "4",label: "综合最优用例",},
      ],
      value: "1",
      tableData: [],
      loading: false,
      classState: [],
      json:{},
    };
  },
  computed: {
    tableHeight(){
      return (this.parentHeight - 260) > 700 ? 700 : (this.parentHeight - 260);
    }
  },
  watch: {
    value:{
      handler(newVal){
        this.reset(newVal);
      },
      immediate:false,
    }
  },
  created() {},
  mounted() {
    this.initTableData(mock_1_json);
    this.json = mock_1_json;
  },
  methods: {
    initTableData(json){
      this.classState = [];

      this.tableData = [];
      json.forEach((element) => {
      let newData = {};
      for (let key in element) {
        if(key != "year" || key != "month"){
          newData[key] = element[key];
        }
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
        let time = parseFloat(item.X)
        let num = parseFloat(item.Y)
        let res = 0.0
        if(time < 0 || time > 44640 || num < 0 || num > 11){
          res = -1
        }
        else{
          if(time <= 60){
            if(num <= 1){
              res = 25 + time * 0.15 * 99 / 100
            }
            else{
              res = 25 + time * 0.15
            }
          }
          else if(time <= 120){
            if(num <= 2){
              res = 25 + time * 0.15 * 985 / 1000
            }
            else{
              res = 25 + time * 0.15
            }
          }
          else if(time <= 180){
            if(num <= 3){
              res = 25 + time * 0.15 * 98 / 100
            }
            else{
              res = 25 + time * 0.15
            }
          }
          else if(time <= 300){
            if(num <= 3){
              res = 25 + time * 0.15 * 975 / 1000
            }
            else{
              res = 25 + time * 0.15
            }
          }
          else {
            if(num <= 6){
              res = 25 + time * 0.15 * 97 / 100
            }
            else{
              res = 25 + time * 0.15
            }
          }
        }
        res = Math.round(res * 10000) / 10000
        item.actual = res
        if(item.actual == parseFloat(item.expectation)){
          item.state = "测试通过"
        }
        else {
          item.state = "测试未通过"
        }
      })
    },
    reset(value){
        if(value === "1"){
          this.json = mock_1_json;
          this.initTableData(mock_1_json);
        }
        else if(value === "2"){
          this.json = mock_2_json;
          this.initTableData(mock_2_json);
        }
        else if(value ==="3"){
          this.json = mock_3_json;
          this.initTableData(mock_3_json);
        }
        else{
          this.json = mock_4_json;
          this.initTableData(mock_4_json);
        }
    }
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
.main-header{
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom:20px;
}
.main-table{
  height: 100%;
  display: flex;
  align-items: center;
}
</style>
