<template>
  <el-table
    :data="tableData"
    border
    style="width: 100%">
    <el-table-column
      prop="date"
      label="日期"
      width="180">
    </el-table-column>
    <el-table-column
      prop="name"
      label="姓名"
      width="180">
    </el-table-column>
    <el-table-column
      prop="address"
      label="项目名称">
    </el-table-column>
  </el-table>
</template>

<script>
export default {
name: 'InforList',
}
</script>

<style>

</style>