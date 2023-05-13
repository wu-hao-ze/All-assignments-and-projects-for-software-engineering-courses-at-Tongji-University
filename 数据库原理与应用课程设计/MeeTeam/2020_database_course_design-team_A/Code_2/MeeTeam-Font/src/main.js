import Vue from 'vue'
import App from './App.vue'
import router from './router'
import ElementUI from 'element-ui'; 
import 'element-ui/lib/theme-chalk/index.css';   
import axios from 'axios';
import store from '@/store'
import qs from 'qs';

Vue.prototype.$qs = qs;
Vue.prototype.$http = axios //添加实例 property
Vue.config.productionTip = false
Vue.use(ElementUI)

new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app')
