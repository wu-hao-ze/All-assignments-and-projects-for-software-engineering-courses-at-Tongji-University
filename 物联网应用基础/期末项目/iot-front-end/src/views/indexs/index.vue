<!--
 * @Author: daidai
 * @Date: 2022-03-04 09:23:59
 * @LastEditors: Please set LastEditors
 * @LastEditTime: 2022-05-07 11:05:02
 * @FilePath: \web-pc\src\pages\big-screen\view\indexs\index.vue
-->
<template>
	<div class="contents">
		<div class="contetn_left">
			<div class="pagetab">
				<!-- <div class="item">实时监测</div> -->
			</div>

			<!--
      <ItemWrap class="contetn_left-top contetn_lr-item" title="设备总览">
        <LeftTop/>
      </ItemWrap>
      -->

			<ItemWrap
				class="contetn_left-center contetn_left-top"
				title="实时新闻"
				style="padding: 0 10px 20px 10px"
			>
				<LeftCenter />
			</ItemWrap>

			<ItemWrap
				class="contetn_left-bottom contetn_lr-item"
				title="比例分布"
				style="padding: 0 10px 16px 10px"
			>
				<LeftBottom />
			</ItemWrap>
		</div>
		<div class="contetn_center">
			<CenterMap class="contetn_center_top" />
			<ItemWrap class="contetn_center-bottom" title="全国疫情统计">
				<CenterBottom />
			</ItemWrap>
		</div>
		<div class="contetn_right">
			<ItemWrap class="contetn_left-bottom contetn_lr-item" title="新增VS治愈">
				<RightTop />
			</ItemWrap>
			<ItemWrap
				class="contetn_left-bottom contetn_lr-item"
				title="确诊排名(TOP8)"
				style="padding: 0 10px 16px 10px"
			>
				<RightCenter />
			</ItemWrap>
			<ItemWrap class="contetn_left-bottom contetn_lr-item" title="MQTT">
				<RightBottom />
			</ItemWrap>
		</div>
	</div>
</template>

<script>
	//import LeftTop from './left-top.vue'
	import LeftBottom from "./left-center.vue";
	import LeftCenter from "./left-bottom.vue";
	import CenterMap from "./center-map.vue";
	import CenterBottom from "./center-bottom.vue";
	import RightTop from "./right-top.vue";
	import RightCenter from "./right-center.vue";
	import RightBottom from "./right-bottom.vue";

	export default {
		components: {
			//LeftTop,
			LeftCenter,
			LeftBottom,
			CenterMap,
			RightTop,
			RightCenter,
			RightBottom,
			CenterBottom,
		},
		data() {
			return {};
		},
		filters: {
			numsFilter(msg) {
				return msg || 0;
			},
		},
		created() {},

		mounted() {},
		methods: {},
	};
</script>
<style lang="scss" scoped>
	// 内容
	.contents {
		.contetn_left,
		.contetn_right {
			width: 540px;
			box-sizing: border-box;
			// padding: 16px 0;
		}

		.contetn_center {
			width: 720px;
		}

		//左右两侧 三个块
		.contetn_lr-item {
			height: 310px;
		}

		.contetn_left-top {
			height: 630px;
		}

		.contetn_center_top {
			width: 100%;
		}

		// 中间
		.contetn_center {
			display: flex;
			flex-direction: column;
			justify-content: space-around;
		}

		.contetn_center-bottom {
			height: 315px;
		}

		//左边 右边 结构一样
		.contetn_left,
		.contetn_right {
			display: flex;
			flex-direction: column;
			justify-content: space-around;
			position: relative;
		}
	}

	@keyframes rotating {
		0% {
			-webkit-transform: rotate(0) scale(1);
			transform: rotate(0) scale(1);
		}
		50% {
			-webkit-transform: rotate(180deg) scale(1.1);
			transform: rotate(180deg) scale(1.1);
		}
		100% {
			-webkit-transform: rotate(360deg) scale(1);
			transform: rotate(360deg) scale(1);
		}
	}
</style>
