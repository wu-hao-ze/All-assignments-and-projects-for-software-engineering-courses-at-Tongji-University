<!--
 * @Author: daidai
 * @Date: 2022-03-01 14:13:04
 * @LastEditors: Please set LastEditors
 * @LastEditTime: 2022-09-27 15:04:49
 * @FilePath: \web-pc\src\pages\big-screen\view\indexs\right-top.vue
-->
<template>
	<Echart
		id="rightTop"
		:options="option"
		class="right_top_inner"
		v-if="pageflag"
		ref="charts"
	/>
	<Reacquire v-else @onclick="getData" style="line-height: 200px">
		重新获取
	</Reacquire>
</template>

<script>
	import { currentGET } from "api/modules";
	import { graphic } from "echarts";
	import axios from "axios";
	import $ from "jquery";
	export default {
		data() {
			return {
				option: {},
				pageflag: false,
				timer: null,
				cityCode: 310000,
				statistic: [],
				dateList: [],
				numList: [],
				numList2: [],
			};
		},
		created() {},

		mounted() {
			//https://view.inews.qq.com/g2/getOnsInfo?name=disease_h5
			// /api/wuhan/app/data/list-total
			// /g2/getOnsInfo?name=disease_h5
			// this.getData();
			this.getData();
		},
		beforeDestroy() {
			this.clearData();
		},
		methods: {
			function(key) {
				var localStorage = window.localStorage;
				if (localStorage) var v = localStorage.getItem(key);
				if (!v) {
					return;
				}
				if (v.indexOf("obj-") === 0) {
					v = v.slice(4);
					return JSON.parse(v);
				} else if (v.indexOf("str-") === 0) {
					return v.slice(4);
				}
			},
			async getData() {
				this.pageflag = true;
				if (this.function("code") == "china") {
					this.cityCode = 310000;
				} else {
					this.cityCode = this.function("code");
				}
				const res = await axios.get("/newsqa/v1/query/pubished/daily/list", {
					params: {
						adCode: this.cityCode,
					},
				});
				if (res) {
					if (res.data.length > 30) {
						this.statistic = res.data.slice(-30);
					}
					// console.log(
					// 	"--------------------------截取历史数据看这里----：",
					// 	this.statistic
					// );
					this.dateList = [];
					this.numList = [];
					this.numList2 = [];
					for (var i = 0; i < this.statistic.length; i++) {
						this.dateList.push(this.statistic[i]._mtime.slice(5, 10));
						this.numList.push(this.statistic[i].confirm_add);
						this.numList2.push(this.statistic[i].newHeal);
					}
					// 加五天
					// console.log(this.statistic[29]._mtime.slice(5, 10)); // 12-17
					for (var i = 0; i < 5; i++) {
						this.dateList.push(
							this.statistic[29]._mtime.slice(5, 8) +
								(Number(this.statistic[29]._mtime.slice(8, 10)) + i + 1)
						);
					}
					// 预测新增
					var t1 = "";
					for (var i = 0; i < 30; i++) {
						t1 += this.numList[i];
						if (i < 29) {
							t1 += ",";
						}
					}
					// console.log(t1);
					let { data: res1 } = await axios.post("/predict/predict/" + t1);
					// console.log(res1.slice(1, -1).split(","));
					res1 = res1.slice(1, -1).split(",");
					res1.forEach(function (value, index) {
						res1[index] = Number(res1[index]);
						if (res1[index] < 0) {
							res1[index] = 0;
						}
					});
					// console.log(res1);
					this.numList = [...this.numList, ...res1];
					// console.log(this.numList);

					// 预测治愈
					var t2 = "";
					for (var i = 0; i < 30; i++) {
						t2 += this.numList2[i];
						if (i < 29) {
							t2 += ",";
						}
					}
					// console.log(t2);
					let { data: res2 } = await axios.post("/predict/predict/" + t2);
					console.log(res2.slice(1, -1).split(","));
					res2 = res2.slice(1, -1).split(",");
					res2.forEach(function (value, index) {
						res1[index] = Number(res2[index]);
						if (res2[index] < 0) {
							res2[index] = 0;
						}
					});
					this.numList2 = [...this.numList2, ...res2];
					// console.log(this.numList2);

					this.$nextTick(() => {
						// numList是新增，numList2是治愈
						this.init(this.dateList, this.numList, this.numList2), this.switper();
					});
				} else {
					this.pageflag = false;
					this.$Message({
						text: res.msg,
						type: "warning",
					});
				}
			},
			clearData() {
				if (this.timer) {
					clearInterval(this.timer);
					this.timer = null;
				}
			},
			//轮询
			switper() {
				if (this.timer) {
					return;
				}
				let looper = (a) => {
					this.getData();
				};
				this.timer = setInterval(looper, 2000);
				let myChart = this.$refs.charts.chart;
				myChart.on("mouseover", (params) => {
					this.clearData();
				});
				myChart.on("mouseout", (params) => {
					this.timer = setInterval(looper, 2000);
				});
			},
			init(xData, yData, yData2) {
				this.option = {
					xAxis: {
						type: "category",
						data: xData,
						boundaryGap: false, // 不留白，从原点开始
						splitLine: {
							show: true,
							lineStyle: {
								color: "rgba(31,99,163,.2)",
							},
						},
						axisLine: {
							// show:false,
							lineStyle: {
								color: "rgba(31,99,163,.1)",
							},
						},
						axisLabel: {
							color: "#7EB7FD",
							fontWeight: "500",
						},
					},
					yAxis: {
						type: "value",
						splitLine: {
							show: true,
							lineStyle: {
								color: "rgba(31,99,163,.2)",
							},
						},
						axisLine: {
							lineStyle: {
								color: "rgba(31,99,163,.1)",
							},
						},
						axisLabel: {
							color: "#7EB7FD",
							fontWeight: "500",
						},
					},
					tooltip: {
						trigger: "axis",
						backgroundColor: "rgba(0,0,0,.6)",
						borderColor: "rgba(147, 235, 248, .8)",
						textStyle: {
							color: "#FFF",
						},
					},
					grid: {
						//布局
						show: true,
						left: "10px",
						right: "30px",
						bottom: "10px",
						top: "28px",
						containLabel: true,
						borderColor: "#1F63A3",
					},
					series: [
						{
							data: yData,
							type: "line",
							smooth: true,
							symbol: "none", //去除点
							name: "新增",
							color: "rgba(252,144,16,.7)",
							areaStyle: {
								//右，下，左，上
								color: new graphic.LinearGradient(
									0,
									0,
									0,
									1,
									[
										{
											offset: 0,
											color: "rgba(252,144,16,.7)",
										},
										{
											offset: 1,
											color: "rgba(252,144,16,.0)",
										},
									],
									false
								),
							},
							markPoint: {
								data: [
									{
										name: "最大值",
										type: "max",
										valueDim: "y",
										symbol: "rect",
										symbolSize: [60, 26],
										symbolOffset: [0, -20],
										itemStyle: {
											color: "rgba(0,0,0,0)",
										},
										label: {
											color: "#FC9010",
											backgroundColor: "rgba(252,144,16,0.1)",
											borderRadius: 6,
											padding: [7, 14],
											borderWidth: 0.5,
											borderColor: "rgba(252,144,16,.5)",
											formatter: "新增：{c}",
										},
									},
									{
										name: "最大值",
										type: "max",
										valueDim: "y",
										symbol: "circle",
										symbolSize: 6,
										itemStyle: {
											color: "#FC9010",
											shadowColor: "#FC9010",
											shadowBlur: 8,
										},
										label: {
											formatter: "",
										},
									},
								],
							},
						},
						{
							data: yData2,
							type: "line",
							smooth: true,
							symbol: "none", //去除点
							name: "治愈",
							color: "rgba(9,202,243,.7)",
							areaStyle: {
								//右，下，左，上
								color: new graphic.LinearGradient(
									0,
									0,
									0,
									1,
									[
										{
											offset: 0,
											color: "rgba(9,202,243,.7)",
										},
										{
											offset: 1,
											color: "rgba(9,202,243,.0)",
										},
									],
									false
								),
							},
							markPoint: {
								data: [
									{
										name: "最大值",
										type: "max",
										valueDim: "y",
										symbol: "rect",
										symbolSize: [60, 26],
										symbolOffset: [0, -20],
										itemStyle: {
											color: "rgba(0,0,0,0)",
										},
										label: {
											color: "#09CAF3",
											backgroundColor: "rgba(9,202,243,0.1)",

											borderRadius: 6,
											borderColor: "rgba(9,202,243,.5)",
											padding: [7, 14],
											formatter: "治愈：{c}",
											borderWidth: 0.5,
										},
									},
									{
										name: "最大值",
										type: "max",
										valueDim: "y",
										symbol: "circle",
										symbolSize: 6,
										itemStyle: {
											color: "#09CAF3",
											shadowColor: "#09CAF3",
											shadowBlur: 8,
										},
										label: {
											formatter: "",
										},
									},
								],
							},
						},
					],
				};
			},
		},
	};
</script>
<style lang='scss' scoped>
	.right_top_inner {
		margin-top: -8px;
	}
</style>