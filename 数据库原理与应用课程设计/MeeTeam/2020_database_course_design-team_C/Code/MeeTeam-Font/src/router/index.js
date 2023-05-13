import Vue from 'vue'
import Router from 'vue-router'


Vue.use(Router)

//导入需要的组件
import LogIn from '@/view/Login.vue'
import InforList from '@/view/InforList.vue'
import CreatList from '@/view/CreatList.vue'
import MyProject from '@/view/MyProject.vue'
import AddUser from '@/view/AddUser.vue'
import AdminPage from '@/view/AdminPage.vue'
import NoticeList from '@/view/NoticeList.vue'
import RequireDetail from '@/view/RequireDetail.vue'
import ChangeList from '@/view/ChangeList.vue'
import ShowNotice from '@/view/ShowNotice.vue'
import CreateNotice from '@/view/CreateNotice.vue'
import UserInformation from '@/view/UserInformation.vue'
export default new Router({
    //在这里声明路由规则
    routes: [
        { path: '/', redirect: '/login' },
        { path: '/login', component: LogIn },
        {
            path: '/users/InforList', name: 'InforList', component: InforList, props: true, meta: {
                keepAlive: true
            }
        },
        {
            path: '/users/RequireDetail', name: 'RequireDetail', component: RequireDetail, props: true, meta: {
                keepAlive: true
            }
        },
        { path: '/users/CreatList', component: CreatList },
        {
            path: '/users/MyProject', name: 'MyProject', component: MyProject, props: true, meta: {
                keepAlive: true
            }
        },
        { path: '/users/ChangeList', name: 'ChangeList', component: ChangeList, props: true },
        { path: '/users/AddUser', component: AddUser },
        { path: '/Admin/AdminPage', component: AdminPage, props: true },
        { path: '/Admin/NoticeList', component: NoticeList, props: true },
        { path: '/Admin/ShowNotice', component: ShowNotice, props: true },
        { path: '/Admin/CreateNotice', component: CreateNotice, props: true },
        { path: '/users/UserInformation', name:'UserInformation',component: UserInformation,props:true,meta:{
            keepAlive:true
        }
     }
    ]
}
)