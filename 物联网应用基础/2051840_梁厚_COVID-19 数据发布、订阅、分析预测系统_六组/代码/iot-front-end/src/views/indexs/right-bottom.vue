<template>
	<div
		v-if="pageflag"
		class="right_center_wrap beautify-scroll-def"
		:class="{ 'overflow-y-auto': !sbtxSwiperFlag }"
	>
		<el-select
			v-model="value"
			placeholder="请选择"
			style="margin-left: 10px; margin-right: 40px; width: 150px; height: 10px"
			collapse-tags
		>
			<el-option
				v-for="item in options"
				:key="item.value"
				:label="item.label"
				:value="item.value"
				style="font-size: 4px"
			>
			</el-option>
		</el-select>
		<el-switch
			v-model="value2"
			active-color="#13ce66"
			inactive-color="#f39f7d"
			active-text="总数"
			inactive-text="今天"
			style="margin-left: 180px"
		>
		</el-switch>
		<div id="main" style="width: 600px; height: 280px"></div>
	</div>
</template>

<script>
	import { currentGET } from "api/modules";
	import vueSeamlessScroll from "vue-seamless-scroll"; // vue2引入方式
	import Kong from "../../components/kong.vue";
	import * as echarts from "echarts";
	import axios from "axios";

	export default {
		components: { vueSeamlessScroll, Kong },

		data() {
			return {
				list: [],
				pageflag: true,
				defaultOption: {
					...this.$store.state.setting.defaultOption,
					limitMoveNum: 3,
					singleHeight: 250,
					step: 0,
				},
				options: [
					{
						value: "中国",
					},
					{
						value: "新疆",
					},
					{
						value: "湖北",
					},
					{
						value: "辽宁",
					},
					{
						value: "广东",
					},
					{
						value: "内蒙古",
					},
					{
						value: "黑龙江",
					},
					{
						value: "河南",
					},
					{
						value: "山东",
					},
					{
						value: "陕西",
					},
					{
						value: "贵州",
					},
					{
						value: "上海",
					},
					{
						value: "重庆",
					},
					{
						value: "西藏",
					},
					{
						value: "福建",
					},
					{
						value: "湖南",
					},
					{
						value: "海南",
					},
					{
						value: "江苏",
					},
					{
						value: "青海",
					},
					{
						value: "广西",
					},
					{
						value: "宁夏",
					},
					{
						value: "浙江",
					},
					{
						value: "河北",
					},
					{
						value: "香港",
					},
					{
						value: "台湾",
					},
					{
						value: "澳门",
					},
					{
						value: "甘肃",
					},
					{
						value: "四川",
					},
					{
						value: "天津",
					},
					{
						value: "江西",
					},
					{
						value: "云南",
					},
					{
						value: "山西",
					},
					{
						value: "北京",
					},
					{
						value: "吉林",
					},
				],
				value: "",
				confirm: 0,
				dead: 0,
				heal: 0,
				value2: true,
			};
		},
		watch: {
			async value(newVal, oldVal) {
				console.log(newVal, oldVal);
				if (newVal === "") return;
				const { data: res1 } = await axios.get("/covid/today?area=" + newVal);
				console.log(res1);
				const { data: res2 } = await axios.get("/covid/total?area=" + newVal);
				console.log(res2);
				if (this.value2) {
					this.confirm = res2.confirm;
					this.dead = res2.dead;
					this.heal = res2.heal;
				} else {
					this.confirm = res1.confirm;
					this.dead = res1.dead;
					this.heal = res1.heal;
				}
				this.getchart();
			},
			async value2(newVal, oldVal) {
				const { data: res1 } = await axios.get("/covid/today?area=" + this.value);
				console.log(res1);
				const { data: res2 } = await axios.get("/covid/total?area=" + this.value);
				console.log(res2);
				if (this.value2) {
					this.confirm = res2.confirm;
					this.dead = res2.dead;
					this.heal = res2.heal;
				} else {
					this.confirm = res1.confirm;
					this.dead = res1.dead;
					this.heal = res1.heal;
				}
				this.getchart();
			},
		},
		computed: {
			sbtxSwiperFlag() {
				let ssyjSwiper = this.$store.state.setting.ssyjSwiper;
				if (ssyjSwiper) {
					this.components = vueSeamlessScroll;
				} else {
					this.components = Kong;
				}
				return ssyjSwiper;
			},
		},
		created() {},

		mounted() {},
		updated() {},
		methods: {
			getchart() {
				var chartDom = document.getElementById("main");
				var myChart = echarts.init(chartDom);
				var option;
				option = {
					title: {
						text: this.value,
						left: "center",
						textStyle: {
							color: "white",
						},
					},
					tooltip: {
						trigger: "item",
					},
					legend: {
						orient: "vertical",
						left: "left",
						textStyle: {
							color: "white",
						},
						top: "center",
					},
					series: [
						{
							type: "pie",
							radius: "50%",
							data: [
								{ value: this.heal, name: "治愈总数" },
								{ value: this.confirm, name: "确诊总数" },
								{ value: this.dead, name: "死亡总数" },
							],
							label: {
								color: "white",
							},
							emphasis: {
								itemStyle: {
									shadowBlur: 10,
									shadowOffsetX: 0,
									shadowColor: "rgba(0, 0, 0, 0.5)",
								},
							},
						},
					],
				};
				option && myChart.setOption(option);
			},
		},
	};
</script>
<style lang='scss' scoped>
	.right_center {
		width: 100%;
		height: 100%;

		.right_center_item {
			display: flex;
			align-items: center;
			justify-content: center;
			height: auto;
			padding: 10px;
			font-size: 14px;
			color: #fff;

			.orderNum {
				margin: 0 20px 0 -20px;
			}

			.inner_right {
				position: relative;
				height: 100%;
				width: 400px;
				flex-shrink: 0;
				line-height: 1.5;

				.dibu {
					position: absolute;
					height: 2px;
					width: 104%;
					background-image: url("../../assets/img/zuo_xuxian.png");
					bottom: -12px;
					left: -2%;
					background-size: cover;
				}
			}

			.info {
				margin-right: 10px;
				display: flex;
				align-items: center;

				.labels {
					flex-shrink: 0;
					font-size: 12px;
					color: rgba(255, 255, 255, 0.6);
				}

				.zhuyao {
					color: $primary-color;
					font-size: 15px;
				}

				.ciyao {
					color: rgba(255, 255, 255, 0.8);
				}

				.warning {
					color: #e6a23c;
					font-size: 15px;
				}
			}
		}
	}

	.right_center_wrap {
		overflow: hidden;
		width: 100%;
		height: 250px;
	}

	.overflow-y-auto {
		overflow-y: auto;
	}
</style>