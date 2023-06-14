<template>
  <el-container class="layout-container">
    <el-aside class="aside" width="auto"
      ><AsideBar class="aside-menu" :is-collapse="isCollapse" />
    </el-aside>
    <el-container>
      <HeadBar @changeCollapse="changeCollapse" />
      <el-main style="height: 100px" class="main"><router-view /></el-main>
      <el-footer style="height: 20px; font-size: 12px; text-align: center">
        软件测试平时作业练习平台</el-footer
      >
    </el-container>
  </el-container>
</template>

<script>
import AsideBar from './components/asidebar'
import HeadBar from './components/headbar'
export default {
  name: 'LayoutIndex',
  components: {
    AsideBar,
    HeadBar,
  },
  props: {},
  data() {
    return {
      isCollapse: false,
    }
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {
    changeCollapse() {
      this.isCollapse = !this.isCollapse
    },
  },
}
</script>

<style scoped lang="less">
.layout-container {
  position: fixed;
  left: 0;
  right: 0;
  top: 0;
  bottom: 0;
}
.aside {
  background-color: #d3dce6;
  .aside-menu {
    height: 100%;
  }
}
.main {
  background-color: #e9eef3;
}
</style>
