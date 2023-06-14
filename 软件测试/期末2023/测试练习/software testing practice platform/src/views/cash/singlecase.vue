<template>
  <div class="single-case">
    <el-form
      class="single-form"
      :label-position="labelPosition"
      label-width="400px"
      :model="formLabelAlign"
    >
      <el-form-item label="本月的通话分钟数X（分钟）">
        <el-input v-model="formLabelAlign.X"></el-input>
      </el-form-item>
      <el-form-item label="本年度至本月的累计未按时缴费的次数Y（次）">
        <el-input v-model="formLabelAlign.Y"></el-input>
      </el-form-item>
      <el-form-item label="每月的电话总费用预期输出">
        <el-input v-model="formLabelAlign.expectation"></el-input>
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
import { testcash } from "@/api/cashtest.js";
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
          X: 0,
          Y: 0,
          expectation: 0
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
      let time = parseFloat(this.formLabelAlign.X)
      let num = parseFloat(this.formLabelAlign.Y)
      let res = 0.0
      if(time < 0 || time > 44640 || num < 0 || num > 11){
        res = -1
      }
      else{
        if(time <= 60){
          if(num <= 1){
            res = 25 + time * 15 * 99 / 10000
          }
          else{
            res = 25 + time * 15 / 100
          }
        }
        else if(time <= 120){
          if(num <= 2){
            res = 25 + time * 15 * 985 / 100000
          }
          else{
            res = 25 + time * 15 / 100
          }
        }
        else if(time <= 180){
          if(num <= 3){
            res = 25 + time * 15 * 98 / 10000
          }
          else{
            res = 25 + time * 15 / 100
          }
        }
        else if(time <= 300){
          if(num <= 3){
            res = 25 + time * 15 * 975 / 100000
          }
          else{
            res = 25 + time * 15 / 100
          }
        }
        else {
          if(num <= 6){
            res = 25 + time * 15 * 97 / 10000
          }
          else{
            res = 25 + time * 15 / 100
          }
        }
      }
      res = Math.round(res * 1000) / 1000
      this.actual = res
    }
  },
};
</script>

<style scoped>
.main-button {
  width:100%;
}
.single-form{
  width:600px;
  top:50%;
  left:50%;
}
</style>
