import Vue from 'vue'
import Router from 'vue-router'


Vue.use(Router)

//导入需要的组件
import LogIn from '@/view/Login.vue'
import InforList from '@/view/InforList.vue'
import CreatList from '@/view/CreatList.vue'
import MyProject from '@/view/MyProject.vue'
import AddUser from '@/view/AddUser.vue'
import RequireDetail from '@/view/RequireDetail.vue'
import ChangeList from '@/view/ChangeList.vue'
import UserInformation from '@/view/UserInformation.vue'
import LogOut from '@/view/Logout.vue'
import PostInfor from '@/view/PostInfor.vue'
import PersonInfo from '@/view/PersonInfo.vue'

//主页
import AdminPage from '@/view/MainPage/AdminPage.vue'
import UserPage from '@/view/MainPage/UserPage.vue'
//通知相关
import NoticeList from '@/view/Notice/NoticeList.vue'
import ShowNotice from '@/view/Notice/ShowNotice.vue'
import CreateNotice from '@/view/Notice/CreateNotice.vue'
import User_NoticeList from '@/view/Notice/User_NoticeList.vue'
import User_ShowNotice from '@/view/Notice/User_ShowNotice.vue'

export default new Router({
    //在这里声明路由规则
    routes: [
        { path: '/', redirect: '/login' },
        { path: '/login', component: LogIn },
        { path: '/users/LogOut', component: LogOut },
        { path: '/users/PostInfor', component: PostInfor },
        { path: '/users/PersonInfo', component: PersonInfo },
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
        {
            path: '/Admin/AdminPage', component: AdminPage,
            children: [
                { path: '/Admin/NoticeList', component: NoticeList },
                { path: '/Admin/CreateNotice', component: CreateNotice },
                { path: '/Admin/ShowNotice', component: ShowNotice }
            ], props: true
        },
        {
            path: '/users/UserInformation', name: 'UserInformation', component: UserInformation, props: true, meta: {
                keepAlive: true
            }
        },
        {
            path: '/users/UserPage', component: UserPage,
            children: [
                { path: '/users/NoticeList', component: User_NoticeList },
                { path: '/users/User_ShowNotice', component: User_ShowNotice }
            ],props:true
        }
    ]
}
)