<template>
  <div id="creatteam-container">
    <el-container>
      <el-main>
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>个人信息</el-breadcrumb-item>
        </el-breadcrumb>
        
        <el-descriptions class="margin-top" title=" " :column="3" :size="size" border>
          <template slot="extra">
          </template>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              管理员用户名
            </template>
            {{ admin_id }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              联系方式
            </template>
            {{ contact_info }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-date"></i>
              注册时间
            </template>
            {{ register_time }}
          </el-descriptions-item>
        </el-descriptions>
      </el-main>
    </el-container>
  </div>
</template>


<script>
import { GetUserInfor } from '@/api/MyInfor.js'

export default {
  name: 'AdminIndormation',
  data() {
    return {
      admin_id: this.$route.query.id,
      contact_info: '',
      register_time: '',
    };
  },
  mounted() {
    this.getlist() //页面一进入就加载表格
    this.getcollect()
  },
  methods: {
    getlist() {
      let vm = this;
      let params = {
        user_id: this.admin_id,
      };
      GetUserInfor(params).then(res => {
        let item = res.data[0];
        vm.contact_info = item.contact_info;
        vm.institution = item.institution;
        vm.register_time = item.register_time.replace("\"", "").replace("\"", "");
        vm.introduction = item.introduction;
      })
    },
  }
}
</script>
  

<style>
#creatteam-container {
  position: relative;
  margin: 10px 0;
  margin-left: 0px;
  height: 100%;
}

.el-aside {
  color: #333;
}
</style>