import Vue from 'vue'
import VueRouter from 'vue-router'
Vue.use(VueRouter)

const routes = [
    {
        path: '/',
        name: 'Root',
        redirect: '/index',
    },
    {
        path: '/index',
        name: 'index',
        component: () => import('../views/Search.vue')
    },

]

const router = new VueRouter({
    routes
})


export default router;