<template>
  <div id="home">
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPage' }"
            >首页</el-breadcrumb-item
          >
          <el-breadcrumb-item>项目管理</el-breadcrumb-item>
          <el-breadcrumb-item>我的项目</el-breadcrumb-item>
          <el-breadcrumb-item>编辑</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <div>
        <el-card>
          <el-header>编辑项目信息</el-header>
          <el-form
            :model="ruleForm"
            :rules="rules"
            ref="ruleForm"
            label-width="160px"
            label-position="left"
            class="demo-ruleForm"
          >
            <el-form-item label="项目名称" prop="project_name">
              <el-input v-model="ruleForm.project_name"></el-input>
            </el-form-item>
            <el-form-item label="项目背景" prop="project_background">
              <el-input v-model="ruleForm.project_background"></el-input>
            </el-form-item>
            <el-form-item label="项目简介" prop="project_introduction">
              <el-input v-model="ruleForm.project_introduction"></el-input>
            </el-form-item>
            <el-form-item label="项目内容" prop="project_content">
              <el-input v-model="ruleForm.project_content"></el-input>
            </el-form-item>
            <el-form-item label="起止时间" prop="due">
              <el-date-picker
                v-model="ruleForm.due"
                type="datetimerange"
                range-separator="至"
                start-placeholder="开始日期"
                end-placeholder="结束日期"
                style="float: left"
                format="yyyy-MM-dd HH:mm:ss"
                value-format="yyyy-MM-dd HH:mm:ss"
              >
              </el-date-picker>
            </el-form-item>
            <el-form-item label="项目当前进度" prop="project_progress">
              <el-select
                v-model="ruleForm.project_progress"
                placeholder="请选择项目当前进度"
              >
                <el-option label="准备阶段" value="准备阶段"></el-option>
                <el-option label="规划阶段" value="规划阶段"></el-option>
                <el-option label="起步阶段" value="起步阶段"></el-option>
                <el-option label="中期阶段" value="中期阶段"></el-option>
                <el-option label="收尾阶段" value="收尾阶段"></el-option>
              </el-select>
            </el-form-item>

            <el-form-item>
              <el-button type="primary" @click="submitForm('ruleForm')"
                >立即创建</el-button
              >
              <el-button @click="resetForm('ruleForm')">重置</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </div>
    </el-main>
  </div>
</template>

<script>
import global_msg from "../../utils/global.js";
import { Changeprojectlist } from "@/api/Myprojectlist.js";
import { Changerequirelist } from "@/api/Myprojectlist.js";

export default {
  name: "ChangeProjectList",
  data() {
    return {
      detailshow: false,
      props: { multiple: true }, //级联选择器确定选项选不选择
      checkList: [],
      ruleForm: {
        project_id: "",
        project_name: "",
        project_backgroud: "",
        project_introduction: "",
        project_content: "",
        due: "",
        project_progress: "",
      },
      rules: {
        project_name: [
          { required: true, message: "请输入项目名称", trigger: "blur" },
          {
            min: 3,
            max: 80,
            message: "长度在 3-80 个字符以内",
            trigger: "blur",
          },
        ],
        project_background: [
          { required: true, message: "请输入项目背景", trigger: "blur" },
          {
            min: 3,
            max: 80,
            message: "长度在 3-80 个字符以内",
            trigger: "blur",
          },
        ],
        project_introduction: [
          { required: true, message: "请输入项目简介", trigger: "blur" },
          {
            min: 3,
            max: 80,
            message: "长度在 3-80 个字符以内",
            trigger: "blur",
          },
        ],
        project_content: [
          { required: true, message: "请输入项目内容", trigger: "blur" },
          {
            min: 3,
            max: 80,
            message: "长度在 3-80 个字符以内",
            trigger: "blur",
          },
        ],
        due: [{ required: true, message: "请选择时间", trigger: "blur" }],
        project_progress: [
          { required: true, message: "请选择项目当前进度", trigger: "blur" },
        ],
      },
    };
  },
  methods: {
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          console.log("submit!");
        } else {
          console.log("error submit!!");
          return false;
        }
      });
      //项目
      var vm=this;
      var Due = new String(this.ruleForm.due);
      //请求地址,this和vm指的是全局
      let param1 = {
        project_id: this.$route.query.p_id,
        project_name: this.ruleForm.project_name,
        project_background: this.ruleForm.project_background,
        project_introduction: this.ruleForm.project_introduction,
        project_content: this.ruleForm.project_content,
        due: Due,
        project_progress: this.ruleForm.project_progress,
      }
      Changeprojectlist(param1).then(function (res) {
        if (res.data === false) {
          vm.$message.error("编辑失败");
          vm.resetForm(formName);
        } else {
          vm.$message.success("编辑成功");
          vm.$router.push({ path: "/users/Myproject" }); //接下来进入到哪个路由
        }
      });
      //添加数据进Project表
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
      this.ruleForm.team_type_detail = "";
      this.detailshow = false;
    },
    teamtypecheck() {
      this.options[0].disabled = false;
      this.options[1].disabled = false;
      this.detailshow = true;
      if (this.ruleForm.team_type === "竞赛") this.options[1].disabled = true;
      else if (this.ruleForm.team_type === "课程项目")
        this.options[0].disabled = true;
    },
  },
};
</script>

<style scoped>
#breadcrumb {
  margin-bottom: 10px;
}
.el-header {
  font-weight: 800;
  font-size: 24px;
  background-color: rgba(255, 255, 255, 0);
  color: #333;
  text-align: center;
  line-height: 10px;
}
.el-main {
  color: #333;
  text-align: left;
  line-height: 220px;
  height: 100%;
}
#home {
  width: 100%;
  min-height: 100vh;
  background-size: 100% 100%;
}
.demo-ruleForm >>> .el-form-item__label /*标题的字体大小 */ {
  font-size: 18px;
}
#checkbox >>> .el-checkbox__label /*复选框的字体大小 */ {
  font-size: 18px;
}
</style>