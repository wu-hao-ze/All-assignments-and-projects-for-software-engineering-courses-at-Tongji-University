<template>
    <div id="admin_navigate">
        <el-button type="primary" @click="goto_NoticeList()">管理公告</el-button>
    </div>
</template>

<script>
export default {
    name: 'AdminPage',
    methods: {
        goto_NoticeList() {
            this.$router.push({ path: "/Admin/NoticeList" });
        }
    }
}
</script>

<style>

</style>