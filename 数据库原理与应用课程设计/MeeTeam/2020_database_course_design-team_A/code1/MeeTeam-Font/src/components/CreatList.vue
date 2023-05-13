<template>
  <div id="home">
    <!--页头-->
    <el-header>发布组队需求</el-header>
    <el-main>
      <el-form :model="ruleForm" :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
        <el-form-item label="需求名称" prop="requirement_name">
          <el-input v-model="ruleForm.requirement_name"></el-input>
        </el-form-item>
        <el-form-item label="组队目的" prop="purpose">
          <el-input v-model="ruleForm.purpose"></el-input>
        </el-form-item>
        <el-form-item label="组队类型" prop="team_type">
          <el-select v-model="ruleForm.team_type" placeholder="请选择组队类型" @change="teamtypecheck">
            <el-option label="竞赛组队" value="竞赛"></el-option>
            <el-option label="课程项目组队" value="课程项目"></el-option>
          </el-select>
          <el-cascader :options="options" :props="props" :show-all-levels="false" clearable
            v-model="ruleForm.team_type_detail" v-show="detailshow" placeholder="请选择组队具体情况"></el-cascader>
        </el-form-item>
        <el-form-item label="组队人数" prop="team_limit">
          <el-select v-model="ruleForm.team_limit" placeholder="请选择组队人数">
            <el-option label="3" value="3"></el-option>
            <el-option label="4" value="4"></el-option>
            <el-option label="5" value="5"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="组队校区" prop="region">
          <el-checkbox-group v-model="ruleForm.region" id="checkbox">
            <el-checkbox label="四平" name="region"></el-checkbox>
            <el-checkbox label="嘉定" name="region"></el-checkbox>
            <el-checkbox label="沪西" name="region"></el-checkbox>
            <el-checkbox label="沪北" name="region"></el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item label="需求细则" prop="details">
          <el-input type="textarea" v-model="ruleForm.details"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="submitForm('ruleForm')">立即创建</el-button>
          <el-button @click="resetForm('ruleForm')">重置</el-button>
        </el-form-item>
      </el-form>
    </el-main>
  </div>
</template>

<script>
import global_msg from '../utils/global.js'
import { creatlist } from '@/api/creatlist.js'
export default {
  name: 'CreatList',
  data() {
    return {
      detailshow: false,
      props: { multiple: true },//级联选择器确定选项选不选择
      options: [
        {
          disabled: false,
          value: 1,
          label: '竞赛组队',
          children: [
            { value: 2, label: '创新创业' },
            { value: 3, label: '数学建模' },
            { value: 4, label: '学科竞赛' },
            { value: 5, label: '其他竞赛' },
          ]
        },
        {
          disabled: false,
          value: 6,
          label: '项目课程组队',
          children: [
            { value: 7, label: '新生院' },
            { value: 8, label: '建筑与城市规划学院' },
            { value: 9, label: '土木工程学院' },
            { value: 10, label: '机械与能源工程学院' },
            { value: 11, label: '经济与管理学院' },
            { value: 12, label: '环境科学与工程学院' },
            { value: 13, label: '材料科学与工程学院' },
            { value: 14, label: '电子与信息工程学院' },
            { value: 15, label: '人文学院' },
            { value: 16, label: '汽车学院' },
            { value: 17, label: '软件学院' },
            { value: 18, label: '其他学院' },
          ]
        },
      ],
      checkList: [],
      ruleForm: {
        require_id: '',
        purpose: '',
        team_type: '',
        team_limit: '',
        details: '',
        require_status: '',
        originator_id: '',
        project_id: '',
        region: [], //地区得到的是一个数组
        team_type_detail: [],//具体的类型得到的也是一个数组
        requirement_name:''
      },
      rules: {
        purpose: [
          { required: true, message: '请输入组队目的', trigger: 'blur' },
          { min: 3, max: 80, message: '长度在 3-80 个字符以内', trigger: 'blur' }
        ],
        team_type: [
          { required: true, message: '请选择组队类型', trigger: 'change' }
        ],
        details: [
          { required: true, message: '请填写需求细则', trigger: 'blur' },
          { min: 3, max: 100, message: '长度在 3-100 个字符以内', trigger: 'blur' }
        ],
        team_limit: [
          { required: true, message: '请选择组队人数', trigger: 'change' }
        ],
        region: [
          { type: 'array', required: true, message: '请至少选择一个校区', trigger: 'change' }
        ],
        requirement_name:[
           { required: true, message: '请输入需求名称', trigger: 'blur' },
          { min: 3, max: 80, message: '长度在 3-80 个字符以内', trigger: 'blur' }
        ]
      }
    };
  },
  methods: {
    submitForm(formName) {
      // console.log(this.ruleForm.team_type_detail);
      this.$refs[formName].validate((valid) => {
        if (valid) {
          console.log('submit!');
        } else {
          console.log('error submit!!');
          return false;
        }
      });
      global_msg.requirenum += 1;//全局需求个数+1
      global_msg.projectnum += 1;//全局项目个数+1
      this.ruleForm.project_id = global_msg.projectnum;//得到全局项目个数
      this.ruleForm.require_id = global_msg.requirenum;//得到全局需求个数
      this.ruleForm.originator_id = global_msg.nowuserid;//得到全局userid
      this.ruleForm.require_status = '0/' + this.ruleForm.team_limit;//改成了0/人数
      var RegionLast = this.ruleForm.region.join(','); // 把数组项拼接成字符串，以逗号,分隔;转换后的地区，需要转成字符串
      var TeamDetailLast = this.ruleForm.team_type_detail.join('-');
      var reg = new RegExp("1,", "g");
      TeamDetailLast = TeamDetailLast.replace(reg, "");//把父结点的值去掉
      reg = new RegExp("6,", "g");
      TeamDetailLast = TeamDetailLast.replace(reg, "");//把父结点的值去掉
      // alert(this.ruleForm.require_id)
      //  alert(this.ruleForm.purpose)
      //  alert(this.ruleForm.team_type)
      //  alert(this.ruleForm.team_limit)
      //  alert(this.ruleForm.details)
      // alert(this.ruleForm.require_status)
      // alert(this.ruleForm.originator_id)
      // alert(this.ruleForm.project_id)
      // alert(this.ruleForm.project_id)
      // alert(RegionLast)
      // alert(this.ruleForm.project_id)
      // alert(TeamDetailLast);
      let vm = this;
      //请求地址,this和vm指的是全局
      let param = {
        require_id: this.ruleForm.require_id,
        purpose: this.ruleForm.purpose,
        team_type: this.ruleForm.team_type,
        team_limit: this.ruleForm.team_limit,
        details: this.ruleForm.details,
        require_status: this.ruleForm.require_status,
        originator_id: this.ruleForm.originator_id,
        project_id: this.ruleForm.project_id,
        region: RegionLast,
        team_type_detail: TeamDetailLast,
        requirement_name: this.ruleForm.requirement_name
      }
      creatlist(param).then(function (res) {
        if (res.data === false) {
          vm.$message.error("提交失败");
          vm.resetForm(formName);
        } else {
          vm.$message.success("提交成功");
          vm.$router.push({ path: "/users/InforList" }); //接下来进入到哪个路由
        }
      })
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
      this.ruleForm.team_type_detail="";
      this.detailshow = false;
    },
    teamtypecheck() {
      this.options[0].disabled = false;
      this.options[1].disabled = false;
      this.detailshow = true;
      if (this.ruleForm.team_type === "竞赛")
        this.options[1].disabled = true;
      else if (this.ruleForm.team_type === "课程项目")
        this.options[0].disabled = true;
    }
  }
}
</script>

<style scoped>
.el-header,
.el-footer {
  font-weight:900;
  font-size:24px;
  background-color: #B3C0D1;
  color: #333;
  text-align: center;
  line-height: 60px;
}
.el-main {
  background-color: #ecf0f5;
  color: #333;
  text-align: left;
  line-height: 220px;
  height: 100%;
}
#home {
  width: 100%;
  min-height: 100vh;
  background-color: #ecf0f5;
  background-size: 100% 100%;
}
.demo-ruleForm >>> .el-form-item__label /*标题的字体大小 */
{
  font-size: 18px;
}
#checkbox >>> .el-checkbox__label /*复选框的字体大小 */
{
  font-size: 18px;
}
</style>