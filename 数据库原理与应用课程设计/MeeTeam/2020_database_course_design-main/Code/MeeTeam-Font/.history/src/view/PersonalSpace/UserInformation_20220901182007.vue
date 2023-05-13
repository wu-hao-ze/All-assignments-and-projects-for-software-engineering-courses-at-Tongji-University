<template>
  <div id="creatteam-container">
    <el-container>
      <el-main>
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPage' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>个人信息</el-breadcrumb-item>
        </el-breadcrumb>
        <el-button type="text" @click="dialogFormVisible=true"><i class="el-icon-edit"></i>编辑信息</el-button>
        <el-dialog title="修改个人信息" :visible.sync="dialogFormVisible">
  <el-form :model="form">
    <el-form-item label="联系方式" :label-width="formLabelWidth">
      <el-input v-model="form.contact_info" autocomplete="off"></el-input>
     </el-form-item>
     <el-form-item label="性别" :label-width="formLabelWidth">
      <el-select v-model="form.gender" placeholder="请选择性别">
        <el-option label="男" value="1"></el-option>
        <el-option label="女" value="0"></el-option>
      </el-select>
    </el-form-item>
     <el-form-item label="就读院校" :label-width="formLabelWidth">
      <el-input v-model="form.institution" autocomplete="off"></el-input>
     </el-form-item>
     <el-form-item label="专业" :label-width="formLabelWidth">
      <el-input v-model="form.major" autocomplete="off"></el-input>
     </el-form-item>
     <el-form-item label="年级" :label-width="formLabelWidth">
      <el-input v-model="form.grade" autocomplete="off"></el-input>
     </el-form-item>
     <el-form-item label="个人简介" :label-width="formLabelWidth">
      <el-input v-model="form.introduction" autocomplete="off"></el-input>
     </el-form-item>
  </el-form>
   <div slot="footer" class="dialog-footer">
    <el-button @click="dialogFormVisible = false">取 消</el-button>
    <el-button type="primary" @click="submit()">确 定</el-button>
  </div>
</el-dialog>
        <el-descriptions class="margin-top" title=" " :column="3" :size="size" border>
          <template slot="extra">
          </template>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-user"></i>
              用户名
            </template>
            {{  username  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-s-custom"></i>
              性别
            </template>
            {{  gender  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-mobile-phone"></i>
              联系方式
            </template>
            {{  contact_info  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-location-outline"></i>
              就读院校
            </template>
            {{  institution  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-office-building"></i>
              专业
            </template>
            {{  major  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-reading"></i>
              年级
            </template>
            {{  grade  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-coin"></i>
              积分
            </template>
            {{  point  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-date"></i>
              注册时间
            </template>
            {{  register_time  }}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <i class="el-icon-tickets"></i>
              个人简介
            </template>
            {{  introduction  }}
          </el-descriptions-item>
        </el-descriptions>
        
        <el-divider href="#anchor1"></el-divider>
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPage' }">首页</el-breadcrumb-item>
          <el-breadcrumb-item>我的收藏</el-breadcrumb-item>
        </el-breadcrumb>
        <br/>
        <div>
          <el-table :data="tableData1" :header-cell-style="{ textAlign: 'center' }"
            :cell-style="{ 'text-align': 'center' }" default-sort="{ prop: 'create_time', order: 'descending' }">
            <el-table-column prop="project_name" label="项目名称" width="150">
            </el-table-column>
            <el-table-column prop="project_introduction" label="项目简介" width="350">
            </el-table-column>
            <el-table-column prop="facorite_time" label="收藏时间" sortable width="150">
            </el-table-column>
            <el-table-column prop="project_status" label="是否有组队需求" width="150">
            </el-table-column>
            <el-table-column label="操作">
              <template slot-scope="scope">
                <el-button size="mini" @click="handleDetail(scope.$index, scope.row)">查看详情</el-button>
                <el-button size="mini" type="danger" @click="handleDelete(scope.$index, scope.row)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </el-main>
    </el-container>
  </div>
</template>

<script>
import global_msg from '../../utils/global.js'
import { GetUserInfor } from '@/api/MyInfor.js'
import { GetMyCollection } from '@/api/MyInfor.js'
import { getproject } from '@/api/MyInfor.js'
import { deletecollect } from '@/api/MyInfor.js'
import { PullInfo } from '@/api/pullinfo.js'
export default {
  name: 'UserInformation',
  data() {
    return {
      username: global_msg.nowuserid,
      gender: '',
      contact_info: '',
      institution: '',
      major: '',
      grade: '',
      point: '',
      register_time: '',
      introduction: '',
      tableData1: [],
      /*判断对话框是否打开，true为打开*/
      dialogFormVisible: false,
      form: {
        contact_info: '', //联系方式
        gender: '' ,    //性别
        institution: '', //就读院校
        major: '',       //专业
        grade:'',      //年级
        introduction: '',//个人简介
        date1: '',
        date2: '',
        delivery: false,
        type: [],
        resource: '',
        desc: ''
        },
        formLabelWidth: '80px'
    };
  },
  mounted() {
    this.getlist() //页面一进入就加载表格
    this.getcollect()
  },
  methods: {
    getlist() {
      let vm = this;
      let params = {
        user_id: this.username,
      };
      GetUserInfor(params).then(res => {
        let item = res.data[0];
        if(item.gender==="0")
        vm.gender="女"
        else
        vm.gender="男"
        // vm.gender = item.gender;
        vm.contact_info = item.contact_info;
        vm.institution = item.institution;
        vm.major = item.major;
        vm.grade = item.grade;
        vm.point = item.point;
        vm.register_time = item.register_time.replace("\"", "").replace("\"", "");
        vm.introduction = item.introduction;
      })
    },
    getcollect() {
      let vm = this;
      let pa1 = {
        user_id: vm.username
      }
      GetMyCollection(pa1).then(res => {
        console.log(res)
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            project_name: '',
            project_introduction: '',
            project_status: '',
            project_id: '',
            facorite_id: '',
            facorite_time: '',
          }
          let pa2 = {
            project_id: item.project_id
          }
          getproject(pa2).then(res1 => {
            let dataa = res1.data[0];
            form.project_name = dataa.project_name;
            form.project_introduction = dataa.project_introduction;
            form.project_status = dataa.project_status;
            form.project_id = dataa.project_id;
            form.facorite_id = item.facorite_id;
            form.facorite_time = item.facorite_time;
            // alert(form.project_id)
          })
          console.log(form)
          vm.tableData1.push(form);
        }
      })
    },
    handleDelete(index, row) {//删除操作
      var id = this.tableData1[index].facorite_id;
      //有个bug，要先在数据库删除，再在列表中删除，否则不能删掉最后一个
      //在数据库中删除
      let param = {
        facorite_id: id,
      }
      deletecollect(param);
      //列表中删除
      this.tableData1.splice(index, 1) //删除下标为index的内容
      this.$message({
        type: 'success',
        message: '删除成功!',
      })
      console.log(index, row);
    },
    handleDetail(index, row) {
      var project_id = this.tableData1[index].project_id;
      console.log(index, row);
      this.$router.push({ path: "/users/ProjectDetail", query: { p_id: project_id } });
    },
    submit(){
      let vm=this;
      let params = {
        ID:vm.username,
        gender:this.form.gender,
        contact_info:this.form.contact_info,
        institution:this.form.institution,
        major:this.form.major,
        introduction:this.form.introduction,
        grade:this.form.grade,
      }
      console.log(params)
      PullInfo(params).then(function(res){
                console.log(res)
                console.log(res.isexist)
                if(res.data)
                {
                  this.dialogFormVisible=false;
                    vm.$router.push({path: "/users/UserInformation"}); //回到个人详细页面
                }
                else{
                  this.dialogFormVisible=false;
                    vm.resetForm()
                }
             })
    },
  }
}
</script>

<style>
#creatteam-container {
  position: relative;
  margin: 10px 0;
  margin-left: 0px;
  height: 100%;
}

.el-aside {
  color: #333;
}
</style>