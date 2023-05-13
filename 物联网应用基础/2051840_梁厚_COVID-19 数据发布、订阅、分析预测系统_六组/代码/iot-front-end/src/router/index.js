/*
 * @Author: daidai
 * @Date: 2022-01-12 14:22:29
 * @LastEditors: Please set LastEditors
 * @LastEditTime: 2022-04-28 14:53:02
 * @FilePath: \web-pc\src\pages\big-screen\router\index.js
 */
import Vue from "vue";
import VueRouter from "vue-router";

Vue.use(VueRouter);

const routes = [  {
  path: '/',
  redirect: '/index',
},
{
  path: '/home',
  name: 'home',
  component: () => import(/* webpackChunkName: "LSD.bighome" */ '../views/home.vue'),
  children:[
    {
      path: '/index',
      name: 'index',
      component: () => import(/* webpackChunkName: "LSD.bighome" */ '../views/indexs/index.vue'),
    }
  ]
}, 
];
const router = new VueRouter({
  mode: "hash",
  // base: process.env.BASE_URL,
  routes
});

export default router;