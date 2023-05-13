<template>
  <div id="logout-container">
    <div style="text-align: center;height: 50px">
      是否退出登录？
    </div>
    <el-form :model="ruleForm" status-icon :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
      <el-form-item>
        <el-button type="primary" @click="logout()">确定</el-button>
        <el-button @click="giveup()">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>


<script>
import global_msg from '../../utils/global.js'

export default
  {
    name: 'LogOut',
    data() {
      return {
        user: this.$route.query.id,
        pass: this.pass,
        isLogin: true
      };
    },
    
    methods:
    {
      giveup() {
        this.$message.success("退出取消");
        this.$router.push({ path: "/users/InforList", query: { id: this.user } }); //接下来进入到哪个路由
      },
      logout() {
        global_msg.nowuserid = '';
        this.$message.success("退出成功");
        this.$router.push({ path: "/login" }); //接下来进入到哪个路由
      }
    }

  }
</script>


<style scoped>
 body {
   margin: 0;
 }

 #logout-container {
   width: 300px;
   height: 150px;
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