<template>
  <div id="NoticeList">
    <div id="body">
      <el-main>
        <div id="breadcrumb">
          <el-breadcrumb separator-class="el-icon-arrow-right">
            <el-breadcrumb-item :to="{ path: '/users/UserPage' }"
              >首页</el-breadcrumb-item
            >
            <el-breadcrumb-item>平台通知</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <el-card>
          <div class="filter-container">
            <el-input
              placeholder="公告名称"
              style="width: 180px"
              class="filter-item"
              v-model="listQuery.notice_title"
              @keyup.enter.native="handleFilter"
            ></el-input>
            &nbsp;
            <el-input
              placeholder="发布人"
              style="width: 180px"
              class="filter-item"
              v-model="listQuery.admin_id"
              @keyup.enter.native="handleFilter"
            ></el-input>
            &nbsp;
            <el-button
              class="filter-item"
              type="primary"
              icon="el-icon-search"
              @click="handleFilter"
            >
              搜索
            </el-button>
          </div>
        </el-card>
        <br />
        <el-card>
          <el-table
            :data="tableData"
            border
            @row-click="goto_user_ShowNotice"
            v-if="isAlive"
            :header-cell-style="{ textAlign: 'center' }"
            :cell-style="{ 'text-align': 'center' }"
          >
            <el-table-column v-if="false" prop="notice_id" label="公告id">
            </el-table-column>
            <el-table-column
              prop="notice_title"
              label="公告标题"
              align="left"
              style="margin: 50px"
            >
            </el-table-column>
            <el-table-column prop="admin_id" label="发布者" align="center">
            </el-table-column>
            <el-table-column
              prop="operate_time"
              label="发布时间"
              align="center"
              sortable
            >
            </el-table-column>
            <el-table-column label="操作">
              <template slot-scope="scope">
                <el-button
                  size="mini"
                  @click="goto_user_ShowNotice(scope.$index)"
                  >查看详情</el-button
                >
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-main>
    </div>
  </div>
</template>
 
<script>
import { fetchList } from "@/api/notice.js";

export default {
  data() {
    return {
      tableData: [],
      isAlive: true,
      listQuery: {
        notice_title: "",
        admin_id: "",
      },
    };
  },
  created() {
    this.getList();
  },
  methods: {
    getList() {
      this.tableData = [];
      var query = JSON.stringify(this.listQuery);
      fetchList(query).then((res) => {
        Object.keys(res.data).forEach((v) => {
          let o = {};
          o.notice_id = res.data[v].notice_id;
          o.notice_title = res.data[v].notice_title;
          o.admin_id = res.data[v].admin_id;
          o.operate_time = res.data[v].operate_time;
          if (res.data[v].operate_type == 1) {
            this.tableData.push(o);
          }
        });
      });
    },
    handleFilter() {
      this.getList();
    },
    goback() {
      this.$router.push({ path: "/users/UserPage" });
    },
    goto_user_ShowNotice(index) {
      var notice_id = this.tableData[index].notice_id;
      this.$router.push({
        path: "/users/User_ShowNotice",
        query: { n_id: notice_id },
      });
    },
  },
};
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

#breadcrumb {
  height: 30px;
  margin-bottom: 10px;
}
</style>