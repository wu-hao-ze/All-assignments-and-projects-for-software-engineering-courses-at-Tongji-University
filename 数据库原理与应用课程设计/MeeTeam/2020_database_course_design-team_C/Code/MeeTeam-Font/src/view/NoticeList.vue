<template>
  <div id="admin">
    <div id="header">
      <el-header>
        <span class="container" @click="goback">
          <div class="button-wrapper">
            <svg width="120" height="42">
              <rect class="rectangle" width="120" height="42" />
            </svg>
            <div class="btn">
              返回
            </div>
          </div>
        </span>公告管理
      </el-header>
    </div>
    <div id="body">
      <el-container>
        <el-aside width="200px">
          <el-menu default-active="/Admin/NoticeList" router>
            <el-menu-item index="/Admin/NoticeList">
              <i class="el-icon-s-order"></i>
              <span>通知总览</span>
            </el-menu-item>
            <!--此处为暂时的路径，等编辑页面做好时候再改-->
            <el-menu-item index="/Admin/CreateNotice">
              <i class="el-icon-plus"></i>
              <span>新建通知</span>
            </el-menu-item>
          </el-menu>
        </el-aside>
        <el-container>
          <el-main>
            <el-table :data="tableData" border @row-click="goto_ShowNotice">
              <el-table-column v-if="false" prop="notice_id" label="公告id">
              </el-table-column>
              <el-table-column prop="title" label="公告标题" align="left" style="margin: 50px;">
              </el-table-column>
              <el-table-column prop="admin_id" label="管理员" align="center">
              </el-table-column>
              <el-table-column prop="status" label="状态" align="center">
              </el-table-column>
              <el-table-column prop="operate_time" label="上次修改时间" align="center">
              </el-table-column>
            </el-table>
          </el-main>
        </el-container>
      </el-container>
    </div>
  </div>
</template>
 
<script>
import { get_all } from '@/api/notice.js'
export default {
  data() {
    this.tableData = [];
    get_all().then(res => {
      Object.keys(res.data).forEach(v => {
        let o = {};
        o.notice_id = res.data[v].notice_id;
        o.title = res.data[v].notice_title;
        o.admin_id = res.data[v].admin_id;
        o.operate_time = res.data[v].operate_time;
        if (res.data[v].operate_type == 0) {
          o.status = "暂存";
        }
        else {
          o.status = "发布";
        }
        console.log(o);
        this.tableData.push(o);
      });
    });
    return {};
  },
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/AdminPage" });
    },
    goto_ShowNotice(row) {
      this.$router.push({ path: "/Admin/ShowNotice", query: { n_id: row.notice_id } });
    }
  },
}
</script>

<style>
#admin {
  width: 100%;
  min-height: 100vh;
  background-color: #ecf0f5;
  background-size: 100% 100%;
}

.el-header {
  font-weight: 900;
  font-size: 24px;
  background-color: #B3C0D1;
  color: #333;
  text-align: center;
  line-height: 60px;
}

.el-aside {
  background-color: #D3DCE6;
  color: #333;
  text-align: center;
}

.el-container {
  background-color: #ecf0f5;
  color: #333;
  text-align: left;
  height: 100%;
}

.container {
  display: inline;
  float: left;
  display: inline-block;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 48px;
}

.button-wrapper {
  display: inline-block;
  position: relative;
  width: 60px;
  height: 30px;
  text-align: center;
}

.rectangle {
  stroke-width: 8px;
  stroke: #ecf0f5;
  fill: transparent;
  /* Core part of the animation */
  stroke-dasharray: 200 500;
  stroke-dashoffset: -372;
  /* 偏移负数，虚线整体右移动了372个单位 */
}

.btn {
  color: white;
  font-size: 24px;
  letter-spacing: 6px;
  position: relative;
  top: -72px;
  left: 30px;
}

@keyframes extend {
  to {
    stroke-dasharray: 600;
    /* 属性用于创建虚线： */
    stroke-dashoffset: 0;
    stroke-width: 4;
    /* 属性定义了一条线，文本或元素轮廓厚度： */
  }
}

.button-wrapper:hover .rectangle {
  animation: 0.5s extend linear forwards;
}
</style>