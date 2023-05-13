<template>
   <div id="login-container">
    <div style="text-align: center;height: 50px">
      登录
    </div>
    <el-form :model="ruleForm" status-icon :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
      <el-form-item label="账号" prop="user">
        <el-input type="text" v-model="user" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item label="密码" prop="pass">
        <el-input type="password" v-model="pass" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm('ruleForm')">提交</el-button>
        <el-button @click="resetForm('ruleForm')">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import axios from "axios";
export default {
   name: 'LogIn',
   data() {
    return {
      user: '',
      pass: '',
      isLogin:false
    };
  },
  methods: {
    resetForm(formName) {//重置表单
      this.user=''
      this.pass=''
    },
     //提交表单
    submitForm(formName) {
      if(this.user===""){
      alert("请输入用户名")
      return;
      }
      if(this.pass===""){
        alert("请输入密码")
        return;
      }
      //请求地址,this和vm指的是全局
      var vm = this;
      axios.get("https://localhost:44338/Login/login?ID="+this.user+"&Word="+this.pass).then(function(res){
        if(res.data===false){
          vm.$message.error("登录失败");
          vm.resetForm(formName);
        }else{
          vm.$message.success("登录成功");
          vm.$router.push({path: "/users/InforList"}); //接下来进入到哪个路由
        }
      })
    }
  }
}
</script>

<style>
 body{
    margin: 0;
  }
  #login-container{
    width: 400px;
    height: 290px;
    background: #e5e9f2;
    position: absolute;
    left: 50%;
    top: 50%;
    margin-left: -220px;
    margin-top: -170px;
    border-radius: 5px;
    padding-top: 40px;
    padding-right: 40px;
  }
</style>