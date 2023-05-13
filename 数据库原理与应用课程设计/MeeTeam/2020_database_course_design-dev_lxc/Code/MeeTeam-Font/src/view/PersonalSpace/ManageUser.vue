<template>
    <div id="NoticeList">
      <div id="body">
        <el-main>
          <div id="breadcrumb">
            <el-breadcrumb separator-class="el-icon-arrow-right">
              <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }"
                >首页</el-breadcrumb-item
              >
              <el-breadcrumb-item>用户管理</el-breadcrumb-item>
            </el-breadcrumb>
          </div>
          <el-card>
            <div class="filter-container">
              <el-input
                placeholder="用户名"
                style="width: 180px"
                class="filter-item"
                v-model="listQuery.notice_title"
                @keyup.enter.native="handleFilter"
              ></el-input>
              &nbsp;
                <el-option
                  v-for="item in operateOptions"
                  :key="item.key"
                  :label="item.display_name"
                  :value="item.key"
                />
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
              <el-table-column v-if="false" prop="user_id" label="用户id">
              </el-table-column>
              <el-table-column
                prop="notice_title"
                label="用户名"
                align="left"
                style="margin: 50px"
              >
              </el-table-column>
              <el-table-column prop="operate_type" label="状态" align="center">
              </el-table-column>
              <el-table-column label="操作" align="center">
                <template slot-scope="scope">
                  <el-button-group>
                    <el-button
                      plain
                      icon="el-icon-edit"
                      @click="block_user(scope.row)"
                    ></el-button>
                      <el-button
                        plain
                        slot="reference"
                        @click.native.stop
                        icon="el-icon-delete"
                      ></el-button>
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
import { BlockUser, UnblockUser } from '@/api/MyInfor';
  
  const operateOptions = [
    { key: "blocked", display_name: "封禁" },
    { key: "unblocked", display_name: "未封禁" },
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
          user_name: "",
          user_id: "",
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
            o.aaccount_status = res.data[v].account_status;
            o.user_name = res.data[v].user_name;
            o.user_id = res.data[v].user_id;
            if (res.data[v].account_status == "blocked") {
              o.operate_type = "封禁";
            } else {
              o.operate_type = "未封禁";
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
      block_user(row) {
        let param = {
          id: row.user_id,
        };
        BlockUser(param).then(function (res) {
          if (res.data) {
            alert("用户封禁成功");
            location.reload();
          } else {
            alert("用户封禁失败");
          }
        });
      },
      unblock_user(row) {
        let param = {
          id: row.user_id,
        };
        UnblockUser(param).then(function (res) {
          if (res.data) {
            alert("用户解禁成功");
            location.reload();
          } else {
            alert("用户解禁失败");
          }
        });
      }
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