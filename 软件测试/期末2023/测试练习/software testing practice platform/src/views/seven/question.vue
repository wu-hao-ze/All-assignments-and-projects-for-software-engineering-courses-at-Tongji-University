<template>
    <div>
        <span style="font-size:24px;"><b>&nbsp;实时C/S问题</b></span>
      <div style="font-size:16px;">
        
      <p >某实时的C/S系统，服务器端应用程序需要接受客户端发送的不同类型的数据包，为了使系统设计达到最优，使用统一接口，接口（通用包）的格式为：</p>
      <img src="./seven.png"/>
       <p> 包中各段的解释如下:</p>
<p>A：用来确定不同的包类型共有4个bit位，该系统有10种包类型，分别用0000、0001、0010、0011、0100、0101、0110、0111、1000、1001来表示；</p>
<p>B：表示本发送包的内容字节数，共64个bit位；</p>
<p>C：表示业务种类，表示“现金查询”，“支票查询”，“存款”，“取款”；</p>
<p>D：是在100到999之间的数值；</p>
<p>E：是在200和500之间或在600到900之间的数值；；</p>
<p>F：输入条件是六位字符串；</p>
<p>G：为保留的256个bits。</p>
<p>用等价类测试的思想，如何划分等价类？并指出强一般等价类用例的个数。</p>
  
  <p style="margin-top:100px;font-size:20px;">符合要求的测试用例如下：</p>
</div>

<p>A={0000，0001，0010，0011、0100，0101，0110，0111，1000，1001，其他值}</p>
<p>B={0，0<x<=64，x>64}</p>
<p>C={现金查询，支票查询，存款，取款}</p>
<p>D={x<100，100<=x<=999，x>999}</p>
<p>E={x<200，200<=x<=500，500<x<600，600<=x<=900，x>900}</p>
<p>F={六位字符串，非六位字符串}</p>
<p>G={256bits，非256bits}</p>
<p>强一般等价类求笛卡尔积，共7290个</p>


    </div>



</template>

<script>
	export default {
		name: "SalesQuestion",
		components: {},
		props: {},
		data() {
			return {
				questionData: [
					{
						minute: "0",
						times: 0,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 3,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 18,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 20,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 22,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "1",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "2",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "4",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "5",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Min-",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Min",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Little",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Large",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Max",
					},
					{
						minute: "0",
						times: 10,
						level: "3",
						amount: "Max+",
					},
					{
						minute: "27",
						times: 0,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 3,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 18,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 20,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 22,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "1",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "2",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "4",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "5",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Min-",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Min",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Little",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Middle",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Large",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Max",
					},
					{
						minute: "27",
						times: 10,
						level: "3",
						amount: "Max+",
					},
				],
			};
		},
		computed: {},
		watch: {},
		created() {},
		mounted() {},
		methods: {},
	};
</script>

<style scoped lang="less">
</style>