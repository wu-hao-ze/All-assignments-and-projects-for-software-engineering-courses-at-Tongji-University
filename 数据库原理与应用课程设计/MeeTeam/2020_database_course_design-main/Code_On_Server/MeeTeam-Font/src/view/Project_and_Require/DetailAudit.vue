<template>
  <div class="auditdiv">
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/Admin/AdminPageContent' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item :to="{ path: '/Admin/AuditProject' }">项目审核</el-breadcrumb-item>
          <el-breadcrumb-item>审核详情</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <p class="title" align="center" label="项目名称">{{ project_name }}</p>
      <el-descriptions title="项目详情" :column="2" border style="margin-top: 20px">
        <el-descriptions-item label="项目发布者">{{ user_id }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ create_time }}</el-descriptions-item>
        <el-descriptions-item label="开始时间">{{ start_time }}</el-descriptions-item>
        <el-descriptions-item label="结束时间">{{ end_time }}</el-descriptions-item>
      </el-descriptions>
      <el-descriptions title="" :column="1" border>
        <el-descriptions-item label="项目背景">{{
            project_background
        }}</el-descriptions-item>
        <el-descriptions-item label="项目简介">{{
            project_introduction
        }}</el-descriptions-item>
        <el-descriptions-item label="项目内容">{{
            project_content
        }}</el-descriptions-item>
        <el-descriptions-item label="是否有组队需求">{{
            project_status
        }}</el-descriptions-item>
      </el-descriptions>
      <br />
      <el-descriptions title="需求详情" :column="2" border>
        <el-descriptions-item label="组队类型">{{ team_type }}</el-descriptions-item>
        <el-descriptions-item label="类型细则">{{
            team_type_details
        }}</el-descriptions-item>
        <el-descriptions-item label="组队人数">{{ team_limit }}</el-descriptions-item>
        <el-descriptions-item label="当前状态">{{ require_status }}</el-descriptions-item>
      </el-descriptions>
      <el-descriptions title="" :column="1" border>
        <el-descriptions-item label="所在校区">{{ region }}</el-descriptions-item>
        <el-descriptions-item label="需求目的">{{ purpose }}</el-descriptions-item>
        <el-descriptions-item label="组队细则">{{ details }}</el-descriptions-item>
      </el-descriptions>
      <el-steps :space="500" :active="1" finish-status="success" align-center
        style="margin-top: 100px; margin-bottom: 100px">
        <el-step title="创建项目"></el-step>
        <el-step title="项目审核"></el-step>
        <el-step title="项目发布"></el-step>
      </el-steps>
      <div class="auditoperation" style="text-align: center">
        <el-button v-if="!showAuditButton" type="warning" @click="
          handleRecommend();
        open1();
        ">推荐上榜</el-button>
      </div>
      <div class="auditoperation" style="display: flex  text-align: center">
        <el-button v-if="showAuditButton" type="success" @click="
          handleAudit('1');
        open2();
        " style="margin-right: 66px">审核通过</el-button>
        <el-button v-if="showAuditButton" type="danger" @click="dialogFormVisible = true">审核不通过</el-button>
        <el-dialog title="审核不通过" :visible.sync="dialogFormVisible">
          <el-form :model="form">
            <el-form-item label="原因说明：">
              <el-input type="textarea" v-model="form.reason"></el-input>
            </el-form-item>
          </el-form>
          <div slot="footer" class="dialog-footer">
            <el-button @click="dialogFormVisible = false">取 消</el-button>
            <el-button type="primary" @click="
              dialogFormVisible = false;
            handleAudit('0');
            ">确 定</el-button>
          </div>
        </el-dialog>
      </div>
    </el-main>
  </div>
</template>


<script>
/* global Vue */
import { get_project } from "@/api/ProjectDetail.js";
import { get_require } from "@/api/ProjectDetail.js";
import { get_username } from "@/api/ProjectDetail.js";
import { detailnum } from "@/api/ProjectDetail.js";
import { audit } from "@/api/audit.js";
import { recommend } from "@/api/audit.js";
import { fetchList } from "@/api/Querylist.js";

export default {
  data() {
    return {
      showAuditButton: false,
      listQuery: {
        project_id: this.$route.query.p_id,
        audit_status: "",
      },
      //项目
      user_id: "",
      create_time: "",
      project_name: "",
      project_background: "",
      project_introduction: "",
      project_content: "",
      project_progress: "",
      project_status: "",
      start_time: "",
      end_time: "",
      //需求
      purpose: "",
      team_type: "",
      team_limit: "",
      details: "",
      require_status: "",
      region: "",
      team_type_details: "",
      dialogFormVisible: false,
      form: {
        reason: "",
      },
      formLabelWidth: "120px",
    };
  },

  created() {
    this.get_Info();
    this.triggerButton();
  },

  methods: {
    get_Info() {
      let para = {
        id: this.$route.query.p_id,
      };
      get_project(para).then((res) => {
        this.create_time = res.data.create_time;
        this.project_name = res.data.project_name;
        this.project_background = res.data.project_background;
        this.project_introduction = res.data.project_introduction;
        this.project_content = res.data.project_content;
        this.project_status = res.data.project_status;
        this.start_time = res.data.due;
        this.start_time = this.start_time.replace('"', "").replace('"', ""); //去掉时间格式的引号
        this.start_time = this.start_time.slice(0, 19);
        this.end_time = res.data.due;
        this.end_time = this.end_time.replace('"', "").replace('"', ""); //去掉时间格式的引号
        this.end_time = this.end_time.slice(20, 39);
        var judge = new String(this.project_status);
        if (judge == "是") {
          get_require(para).then((res) => {
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
          });
        } else {
          this.purpose = "-暂无-";
          this.team_type = "-暂无-";
          this.team_limit = "-暂无-";
          this.requirement_status = "-暂无-";
          this.details = "-暂无-";
          this.region = "-暂无-";
          this.team_type_details = "-暂无-";
        }
      });
    },
    triggerButton() {
      var query = JSON.stringify(this.listQuery);
      fetchList(query).then((response) => {
        this.showAuditButton = !parseInt(response.data[0].audit_status);
        console.log(response.data[0]);
        console.log(response.data[0].audit_status);
        console.log(this.showAuditButton);
      });
    },
    open1() {
      this.$confirm("此操作将推荐该项目至首页, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          this.$message({
            type: "success",
            message: "项目推荐成功!",
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消推荐。",
          });
        });
    },
    open2() {
      this.$confirm("此操作将审核通过并发布该项目, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          this.$message({
            type: "success",
            message: "审核发布成功!",
          });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "审核未通过！",
          });
        });
    },
    handleRecommend() {
      let param = {
        project_id: this.$route.query.p_id,
      };
      recommend(param);
    },
    handleAudit(param) {
      let params = {
        project_id: this.$route.query.p_id,
        admin_id: this.$route.query.id,
        result: param,
        reason: "",
      };
      if (param == "1") {
        audit(params);
      } else {
        params.reason = this.form.reason;
        audit(params);
      }
    },
  },
};
</script>


<style>
.auditdiv {
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
</style>
