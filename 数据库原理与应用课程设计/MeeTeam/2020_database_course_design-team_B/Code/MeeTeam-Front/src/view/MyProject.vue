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
        <el-table :data="tableData" :default-sort="{ prop: 'date', order: 'descending' }">
          <el-table-column prop="projectname" label="需求名称" width="140">
          </el-table-column>
          <el-table-column prop="team_type" label="需求类型" width="140">
          </el-table-column>
          <el-table-column prop="region" label="所在校区" width="200">
          </el-table-column>
          <el-table-column prop="details" label="需求简介" width="400">
          </el-table-column>
          <el-table-column prop="date" label="发布时间" sortable>
          </el-table-column>
          <el-table-column label="操作">
            <template slot-scope="scope">
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
import { deleteteam } from '@/api/Myteamlist.js'
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
            projectname: '',
            originator_id: '',
            team_type: '',
            region: '',
            details: '',
            date: '',
            require_id: '',
          }
          form.projectname = item.requirement_name;
          form.originator_id = item.originator_id;
          form.team_type = item.team_type;
          form.details = item.details;
          form.date = item.release_time;
          form.date = form.date.replace("\"", "").replace("\"", "");//去掉时间格式的引号
          form.region = item.region;
          form.require_id = item.require_id;
          if (form.originator_id === global_msg.nowuserid)
            vm.tableData.push(form);
        }
      })
    },
    handleEdit(index, row) { //编辑操作
      // var require_id = this.tableData[index].require_id;
      this.$router.push({ name: 'ChangeList', params: { re_id: row.require_id } });
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
        var id = this.tableData[index].require_id;
        //再在数据库中删除
        let param = {
          require_id: id,
        }
        deleteteam(param);
        console.log(index, row);
      }).catch((err) => {
        this.$message({
          type: 'error',
          message: err
        })
      })
    }
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