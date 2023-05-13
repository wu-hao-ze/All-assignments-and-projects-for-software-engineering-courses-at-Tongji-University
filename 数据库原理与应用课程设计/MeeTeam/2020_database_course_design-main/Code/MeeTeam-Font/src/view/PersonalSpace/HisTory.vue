<template>

  <div class="block">
    <p1 class="head">以下内容为个人浏览记录</p1>
    <br><br><br><br>
    <el-timeline v-for="o in datalist" :key="o.user_id">
      <el-timeline-item placement="top" class="item" color='#409EFF' type="primary">
        <el-card>
          <div class="content">

            <span class="text" style="color:orange">浏览项目id:  {{ o.proj_id }}</span>
            <br><br><br>
            <span class="text" style="color:green">浏览时间: {{ o.browse_time }}</span>

          </div>
        </el-card>
      </el-timeline-item>
      <!--
<el-timeline-item timestamp="2018/4/3" placement="top">
      <el-card>
        <h4>更新 Github 模板</h4>
        <p>王小虎 提交于 2018/4/3 20:46</p>
      </el-card>
    </el-timeline-item>
    <el-timeline-item timestamp="2018/4/2" placement="top">
      <el-card>
        <h4>更新 Github 模板</h4>
        <p>王小虎 提交于 2018/4/2 20:46</p>
      </el-card>
    </el-timeline-item>
    -->
    </el-timeline>
  </div>

</template>

<script>
import { gethistory } from '@/api/GetHistory';
import global_msg from "../../utils/global.js";

export default {
  data() {
    return {
      datalist: [],
      user_id: global_msg.nowuserid
    }
  },

  created() {
    this.getlist()
  },

  methods: {
    getlist() {
      let params = {
        user_id: this.user_id
      };
      //console.log(params);
      gethistory(params).then((res) => {
        console.log(res);
        let vm = this;
        for (let item of res.data) {
          let form = {
            user_id: "",
            proj_id: "",
            browse_time: ""
          };
          form.user_id = item.user_id;
          form.browse_time = item.browse_time;
          form.proj_id = item.project_id;

          vm.datalist.push(form);
        }
      })
      //.catch(console.log("something wrong"));

      console.log(this.datalist);
    }
  }
}
</script>

<style>
.block {
  margin-left: 10%;
}

.head {
  background-color:lightgreen;
  float: left;
  font-size: 30px;
}

.item {
  color: #ffffff;
  border-left: 2px solid #fff;
  margin-left: 25px;
  font-size: 15px;
}

.el-card {
  border-radius: 15px;
  width: 50%;
  height: 150px;
}

.el-card:hover {
  margin-right: -10px;
  margin: 30px;
}

.content {
  margin-bottom: 5px;
}

.text{
  font-size: larger;
  margin-top: 10px;
}
</style>