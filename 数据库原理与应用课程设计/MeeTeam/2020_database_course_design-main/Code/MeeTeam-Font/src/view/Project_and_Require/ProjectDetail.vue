·<template>
  <div class="detaildiv">
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPageContent' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>项目管理</el-breadcrumb-item>
          <el-breadcrumb-item :to="{ path: '/users/InforList' }">平台项目列表</el-breadcrumb-item>
          <el-breadcrumb-item>项目详情</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <p class="title" align="center" label="项目名称">{{ project_name }}</p>
      <el-descriptions title="项目详情" :column="2" border style="margin-top: 20px;">
        <el-descriptions-item label="项目发布者">{{ user_id }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ create_time }}</el-descriptions-item>
        <el-descriptions-item label="开始时间">{{ start_time }}</el-descriptions-item>
        <el-descriptions-item label="结束时间">{{ end_time }}</el-descriptions-item>
        <el-descriptions-item label="项目当前进展">{{ project_progress }}</el-descriptions-item>
        <el-descriptions-item label="是否有组队需求">{{ project_status }}</el-descriptions-item>
      </el-descriptions>
      <el-descriptions title :column="1" border>
        <el-descriptions-item label="项目背景">{{ project_background }}</el-descriptions-item>
        <el-descriptions-item label="项目简介">{{ project_introduction }}</el-descriptions-item>
        <el-descriptions-item label="项目内容">{{ project_content }}</el-descriptions-item>
      </el-descriptions>
      <br />
      <el-descriptions title="需求详情" :column="2" border>
        <el-descriptions-item label="组队类型">{{ team_type }}</el-descriptions-item>
        <el-descriptions-item label="类型细则">{{ team_type_details }}</el-descriptions-item>
        <el-descriptions-item label="组队人数">{{ team_limit }}</el-descriptions-item>
        <el-descriptions-item label="当前状态">{{ require_status }}</el-descriptions-item>
      </el-descriptions>
      <el-descriptions title :column="1" border>
        <el-descriptions-item label="所在校区">{{ region }}</el-descriptions-item>
        <el-descriptions-item label="需求目的">{{ purpose }}</el-descriptions-item>
        <el-descriptions-item label="组队细则">{{ details }}</el-descriptions-item>
      </el-descriptions>
      <el-steps
        :space="500"
        :active="active"
        finish-status="success"
        align-center
        style="margin-top: 100px;margin-bottom: 100px"
      >
        <el-step title="发布项目"></el-step>
        <el-step title="招募队友"></el-step>
        <el-step title="项目完结"></el-step>
      </el-steps>
      <el-button
        type="success"
        plain
        style="float: right;margin-right: 80px"
        @click="handleApply()"
        v-if="this.project_progress === '招募中'"
      >申请加入</el-button>
      <el-button
        type="submit"
        plain
        style="float: left;margin-left: 20px"
        @click="handlefavorite()"
      >
        收藏{{ favorite_num
        }}
      </el-button>
      <br><br>
      <br><br>
      <p align="left" class="eva"><strong>评论</strong>{{eva_num}}</p>
      <br>
      <el-form
            :model="ruleForm"
            :rules="rules"
            ref="ruleForm"
            label-width="160px"
            label-position="left"
            class="demo-ruleForm"
            inline="true" 
            align="left"
          >
            <el-form-item label="" prop="eva" label-width="20px" >
              <font align="left" class="uid">{{username}}</font>
              &nbsp;
              <font>想说：</font>
            </el-form-item>
            <el-form-item label="" prop="eva_content" label-width="30px" >
              <el-input v-model="ruleForm.eva_content" class="blank" placeholder="欢迎发表评论(.w.)"></el-input>
            </el-form-item>
            <el-form-item>
              <el-button
                type="success"
                plain
                style="float: left;margin-left: 20px"
                @click="handleevaluation('ruleForm')"
              >评论{{evaluation_num}}
              </el-button>
            </el-form-item>
      </el-form>
      <hr />
      <br />
          <el-dialog :visible.sync="dialogVisible">
            <div>
              <el-avatar :size="90" :fit="fit" :src="base64"></el-avatar>
            </div>
            <div>
              <el-button type="primary" v-if='relation=="无"' @click="handlefollow">关注</el-button>
              <el-button type="info" v-if='relation=="关注"' @click="handlefollow">取消关注</el-button>
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
            <el-table
              :data="project_experience"
              :header-cell-style="{ textAlign: 'center' }"
              :cell-style="{ 'text-align': 'center' }"
              height="200"
            >
              <el-table-column prop="project_name" label="项目名称"></el-table-column>
              <el-table-column prop="create_time" label="发布时间" sortable></el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                  <el-button size="mini" @click="project_detail(scope.$index)">查看详情</el-button>
                </template>
              </el-table-column>
            </el-table>
            <el-divider content-position="left">
              <h4>收藏夹</h4>
            </el-divider>
            <el-table
              :data="facorite"
              :header-cell-style="{ textAlign: 'center' }"
              :cell-style="{ 'text-align': 'center' }"
              height="200"
            >
              <el-table-column prop="project_name" label="项目名称"></el-table-column>
              <el-table-column prop="create_time" label="发布时间" sortable></el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                  <el-button size="mini" @click="project_detail(scope.$index)">查看详情</el-button>
                </template>
              </el-table-column>
            </el-table>
            <div slot="footer" class="dialog-footer">
              <el-button type="primary" @click="dialogVisible = false">关闭</el-button>
            </div>
          </el-dialog>
          <div class="table">
            <el-table
              @row-click="handlecheck"
              :data="tableData"
              :show-header="false"
              :cell-style="{ 'text-align': 'left' }"
              :default-sort="{ prop: 'date', order: 'descending' }"
              class="eltable"
            >
              <el-table-column prop="evaluator_id,eva_content,eva_time"  width="750">
              <template slot-scope="scope">
              <font class="evaid">{{scope.row.evaluator_id}}</font>&nbsp;
              <font size="1px">说：</font>
              <font>{{scope.row.eva_content}}</font><br>
              <font size="1px">{{scope.row.eva_time}}</font>
              </template>
              </el-table-column>
            </el-table>
          </div>
    </el-main>
  </div>
</template>


<script>
/* global Vue */
import global_msg from '../../utils/global.js'
import { get_project } from '@/api/ProjectDetail.js'
import { get_require } from '@/api/ProjectDetail.js'
import { get_eva } from '@/api/ProjectDetail.js'
import { get_username } from '@/api/ProjectDetail.js'
import { get_leader } from '@/api/ProjectDetail.js'
import { detailnum } from '@/api/ProjectDetail.js'
import { createevalist } from '@/api/CreateList.js'
import { AddMyCollection } from '@/api/MyInfor.js'
import { SelectAllCollection } from '@/api/MyInfor.js'
import { Ifcollect } from '@/api/MyInfor.js'
import { createuser_project } from '@/api/CreateList.js'
import { GetUserInfor } from '@/api/MyInfor.js'
import { my_project } from '@/api/MyInfor.js'
import { GetMyCollection } from '@/api/MyInfor.js'
import { getproject } from '@/api/MyInfor.js'
import { openfile } from "@/api/file_load.js";
import { getRe } from "@/api/followother.js"
import { pullRe } from "@/api/followother.js"
export default {
  data() {
    return {
      //项目
      username: global_msg.nowuserid,
      user_id: '',
      create_time: '',
      project_name: '',
      project_background: '',
      project_introduction: '',
      project_content: '',
      project_progress: '',
      active:0,
      project_status: '',
      start_time: '',
      end_time: '',
      //需求
      purpose: '',
      team_type: '',
      team_limit: '',
      details: '',
      require_status: '',
      region: '',
      team_type_details: '',
      //评论
      tableData: [],
      ruleForm: {
        eva: ""
      },
      rules: {
        eva_content: [
          { required: true, message: "请输入评论内容", trigger: "blur" },
          {
            min: 3,
            max: 80,
            message: "长度在 3-80 个字符以内",
            trigger: "blur",
          },
        ],
      },
      //查看其他用户信息
      his: {
        user_id: "",
        gender: "",
        contact_info: "",
        institution: "",
        grade: "",
        introduction: "",
        point: ""
      },
      dialogVisible: false,
      formLabelWidth: '120px',
      project_experience: [],
      facorite: [],
      relation: "无",
      hisid: ""
    };
  },
  created() {
    this.get_Info();
  },
  methods: {
    handlecheck(row) {
      this.dialogVisible = true;
      this.get_hisinfo(row.evaluator_id);
    },
    get_hisinfo(his_id) {
      let params = {
        user_id: his_id
      }
      let params1 = {
        target: "user",
        id: his_id,
      };
      let params2 = {
        MyID: global_msg.nowuserid,
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
    handlefollow() {
      if (this.relation == "关注") {
        this.relation = "无";
        let params = {
          MyID: global_msg.nowuserid,
          ID: this.hisid,
          relationship:"无"
        }
        pullRe(params).then(function (res) {
          if (res == 1)
            console.log("关注成功");
          else
            console.log("失败！");
        })
      }
      else {
        this.relation = "关注";
        let params = {
          MyID: global_msg.nowuserid,
          ID: this.hisid,
          relationship:"关注"
        }
        pullRe(params);
      }
    },
    project_detail(index) {
      //进入项目详情页面
      var project_id = this.tabledata[index].project_id;
      //console.log(index, row);
      this.$router.push({
        path: "/users/ProjectDetail",
        query: { p_id: project_id },
      });
    },
    goback() {
      this.$router.push({ path: "/users/InforList" });
    },
    get_Info() {
      get_eva().then(function (res0) {
        global_msg.evaluationnum = res0.data.length;//改变全局projectnum
      })
      let para = {
        id: this.$route.query.p_id
      };
      get_project(para).then(res => {
        this.create_time = res.data.create_time;
        this.project_name = res.data.project_name;
        this.project_background = res.data.project_background;
        this.project_introduction = res.data.project_introduction;
        this.project_content = res.data.project_content;
        this.project_progress = res.data.project_progress;
        if(this.project_progress=="已发布")
          this.active=1;
        else if(this.project_progress=='招募中')
          this.active="2";
        else if(this.project_progress=='已完结')
          this.active="3";
        this.project_status = res.data.project_status;
        this.start_time = res.data.due;
        this.start_time = this.start_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
        this.start_time = this.start_time.slice(0, 19);
        this.end_time = res.data.due;
        this.end_time = this.end_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
        this.end_time = this.end_time.slice(20, 39);
        var judge = new String(this.project_status);
        get_leader(para).then(res => {
          this.user_id = res.data;
        })
        if (judge == '是') {
          get_require(para).then(res => {
            this.purpose = res.data.purpose;
            this.team_type = res.data.team_type;
            this.team_limit = res.data.team_limit;
            this.require_status = res.data.require_status;
            this.details = res.data.details;
            this.region = res.data.region;
            let dd = res.data.team_type_details.split("-");
            for (let ee of dd) {
              var ff = detailnum(Number(ee));
              this.team_type_details = this.team_type_details + ff + "   ";
            }
          })
        }
        else {
          this.purpose = "-暂无-";
          this.team_type = "-暂无-";
          this.team_limit = "-暂无-";
          this.requirement_status = "-暂无-";
          this.details = "-暂无-";
          this.region = "-暂无-";
          this.team_type_details = "-暂无-";
        }
      })
      var vm = this;//全局变量
      get_eva().then(function (res) {
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            eve_time: '',
            eva_content: '',
            evaluator_id: '',
            project_id: '',
          }
          form.project_id = item.project_id;
          form.eva_time = item.eva_time;
          form.eva_content = item.eva_content;
          form.evaluator_id = item.evaluator_id;
          var pid = new String(item.project_id);
          var vid = new String(vm.project_id);
          if (vm.$route.query.p_id == item.project_id)
            vm.tableData.push(form);
        }
      })
    },
    handlefavorite(index, row) { //收藏操作,需要间隔时间长一点，否则会收藏失败
      SelectAllCollection().then(function (res1) {
        global_msg.facoritenum = res1.data.length + 1;//改变全局facoritenum
      })
      var vm = this;
      var aa;
      let pp = {
        owner_id: global_msg.nowuserid,
        project_id: vm.$route.query.p_id
      }
      Ifcollect(pp).then(function (res) {
        aa = res.data;
        if (aa === false) {//被收藏过
          vm.$message.error("已经被收藏过");
        } else {
          global_msg.facoritenum = Number(global_msg.facoritenum) + 1
          let pa = {
            facorite_id: Number(global_msg.facoritenum),
            owner_id: global_msg.nowuserid,
            project_id: vm.$route.query.p_id
          }
          AddMyCollection(pa).then(function (res) {
            if (res.data === false) {
              vm.$message.error("收藏失败");
            }
            else {
              vm.$message.success("收藏成功");
            }
          })
        }
      })
    },
    handleevaluation(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          console.log("submit!");
        } else {
          console.log("error submit!!");
          return false;
        }
      });
      var vm = this;
      let param = {
        project_eva_id: global_msg.evaluationnum + 1,
        eva_content: this.ruleForm.eva_content,
        evaluator_id: global_msg.nowuserid,
        project_id: vm.$route.query.p_id,
      }
      createevalist(param).then(function (res) {
        if (res.data === false) {
          vm.$message.error("评论失败");
        }
        else {
          vm.$message.success("评论成功");
          location. reload();
        }
      })
    },
    handleApply() { //申请操作
      var vm = this;
      let param = {
        user_id: global_msg.nowuserid,
        project_id: vm.$route.query.p_id,
        duty: "申请者",
      }
      createuser_project(param).then(function (res) {
        if (res.data === false) {
          vm.$message.error("申请失败");
        }
        else {
          vm.$message.success("申请成功");
        }
      })
    },
    handleClick(tab, event) {
      console.log(tab, event);
    },
  }
}
</script>

<style scoped>
.detaildiv {
  /* position: absolute; */
  height: 100%;
  bottom: 0%;
  right: 30px;
  left: 30px;
}
.title {
  margin: 0;
  background: #f0f0f0;
  line-height: 50px;
  font-size: 25px;
  font-weight: bold;
  color: #000000;
}

.eva {
  margin: 0;
  font-size: 20px;
  font-weight: bold;
}

.uid {
  margin: 0;
  font-size: 20px;
}

.blank {
    margin: 0px;
    width: 800px;
}

.evaid {
    padding:2px;
    font-size:15px;
    background:#545c64;
    color:#ffffff;
    border-radius: 5px;
}

.eltable{
  border-collapse:separator;
  background-color:#ffffff;
  border-radius: 10px 10px 10px 10px;
}
</style>