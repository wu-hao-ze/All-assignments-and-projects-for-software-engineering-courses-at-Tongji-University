<template>
  <div id="body">
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }"
            >首页</el-breadcrumb-item
          >
          <el-breadcrumb-item>通知管理</el-breadcrumb-item>
          <el-breadcrumb-item>新建通知</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <el-form :model="FormData" ref="FormData">
        <el-form-item>
          <el-input
            type="text"
            v-model="FormData.title"
            placeholder="请输入公告标题"
          ></el-input>
        </el-form-item>
        <el-form-item>
          <el-input
            type="textarea"
            rows="10"
            v-model="FormData.content"
            placeholder="请输入公告内容"
          ></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="info" @click="onSubmit(0)">暂存公告</el-button>
          <el-button type="primary" @click="onSubmit(1)">发布公告</el-button>
        </el-form-item>
      </el-form>
    </el-main>
  </div>
</template>

<script>
import { get_notice } from "@/api/notice.js";
import { create_notice } from "@/api/notice.js";
import global_msg from "../../utils/global.js";

export default {
  data() {
    return {
      FormData: {
        title: "",
        content: "",
      },
    };
  },
  created() {
    this.get_NoticeInfo();
  },
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/NoticeList" });
    },
    get_NoticeInfo() {
      if (this.$route.query.n_id) {
        let params = {
          id: this.$route.query.n_id,
        };
        get_notice(params).then((res) => {
          this.FormData.title = res.data.notice_title;
          this.FormData.content = res.data.notice_content;
        });
      }
    },
    onSubmit(para) {
      let o = {
        notice_id: "",
        notice_title: this.FormData.title,
        notice_content: this.FormData.content,
        admin_id: global_msg.nowadminid,
        operate_type: para,
        operate_time: "",
      };
      var not = JSON.stringify(o);
      console.log(not);

      create_notice(not).then(function (res) {
        if (!res) {
          alert("公告修改失败！");
        } else {
          if (para == 0) {
            alert("公告暂存成功！");
          } else {
            alert("公告发布成功！");
          }
        }
      });
    },
  },
};
</script>

<style>
#breadcrumb {
  height: 30px;
  margin-bottom: 10px;
}

#edit {
  caret-color: auto;
}

#body {
  position: relative;
  min-height: 100vh;
  height: 100%;
}
</style>