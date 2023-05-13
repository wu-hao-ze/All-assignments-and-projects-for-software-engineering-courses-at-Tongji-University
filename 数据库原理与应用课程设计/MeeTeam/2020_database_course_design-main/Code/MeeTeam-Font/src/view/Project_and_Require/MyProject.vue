<template>
  <div id="myproject-container">
    <!-- <el-header style="text-align: right; font-size: 15px; top:50%;">
        <el-badge :value="0" class="item">
          <el-button>聊天</el-button>
        </el-badge>
        <el-badge :value="3" class="item">
          <el-button>评论</el-button>
        </el-badge>
        <el-badge :value="1" class="item" type="primary">
          <el-button>申请</el-button>
        </el-badge>
        <span>{{ username }}</span>
      </el-header> -->
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPage' }"
            >首页</el-breadcrumb-item
          >
          <el-breadcrumb-item>项目管理</el-breadcrumb-item>
          <el-breadcrumb-item>我的项目</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <el-tabs v-model="activeName">
        <el-tab-pane label="我发布的" name="我发布的" @tab-click="handleClick">
          <div>
            <el-table
              :data="tableData"
              :header-cell-style="{ textAlign: 'left' }"
              :cell-style="{ 'text-align': 'left' }"
              default-sort="{ prop: 'date', order: 'descending' }"
            >
              <el-table-column prop="project_name" label="项目名称" width="200">
              </el-table-column>
              <el-table-column
                prop="project_progress"
                label="项目当前进度"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="create_time"
                label="发布时间"
                sortable
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="project_status"
                label="是否有组队需求"
                width="200"
              >
              </el-table-column>
              <el-table-column label="操作" >
                <template slot-scope="scope">
                  <el-button
                    size="mini"
                    @click="handleDetail(scope.$index, scope.row),addhistory(username,scope.row.project_id)"
                    >查看详情</el-button
                  >
                  <el-button
                    size="mini"
                    type="primary"
                    @click="handleEditwithRequire(scope.$index, scope.row)"
                    v-if="scope.row.project_status === '是'"
                    >编辑</el-button
                  >
                  <el-button
                    size="mini"
                    type="primary"
                    @click="handleEditwithoutRequire(scope.$index, scope.row)"
                    v-if="scope.row.project_status === '否'"
                    >编辑</el-button
                  >
                  <br /><br />
                  <el-button
                    size="mini"
                    type="danger"
                    @click="handleDelete(scope.$index, scope.row)"
                    >删除</el-button
                  >
                  <el-button
                    size="mini"
                    type="warning"
                    @click="handleEndProgress(scope.$index, scope.row)"
                    v-if="scope.row.project_progress != '已完结'"
                    >完结项目</el-button
                  >
                  <el-button
                    size="mini"
                    type="success"
                    @click="handleApply(scope.$index, scope.row)"
                    v-if="scope.row.project_progress === '招募中'"
                    >管理团队</el-button
                  >
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-tab-pane>
        <el-tab-pane label="我申请的" name="我申请的">
          <div>
            <el-table
              :data="tableData0"
              :header-cell-style="{ textAlign: 'center' }"
              :cell-style="{ 'text-align': 'center' }"
              default-sort="{ prop: 'date', order: 'descending' }"
            >
              <el-table-column
                prop="project0_name"
                label="项目名称"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="project0_progress"
                label="项目当前进度"
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="create0_time"
                label="发布时间"
                sortable
                width="200"
              >
              </el-table-column>
              <el-table-column
                prop="project0_duty"
                label="当前状态"
                width="200"
              >
              </el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                  <el-button
                    size="mini"
                    @click="handleDetail0(scope.$index, scope.row),addhistory(username,scope.row.project0_id)"
                    >查看详情</el-button
                  >
                  <el-button
                    size="mini"
                    type="danger"
                    @click="handleDeleteApply(scope.$index, scope.row)"
                    v-if="scope.row.project0_duty === '申请者'"
                    >取消申请</el-button
                  >
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-tab-pane>
      </el-tabs>
    </el-main>
  </div>
</template>

<script>
import global_msg from "../../utils/global.js";
import { getlistInfor } from "@/api/Inforlist.js";
import { deleteproject } from "@/api/Myprojectlist.js";
import { deleteapply } from "@/api/Myprojectlist.js";
import { UpdateProgress } from "@/api/Myprojectlist.js";
import { get_username } from "@/api/ProjectDetail.js";
import { addviewhistory } from "@/api/Addhistory";
import { get_member } from '@/api/ProjectDetail.js'
import { UpdatePoint } from '@/api/getinfo.js'

export default {
  name: "MyProject",
  data() {
    return {
      username: global_msg.nowuserid,
      activeName: "我发布的",
      tableData: [],
      tableData0: [],
    };
  },
  mounted() {
    this.getlist(); //页面一进入就加载表格
  },
  methods: {
    goback() {
      this.$router.push({ path: "/users/InforList" }); //返回
    },
    getlist() {
      var vm = this; //全局变量
      getlistInfor().then(function (res) {
        for (let item of res.data) {
          let form = {
            //设置添加数据的格式
            //发布表
            project_name: "",
            project_progress: "",
            create_time: "",
            project_status: "",
            project_id: "",
            //申请表
            project0_name: "",
            project0_progress: "",
            create0_time: "",
            project0_status: "",
            project0_id: "",
            operation: "",
          };
          //发布表
          form.project_name = item.project_name;
          form.project_introduction = item.project_introduction;
          form.project_progress = item.project_progress;
          form.create_time = item.create_time;
          form.create_time = form.create_time.replace('"', "").replace('"', ""); //去掉时间格式的引号
          form.project_status = item.project_status;
          form.project_id = item.project_id;
          //申请表
          form.project0_name = item.project_name;
          form.project0_introduction = item.project_introduction;
          form.project0_progress = item.project_progress;
          form.create0_time = item.create_time;
          form.create0_time = form.create0_time
            .replace('"', "")
            .replace('"', ""); //去掉时间格式的引号
          form.project0_id = item.project_id;
          let para = {
            pid: form.project_id,
            uid: global_msg.nowuserid,
          };
          get_username(para).then((res) => {
            form.user_id = res.data.user_id;
            if (res.data.duty == "发布者") vm.tableData.push(form);
            if (res.data.duty == "申请者") {
              vm.tableData0.push(form);
              form.project0_duty = "申请者";
            }
            if (res.data.duty == "组员") {
              vm.tableData0.push(form);
              form.project0_duty = "组员";
            }
            if (res.data.duty == "已拒绝") {
              vm.tableData0.push(form);
              form.project0_duty = "已拒绝";
            }
            if (res.data.duty == "已踢出") {
              vm.tableData0.push(form);
              form.project0_duty = "已踢出";
            }
          });
        }
      });
    },
    handleDetail(index, row) {
      //查看详情操作(发布表)
      var project_id = this.tableData[index].project_id;
      console.log(index, row);
      this.$router.push({
        path: "/users/ProjectDetail",
        query: { p_id: project_id },
      });
    },
    handleDetail0(index, row) {
      //查看详情操作(申请表)
      var project_id = this.tableData0[index].project0_id;
      console.log(index, row);
      this.$router.push({
        path: "/users/ProjectDetail",
        query: { p_id: project_id },
      });
    },
    handleApply(index, row) {
      //审核申请操作
      var project_id = this.tableData[index].project_id;
      console.log(index, row);
      this.$router.push({
        path: "/users/AuditApply",
        query: { p_id: project_id },
      });
    },
    handleEditwithRequire(index, row) {
      //编辑操作
      var project_id = this.tableData[index].project_id;
      console.log(index, row);
      this.$router.push({
        path: "/users/ChangeListwithRequire",
        query: { p_id: project_id },
      });
    },
    handleEditwithoutRequire(index, row) {
      //编辑操作
      var project_id = this.tableData[index].project_id;
      console.log(index, row);
      this.$router.push({
        path: "/users/ChangeListwithoutRequire",
        query: { p_id: project_id },
      });
    },
    handleEndProgress(index, row){
      //完结项目操作
      var project_id = this.tableData[index].project_id;
      let para={
        project_id:this.tableData[index].project_id,
        project_progress:"已完结"
      }
      UpdateProgress(para);
      console.log(index, row);
      location.reload();
      let para0 = {
        pid: this.tableData[index].project_id,
        duty: "组员"
      };
      get_member(para0).then(function (res) {
        for (let item of res.data) {
          let pa={
            user_id:item.user_id
          }
          UpdatePoint(pa);
        }
      })
      let pa0={
          user_id:global_msg.nowuserid,
      }
      UpdatePoint(pa0);
      this.$message({
            type: "success",
            message: "项目已完结!",
          });
    },
    handleDelete(index, row) {
      //删除操作
      this.$confirm("此操作将永久删除该数据, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          //有个bug
          var id = this.tableData[index].project_id;
          //再在数据库中删除
          let param = {
            project_id: id,
          };
          deleteproject(param);
          this.tableData.splice(index, 1); //删除下标为index的内容
          this.$message({
            type: "success",
            message: "删除成功!",
          });
          console.log(index, row);
        })
        .catch((err) => {
          this.$message({
            type: "error",
            message: err,
          });
        });
    },
    handleDeleteApply(index, row) {
      //删除操作
      this.$confirm("此操作将永久删除该数据, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          var id = this.tableData0[index].project0_id;
          //再在数据库中删除
          let param = {
            pid: id,
            uid: global_msg.nowuserid,
          };
          deleteapply(param);
          this.tableData.splice(index, 1); //删除下标为index的内容
          this.$message({
            type: "success",
            message: "删除成功!",
          });
          console.log(index, row);
        })
        .catch((err) => {
          this.$message({
            type: "error",
            message: err,
          });
        });
      console.log(index, row);
    },
    handleClick(tab, event) {
      console.log(tab, event);
    },

     /*触发添加浏览历史*/
     addhistory(user_id,proj_id){
      let params={
        user_id:user_id,
        project_id:proj_id
      };
      console.log(params);
      addviewhistory(params);
    }
  },
};
</script>

<style>
#breadcrumb {
  margin-bottom: 10px;
}

#myproject-container {
  position: relative;
  min-height: 100vh;
  height: 100%;
}

.el-header {
  background-color: #b3c0d1;
  color: #333;
  line-height: 55px;
  font-weight: 900;
  font-style: normal;
}

.item {
  margin-top: 0px;
  margin-right: 10px;
}

.container {
  display: inline;
  float: left;
  display: inline-block;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 48px;
}
</style>
