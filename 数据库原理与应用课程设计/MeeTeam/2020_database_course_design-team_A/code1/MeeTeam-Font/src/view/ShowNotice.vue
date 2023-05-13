<template>
  <div id="show">
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
    <div id="main_page">
      <el-main>
        <div id="app">
          <h2 v-text="title"></h2>
          <span v-text="content"></span>
        </div>
        <el-button type="text" @click="goto_create_notice()">编辑公告</el-button>
      </el-main>
    </div>
  </div>
</template>
 
<script>
/* global Vue */
import { get_notice } from '@/api/notice.js'
let not = {
  notice_title: '',
  notice_content: ''
};
export default {
  data() {
    return {
      title: '',
      content: ''
    }
  },
  created() {
    this.get_NoticeInfo();
  },
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/NoticeList" });
    },
    goto_create_notice() {
      var n_id = this.$route.query.n_id;
      this.$router.push({ path: "/Admin/CreateNotice", query: { n_id: n_id } });
    },
    get_NoticeInfo() {
      let para = {
        id: this.$route.query.n_id
      };
      get_notice(para).then(res => {
        this.title = res.data.notice_title;
        this.content = res.data.notice_content;
      })
    }
  }
}
</script>

<style>
#show {
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