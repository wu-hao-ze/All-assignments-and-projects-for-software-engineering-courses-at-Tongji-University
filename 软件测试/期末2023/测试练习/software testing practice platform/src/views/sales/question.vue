<template>
    <div>
        <span style="font-size:24px;"><b>&nbsp;电脑销售系统</b></span>
      <div style="font-size:16px;">
        
      <p >电脑销售市场中，不同种硬件有不同的售价，销售员的佣金与他们的销售额挂钩，现研究电脑销售系统，相关描述如下：</p>
       <p> 三种硬件的价格和库存：	主机（25￥单位价格，每月最多销售的数量为70），显示器（30￥单位价格，每月最多销售数量为80），外设（45￥单位价格，每月最多销售的数量为90）</p>
<p>其他规定：	每个销售员每月至少销售一台完整的机器，当系统的主机这个变量接受到-1值的时候，系统自动统计该销售员本月的销售总额。</p>
<p>佣金计算方式：	当销售额小于等于1000（包括1000）按照10%提佣金，当销售额在1000-1800之间（包括1800）的时候按照15%提佣金，当销售额大于1800时按照20%提佣金。</p>
  
</div>

    </div>

</template>

<script>

export default {
  name: "SalesQuestion",
  components: {},
  props: {},
  data() {
    return { }
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {

  },
};
</script>

<style scoped lang="less">


</style>