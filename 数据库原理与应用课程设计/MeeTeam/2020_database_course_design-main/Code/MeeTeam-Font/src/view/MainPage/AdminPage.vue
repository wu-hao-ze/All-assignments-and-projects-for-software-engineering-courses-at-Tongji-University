<template>
  <div id="admin_page">
    <div id="nav">
      <img src="../../assets/logo.png" id="logo" height="40px" />
      <span id="title">|</span>
      <span id="title">平台管理</span>
      <div id="admin_id">
        <el-dropdown @command="handleCommand">
          <span class="el-dropdown-link">
            {{ admin_id }}
          </span>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item icon="el-icon-user" command="a">
              个人中心
            </el-dropdown-item>
            <el-dropdown-item icon="el-icon-switch-button" command="b"
              >退出登陆</el-dropdown-item
            >
          </el-dropdown-menu>
        </el-dropdown>
      </div>
    </div>
    <div id="body">
      <div id="side_menu">
        <el-row class="tac">
          <el-col>
            <el-menu
              default-active="2"
              class="el-menu-vertical-demo"
              background-color="#545c64"
              text-color="#fff"
              active-text-color="#ffd04b"
              :router="true"
            >
              <el-menu-item index="/Admin/AdminPageContent">
                <i class="el-icon-s-home"></i>首页
              </el-menu-item>
              <el-submenu index="1">
                <template slot="title">
                  <i class="el-icon-s-check"></i>
                  <span>审核管理</span>
                </template>
                <el-menu-item-group>
                  <el-menu-item index="/Admin/AuditProject">项目审核</el-menu-item>
                </el-menu-item-group>
              </el-submenu>
              <el-submenu index="2">
                <template slot="title">
                  <i class="el-icon-notebook-2"></i>
                  <span>通知管理</span>
                </template>
                <el-menu-item-group>
                  <el-menu-item index="/Admin/NoticeList">通知总览</el-menu-item>
                  <el-menu-item index="/Admin/CreateNotice">新建通知</el-menu-item>
                </el-menu-item-group>
              </el-submenu>
              <el-menu-item index="/Admin/ManageUser">
                <i class="el-icon-user-solid"></i>用户管理
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
import AdminPage from "./AdminPage.vue";
import NoticeList from "../Notice/NoticeList.vue";
import global_msg from "../../utils/global.js";
export default {
  data() {
    return {
      admin_id: "",
    };
  },
  created() {
    this.get_admin();
  },
  methods: {
    get_admin() {
      this.admin_id = global_msg.nowadminid;
    },
    handleCommand(command) {
      if (command == "a") {
        this.$router.push({ path: "/Admin/AdminInformation" });
      }
      if (command == "b") {
        this.$confirm("确定退出登陆吗", "提示", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning",
        })
          .then(() => {
            this.$message({
              type: "success",
              message: "退出成功",
            });
            this.$router.push({ path: "/login" }); //接下来进入到哪个路由
          })
          .catch(() => {
            this.$message({
              type: "info",
              message: "退出取消",
            });
          });
      }
    },
  },
};
</script>

<style scoped>
#admin_page {
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

#admin_id {
  margin-left: 73%;
}

.el-dropdown-link {
  cursor: pointer;
  color: white;
  font-size: 20px;
}

#admin_page {
  /*background-color: #ecf0f5;*/
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
