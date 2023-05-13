<template>
  <div class="detaildiv">
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPage' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>项目管理</el-breadcrumb-item>
          <el-breadcrumb-item>我的项目</el-breadcrumb-item>
          <el-breadcrumb-item>管理团队</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <p class="title" align="center" label="项目名称">{{  project_name  }}</p>
      <el-tabs v-model="activeName">
        <el-tab-pane label="组员列表" name="组员列表" @tab-click="handleClick">
          <div>
          <el-table :data="tableData0" :header-cell-style="{ textAlign: 'center' }"
          :cell-style="{ 'text-align': 'center' }" default-sort="{ prop: 'date', order: 'descending' }">
          <el-table-column prop="teamember_id" label="组员id" width="200">
          </el-table-column>
          <el-table-column label="操作">
            <template slot-scope="scope">
              <el-button size="mini" @click="handlecheck(scope.$index, scope.row)">查看申请人主页</el-button>
              <el-button size="mini" type="danger" @click="handlerun(scope.$index, scope.row)">踢出团队</el-button>
            </template>
          </el-table-column>
        </el-table>
          </div>
        </el-tab-pane>
        <el-tab-pane label="申请人列表" name="申请人列表">
          <div>
          <el-table :data="tableData" :header-cell-style="{ textAlign: 'center' }"
          :cell-style="{ 'text-align': 'center' }" default-sort="{ prop: 'date', order: 'descending' }">
          <el-table-column prop="applicant_id" label="申请人id" width="200">
          </el-table-column>
          <el-table-column label="操作">
            <template slot-scope="scope">
              <el-button size="mini" @click="handlecheck(scope.$index, scope.row)">查看申请人主页</el-button>
              <el-button size="mini" type="success" @click="agree(scope.$index, scope.row)">同意</el-button>
              <el-button size="mini" type="danger" @click="refuse(scope.$index, scope.row)">拒绝</el-button>
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
import global_msg from '../../utils/global.js'
import { get_member} from '@/api/ProjectDetail.js'
import { Audit} from '@/api/Myprojectlist.js'
export default {
  name: 'MyProject',
  data() {
    return {
      username: global_msg.nowuserid,
      tableData: [],
      tableData0: [],
      activeName: "组员列表",
    };
  },
  mounted() {
    this.getlist() //页面一进入就加载表格
  },
  methods: {
    goback() {
      this.$router.push({ path: "/users/InforList" }); //返回
    },
    getlist() {
      var vm = this;//全局变量
      let para0 = {
            pid: vm.$route.query.p_id,
            duty: "组员"
      };
      let para = {
            pid: vm.$route.query.p_id,
            duty: "申请者"
      };
      get_member(para0).then(function (res) {
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            teamember_id: '',
          }
          //发布表
          form.teamember_id = item.user_id;
          vm.tableData0.push(form);
        }
      })
      get_member(para).then(function (res) {
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            applicant_id: '',
          }
          //发布表
          form.applicant_id = item.user_id;
          vm.tableData.push(form);
        }
      })
    },
    agree(index, row) { //同意操作
     var vm=this;
      let para = {
            pid: vm.$route.query.p_id,
            uid: vm.tableData[index].applicant_id,
            dy:"组员"
      }
      Audit(para).then(function (res) {
        if (res.data === false) {
              vm.$message.error("操作失败");
            }
            else {
              vm.$message.success("操作成功");
            }
      })
      console.log(index, row);
    },
    refuse(index, row) { //同意操作
      var vm=this;
      let para = {
            pid: vm.$route.query.p_id,
            uid: vm.tableData[index].applicant_id,
            dy:"已拒绝"
      }
      Audit(para).then(function (res) {
        if (res.data === false) {
              vm.$message.error("操作失败");
            }
            else {
              vm.$message.success("操作成功");
            }
      })
      console.log(index, row);
    },
  }
}
</script>

<style>
#breadcrumb {
  margin-bottom: 10px;
}

.myproject-container {
  position: absolute;
  margin-left: 0px;
  height: 100%;
  bottom: 0%;
  right: 0px;
  left: 0px;
}

.el-header {
  background-color: #B3C0D1;
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
