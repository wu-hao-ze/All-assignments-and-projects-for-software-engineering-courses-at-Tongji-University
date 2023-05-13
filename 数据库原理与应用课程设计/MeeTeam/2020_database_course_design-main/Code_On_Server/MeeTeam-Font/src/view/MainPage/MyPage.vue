<template>
  <div id="user_page">
    <div id="nav">
      <img src="../../assets/logo.png" id="logo" height="40px" />
      <span id="title">|</span>
      <span id="title">个人信息</span>
      <div id="user_id">
        <el-dropdown @command="handleCommand">
          <span class="el-dropdown-link">{{ user_id }}</span>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item icon="el-icon-user" command="a">个人中心</el-dropdown-item>
            <el-dropdown-item icon="el-icon-switch-button" command="b">退出登陆</el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </div>
      <span>
        <el-avatar size="20" :fit="fit" :src="base64"></el-avatar>
      </span>
    </div>
    <div id="body">
      <div id="side_menu">
        <el-row class="tac">
          <el-col>
            <el-menu default-active="2" class="el-menu-vertical-demo" background-color="#545c64" text-color="#fff"
              active-text-color="#ffd04b" :router="true">
              <el-menu-item @click="goto_userpagecontent">
                <i class="el-icon-s-home"></i>首页
              </el-menu-item>
              <el-menu-item @click="goto_userinfo">
                <i class="el-icon-user"></i>个人信息
              </el-menu-item>
              <el-menu-item @click="goto_history">
                <i class="el-icon-time"></i>历史记录
              </el-menu-item>
            </el-menu>
          </el-col>
        </el-row>
      </div>
      <div id="main">
        <el-main>
          <router-view></router-view>
        </el-main>
      </div>
    </div>
  </div>
</template>

<script>
import { openfile } from "@/api/file_load.js";
export default {
  data() {
    return {
      user_id: '',
      base64: ''
    }
  },

  created() {
    this.get_user();
  },
  
  methods: {
    goto_userpagecontent() {
      this.$router.push({ path: "/users/UserPageContent", query: { id: this.user_id } });
    },
    goto_userinfo() {
      this.$router.push({ path: "/users/UserInformation", query: { id: this.user_id } });
    },
    goto_history() {
      this.$router.push({ path: "/users/UserHistory", query: { id: this.user_id } });
    },
    get_user() {
      this.user_id = this.$route.query.id;
      let params = {
        target: 'user',
        id: this.$route.query.id
      };
      openfile(params).then((res) => {
        this.base64 = 'data:;base64,' + res.data;
      });
    },
    handleCommand(command) {
      if (command == 'a') {
        this.$router.push({ path: "/users/UserInformation", query: { id: this.user_id } });
      }
      if (command == 'b') {
        this.$confirm('确定退出登陆吗', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          this.$message({
            type: 'success',
            message: '退出成功'
          });
          this.$router.push({ path: "/login" }); //接下来进入到哪个路由
        }).catch(() => {
          this.$message({
            type: 'info',
            message: '退出取消'
          });
        });
      }
    },
  }
}
</script>

<style scoped>
#user_page {
  caret-color: transparent;
  background-image: url("@/assets/bg3.jpg");
}

#nav {
  background-color: black;
  height: 60px;
  width: 100%;
  display: flex;
  padding-left: 0;
  align-items: center;
  color: white;
  font-size: 20px;
}

#user_id {
  margin-left: 68%;
  margin-right: 10px;
}

.el-dropdown-link {
  cursor: pointer;
  color: white;
  font-size: 20px;
}

#user_page {
  background-color: white;
  background-size: 100% 100%;
}

#logo {
  margin-left: 20px;
}

#title {
  margin-left: 20px;
}

#side_menu {
  position: absolute;
  width: 15%;
  margin-top: 0px;
  height: 100%;
  background-color: #545c64;
}

#main {
  background-color: rgba(255, 255, 255, 0.7);
}
</style>

