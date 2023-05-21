<template>
  <div id="NoticeList">
    <div id="body">
      <el-main>
        <div id="breadcrumb">
          <el-breadcrumb separator-class="el-icon-arrow-right">
            <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }"
              >首页</el-breadcrumb-item
            >
            <el-breadcrumb-item>通知管理</el-breadcrumb-item>
            <el-breadcrumb-item>通知总览</el-breadcrumb-item>
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
              placeholder="管理员"
              style="width: 180px"
              class="filter-item"
              v-model="listQuery.admin_id"
              @keyup.enter.native="handleFilter"
            ></el-input>
            &nbsp;
            <el-select
              placeholder="发布状态"
              clearable
              style="width: 180px"
              class="filter-item"
              v-model="listQuery.operate"
            >
              <el-option
                v-for="item in operateOptions"
                :key="item.key"
                :label="item.display_name"
                :value="item.key"
              />
            </el-select>
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
            @row-click="goto_ShowNotice"
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
            <el-table-column prop="admin_id" label="管理员" align="center">
            </el-table-column>
            <el-table-column prop="operate_type" label="状态" align="center">
            </el-table-column>
            <el-table-column
              prop="operate_time"
              label="发布时间"
              align="center"
              sortable
            >
            </el-table-column>
            <el-table-column label="操作" align="center">
              <template slot-scope="scope">
                <el-button-group>
                  <el-button
                    plain
                    icon="el-icon-edit"
                    @click="goto_edit(scope.row)"
                  ></el-button>
                  <el-popconfirm
                    @confirm="delete_not(scope.row)"
                    title="确定删除吗？"
                  >
                    <el-button
                      plain
                      slot="reference"
                      @click.native.stop
                      icon="el-icon-delete"
                    ></el-button>
                  </el-popconfirm>
                </el-button-group>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-main>
    </div>
  </div>
</template>
 
<script scoped>
import { get_all } from "@/api/notice.js";
import { delete_notice } from "@/api/notice.js";
import { fetchList } from "@/api/notice.js";

const operateOptions = [
  { key: "0", display_name: "暂存" },
  { key: "1", display_name: "发布" },
];

const operateKeyValue = operateOptions.reduce((acc, cur) => {
  acc[cur.key] = cur.display_name;
  return acc;
}, {});

export default {
  filters: {
    operateFilter(operate) {
      return operateKeyValue[operate];
    },
  },
  data() {
    return {
      tableData: [],
      isAlive: true,
      listQuery: {
        notice_title: "",
        admin_id: "",
        operate: "",
      },
      operateOptions,
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
          if (res.data[v].operate_type == 0) {
            o.operate_type = "暂存";
          } else {
            o.operate_type = "发布";
          }
          console.log(o);
          this.tableData.push(o);
        });
      });
    },
    handleFilter() {
      this.getList();
    },
    goback() {
      this.$router.push({ path: "/Admin/AdminPage" });
    },
    goto_ShowNotice(row) {
      this.$router.push({
        path: "/Admin/ShowNotice",
        query: { n_id: row.notice_id },
      });
    },
    goto_edit(row) {
      this.$router.push({
        path: "/Admin/CreateNotice",
        query: { n_id: row.notice_id },
      });
    },
    delete_not(row) {
      let param = {
        id: row.notice_id,
      };
      delete_notice(param).then(function (res) {
        if (res.data) {
          alert("公告删除成功");
          location.reload();
        } else {
          alert("公告删除失败");
        }
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