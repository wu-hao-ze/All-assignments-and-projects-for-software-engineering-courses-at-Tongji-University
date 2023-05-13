<template>
    <el-menu :default-active="activeIndex" class="el-menu-demo" mode="horizontal" @select="handleSelect"
        background-color="#545c64" text-color="#fff" active-text-color="#ffd04b">>
        <el-submenu index="1">
            <template slot="title">项目内容</template>
            <el-menu-item index="1-1">需求列表</el-menu-item>
            <el-menu-item index="1-2">添加需求</el-menu-item>
        </el-submenu>
        <el-submenu index="2">
            <template slot="title">我的工作台</template>
            <el-menu-item index="2-1">我的项目</el-menu-item>
        </el-submenu>
        <span style="margin-right:35px;color:white" @click="personal">{{ username }}</span>
    </el-menu>
</template>


<script>
import global_msg from '@/utils/global.js'
export default {
    name: 'navTop',
    data() {
        return {
            username: global_msg.nowuserid,
            activeIndex: '1-1',
        };
    },
    methods: {
        handleSelect(key, keyPath) {
            if (key === "1-1")
                this.$router.push({ path: "/users/InforList" });
            if (key === "1-2")
                this.$router.push({ path: "/users/CreatList" });
            if (key === "2-1")
                this.$router.push({ path: "/users/MyProject" });
        },
        personal() {
            this.$router.push({ path: "/users/UserInformation" });
        }
    }
}
</script>


<style scoped>
.el-menu-demo {
    text-align: right;
    font-size: 20px;
    height: 60px;
    line-height: 60px;
}
</style>