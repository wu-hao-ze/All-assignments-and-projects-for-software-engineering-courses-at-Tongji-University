<template>
  <div id="NoticeList">
    <div id="body">
      <el-main>
        <div id="breadcrumb">
          <el-breadcrumb separator-class="el-icon-arrow-right">
            <el-breadcrumb-item :to="{ path: '/users/UserPage' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item>平台通知</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <el-table :data="tableData" border @row-click="goto_user_ShowNotice" v-if="isAlive">
          <el-table-column v-if="false" prop="notice_id" label="公告id">
          </el-table-column>
          <el-table-column prop="title" label="公告标题" align="left" style="margin: 50px;">
          </el-table-column>
          <el-table-column prop="admin_id" label="发布者" align="center">
          </el-table-column>
          <el-table-column prop="operate_time" label="上次修改时间" align="center">
          </el-table-column>
        </el-table>
      </el-main>
    </div>
  </div>
</template>
 
<script>
import { get_all } from '@/api/notice.js'
export default {
  data() {
    this.tableData = [];
    get_all().then(res => {
      Object.keys(res.data).forEach(v => {
        let o = {};
        o.notice_id = res.data[v].notice_id;
        o.title = res.data[v].notice_title;
        o.admin_id = res.data[v].admin_id;
        o.operate_time = res.data[v].operate_time;
        if (res.data[v].operate_type) {
          this.tableData.push(o);
        }
      });
    });
    return {
      isAlive: true
    };
  },
  methods: {
    goback() {
      this.$router.push({ path: "/users/UserPage" });
    },
    goto_user_ShowNotice(row) {
      this.$router.push({ path: "/users/User_ShowNotice", query: { n_id: row.notice_id } });
    }
  },
}
</script>

<style>
.el-main {
  margin-left: 8%;
}

#body {
  position: relative;
  min-height: 100vh;
  height: 100%;
}

#breadcrumb{
  height: 30px;
  margin-bottom: 10px;
}
</style>