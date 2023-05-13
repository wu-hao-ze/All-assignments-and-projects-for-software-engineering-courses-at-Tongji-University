<template>
  <div class="block">
    <p1 class="head">以下内容为个人浏览记录</p1>
    <br /><br /><br /><br />
    <el-timeline v-for="o in datalist" :key="o.user_id">
      <el-timeline-item placement="top" class="item" color="#409EFF" type="primary">
        <div id="card">
          <el-card>
            <div class="content" @click="goto_detail(o)">
              <span class="text" style="color:orange">浏览项目: {{ o.proj_name }}</span>
              <br />
              <br />
              <br />
              <span class="text" style="color:green">浏览时间: {{ o.browse_time }}</span>
            </div>
          </el-card>
        </div>
      </el-timeline-item>
    </el-timeline>
  </div>
</template>

<script>
import { gethistory } from '@/api/GetHistory';
import { get_project } from '@/api/ProjectDetail.js'
export default {
  data() {
    return {
      datalist: [],
      user_id: this.$route.query.id
    }
  },

  created() {
    this.getlist()
  },

  methods: {
    goto_detail(o) {
      var project_id = o.proj_id;
      this.$router.push({
        path: "/users/ProjectDetail",
        query: { p_id: project_id, id: this.$route.query.id },
      });
    },
    getlist() {
      let params = {
        user_id: this.user_id
      };
      //console.log(params);
      gethistory(params).then((res) => {
        //console.log(res);
        let vm = this;
        for (let item of res.data) {
          let form = {
            user_id: "",
            proj_id: "",
            proj_name: "",
            browse_time: ""
          };
          let para = {
            id: item.project_id
          };
          get_project(para).then(res => {
            if (res.data) {
              form.proj_name = res.data.project_name;
              form.user_id = item.user_id;
              form.browse_time = item.browse_time;
              form.proj_id = item.project_id;
              vm.datalist.push(form);
            }
          });
        }
      })
      //console.log(this.datalist);
    }
  }
}
</script>


<style>
.block {
  margin-left: 10%;
}

.head {
  background-color: lightgreen;
  float: left;
  font-size: 30px;
}

.item {
  color: #ffffff;
  border-left: 2px solid #fff;
  margin-left: 25px;
  font-size: 15px;
}

.content {
  margin-bottom: 5px;
}

#card {
  border-radius: 15px;
  width: 50%;
  height: 150px;
}

#card:hover {
  margin-right: -10px;
  margin: 30px;
}

.text {
  font-size: larger;
  margin-top: 10px;
}
</style>