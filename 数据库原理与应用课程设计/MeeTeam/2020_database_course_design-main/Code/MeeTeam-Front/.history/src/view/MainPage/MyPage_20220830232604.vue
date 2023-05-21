<template>
    <div id="user_page">
        <div id="nav">
            <img src="../../assets/logo.png" id="logo" height="40px" />
            <span id="title">|</span>
            <span id="title"> 个人信息 </span>
            <div id="user_id">
                <el-dropdown @command="handleCommand">
                    <span class="el-dropdown-link">
                        {{  user_id  }}
                    </span>
                    <el-dropdown-menu slot="dropdown">
                        <el-dropdown-item icon="el-icon-user" command="a"> 个人中心 </el-dropdown-item>
                        <el-dropdown-item icon="el-icon-switch-button" command="b">退出登陆</el-dropdown-item>
                    </el-dropdown-menu>
                </el-dropdown>
            </div>
            <div>
                <el-avatar shape="square" :size="100" :fit="fit" :src="base64"></el-avatar>
            </div>
        </div>
        <div id="body">
            <div id="side_menu">
                <el-row class="tac">
                    <el-col>
                        <el-menu default-active="2" class="el-menu-vertical-demo" background-color="#545c64"
                            text-color="#fff" active-text-color="#ffd04b" :router="true">
                            <el-menu-item index="/users/UserPage">
                                <i class="el-icon-s-home"></i>首页
                            </el-menu-item>
                            <el-menu-item index="/users/UserInformation">
                                <i class="el-icon-user"></i>个人信息
                            </el-menu-item>
                            <!-- <el-menu-item @click="goAnchor('#anchor1')">
                                <i class="el-icon-star-off"></i>我的收藏
                            </el-menu-item>
                            <el-menu-item index="/users/UserHistory">
                                <i class="el-icon-time"></i>历史记录
                            </el-menu-item> -->
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
import global_msg from '../../utils/global.js'
export default {
    data() {
        return {
            user_id: ''
        }
    },
    created() {
        this.get_user();
    },
    methods: {
        get_user() {
            this.user_id = global_msg.nowuserid;
        },
        handleCommand(command) {
            if (command == 'a') {
                this.$router.push({ path: "/users/UserInformation" });
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
    margin-left: 70%;
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
</style>

