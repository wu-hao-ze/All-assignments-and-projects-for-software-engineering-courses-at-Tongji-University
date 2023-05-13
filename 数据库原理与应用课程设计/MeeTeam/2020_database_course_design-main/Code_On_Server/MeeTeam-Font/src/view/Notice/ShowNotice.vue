<template>
  <div id="body">
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
</template>
 

<script>
/* global Vue */
import { get_notice } from "@/api/notice.js";
export default {
  data() {
    return {
      user: this.$route.query.id,
      title: "",
      content: "",
    };
  },

  created() {
    this.get_NoticeInfo();
  },
  
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/NoticeList", query: { id: this.user } });
    },
    goto_create_notice() {
      var n_id = this.$route.query.n_id;
      this.$router.push({ path: "/Admin/CreateNotice", query: { n_id: n_id, id: this.user } });
    },
    get_NoticeInfo() {
      let para = {
        id: this.$route.query.n_id,
      };
      get_notice(para).then((res) => {
        this.title = res.data.notice_title;
        this.content = res.data.notice_content;
      });
    },
  },
};
</script>


<style>
#app {
  text-align: center;
}

#breadcrumb {
  height: 30px;
  margin-bottom: 10px;
}

#body {
  position: relative;
  min-height: 100vh;
  height: 100%;
}
</style>