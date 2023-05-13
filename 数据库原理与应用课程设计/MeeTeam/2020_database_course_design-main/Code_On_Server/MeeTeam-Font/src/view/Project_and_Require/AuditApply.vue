<template>
  <div class="detaildiv">
    <el-dialog :visible.sync="dialogFormVisible">
      <div>
        <el-avatar :size="90" :fit="fit" :src="base64"></el-avatar>
      </div>
      <div>
        <el-button type="primary" v-if='relation == "无"' @click="handlefollow">关注</el-button>
        <el-button type="info" v-if='relation == "关注"' @click="handlefollow">取消关注</el-button>
      </div>
      <el-divider content-position="left">
        <h4>用户信息</h4>
      </el-divider>
      <el-descriptions column="2">
        <el-descriptions-item label="用户id">
          <span v-text="this.his.user_id"></span>
        </el-descriptions-item>
        <el-descriptions-item label="性别">
          <span v-text="this.his.gender"></span>
        </el-descriptions-item>
        <el-descriptions-item label="联系方式">
          <span v-text="this.his.contact_info"></span>
        </el-descriptions-item>
        <el-descriptions-item label="就读院校">
          <span v-text="this.his.institution"></span>
        </el-descriptions-item>
        <el-descriptions-item label="专业">
          <span v-text="this.his.major"></span>
        </el-descriptions-item>
        <el-descriptions-item label="年级">
          <span v-text="this.his.grade"></span>
        </el-descriptions-item>
        <el-descriptions-item label="个人简介" span="2">
          <span v-text="this.his.introduction"></span>
        </el-descriptions-item>
        <el-descriptions-item label="积分" span="2">
          <span v-text="this.his.point"></span>
        </el-descriptions-item>
      </el-descriptions>
      <el-divider content-position="left">
        <h4>项目经历</h4>
      </el-divider>
      <el-table :data="project_experience" :header-cell-style="{ textAlign: 'center' }"
        :cell-style="{ 'text-align': 'center' }" height="200">
        <el-table-column prop="project_name" label="项目名称"></el-table-column>
        <el-table-column prop="create_time" label="发布时间" sortable></el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button size="mini" @click="project_detail(scope.row)">查看详情</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-divider content-position="left">
        <h4>收藏夹</h4>
      </el-divider>
      <el-table :data="facorite" :header-cell-style="{ textAlign: 'center' }" :cell-style="{ 'text-align': 'center' }"
        height="200">
        <el-table-column prop="project_name" label="项目名称" width="160"></el-table-column>
        <el-table-column prop="create_time" label="发布时间" sortable width="160"></el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button size="mini" @click="project_detail(scope.row)">查看详情</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="dialogFormVisible = false">关闭</el-button>
      </div>
    </el-dialog>
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPage' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>项目管理</el-breadcrumb-item>
          <el-breadcrumb-item>我的项目</el-breadcrumb-item>
          <el-breadcrumb-item>管理团队</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <p class="title" align="center" label="项目名称">{{ project_name }}</p>
      <el-tabs v-model="activeName">
        <el-tab-pane label="组员列表" name="组员列表" @tab-click="handleClick">
          <div>
            <el-table :data="tableData0" :header-cell-style="{ textAlign: 'center' }"
              :cell-style="{ 'text-align': 'center' }" default-sort="{ prop: 'date', order: 'descending' }">
              <el-table-column prop="teamember_id" label="组员id" width="200"></el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                  <el-button size="mini" @click="handlecheck1(scope.row)">查看申请人主页</el-button>
                  <el-button size="mini" type="danger" @click="handleout(scope.row)">踢出团队</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-tab-pane>
        <el-tab-pane label="申请人列表" name="申请人列表">
          <div>
            <el-table :data="tableData" :header-cell-style="{ textAlign: 'center' }"
              :cell-style="{ 'text-align': 'center' }" default-sort="{ prop: 'date', order: 'descending' }">
              <el-table-column prop="applicant_id" label="申请人id" width="200"></el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                  <el-button size="mini" @click="handlecheck2(scope.row)">查看申请人主页</el-button>
                  <el-button size="mini" type="success" @click="agree(scope.row)">同意</el-button>
                  <el-button size="mini" type="danger" @click="refuse(scope.row)">拒绝</el-button>
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
import { get_member } from '@/api/ProjectDetail.js'
import { Audit } from '@/api/Myprojectlist.js'
import { GetUserInfor } from '@/api/MyInfor.js'
import { my_project } from '@/api/MyInfor.js'
import { GetMyCollection } from '@/api/MyInfor.js'
import { getproject } from '@/api/MyInfor.js'
import { getRe } from "@/api/followother.js"
import { pullRe } from "@/api/followother.js"
import { openfile } from "@/api/file_load.js"
export default {
  name: 'MyProject',
  data() {
    return {
      username: this.$route.query.id,
      tableData: [],
      tableData0: [],
      activeName: "组员列表",
      dialogFormVisible: false,
      base64: "",
      his: {
        user_id: "",
        gender: "",
        contact_info: "",
        institution: "",
        grade: "",
        introduction: "",
        point: ""
      },
      formLabelWidth: '120px',
      project_experience: [],
      facorite: [],
      relation: "无",
      hisid: ""
    };
  },
  mounted() {
    this.getlist()//页面一进入就加载表格
  },
  methods: {
    goback() {
      this.$router.push({ path: "/users/InforList", query: { id: this.$route.query.id } }); //返回
    },
    get_hisinfo(his_id) {
      let params = {
        user_id: his_id
      };
      let params1 = {
        target: "user",
        id: his_id,
      };
      let params2 = {
        MyID: this.$route.query.id,
        ID: his_id
      };
      getRe(params2).then((res) => {
        if (res.data == "关注")
          this.relation = "关注";
        else
          this.relation = "无";
      })
      openfile(params1).then((res) => {
        this.base64 = "data:;base64," + res.data;
      });
      GetUserInfor(params).then((res) => {
        let item = res.data[0];
        this.his.user_id = item.user_id;
        if (item.gender === "0")
          this.his.gender = "女"
        else
          this.his.gender = "男"
        // this.his.gender = item.gender;
        this.his.contact_info = item.contact_info;
        this.his.institution = item.institution;
        this.his.major = item.major;
        this.his.grade = item.grade;
        this.his.point = item.point;
        this.his.introduction = item.introduction;
        //console.log(res.data);
      })
      my_project(params).then((res1) => {
        this.project_experience = res1.data;
        //console.log(res1.data);
      })
      GetMyCollection(params).then((res2) => {
        Object.keys(res2.data).forEach((v) => {
          let params1 = {
            project_id: res2.data[v].project_id
          }
          getproject(params1).then((res4) => {
            this.facorite = res4.data;
            //console.log(res4.data);
          })
        })
      })
    },
    project_detail(row) {
      //进入项目详情页面
      var project_id = row.project_id;
      this.$router.push({
        path: "/users/ProjectDetail",
        query: { p_id: project_id, id: this.$route.query.id },
      });
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
      get_member(para0).then((res) => {
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            teamember_id: '',
          }
          //组员表
          form.teamember_id = item.user_id;
          vm.tableData0.push(form);
        }
      })
      get_member(para).then((res) => {
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            applicant_id: '',
          }
          //申请表
          form.applicant_id = item.user_id;
          vm.tableData.push(form);
        }
      })
    },
    agree(row) { //同意操作
      var vm = this;
      let para = {
        pid: vm.$route.query.p_id,
        uid: row.applicant_id,
        dy: "组员"
      }
      Audit(para).then((res) => {
        if (res.data === false) {
          vm.$message.error("操作失败");
        }
        else {
          vm.$message.success("操作成功");
        }
      })
      location.reload();
    },
    refuse(row) { //同意操作
      var vm = this;
      let para = {
        pid: vm.$route.query.p_id,
        uid: row.applicant_id,
        dy: "已拒绝"
      }
      Audit(para).then((res) => {
        if (res.data === false) {
          vm.$message.error("操作失败");
        }
        else {
          vm.$message.success("操作成功");
        }
      })
      location.reload();
    },
    handleout(row) { //踢出操作
      var vm = this;
      let para = {
        pid: vm.$route.query.p_id,
        uid: row.teamember_id,
        dy: "已踢出"
      }
      Audit(para).then((res) => {
        if (res.data === false) {
          vm.$message.error("操作失败");
        }
        else {
          vm.$message.success("操作成功");
        }
      })
      location.reload();
    },
    handlecheck1(row) {
      console.log(this);
      this.dialogFormVisible = true;
      this.get_hisinfo(row.teamember_id);
      this.hisid = row.teamember_id;
    },
    handlecheck2(row) {
      //console.log(this);
      this.dialogFormVisible = true;
      this.get_hisinfo(row.applicant_id);
      this.hisid = row.applicant_id;
    },
    handlefollow() {
      if (this.relation == "关注") {
        this.relation = "无";
        let params = {
          MyID: this.$route.query.id,
          ID: this.hisid,
          relationship: "无"
        }
        pullRe(params).then((res) => {
          if (res == 1)
            console.log("关注成功");
          else
            console.log("失败！");
        })
      }
      else {
        this.relation = "关注";
        let params = {
          MyID: this.$route.query.id,
          ID: this.hisid,
          relationship: "关注"
        }
        pullRe(params);
      }
    }
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
