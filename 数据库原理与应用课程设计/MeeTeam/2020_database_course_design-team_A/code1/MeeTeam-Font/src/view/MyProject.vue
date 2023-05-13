<template>
  <div id="myproject-container">
    <el-container>
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
        <el-table :data="tableData" :header-cell-style="{textAlign:'center'}" :cell-style="{'text-align':'center'}" default-sort="{ prop: 'date', order: 'descending' }">
          <el-table-column prop="project_name" label="项目名称" width="200">
          </el-table-column>
          <el-table-column prop="project_progress" label="项目当前进度" width="200">
          </el-table-column>
          <el-table-column prop="create_time" label="发布时间" sortable width="200">
          </el-table-column>
          <el-table-column prop="project_status" label="是否有组队需求" width="200">
          </el-table-column>
           <el-table-column label="操作">
            <template slot-scope="scope">
              <el-button size="mini" @click="handleDetail(scope.$index, scope.row)">查看详情</el-button>
              <el-button size="mini" @click="handleEdit(scope.$index, scope.row)">编辑</el-button>
              <el-button size="mini" type="danger" @click="handleDelete(scope.$index, scope.row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-main>
    </el-container>
  </div>
</template>

<script>
import global_msg from '../utils/global.js'
import { getlistInfor } from '@/api/Inforlist.js'
import { deleteproject } from '@/api/Myprojectlist.js'
import { get_username } from '@/api/ProjectDetail.js'
export default {
  name: 'MyProject',
  data() {
    return {
      username: global_msg.nowuserid,
      tableData: [],
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
      getlistInfor().then(function (res) {
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            project_name: '',
            project_progress: '',
            create_time: '',
            project_status: '',
            project_id: '',
          }
          form.project_name = item.project_name;
          form.project_introduction = item.project_introduction;
          form.project_progress = item.project_progress;
          form.create_time = item.create_time;
          form.create_time = form.create_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
          form.project_status = item.project_status;
          form.project_id = item.project_id;
          let para = {
            id: form.project_id
          };
          get_username(para).then(res => {
            form.user_id= res.data.user_id;
            if (res.data.user_id == global_msg.nowuserid )
                vm.tableData.push(form);
          })
        }
      })
    },
    handleDetail(index, row) { //查看详情操作
      var project_id = this.tableData[index].project_id;
      console.log(index, row);
      this.$router.push({ path: "/users/ProjectDetail", query: { p_id: project_id } });
    },
    handleEdit(index, row) { //编辑操作
      var project_id = this.tableData[index].project_id;
      this.$router.push({ name: 'ChangeList', params: { p_id: project_id } });
      console.log(index, row);
    },
    handleDelete(index, row) {//删除操作
      this.$confirm('此操作将永久删除该数据, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.tableData.splice(index, 1) //删除下标为index的内容
        this.$message({
          type: 'success',
          message: '删除成功!',
        })
        var id = this.tableData[index].project_id;
        //再在数据库中删除
        let param = {
          project_id: id,
        }
        deleteproject(param);
        console.log(index, row);
      }).catch((err) => {
        this.$message({
          type: 'error',
          message: err
        })
      })
    },
  }
}
</script>

<style>
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