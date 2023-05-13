import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

//导入需要的组件
//账号操作相关
import AddUser from '@/view/LogManager/AddUser.vue'
import LogOut from '@/view/LogManager/Logout.vue'
import LogIn from '@/view/LogManager/Login.vue'
import ForgetPass from '@/view/LogManager/ForgetPass.vue'
//主页
import AdminPage from '@/view/MainPage/AdminPage.vue'
import AdminPageContent from '@/view/MainPage/AdminPageContent.vue'
import UserPage from '@/view/MainPage/UserPage.vue'
import MyPage from '@/view/MainPage/MyPage.vue'
import UserPageContent from '@/view/MainPage/UserPageContent.vue'
//通知相关
import NoticeList from '@/view/Notice/NoticeList.vue'
import ShowNotice from '@/view/Notice/ShowNotice.vue'
import CreateNotice from '@/view/Notice/CreateNotice.vue'
import User_NoticeList from '@/view/Notice/User_NoticeList.vue'
import User_ShowNotice from '@/view/Notice/User_ShowNotice.vue'
//项目需求相关
import ChangeListwithRequire from '@/view/Project_and_Require/ChangeListwithRequire.vue'
import ChangeListwithoutRequire from '@/view/Project_and_Require/ChangeListwithoutRequire.vue'
import AuditProject from '@/view/Project_and_Require/AuditProject.vue'
import AuditRequire from '@/view/Project_and_Require/AuditRequire.vue'
import DetailAudit from '@/view/Project_and_Require/DetailAudit.vue'
import CreatList from '@/view/Project_and_Require//CreatList.vue'
import MyProject from '@/view/Project_and_Require/MyProject.vue'
import ProjectDetail from '@/view/Project_and_Require/ProjectDetail.vue'
import InforList from '@/view/Project_and_Require/InforList.vue'
import ProjList from '@/view/Project_and_Require/ProjList.vue'
import AuditApply from '@/view/Project_and_Require/AuditApply.vue'
//用户信息
import DisplayPage from '@/view/PersonalSpace/DisplayPage.vue'
import UserInformation from '@/view/PersonalSpace/UserInformation.vue'
import PersonInfo from '@/view/PersonalSpace/PersonInfo.vue'
import UserHistory from '@/view/PersonalSpace/HisTory.vue'
import UserCollection from '@/view/PersonalSpace/CollecTion.vue'
import AdminInformation from '@/view/PersonalSpace/AdminInformation'
import ChangeUserInfo from '@/view/PersonalSpace/ChangeUserInfo.vue'
import ManageUser from '@/view/PersonalSpace/ManageUser.vue'
export default new Router({
    //在这里声明路由规则
    routes: [
        { path: '/', redirect: '/login' },
        { path: '/login', component: LogIn },
        { path: '/users/LogOut', component: LogOut },
        { path: '/users/PersonInfo', component: PersonInfo },
        { path: '/users/AddUser', component: AddUser },
        { path: '/users/ForgetPass', component: ForgetPass },
        //管理员主页及其子页面
        {
            path: '/Admin/AdminPage', component: AdminPage,
            children: [
                { path: '/Admin/AdminPageContent', component: AdminPageContent },
                { path: '/Admin/AuditProject', component: AuditProject },
                { path: '/Admin/DetailAudit', component: DetailAudit },
                { path: '/Admin/AuditRequire', component: AuditRequire },
                { path: '/Admin/NoticeList', component: NoticeList },
                { path: '/Admin/CreateNotice', component: CreateNotice },
                { path: '/Admin/ShowNotice', component: ShowNotice },
                { path: '/Admin/AdminInformation',component: AdminInformation},
                { path: '/Admin/ManageUser',component:ManageUser}
            ], props: true
        },
        //用户主页及其子页面
        {
            path: '/users/UserPage', component: UserPage,
            children: [
                { path: '/users/NoticeList', component: User_NoticeList },
                { path: '/users/CreatList', component: CreatList },
                { path: '/users/User_ShowNotice', component: User_ShowNotice },
                { path: '/users/InforList', component: InforList },
                { path: '/users/MyProject', component: MyProject },
                { path: '/users/ProjectDetail', component: ProjectDetail },
                { path: '/users/UserPageContent', component: UserPageContent },
                { path: '/users/ProjList', component: ProjList },
                { path:'/users/AuditApply',component:AuditApply},
                { path:'/users/ChangeListwithRequire',component:ChangeListwithRequire},
                { path:'/users/ChangeListwithoutRequire',component:ChangeListwithoutRequire}
            ], props: true
        },
        { path: '/users/DisplayPage', component: DisplayPage },
        //个人信息主页及其子页面
        {
            path: '/users/MyPage', component: MyPage,
            children: [
                { path: '/users/UserInformation', name: 'UserInformation', component: UserInformation },
                { path: '/users/UserHistory', name: 'UserHistory', component: UserHistory },
                { path: '/users/UserCollection', name: 'UserCollection', component: UserCollection },
                { path: '/users/ChangeUserInfo', component: ChangeUserInfo }
            ]
        }
    ]
}
)
