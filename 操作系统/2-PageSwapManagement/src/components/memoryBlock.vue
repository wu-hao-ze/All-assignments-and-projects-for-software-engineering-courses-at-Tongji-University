<template>
  <!--需要10个内存块-->

  <el-space wrap size="mini" :spacer="spacer">
    <el-card class="box-card" style="width: 150px">
      <template #header>
        <div class="card-header">
          <span v-if="appears">第{{ page }}页</span>
        </div>
      </template>
      <span v-if="firstAppear" style="color: #b3b5bb">空</span>
      <div v-for="nx in 10" :key="nx">
        <transition
          name="el-fade-in"
          :ref="
            el => {
              if (el) box[nx - 1] = el
            }
          "
        >
          <div
            class="transition-box"
            :style="'background-color: ' + colorNow"
            :ref="
              el => {
                if (el) divs[nx - 1] = el
              }
            "
            v-show="appears"
          >
            {{ nx - 1 }}
          </div>
        </transition>
      </div>
    </el-card>
  </el-space>
</template>

<script>
import { h } from 'vue'
import { ElDivider } from 'element-plus'
export default {
  name: 'memoryBlock',
  data: function() {
    return {
      appears: false,
      page: 0,
      divs: [],
      show: true,
      box: [],
      colorNow: '#6db2fa',
      redcolor: '#d43346',
      curcolor: '#6db2fa',
      spacer: h(ElDivider, { direction: 'vertical' }),
      firstAppear: true,
    }
  },
  watch: {},
  methods: {
    appear() {
      this.appears = !this.appears
      this.firstAppear = false
    },

    move(index) {
      let block = this.box[index].style.backgroundColor
      this.box[index].style.backgroundColor = this.redcolor

      var that = this
      setTimeout(() => {
        that.box[index].style.backgroundColor = that.colorNow
      }, 500)

      console.log(block)
    },
    state() {
      return this.appears
    },
    disapppear() {
      this.appears = false
    },
  },
}
</script>

<style scoped>
.transition-box {
  width: 100px;
  height: 20px;

  text-align: center;
  color: #fff;
  border: 1px solid #b3b5bb;
  border-radius: 4px;
}

.el-col {
  border-radius: 4px;
}
</style>
