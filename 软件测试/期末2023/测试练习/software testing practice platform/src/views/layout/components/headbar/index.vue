<template>
  <el-header class="header">
    <div>
      <i class="el-icon-s-fold" @click="changeCollapse" style="margin-right: 20px"></i>
      <span>软件测试平台</span>
    </div>

    <el-dropdown>
      <div class="avatar-bar">
        <span style="margin-right: 20px">whz</span>
      </div>
    </el-dropdown>
  </el-header>
</template>

<script>
export default {
  name: 'HeadBar',
  components: {},
  props: {},
  data() {
    return {}
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {
    changeCollapse() {
      this.$emit('changeCollapse', 0)
    },
  },
}
</script>

<style scoped lang="less">
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.avatar-bar {
  display: flex;
  justify-content: center;
  align-items: center;
  .avatar {
    margin-right: 10px;
  }
}
</style>
