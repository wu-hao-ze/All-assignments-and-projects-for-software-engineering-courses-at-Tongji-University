<template>
    <div>
        <span style="font-size:24px;"><b>&nbsp;ATM系统</b></span>
      <div style="font-size:16px;">
        
      <p >构建ATM系统的state transition diagram, 利用state transition testing方法将其转换成transition tree，而后基于transition tree的路径设计测试用例，要考虑robustness。</p>

  
  <p style="margin-top:100px;font-size:20px;">状态图：</p>
  <img style="width:60%;height:60%;" src="./1.png"/>
   <p style="margin-top:100px;font-size:20px;">Transition Tree：</p>
   <img style="width:60%;height:60%;" src="./2.png"/>
   <p style="margin-top:100px;font-size:20px;">用例设计 ：</p>
        <img style="width:60%;height:60%;" src="./3.png"/>
</div>




    </div>



</template>

<script>

export default {
  name: "SalesQuestion",
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

  },
};
</script>

<style scoped lang="less">


</style>