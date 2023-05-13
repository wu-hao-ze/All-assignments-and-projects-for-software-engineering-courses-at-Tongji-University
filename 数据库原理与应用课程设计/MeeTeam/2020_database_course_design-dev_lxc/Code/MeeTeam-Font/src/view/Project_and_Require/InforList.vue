<template>
  <div id="body">
    <el-main>
      <div id="breadcrumb">
        <el-breadcrumb separator-class="el-icon-arrow-right">
          <el-breadcrumb-item :to="{ path: '/users/UserPageContent' }"
            >首页</el-breadcrumb-item
          >
          <el-breadcrumb-item>项目管理</el-breadcrumb-item>
          <el-breadcrumb-item>平台项目列表</el-breadcrumb-item>
        </el-breadcrumb>
      </div>

      <div style="height: 50px">
        <div class="filter-container">
          <el-input
            v-model="listQuery.project_name"
            placeholder="项目名称"
            style="width: 180px"
            class="filter-item"
            @keyup.enter.native="handleFilter"
          ></el-input>
          &nbsp;
          <el-input
            v-model="listQuery.publisher"
            placeholder="项目发布者"
            style="width: 180px"
            class="filter-item"
            @keyup.enter.native="handleFilter"
          ></el-input>
          &nbsp;
          <el-select
            v-model="listQuery.hav_require"
            placeholder="是否有组队需求"
            clearable
            style="width: 180px"
            class="filter-item"
          >
            <el-option
              v-for="item in havreqOptions"
              :key="item.key"
              :label="item.display_name"
              :value="item.key"
            />
          </el-select>
          &nbsp;
          <el-button
            class="filter-item"
            type="primary"
            icon="el-icon-search"
            @click="searchContList"
            style="float: right"
          >
            搜索
          </el-button>
        </div>
      </div>

      <!--
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
      <el-button class="filter-item" type="primary" icon="el-icon-search" @click="searchContList" style="float:left">
        搜索
      </el-button>
    </div>
        -->

      <br />
      <!--
    <div>
      <el-table :data="table_Data" :header-cell-style="{ textAlign: 'center' }" :cell-style="{ 'text-align': 'center' }"
        default-sort="{ prop: 'create_time', order: 'descending' }">
        <el-table-column prop="project_name" label="项目名称" width="100">
        </el-table-column>
        <el-table-column prop="project_introduction" label="项目简介" width="350">
        </el-table-column>
        <el-table-column prop="project_progress" label="项目当前进度" width="150">
        </el-table-column>
        <el-table-column prop="create_time" label="发布时间" sortable width="150">
        </el-table-column>
        <el-table-column prop="project_status" label="是否有组队需求" width="150">
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button size="mini" @click="handleLook(scope.$index, scope.row)">查看详情</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
    -->

      <template>
        <div>
          <el-row :glutter="10">
            <el-col
              :span="6"
              v-for="(o, index) in tabledata"
              :key="index"
              :offset="2"
            >
              <div style="margin-top: 15px">
                <!--这一层div的作用是什么-->
                <el-card :body-style="{ padding: '0px' }" shadow="hover">
                  <img :src="o.Project_Imgimg_path" class="image" />
                  <div class="text">
                    {{ o.project_name }}<br />
                    发起时间：{{ o.create_time }}<br />
                    项目进度：{{ o.project_progress }}<br />
                    当前是否有组队需求：{{ o.project_status }}<br /><br />
                    <template>
                      <el-button
                        type="success"
                        plain
                        size="mini"
                        style="float: right"
                        @click="handleLook(index, o.row),addhistory(username,o.project_id)"
                      >
                        了解更多</el-button
                      >
                    </template>
                    <br />
                  </div>
                </el-card>
              </div>
              <br /><br />
            </el-col>
          </el-row>
        </div>
      </template>
    </el-main>
  </div>
</template>

<script>
import global_msg from "../../utils/global.js";
import { getlistInfor } from "@/api/Inforlist.js";
import { getrequireInfor } from "@/api/Inforlist.js";
import { openfile } from "@/api/file_load.js";
import { fetchList } from "@/api/Querylist.js";
import { addviewhistory } from "@/api/Addhistory";

const havreqOptions = [
  { key: "1", display_name: "是" },
  { key: "0", display_name: "否" },
];

export default {
  name: "InforList",
  data() {
    return {
      activeName: "项目列表",
      username: global_msg.nowuserid,
      table_Data: [],
      tabledata: [],

      /*
      以下供检索功能用
      */
      listQuery: {
        page: 1,
        //limit: 20,
        project_name: "",
        publisher: "",
        hav_require: "",
        audit_status: "",
      },
      havreqOptions,
    };
  },

  mounted() {
    this.getlist(), //页面一进入就加载表格
      this.gettable();
  },

  methods: {
    refresh() {
      location.reload();
    },

    gettable() {
      getlistInfor()
        .then((res) => {
          let vm = this;
          global_msg.projectnum = res.data.length; //改变全局requirenum
          //console.log(res);
          // console.log(res.data.length);
          // console.log(res.data[0]);
          // console.log(res.data[0].details);
          for (let item of res.data) {
            let form = {
              //设置添加数据的格式
              project_name: "",
              project_introduction: "",
              project_progress: "",
              create_time: "",
              project_status: "",
              project_id: "",
              Project_Imgimg_path: "",
            };

            form.project_name = item.project_name;
            form.project_introduction = item.project_introduction;
            form.project_progress = item.project_progress;
            form.create_time = item.create_time;
            form.create_time = form.create_time
              .replace('"', "")
              .replace('"', ""); //去掉时间格式的引号
            form.project_status = item.project_status;
            form.project_id = item.project_id;

            let param = {
              target: "project",
              id: form.project_id,
            };

            openfile(param).then((res) => {
              if (res.data) {
                form.Project_Imgimg_path = "data:;base64," + res.data;
              }
            });

            //console.log(form);

            vm.tabledata.push(form);
            // console.log(form);
            // console.log(vm.table_Data);
          }
        })
        .catch((res) => {
          console.log(res);
        });
    },

    handleLook(index, row) {
      //进入项目详情页面
      var project_id = this.tabledata[index].project_id;
      //console.log(index, row);
      this.$router.push({
        path: "/users/ProjectDetail",
        query: { p_id: project_id },
      });
    },

    getList2() {
      var query = JSON.stringify(this.listQuery);
      fetchList(query).then((response) => {
        //console.log(response);
        this.tabledata = response.data;
        for (let item of this.tabledata) {
          let param = {
            target: "project",
            id: item.project_id,
          };

          openfile(param).then((res) => {
            if (res.data) {
              item.Project_Imgimg_path = "data:;base64," + res.data;
            }
          });
        }
        //console.log(this.table_Data);
      });
    },

    /*
   handleLook(index, row) {//进入项目详情页面
 var project_id = this.table_Data[index].project_id;
 console.log(index, row);
 this.$router.push({ path: "/users/ProjectDetail", query: { p_id: project_id } });
},
*/

    /*搜索功能的完善*/
    searchContList() {
      this.listQuery.page = 1;
      this.getList2();
    },

    /*
    以下为旧版本里的methods,可能有些调用在html中被注释了
    */
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
      var vm = this; //全局变量
      getrequireInfor().then(function (res1) {
        global_msg.requirenum = res1.data.length; //改变全局projectnum
      });
      getlistInfor().then(function (res) {
        global_msg.projectnum = res.data.length; //改变全局requirenum
        // console.log(res);
        // console.log(res.data.length);
        // console.log(res.data[0]);
        // console.log(res.data[0].details);
        for (let item of res.data) {
          let form = {
            //设置添加数据的格式
            project_name: "",
            project_introduction: "",
            project_progress: "",
            create_time: "",
            project_status: "",
            project_id: "",
          };

          form.project_name = item.project_name;
          form.project_introduction = item.project_introduction;
          form.project_progress = item.project_progress;
          form.create_time = item.create_time;
          form.create_time = form.create_time.replace('"', "").replace('"', ""); //去掉时间格式的引号
          form.project_status = item.project_status;
          form.project_id = item.project_id;

          vm.table_Data.push(form);
          // console.log(form);
          // console.log(vm.table_Data);
        }
      });
    },

    /*触发添加浏览历史*/
    addhistory(user_id,proj_id){
      let params={
        user_id:user_id,
        project_id:proj_id
      };
      console.log(params);
      addviewhistory(params);
    }
  },
};
</script>

<style scoped>
#breadcrumb {
  margin-bottom: 10px;
}
</style>

<style>
.button {
  padding: 0;
  min-height: auto;
}

.image {
  width: 100%;
  display: block;
}

.text {
  padding: 16px;
  line-height: 30px;
}

/*
  这里的后两个是对于card自适应高度的一种可能可行的解决方案
  */
.el-row {
  margin-bottom: 20px;
  display: flex;
  flex-wrap: wrap;
}

.el-row .el-card {
  min-width: 100%;
  height: 100%;
  margin-right: 20px;
  transition: all 0.5s;
}
</style>
  
  <style scoped>
.time {
  font-size: 13px;
  color: #999;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
  padding: 0;
  float: right;
}

.image {
  width: 100%;
  display: block;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}

.clearfix:after {
  clear: both;
}

#body {
  position: relative;
  min-height: 100vh;
  height: 100%;
}
</style>