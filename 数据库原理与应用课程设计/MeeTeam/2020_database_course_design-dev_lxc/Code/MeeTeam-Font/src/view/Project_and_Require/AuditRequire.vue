<template>
    <div id="AuditRequire"> 
        <div id="body">
            <el-main>
                <div id="breadcrumb">
                    <el-breadcrumb separator-class="el-icon-arrow-right">
                        <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }">首页</el-breadcrumb-item>
                        <el-breadcrumb-item>审核管理</el-breadcrumb-item>
                        <el-breadcrumb-item>需求审核</el-breadcrumb-item>
                    </el-breadcrumb>
                </div>
            </el-main>
        </div>
    </div>
</template>

<script>
export default{
    data(){
        return{}
    }
}
</script>

<style>
</style>