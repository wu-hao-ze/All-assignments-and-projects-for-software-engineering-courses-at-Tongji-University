<template>
  <div id="NoticeList">
    <div id="body">
      <el-main>
        <div id="breadcrumb">
          <el-breadcrumb separator-class="el-icon-arrow-right">
            <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item>通知管理</el-breadcrumb-item>
            <el-breadcrumb-item>通知总览</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <el-table :data="tableData" border @row-click="goto_ShowNotice" v-if="isAlive">
          <el-table-column v-if="false" prop="notice_id" label="公告id">
          </el-table-column>
          <el-table-column prop="title" label="公告标题" align="left" style="margin: 50px;">
          </el-table-column>
          <el-table-column prop="admin_id" label="管理员" align="center">
          </el-table-column>
          <el-table-column prop="status" label="状态" align="center">
          </el-table-column>
          <el-table-column prop="operate_time" label="发布时间" align="center">
          </el-table-column>
          <el-table-column label="操作" align="center">
            <template slot-scope="scope">
              <el-button-group>
                <el-button plain icon="el-icon-edit" @click="goto_edit(scope.row)"></el-button>
                <el-popconfirm @confirm=delete_not(scope.row) title="确定删除吗？">
                  <el-button plain slot="reference" @click.native.stop icon="el-icon-delete"></el-button>
                </el-popconfirm>
              </el-button-group>
            </template>
          </el-table-column>
        </el-table>
      </el-main>
    </div>
  </div>
</template>
 
<script>
import { get_all } from '@/api/notice.js'
import { delete_notice } from '@/api/notice.js'
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
        if (res.data[v].operate_type == 0) {
          o.status = "暂存";
        }
        else {
          o.status = "发布";
        }
        console.log(o);
        this.tableData.push(o);
      });
    });
    return {
      isAlive: true
    };
  },
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/AdminPage" });
    },
    goto_ShowNotice(row) {
      this.$router.push({ path: "/Admin/ShowNotice", query: { n_id: row.notice_id } });
    },
    goto_edit(row) {
      this.$router.push({ path: "/Admin/CreateNotice", query: { n_id: row.notice_id } });
    },
    delete_not(row) {
      let param = {
        id: row.notice_id
      };
      delete_notice(param).then(function (res) {
        if (res.data) {
          alert("公告删除成功");
          location.reload();
        }
        else {
          alert("公告删除失败");
        }
      })
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