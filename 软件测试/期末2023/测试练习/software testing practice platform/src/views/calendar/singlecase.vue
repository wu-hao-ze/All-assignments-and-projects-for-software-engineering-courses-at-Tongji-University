<template>
  <div class="single-case">
    <el-form
      class="single-form"
      :label-position="labelPosition"
      label-width="400px"
      :model="formLabelAlign"
    >
      <el-form-item label="年份">
        <el-input v-model="formLabelAlign.year"></el-input>
      </el-form-item>
      <el-form-item label="月份">
        <el-input v-model="formLabelAlign.month"></el-input>
      </el-form-item>
      <el-form-item label="天数">
        <el-input v-model="formLabelAlign.day"></el-input>
      </el-form-item>
    </el-form>
          <el-button
        class="main-button"
        type="success"
        plain
        @click="doTest"
        :loading="loading"
        >进行测试<i class="el-icon-upload el-icon--right"></i
      ></el-button>
        <div>
    <span>实际输出：{{actual}}</span>
  </div>
  </div>
</template>

<script>

import { testcalendar } from "@/api/calendartest.js";

export default {
  name: "SingleCase",
  components: {},
  props: {},
  data() {
    return {
      actual:"",
      info:"",
      labelPosition: 'right',
        formLabelAlign: {
          year: 0,
          month: 0,
          day: 0,
          expectation:0,
        }, 
        date:"",
        loading:false,

    };
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {
    doTest(){
      let y = parseInt(this.formLabelAlign.year)
      let m = parseInt(this.formLabelAlign.month)
      let d = parseInt(this.formLabelAlign.day)
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
      this.actual = res
    }
  },
};
</script>

<style scoped>
.item {
  margin-bottom: 10px;
}
.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
.clearfix:after {
  clear: both;
}
.main-form {
  margin-top: 10px;
}
.main-button {
  width:100%;

}
.box-card {
  padding: 0;
}
.single-form{
  width:600px;
  top:50%;
  left:50%;
}
</style>
