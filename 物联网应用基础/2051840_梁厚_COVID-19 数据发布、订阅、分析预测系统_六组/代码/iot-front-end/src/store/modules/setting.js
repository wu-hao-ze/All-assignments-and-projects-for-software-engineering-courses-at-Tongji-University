/*
 * @Author: daidai
 * @Date: 2021-12-06 11:01:16
 * @LastEditors: Please set LastEditors
 * @LastEditTime: 2022-04-28 15:01:33
 * @FilePath: \web-pc\src\store\modules\setting.js
 */
import {isObject} from '@/lib/types'
export default {
    state: () => ({
        sbtxSwiper: true,//设备提醒轮播
        ssyjSwiper:true,//实时预警轮播
        isScale:true,//是否进行全局适配
        defaultOption: {
            step: 4.4, // 数值越大速度滚动越快
            hoverStop: true, // 是否开启鼠标悬停stop
            openWatch: true, // 开启数据实时监控刷新dom
            direction: 1, // 0向下 1向上 2向左 3向右
            limitMoveNum: 4, // 开始无缝滚动的数据量 this.dataList.length
            singleHeight: 0, // 单步运动停止的高度(默认值0是无缝不停止的滚动) direction => 0/1
            singleWidth: 0, // 单步运动停止的宽度(默认值0是无缝不停止的滚动) direction => 2/3
            waitTime: 3000 // 单步运动停止的时间(默认值1000ms)
          },
          echartsAutoTime:5000,//echarts 图自动请求接口时间
          refreshTime:5000
    }),
    getters: {
        //根据菜单路径获取 菜单信息
    },
    mutations: {
        initSwipers(state){
            let flags = JSON.parse(localStorage.getItem('settingData'))
            // console.log(flags);
            if(flags && isObject(flags)){
               for (const key in flags) {
                   if (state.hasOwnProperty.call(flags, key)&&flags.hasOwnProperty.call(flags, key)) {
                       const element = flags[key];
                       state[key]=element
                   }
               }
            }
        },
        updateSwiper(state, {val,type}) {
            state[type]=val
            localStorage.setItem('settingData',JSON.stringify({
                sbtxSwiper:state.sbtxSwiper,
                ssyjSwiper:state.ssyjSwiper,
                aztpSwiper:state.aztpSwiper,
                isScale:state.isScale
            }))
        },

    },
    actions: {
       
    },
}