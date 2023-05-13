<template>
  <el-main>
    <el-breadcrumb separator-class="el-icon-arrow-right">
      <el-breadcrumb-item :to="{ path: '/users/UserPage' }"
        >首页</el-breadcrumb-item
      >
      <el-breadcrumb-item :to="{ path: '/users/NoticeList' }"
        >平台通知</el-breadcrumb-item
      >
      <el-breadcrumb-item>通知详情</el-breadcrumb-item>
    </el-breadcrumb>
    <div id="app">
      <h2 v-text="title"></h2>
      <span v-text="content"></span>
    </div>
  </el-main>
</template>
 
<script>
/* global Vue */
import { get_notice } from "@/api/notice.js";
export default {
  data() {
    return {
      title: "",
      content: "",
    };
  },
  created() {
    this.get_NoticeInfo();
  },
  methods: {
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
</style>