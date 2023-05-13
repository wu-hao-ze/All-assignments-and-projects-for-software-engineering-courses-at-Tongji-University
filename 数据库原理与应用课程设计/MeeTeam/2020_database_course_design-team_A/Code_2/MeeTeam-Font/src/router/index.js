import Vue from 'vue'
import Router from 'vue-router'


Vue.use(Router)

//导入需要的组件
import LogIn from '@/components/Login.vue'
import InforList from '@/components/InforList.vue'
import PostInfor from '@/components/PostInfor.vue'
export default new Router({
    //在这里声明路由规则
    routes: [
        {path: '/', redirect: '/users'},
        {path: '/users', component: LogIn},
        {path: '/users/InforList', component: InforList},
        {path: '/users/PostInfor', component: PostInfor},
    ],
})