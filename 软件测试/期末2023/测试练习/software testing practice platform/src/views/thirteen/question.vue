<template>
  <div>
    <span style="font-size: 24px"><b>&nbsp;销售系统</b></span>
    <div style="font-size: 16px">
      <p>
        一销售系统，如果销售员的年销售额大于200万RMB且请假天数不超过10天的情况下，现金到帐大于等于60%，则佣金（提成）系数为7，即佣金值为销售额除以佣金系数；现金到帐小于60%，佣金不予计算。所有其他情况且现金到帐小于等于85%，则按佣金系数均为6计算佣金，现金到账大于85%，佣金系数按5处理。
      </p>
      <p>
        根据题意设计流程图并设计测试用例实现白盒测试（White Box
        Test）的1）语句覆盖，2）判断覆盖，3）条件覆盖，4）判断—条件覆盖，5）条件组合覆盖（测试用例及覆盖表示要清晰）。
      </p>
    </div>
    <img src="./13.png" />
    <p style="margin-top: 50px">语句覆盖：</p>
    <img src="./1.png" />
    <p style="margin-top: 50px">判断覆盖：</p>
    <img src="./2.png" />
    <p style="margin-top: 50px">条件覆盖：</p>
    <img src="./3.png" />
    <img src="./31.png" />
    <p style="margin-top: 50px">判断——条件覆盖：</p>
    <img src="./4.png" />
    <p style="margin-top: 50px">条件——组合覆盖：</p>
    <img src="./5.png" />
    <img src="./51.png" />
  </div>
</template>

<script>
export default {
  name: 'SalesQuestion',
  components: {},
  props: {},
  data() {
    return {
      questionData: [
        {
          con: '销售额>2000000',
          t: 'T1',
          f: 'F1',
        },
        {
          con: '请假天数<=10 ',
          t: 'T2',
          f: 'F2',
        },
        {
          con: '现金到账<=0.85',
          t: 'T3',
          f: 'F3',
        },
        {
          con: '现金到账>=0.6',
          t: 'T4',
          f: 'F4',
        },
        {
          con: '销售额<0',
          t: 'T5',
          f: 'F5',
        },
        {
          con: '请假天数<0',
          t: 'T6',
          f: 'F6',
        },
        {
          con: '现金到账<0',
          t: 'T7',
          f: 'F7',
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
