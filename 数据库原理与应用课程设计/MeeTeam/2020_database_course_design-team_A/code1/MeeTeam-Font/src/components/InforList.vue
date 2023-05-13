<template>
  <div id="creatteam-container">
    <el-container style="height: 100%; border: 1px solid #eee">
      <!-- <el-aside width="250px" style="background-color: rgb(238, 241, 246)">
        <el-menu :default-openeds="['1', '3']">
          
        </el-menu>
      </el-aside> -->
      <el-container>
        <!-- <el-header style="text-align: right; font-size: 15px; top:50%;">
          <el-button type="primary" plain @click="additem">发布项目</el-button>
          <el-button type="warning" plain @click="myitem">我的项目</el-button>
          <el-dropdown>
            <i class="el-icon-setting" style="margin: 30px"></i>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>用户信息</el-dropdown-item>
              <el-dropdown-item>系统设置</el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
          <span>{{ username }}</span>
        </el-header> -->
        <el-main>
          <div style="height: 50px;">
            <el-autocomplete class="inline-input" v-model="state1" :fetch-suggestions="querySearch" placeholder="项目名称"
              @select="handleSelect" style="float:left;margin-right:30px;width:150px;"></el-autocomplete>
            <el-autocomplete class="inline-input" v-model="state1" :fetch-suggestions="querySearch" placeholder="项目发布人"
              @select="handleSelect" style="float:left;margin-right:30px;width:150px;"></el-autocomplete>
           <el-select v-model="value2" multiple placeholder="项目当前进度" style="float:left;margin-right:30px;width:150px;">
              <el-option v-for="item in options3" :key="item.value" :label="item.label" :value="item.value">
              </el-option>
            </el-select>
            <el-select v-model="value2" multiple placeholder="是否有组队需求" style="float:left;margin-right:30px;width:150px;">
              <el-option v-for="item in options3" :key="item.value" :label="item.label" :value="item.value">
              </el-option>
            </el-select>
            <el-button class="filter-item" type="primary" icon="el-icon-search" @click="searchContList"
              style="float:left">搜索
            </el-button>
          </div>
          <br><br>
              <div>
                <el-table :data="tableData" :header-cell-style="{textAlign:'center'}" :cell-style="{'text-align':'center'}" default-sort="{ prop: 'create_time', order: 'descending' }">
                  <el-table-column prop="project_name" label="项目名称" width="200">
                  </el-table-column>
                  <el-table-column prop="project_introduction" label="项目简介" width="400">
                  </el-table-column>
                  <el-table-column prop="project_progress" label="项目当前进度" width="200">
                  </el-table-column>
                  <el-table-column prop="create_time" label="发布时间" sortable width="200">
                  </el-table-column>
                   <el-table-column prop="project_status" label="是否有组队需求" width="200">
                  </el-table-column>
                  <el-table-column label="操作">
                    <template slot-scope="scope">
                      <el-button size="mini" @click="handleLook(scope.$index, scope.row)">查看详情</el-button>
                    </template>
                  </el-table-column>
                </el-table>
              </div>
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import global_msg from '../utils/global.js'
import { getlistInfor } from '@/api/Inforlist.js'
import { getrequireInfor } from '@/api/Inforlist.js'
export default {
  name: 'InforList',
  data() {
    return {
      activeName: "项目列表",
      username: global_msg.nowuserid,
      tableData: [],
    };
  },
  mounted() {
    this.getlist() //页面一进入就加载表格
  },
  methods: {
    handleLook(index, row) {//进入项目详情页面
      var project_id = this.tableData[index].project_id;
      console.log(index, row);
      this.$router.push({ name: 'ProjectDetail', params: { p_id: project_id } });
    },
    handleClick(tab, event) {
      console.log(tab, event);
    },
    additem: function () {
      this.$router.push({ path: "/users/CreatList" }); //接下来进入到CreatList
    },
    myitem: function () {
      this.$router.push({ path: "/users/MyProject" }); //接下来进入到MyProject
    },
    getlist() {
      var vm = this;//全局变量
      getrequireInfor().then(function (res1) {
        global_msg.requirenum = res1.data.length;//改变全局projectnum
        })
      getlistInfor().then(function (res) {
        global_msg.projectnum = res.data.length;//改变全局requirenum
        // console.log(res);
        // console.log(res.data.length);
        // console.log(res.data[0]);
        // console.log(res.data[0].details);
        for (let item of res.data) {
          let form = {//设置添加数据的格式
            project_name: '',
            project_introduction: '',
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
          
          vm.tableData.push(form);
          // console.log(form);
          // console.log(vm.tableData);
        }
      })
    }
  }
}
</script>

<style scoped>
#creatteam-container {
  /* position: absolute; */
  margin-left: 0px;
  height: 100%;
  bottom: 0%;
  right: 0px;
  left: 0px;
}

.el-header {
  background-color: #B3C0D1;
  color: #333;
  line-height: 60px;
}

.el-aside {
  color: #333;
}
</style>