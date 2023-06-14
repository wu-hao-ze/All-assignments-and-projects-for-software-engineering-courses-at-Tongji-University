<template>
  <div class="single-case">
    <el-form
      class="single-form"
      :label-position="labelPosition"
      label-width="400px"
      :model="formLabelAlign"
    >
      <el-form-item label="销售的主机数量M（台）">
        <el-input v-model="formLabelAlign.M"></el-input>
      </el-form-item>
      <el-form-item label="销售的显示器数量I（台）">
        <el-input v-model="formLabelAlign.I"></el-input>
      </el-form-item>
      <el-form-item label="销售的外设数量P（套）">
        <el-input v-model="formLabelAlign.P"></el-input>
      </el-form-item>
      <el-form-item label="预计销售额（元）">
        <el-input v-model="formLabelAlign.pre_amount"></el-input>
      </el-form-item>
      <el-form-item label="预计佣金（元）">
        <el-input v-model="formLabelAlign.pre_earn"></el-input>
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
    <span>实际销售额：{{A}}</span>
    <el-divider direction="vertical"></el-divider>
    <span>实际佣金：{{E}}</span>
  </div>
  </div>
</template>

<script>
import { testsales } from "@/api/salestest.js";
export default {
  name: "SingleCase",
  components: {},
  props: {},
  data() {
    return {
        S:"",
        A:"",
        E:"",
      labelPosition: 'right',
        formLabelAlign: {
          M: "",
          I: "",
          P: "",
          predict:"正常/错误",
          pre_amount:"",
          pre_earn:"",
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
      let m = this.formLabelAlign.M
      let i = this.formLabelAlign.I
      let p = this.formLabelAlign.P
      let res1 = 0.0
      let res2 = 0.0
      if(m <= 0 || i <= 0 || p <= 0 || m > 70 || i > 80 || p > 90){
        res1 = -1
        res2 = -1
      }
      else {
        res1 = m * 25 + i * 30 + p * 45
        if(res1 <= 1000){
          res2 = res1 * 0.1
        }
        else if(res1 <= 1800){
          res2 = res1 * 0.15
        }
        else {
          res2 = res1 * 0.2
        }
        res2 = Math.round(res2 * 100) / 100
      }
      this.A = res1
      this.E = res2
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
