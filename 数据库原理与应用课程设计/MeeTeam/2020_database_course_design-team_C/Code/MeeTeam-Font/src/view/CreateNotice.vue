<template>
  <div id="edit">
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
        <el-form :model="FormData" ref="FormData">
          <el-form-item>
            <el-input type="text" v-model="FormData.title" placeholder="请输入公告标题"></el-input>
          </el-form-item>
          <el-form-item>
            <el-input type="text" v-model="FormData.content" placeholder="请输入公告内容"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="info" @click="onSubmit(0)">暂存公告</el-button>
            <el-button type="primary" @click="onSubmit(1)">发布公告</el-button>
          </el-form-item>
        </el-form>
      </el-main>
    </div>
  </div>
</template>

<script>
import { get_notice } from '@/api/notice.js'
import { create_notice } from '@/api/notice.js'
import global_msg from '../utils/global.js'

export default {
  data() {
    return {
      FormData: {
        title: '',
        content: ''
      }
    };
  },
  created() {
    this.get_NoticeInfo();
  },
  methods: {
    goback() {
      this.$router.push({ path: "/Admin/NoticeList" });
    },
    get_NoticeInfo() {
      if (this.$route.query.n_id) {
        let params = {
          id: this.$route.query.n_id
        };
        get_notice(params).then(res => {
          this.FormData.title = res.data.notice_title;
          this.FormData.content = res.data.notice_content;
        })
      }
    },
    onSubmit(para) {
      let o = {
        notice_id: '',
        notice_title: this.FormData.title,
        notice_content: this.FormData.content,
        admin_id: global_msg.nowadminid,
        operate_type: para,
        operate_time: ''
      }
      var not = JSON.stringify(o);
      console.log(not);

      create_notice(not).then(function (res) {
        if (!res) {
          alert("公告修改失败！");
        }
        else {
          if (para == 0) {
            alert("公告暂存成功！");
          }
          else {
            alert("公告发布成功！");
          }

        }
      })
    }
  }
}
</script>

<style>
#edit {
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