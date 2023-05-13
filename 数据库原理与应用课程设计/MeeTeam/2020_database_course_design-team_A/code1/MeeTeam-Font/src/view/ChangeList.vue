<template>
  <div id="home">
    <!--页头-->
    <el-header>
      <span class="container" @click="goback">
        <div class="button-wrapper">
          <svg width="120" height="42">
            <rect class="rectangle" width="120" height="42" />
          </svg>
          <div class="btn">
            返回
          </div>
        </div>
      </span>改变组队需求
    </el-header>
    <el-main>
      <el-form :model="ruleForm" :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
        <el-form-item label="需求名称" prop="requirement_name">
          <el-input v-model="ruleForm.requirement_name"></el-input>
        </el-form-item>
        <el-form-item label="需求目的" prop="purpose">
          <el-input v-model="ruleForm.purpose"></el-input>
        </el-form-item>
        <el-form-item label="需求类型" prop="team_type">
          <el-select v-model="ruleForm.team_type" placeholder="请选择需求类型" @change="teamtypecheck">
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
import { get_project } from '@/api/ProjectDetail'
import { changelist } from '@/api/Myprojectlist.js'
export default {
  name: "ChangeList",
  props: ['re_id'],
  data() {
    return {
      LastTime: '',
      detailshow: true,
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
        requirement_name: ''
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
        requirement_name: [
          { required: true, message: '请输入需求名称', trigger: 'blur' },
          { min: 3, max: 80, message: '长度在 3-80 个字符以内', trigger: 'blur' }
        ]
      }
    };
  },
  mounted() {
    this.formbegin();//一开始就填写表单
  },
  methods: {
    goback() {
      this.$router.push({ path: "/users/MyProject" }); //返回
    },
    formbegin() {
      var vm = this;
      let params = {
        require_id: vm.re_id,
      }
      get_project(params).then(function (res) {
        let item = res.data[0];
        vm.ruleForm.require_id = vm.re_id;
        var strs = new Array(); // 定义一数组
        strs = item.region.split(",");
        vm.ruleForm.region = strs;
        item.release_time = item.release_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
        vm.LastTime = item.release_time;
        vm.ruleForm.purpose = item.purpose;
        vm.ruleForm.team_type = item.team_type;
        vm.ruleForm.team_limit = item.team_limit;
        vm.ruleForm.details = item.details;
        vm.ruleForm.requirement_status = item.requirement_status;
        vm.ruleForm.originator_id = item.originator_id;
        vm.ruleForm.project_id = item.project_id;//这个暂时还没有和项目连接起来
        vm.ruleForm.requirement_name = item.requirement_name;
        vm.ruleForm.require_status = item.require_status;
        if (vm.ruleForm.team_type === "竞赛")
          vm.options[1].disabled = true;
        else if (vm.ruleForm.team_type === "课程项目")
          vm.options[0].disabled = true;
        // var strs2 = new Array(); // 定义一数组,这个不好转换
        // strs2=item.team_type_details.split("-");
        // console.log(strs2);
        // vm.ruleForm.team_type_details= strs2;
      })
    },
    submitForm(formName) {
      console.log(this.ruleForm.team_type_detail);
      this.$refs[formName].validate((valid) => {
        if (valid) {
          console.log('submit!');
        } else {
          console.log('error submit!!');
          return false;
        }
      });
      // this.ruleForm.project_id = global_msg.projectnum;//得到全局项目个数
      // this.ruleForm.require_id = global_msg.requirenum;//得到全局需求个数
      // this.ruleForm.originator_id = global_msg.nowuserid;//得到全局userid
      this.ruleForm.require_status = '0/' + this.ruleForm.team_limit;//改成了0/人数
      var RegionLast = this.ruleForm.region.join(','); // 把数组项拼接成字符串，以逗号,分隔;转换后的地区，需要转成字符串
      var TeamDetailLast = this.ruleForm.team_type_detail.join('-');
      var reg = new RegExp("1,", "g");
      TeamDetailLast = TeamDetailLast.replace(reg, "");//把父结点的值去掉
      reg = new RegExp("6,", "g");
      TeamDetailLast = TeamDetailLast.replace(reg, "");//把父结点的值去掉
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
      changelist(param).then(function (res) {
        if (res.data === false) {
          vm.$message.error("提交失败");
          vm.resetForm(formName);
        } else {
          vm.$message.success("提交成功");
          vm.$router.push({ path: "/users/MyProject" }); //接下来进入到哪个路由
        }
      })
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
      this.ruleForm.team_type_detail = "";
      this.detailshow = false;
    },
    teamtypecheck(ee) {
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
  font-weight: 900;
  font-size: 24px;
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

.demo-ruleForm>>>.el-form-item__label

/*标题的字体大小 */
  {
  font-size: 18px;
}

#checkbox>>>.el-checkbox__label

/*复选框的字体大小 */
  {
  font-size: 18px;
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

.button-wrapper {
  display: inline-block;
  position: relative;
  width: 60px;
  height: 30px;
  text-align: center;
}

.rectangle {
  stroke-width: 8px;
  stroke: #ecf0f5;
  fill: transparent;
  /* Core part of the animation */
  stroke-dasharray: 200 500;
  stroke-dashoffset: -372;
  /* 偏移负数，虚线整体右移动了372个单位 */
}

.btn {
  color: white;
  font-size: 24px;
  letter-spacing: 6px;
  position: relative;
  top: -72px;
  left: 30px;
}

@keyframes extend {
  to {
    stroke-dasharray: 600;
    /* 属性用于创建虚线： */
    stroke-dashoffset: 0;
    stroke-width: 4;
    /* 属性定义了一条线，文本或元素轮廓厚度： */
  }
}

.button-wrapper:hover .rectangle {
  animation: 0.5s extend linear forwards;
}
</style>