<template>
  <div>
    <span style="font-size: 24px"><b>&nbsp;电信收费问题</b></span>
    <div style="font-size: 16px">
      <p>研究一个与我们的生活息息相关的电信收费问题系统，需求描述如下：</p>
      <p>
        A.
        <b>每月的电话总费用=基本月租费+折扣后的实际的通话费</b
        >，如果没有折扣则按实际通话费计算，基本月租费为25元，每分钟通话费为0.15元。
      </p>
      <p>
        B. 实际通话费是否有折扣与当月的通话时间（分钟）和本年度至本月的累计未按时缴费的次数有关。
      </p>
      <p>
        C.
        当月的通话分钟数和折扣比例及本年度未按时缴费次数之间有直接的对应关系，如果本年度的未按时缴费的次数超过本月通话时间所对应的容许值则免于折扣，并按实际的通话费计算。
      </p>
      <p>D. 通话时间和折扣比例及未按时缴费次数的关系为：</p>
      <el-table :data="questionData" style="width: 100%; height: 1500px; font-size: 16px">
        <el-table-column prop="minute" label="本月通话的分钟数" width="400" align="center">
        </el-table-column>
        <el-table-column
          prop="times"
          label="通话时间段的最大容许不按时缴费次数"
          width="400"
          align="center">
        </el-table-column>
        <el-table-column prop="discount" label="通话时间段的折扣率" align="center">
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CashQuestion',
  components: {},
  props: {},
  data() {
    return {
      questionData: [
        {
          minute: '0<通话时间≤60',
          times: 1,
          discount: '1.0%',
        },
        {
          minute: '60<通话时间≤120',
          times: 2,
          discount: '1.5%',
        },
        {
          minute: '120<通话时间≤180',
          times: 3,
          discount: '2.0%',
        },
        {
          minute: '180<通话时间≤300',
          times: 3,
          discount: '2.5%',
        },
        {
          minute: '通话时间>300',
          times: 6,
          discount: '3.0%',
        },
      ],
    }
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {},
}
</script>

<style scoped lang="less"></style>
