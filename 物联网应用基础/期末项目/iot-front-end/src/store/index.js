/*
 * @Author: your name
 * @Date: 2021-07-26 09:32:49
 * @LastEditTime: 2022-04-26 09:12:33
 * @LastEditors: Please set LastEditors
 * @Description: In User Settings Edit
 * @FilePath: \web-pc\src\pages\big-screen\store\index.js
 */
import Vuex from 'vuex';
import Vue from 'vue';

Vue.use(Vuex)
const modulesFiles = require.context('./modules', true, /\.js$/)
const modules = modulesFiles.keys().reduce((modules, modulePath) => {
  const moduleName = modulePath.replace(/^\.\/(.*)\.\w+$/, '$1')
  const value = modulesFiles(modulePath)
  modules[moduleName]=value.default
  modules[moduleName].namespaced = true;   //打开命名空间
  return modules
}, {})
export default new Vuex.Store({
  modules,
  state: {

  },
  mutations: {
      setCollapsed(state,value){
      },
  },
  actions: {
    getUserdata({commit}){

    },
    
  },
  
})
