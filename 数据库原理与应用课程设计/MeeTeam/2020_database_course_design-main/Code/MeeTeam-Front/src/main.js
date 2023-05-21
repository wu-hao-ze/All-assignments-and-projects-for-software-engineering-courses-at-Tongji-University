import Vue from 'vue'
import App from './App.vue'
import router from './router'
import ElementUI from 'element-ui'; 
import 'element-ui/lib/theme-chalk/index.css';   
import axios from 'axios';
import store from '@/store'
import qs from 'qs';
import { createApp } from 'vue'
import VueParticles from 'vue-particles'  

Vue.prototype.$qs = qs; //添加实例 property
Vue.config.productionTip = false
Vue.use(ElementUI)
Vue.prototype.$http=axios;
Vue.use(VueParticles);

new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app')
