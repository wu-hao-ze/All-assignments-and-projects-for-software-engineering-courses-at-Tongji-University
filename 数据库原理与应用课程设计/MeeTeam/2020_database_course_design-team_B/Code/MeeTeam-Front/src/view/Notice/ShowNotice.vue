<template>
  <div id="show">
    <div id="main_page">
      <el-main>
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>通知管理</el-breadcrumb-item>
          <el-breadcrumb-item>通知详情</el-breadcrumb-item>
        </el-breadcrumb>
        <div id="app">
          <h2 v-text="title"></h2>
          <span v-text="content"></span>
        </div>
      </el-main>
    </div>
  </div>
</template>
 
<script>
/* global Vue */
import { get_notice } from '@/api/notice.js'
export default {
  data() {
    return {
      title: '',
      content: ''
    }
  },
  created() {
    this.get_NoticeInfo();
  },
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/NoticeList" });
    },
    goto_create_notice() {
      var n_id = this.$route.query.n_id;
      this.$router.push({ path: "/Admin/CreateNotice", query: { n_id: n_id } });
    },
    get_NoticeInfo() {
      let para = {
        id: this.$route.query.n_id
      };
      get_notice(para).then(res => {
        this.title = res.data.notice_title;
        this.content = res.data.notice_content;
      })
    }
  }
}
</script>

<style>
#show {
  width: 100%;
  min-height: 100vh;
  background-color: #ecf0f5;
  background-size: 100% 100%;
}

#app {
  text-align: center;
}

#breadcrumb {
  height: 30px;
  margin-bottom: 10px;
}
</style>