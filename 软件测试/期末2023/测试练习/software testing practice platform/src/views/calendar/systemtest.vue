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

    <el-divider content-position="right">测试用例</el-divider>

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
          prop="year"
          label="年份"
          width="120"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="month"
          width="120"
          label="月份"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="day"
          width="120"
          label="天"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="expectation"
          label="预期输出"
          align="center"
        ></el-table-column>
        <el-table-column
          prop="actual"
          label="实际输出"
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
import mock_1_json from "@/mock/calendar/calendar_mock_1.json";
import mock_2_json from "@/mock/calendar/calendar_mock_2.json";
import mock_3_json from "@/mock/calendar/calendar_mock_3.json";
import { testcalendar } from "@/api/calendartest.js";
export default {
  name: "SystemTest",
  components: {},
  props: ["parentHeight"],
  data() {
    return {
      options: [
        { value: "1", label: "健壮边界值分析" },
        { value: "2", label: "强健壮等价类测试" },
        { value: "3", label: "决策表法"},
      ],
      value: "1",
      tableData: [],
      loading: false,
      classState: [],
    };
  },
  computed: {
    tableHeight(){
      return (this.parentHeight - 260) > 650 ? 650 : (this.parentHeight - 260);
    }
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
        newData["info"] = "";
        newData["state"] = null;
        this.tableData.push(newData);
      });
    },
    tableRowClassName({ row, rowIndex }) {
      return this.classState[rowIndex];
    },
    doTest() {
      this.tableData.forEach((item) => {
        let y = parseInt(item.year)
        let m = parseInt(item.month)
        let d = parseInt(item.day)
        let resY = y
        let resM = m
        let resD = d
        let res = ""
        if(y < 1900 || y > 2100 || m < 1 || m > 12 || d < 1 || d > 31){
          res = "-1"
        }
        else {
          if(m == 1 || m == 3 || m == 5 || m == 7 || m == 8 || m == 10 || m == 12){
            resD = d + 1
            if(resD > 31){
              resM = m + 1
              resD = 1
              if(resM > 12){
                resY = y + 1
                resM = 1
              }
            }
          }
          else if(m == 4 || m == 6 || m == 9 || m == 11){
            if(d > 30){
              res = "-1"
            }
            else {
              resD = d + 1
              if(resD > 30){
                resM = m + 1
                resD = 1
                if(resM > 12){
                  resY = y + 1
                  resM = 1
                }
              }
            }
          }
          else {
            if((y == 1900 || y == 2100)||(y % 4 != 0)){
              if(d > 28){
                res = "-1"
              }
              else {
                resD = d + 1
                if(resD > 28){
                  resM = m + 1
                  resD = 1
                  if(resM > 12){
                    resY = y + 1
                    resM = 1
                  }
                }
              }
            }
            else {
              if(d > 29){
                res = "-1"
              }
              else {
                resD = d + 1
                if(resD > 29){
                  resM = m + 1
                  resD = 1
                  if(resM > 12){
                    resY = y + 1
                    resM = 1
                  }
                }
              }
            }
          }
        }
        if(res != "-1"){
          res = resY.toString() + "-" + resM.toString() + "-" +resD.toString();
        }
        item.actual = res
        if(item.actual == item.expectation){
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
      } else if (value === "2") {
        this.initTableData(mock_2_json);
      } else if (value === "3") {
        this.initTableData(mock_3_json);
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
