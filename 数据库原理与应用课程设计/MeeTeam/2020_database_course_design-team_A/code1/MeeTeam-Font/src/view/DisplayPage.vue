<template>
    <div id="DisplayPage">
        <div id="div1">
            <div>
                <el-avatar :size="100" src=""> user </el-avatar>
                <!-- src里面加入图片 -->
                <el-button id="b1" type="primary" @click="follow(ID, MyID)">{{ relationship }}</el-button>
            </div>
        </div>
        <div id="div2">
            <el-descriptions title="个人信息" direction="vertical" :column="1" border>
                <el-descriptions-item label="ID">{{ ID }}</el-descriptions-item>
                <el-descriptions-item label="姓名">{{ name }}</el-descriptions-item>
                <el-descriptions-item label="手机号">{{ phone_number }}</el-descriptions-item>
                <el-descriptions-item label="注册时间">{{ RegisterTime }}</el-descriptions-item>
                <el-descriptions-item label="就读院校">{{ institution }}</el-descriptions-item>
                <el-descriptions-item label="专业">{{ major }}</el-descriptions-item>
                <el-descriptions-item label="个人简介">{{ introduction }}</el-descriptions-item>
                <el-descriptions-item label="个人积分">{{ point }}</el-descriptions-item>
            </el-descriptions>
        </div>
        <div id="div3">
            <template>
                <el-table :data="user_project" border style="width: 100%">
                    <el-table-column prop="user_id" label="项目名称" width="360">
                    </el-table-column>
                    <el-table-column prop="duty" label="职责" width="360">
                    </el-table-column>
                </el-table>
            </template>
        </div>
    </div>
</template>

<script>
import global_msg from '../utils/global.js'
import { getInfo } from '@/api/getinfo.js'
import { getRe } from '@/api/followother.js'
import { pullRe } from '@/api/followother.js'
import { getProject } from '@/api/getproject.js'
import { getProjectname } from '@/api/getproject.js'
export default {
    name: 'MeeTeamFontDisplayPage',

    data() {
        return {
            MyID: "",
            ID: "",
            name: "",
            RegisterTime: "",//注册时间
            gender: "",//性别
            phone_number: "",//联系方式
            institution: "",//就读院校
            major: "",//专业
            introduction: "",//个人简介
            point: "0",//个人积分
            relationship: "关注",
            user_project: [{
                project_id: "",
                user_id: "",
                duty: "",
            }],
        };
    },

    mounted() {
        this.ID = global_msg.otherID;
        this.MyID = global_msg.MyID;
        let vm = this;
        let param = {
            ID: this.ID
        }
        //获取参加的项目
        getProject(param).then(function (res) {
            if (res.data) {
                vm.user_project = res.data;
                vm.user_project.forEach((item) => {
                    let params1={
                        project_id:item.project_id
                    }
                    getProjectname(params1).then(function (res) {
                        item.user_id = res.data;
                    })
                })

            }

        })

        //获取本页面的个人信息
        getInfo(param).then(function (res) {

            if (res.data) {
                vm.name = res.data.user_name
                vm.RegisterTime = res.data.register_time
                vm.gender = res.gender
                vm.phone_number = res.phone_number
                vm.institution = res.institution
                vm.major = res.major
                vm.introduction = res.introduction
                vm.point = res.data.point
            }
            else {
                vm.resetForm()
            }
        })
        let params = {
            MyID: this.MyID,
            ID: this.ID
        }//访问user_user表
        getRe(params).then(function (res) {
            if (res.data) {
                if (res.data.related == vm.MyID + "没关注" + vm.ID) {
                    vm.relationship = "关注"
                }
                else if (res.data.related == vm.MyID + "关注了" + vm.ID) {
                    vm.relationship = "已关注"
                }
                else if (res.data.related == "互关") {
                    vm.relationship = "互关"
                }
            }
        })
    },

    methods: {
        follow(ID, MyID) {
            let vm = this;
            let param = {
                MyID: vm.MyIDID,
                ID: vm.ID,
                relationship: vm.relationship
            }
            if (vm.relationship == "关注") {
                getRe(param).then(function (res) {
                    //对方已经关注了我，我试图与其互关
                    if (res.data.related == ID + "关注了" + MyID) {
                        param.relationship = ID + "关注了" + MyID
                        pullRe(param).then(function (res) {
                            if (res.data) {
                                vm.relationship = "互关"
                            }
                        })
                    }
                    else {
                        //对方没有关注我，我试图关注对方
                        pullRe(param).then(function (res) {
                            param.relationship = "关注"
                            if (res.data) {
                                vm.relationship = "已关注"
                            }
                        })
                    }
                })
            }
            else if (vm.relationship == "已关注") {
                //取消单方面的关注
                param.relationship = "已关注"
                pullRe(param).then(function (res) {
                    if (res.data) {
                        vm.relationship = "关注"
                    }
                })
            } else if (vm.relationship == "互关")//互关但是我试图取消关注
            {
                param.relationship = "互关"
                pullRe(param).then(function (res) {
                    if (res.data) {
                        vm.relationship = "关注"
                    }
                })
            }

        }
    },
};
</script>

<style>
#DisplayPage {
    padding: 20px;
    background-color: #eff1f5;

}

#div1 {
    position: relative;
    padding-left: 30px;
    background-color: #fff;
    margin-bottom: 20px;
}

#b1 {
    position: absolute;
    right: 20px;
    top: 50%;
    transform: translateY(-50%);
}

#div2 {
    padding-left: 20px;
    padding-top: 20px;
    padding-bottom: 20px;
    background-color: #fff;
}

#div3 {
    padding: 20px;
    padding-top: 20px;
}
</style>