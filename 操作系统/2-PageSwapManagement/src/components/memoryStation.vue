<!--内存状态-->

<template>
  <div>
    <h4>内存中的页面图示</h4>
    <!-- 4个内存块 -->
    <el-row>
      <el-col v-for="nx in 4" :key="nx" :span="0">
        <memory-block
          :ref="
            el => {
              if (el) divs[nx - 1] = el
            }
          "
        ></memory-block>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import MemoryBlock from '@/components/memoryBlock'
export default {
  name: 'memoryStation',
  components: { MemoryBlock },
  data: () => ({
    divs: [],
    empty: true,
  }),
  methods: {
    memoryAppear(index) {
      this.empty = false
      this.divs[index].appear()
      this.divs[index].page = index
      return
    },
    changePage(index, newPage) {
      this.divs[index].page = newPage

      //隐藏再出现

      this.divs[index].disapppear()
      let that = this
      setTimeout(() => {
        that.divs[index].appear()
      }, 300)
    },
    //动画块移动
    move(index, num) {
      this.divs[index].move(num)
    },
    blockEmpty() {
      for (let i = 0; i < this.divs.length; ++i) {
        this.divs[i].disapppear()
        this.divs[i].firstAppear = true
      }
      this.empty = true
    },
  },
}
</script>

<style scoped></style>
