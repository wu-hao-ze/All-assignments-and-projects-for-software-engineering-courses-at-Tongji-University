<template>
  <el-table
    :data="tableData"
    border
    style="width: 100%">
    <el-table-column
      prop="date"
      label="日22期"
      width="180">
    </el-table-column>
    <el-table-column
      prop="name"
      label="姓2名"
      width="180">
    </el-table-column>
    <el-table-column
      prop="address"
      label="项目2名称"
      width="200">
      </el-table-column>
  </el-table>
</template>

<script>
export default {
  name: 'PostInfor',
}
</script>

<style>

</style>