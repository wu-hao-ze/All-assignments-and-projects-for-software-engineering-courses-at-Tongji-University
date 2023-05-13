<template>
    <div id="user_page">
        <div id="nav">
            <img src="../../assets/logo.png" id="logo" height="40px" />
            <span id="title">|</span>
            <span id="title">大学生组队信息平台</span>
            <div id="user_id">
                <el-link type="info" @click="goto_userInfo()" style="color:white;font-size: 20px;">
                    <span v-text="user_id"></span>
                </el-link>
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
                            <el-submenu index="1">
                                <template slot="title">
                                    <i class="el-icon-s-custom"></i>
                                    <span>项目管理</span>
                                </template>
                                <el-menu-item-group>
                                    <el-menu-item index="1-1">我的项目</el-menu-item>
                                    <el-menu-item index="1-2">新建项目</el-menu-item>
                                </el-menu-item-group>
                            </el-submenu>
                            <el-menu-item index="/users/NoticeList">
                                <i class="el-icon-s-home"></i>平台通知
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
        goto_userInfo() {
            this.$router.push({ path: "/users/UserInformation" });
        }
    }
}
</script>

<style>
#user_page {
    caret-color: transparent;
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
    margin-left: 65%;
}


.el-link {
    font-size: 20px;
}

#user_page {
    background-color: #ecf0f5;
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

