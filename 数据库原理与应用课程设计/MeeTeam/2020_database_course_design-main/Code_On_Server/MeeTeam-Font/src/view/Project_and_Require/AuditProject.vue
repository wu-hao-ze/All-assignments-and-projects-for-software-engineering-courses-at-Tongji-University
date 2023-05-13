<template>
  <div id="AuditProject">
    <div id="body">
      <el-main>
        <div id="breadcrumb">
          <el-breadcrumb separator-class="el-icon-arrow-right">
            <el-breadcrumb-item :to="{ path: '/Admin/AdminPageContent' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item>审核管理</el-breadcrumb-item>
            <el-breadcrumb-item>项目审核</el-breadcrumb-item>
          </el-breadcrumb>
        </div>

        <el-card>
          <div class="filter-container">
            <el-input v-model="listQuery.project_name" placeholder="项目名称" style="width: 180px" class="filter-item"
              @keyup.enter.native="handleFilter"></el-input>
            &nbsp;
            <el-input v-model="listQuery.publisher" placeholder="项目发布者" style="width: 180px" class="filter-item"
              @keyup.enter.native="handleFilter"></el-input>
            &nbsp;
            <el-select v-model="listQuery.hav_require" placeholder="是否有组队需求" clearable style="width: 180px"
              class="filter-item">
              <el-option v-for="item in havreqOptions" :key="item.key" :label="item.display_name" :value="item.key" />
            </el-select>
            &nbsp;
            <el-select v-model="listQuery.audit_status" placeholder="审核状态" clearable style="width: 180px"
              class="filter-item">
              <el-option v-for="item in statusOptions" :key="item.key" :label="item.display_name" :value="item.key" />
            </el-select>
            &nbsp;
            <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
              搜索
            </el-button>
          </div>
        </el-card>
        <br />

        <el-card>
          <el-table :data="tableData" :header-cell-style="{ textAlign: 'center' }"
            :cell-style="{ 'text-align': 'center' }">
            <el-table-column prop="project_name" label="项目名称" width="160">
            </el-table-column>
            <el-table-column prop="project_introduction" label="项目简介" width="200">
            </el-table-column>
            <el-table-column prop="User_Projectuser_id" label="发布者" width="150">
            </el-table-column>
            <el-table-column prop="create_time" label="发布时间" sortable width="160">
            </el-table-column>
            <el-table-column prop="project_status" label="是否有组队需求" width="160">
            </el-table-column>
            <el-table-column prop="audit_status" label="审核状态" width="160">
              <template slot-scope="scope">
                <span v-if="scope.row.audit_status == 0">未审核</span>
                <span v-else>已审核</span>
              </template>
            </el-table-column>
            <el-table-column label="操作">
              <template slot-scope="scope">
                <el-button size="mini" @click="handleCheck(scope.row)">查看详情</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-main>
    </div>
  </div>
</template>

<script>
import { getlistInfor } from "@/api/Inforlist.js";
import { getrequireInfor } from "@/api/Inforlist.js";
import { get_username } from "@/api/ProjectDetail.js";
import { fetchList } from "@/api/Querylist.js";

const havreqOptions = [
  { key: "1", display_name: "是" },
  { key: "0", display_name: "否" },
];

// arr to obj
const havreqKeyValue = havreqOptions.reduce((acc, cur) => {
  acc[cur.key] = cur.display_name;
  return acc;
}, {});

const statusOptions = [
  { key: "1", display_name: "已审核" },
  { key: "0", display_name: "未审核" },
];

// arr to obj
const statusKeyValue = statusOptions.reduce((acc, cur) => {
  acc[cur.key] = cur.display_name;
  return acc;
}, {});

export default {
  name: "AuditProject",
  //components: { Pagination },
  filters: {
    havreqFilter(havreq) {
      return havreqKeyValue[havreq];
    },
    statusFilter(status) {
      return statusKeyValue[status];
    },
  },

  data() {
    return {
      activeName: "项目列表",
      adminname: this.$route.query.id,
      tableData: [],
      total: 0,
      listQuery: {
        page: 1,
        //limit: 20,
        project_name: "",
        publisher: "",
        hav_require: "",
        audit_status: "",
      },
      havreqOptions,
      statusOptions,
    };
  },

  created() {
    this.getList(); //页面一进入就加载表格
  },
  
  methods: {
    getList() {
      var query = JSON.stringify(this.listQuery);
      fetchList(query).then((response) => {
        //console.log(response);
        this.tableData = response.data;
        console.log(this.tableData);
      });
    },
    handleFilter() {
      this.listQuery.page = 1;
      this.getList();
    },
    handleCheck(row) {
      //详情审核操作
      var project_id = row.project_id;
      this.$router.push({
        path: "/Admin/DetailAudit",
        query: { p_id: project_id, id: this.$route.query.id },
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

.el-card {
  box-shadow: 0 5px 5px rgba(0, 0, 0, 0, 15);
}

.filter-container {
  padding-bottom: 10px;
}
</style>
