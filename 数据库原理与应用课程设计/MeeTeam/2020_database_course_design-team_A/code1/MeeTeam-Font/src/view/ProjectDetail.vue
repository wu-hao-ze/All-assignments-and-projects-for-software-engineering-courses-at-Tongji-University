<template>
    <div class="detaildiv">
        <p class="title" align="center" label="项目名称">{{ project_name }}</p>
        <el-descriptions title="项目详情" :column="2" border
            style="margin-top: 20px;margin-right: 80px">
             <el-descriptions-item label="项目发布者">{{ user_id }}</el-descriptions-item>
             <el-descriptions-item label="创建时间">{{ create_time }}</el-descriptions-item>
             <el-descriptions-item label="开始时间">{{ start_time }}</el-descriptions-item>
             <el-descriptions-item label="结束时间">{{ end_time }}</el-descriptions-item>
             <el-descriptions-item label="项目当前进展">{{ project_progress }}</el-descriptions-item>
             <el-descriptions-item label="是否有组队需求">{{ project_status }}</el-descriptions-item>
        </el-descriptions>
        <el-descriptions title="" :column="1" border
            style="margin-right: 80px">
             <el-descriptions-item label="项目背景">{{ project_background }}</el-descriptions-item>
             <el-descriptions-item label="项目简介">{{ project_introduction }}</el-descriptions-item>
             <el-descriptions-item label="项目内容">{{ project_content }}</el-descriptions-item>
        </el-descriptions>
        <br>
        <el-descriptions title="需求详情" :column="2" border
            style="margin-right: 80px">
            <el-descriptions-item label="组队类型">{{ team_type }}</el-descriptions-item>
            <el-descriptions-item label="类型细则">{{ team_type_details }}</el-descriptions-item>
            <el-descriptions-item label="组队人数">{{ team_limit }}</el-descriptions-item>
            <el-descriptions-item label="当前状态">{{ require_status }}</el-descriptions-item>
        </el-descriptions>
        <el-descriptions title="" :column="1" border
            style="margin-right: 80px">
            <el-descriptions-item label="所在校区">{{ region }}</el-descriptions-item>
            <el-descriptions-item label="需求目的">{{ purpose }}</el-descriptions-item>
            <el-descriptions-item label="组队细则">{{ details }}</el-descriptions-item>
        </el-descriptions>
        <el-steps :space="500" :active="1" finish-status="success" align-center
         style="margin-top: 100px;margin-bottom: 100px">
            <el-step title="发布项目"></el-step>
            <el-step title="招募队友"></el-step>
            <el-step title="项目完结"></el-step> 
        </el-steps>
        <el-button type="success" plain style="float: right;margin-right: 80px">申请加入</el-button>
        <el-button type="info" plain style="float: right;margin-right: 30px">联系发起人</el-button>
        <el-button type="submit" plain style="float: left;margin-left: 20px" 
         @click="handlefavorite(scope.$index, scope.row)">收藏{{ favorite_num }}</el-button>
        <el-button type="success" plain style="float: left;margin-left: 20px" 
         @click="handleevaluation()">评论{{ evaluation_num }}</el-button>
         <br><br><br>
         <el-tabs v-model="activeName">
            <el-tab-pane label="展开评论列表" name="展开评论列表" @tab-click="handleClick">
              <div>
                <el-main>
                <el-table :data="tableData" :header-cell-style="{textAlign:'center'}" :cell-style="{'text-align':'center'}" :default-sort="{ prop: 'date', order: 'descending' }">
                  <el-table-column prop="evaluator_id" label="用户id" width="200">
                  </el-table-column>
                  <el-table-column prop="eva_content" label="评论内容" width="800">
                  </el-table-column>
                  <el-table-column prop="eva_time" label="评论时间" sortable  width="200">
                  </el-table-column>
                </el-table>
                </el-main>
              </div>
            </el-tab-pane>
            <el-tab-pane label="收起评论列表" name="收起评论列表">
            <div>
            </div>
            </el-tab-pane> 
        </el-tabs>
    </div>
    
</template>


<script>
/* global Vue */
import global_msg from '../utils/global.js'
import { get_project } from '@/api/ProjectDetail.js'
import { get_require } from '@/api/ProjectDetail.js'
import { get_eva } from '@/api/ProjectDetail.js'
import { get_username } from '@/api/ProjectDetail.js'
import { detailnum } from '@/api/ProjectDetail.js'
import { createevalist } from '@/api/CreateList.js'
export default {
  data() {
     return {
      //项目
      user_id:'',
      create_time:'',
      project_name: '',
      project_background: '',
      project_introduction: '',
      project_content: '',
      project_progress: '',
      project_status: '',
      start_time:'',
      end_time:'',
      //需求
      purpose: '',
      team_type: '',
      team_limit: '',
      details: '',
      require_status: '',
      region: '',
      team_type_details: '',
      //评论
      activeName: "展开评论列表",
      tableData: [],
    };
  },
  created() {
    this.get_Info();
  },
  methods: {
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
          this.project_status = res.data.project_status;
          this.start_time = res.data.due;
          this.start_time = this.start_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
          this.start_time = this.start_time.slice(0,19);
          this.end_time = res.data.due;
          this.end_time = this.end_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
          this.end_time = this.end_time.slice(20,39);
          var judge=new String(this.project_status);
          get_username(para).then(res => {
            this.user_id= res.data.user_id;
            })
          if(judge=='是'){
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
          else{
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
              form.project_id= item.project_id;
              form.eva_time = item.eva_time;
              form.eva_content = item.eva_content;
              form.evaluator_id = item.evaluator_id;
              var pid=new String(item.project_id);
              var vid=new String(vm.project_id);
              if (vm.$route.query.p_id==item.project_id )
                 vm.tableData.push(form);
            } 
       })   
    },
    handlefavorite(index, row) { //收藏操作
       
    },
    handleevaluation() { //评论操作
        var vm=this;
        var content=prompt("请输入评论",""); // 弹出input框
        let param={
            project_eva_id:global_msg.evaluationnum+1,
            eva_content:content,
            evaluator_id:global_msg.nowuserid,
            project_id:vm.$route.query.p_id,
        }
        createevalist(param).then(function (res) {
          if (res.data === false) {
            vm.$message.error("提交失败");
          } 
          else{
            alert("提交成功！");
          }
      })
    },
    handleClick(tab, event) {
      console.log(tab, event);
    },
 }
}
</script>

<style>
.detaildiv {
    /* position: absolute; */
    height: 100%;
    bottom: 0%;
    right: 30px;
    left: 30px;
}
.title {
    margin:0;
    background:#F0F0F0;
    line-height:50px;
    font-size:25px;
    font-weight: bold;   
    color:#000000;
}
</style>