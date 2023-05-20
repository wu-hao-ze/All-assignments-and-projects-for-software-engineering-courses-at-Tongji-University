<template>
  <div class="single-case">
    <el-form
      class="single-form"
      :label-position="labelPosition"
      label-width="400px"
      :model="formLabelAlign"
    >
      <el-form-item label="第一条边的值">
        <el-input v-model="formLabelAlign.A"></el-input>
      </el-form-item>
      <el-form-item label="第二条边的值">
        <el-input v-model="formLabelAlign.B"></el-input>
      </el-form-item>
      <el-form-item label="第三条边的值">
        <el-input v-model="formLabelAlign.C"></el-input>
      </el-form-item>
      <el-form-item label="程序预期输出">
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
import { testtriangle } from "@/api/triangletest.js";
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
          A: "",
          B: "",
          C: "",
          expectation: ""
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
    doTest: function () {
      let a = parseFloat(this.formLabelAlign.A)
      let b = parseFloat(this.formLabelAlign.B)
      let c = parseFloat(this.formLabelAlign.C)
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
      this.actual = ans
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
