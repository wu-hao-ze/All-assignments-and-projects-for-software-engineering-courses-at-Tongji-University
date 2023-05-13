<template>
    <div id="NoticeList">
        <div id="body">
            <el-main>
                <div id="breadcrumb">
                    <el-breadcrumb separator-class="el-icon-arrow-right">
                        <el-breadcrumb-item :to="{ path: '/Admin/AdminPage' }">首页</el-breadcrumb-item>
                        <el-breadcrumb-item>用户管理</el-breadcrumb-item>
                    </el-breadcrumb>
                </div>

                <el-card>
                    <div class="filter-container">
                        <el-input placeholder="请在此处输入您想要搜索的用户名" style="width: 250px" class="filter-item"
                            v-model="listQuery.user_id" @keyup.enter.native="handleFilter"></el-input>
                        &nbsp;
                        <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter">
                            搜索
                        </el-button>
                    </div>
                </el-card>
                <br />

                <el-card>
                    <el-table :data="tableData" border :header-cell-style="{ textAlign: 'center' }"
                        :cell-style="{ 'text-align': 'center' }">
                        <el-table-column prop="user_id" label="用户名" align="left" style="margin: 50px" min-width="35%">
                        </el-table-column>
                        <el-table-column prop="account_status" label="状态" align="center" min-width="20%">
                        </el-table-column>
                        <el-table-column label="操作" align="center">
                            <template slot-scope="scope">
                                <el-button-group>
                                    <el-button plain type="info" icon="el-icon-remove" @click="block_user(scope.row)"
                                        :disabled="scope.row.account_status == '封禁' ? true : false"> 封禁该用户</el-button>
                                    <el-button plain type="success" icon="el-icon-success"
                                        @click="unblock_user(scope.row)" style="margin-left:50px;"
                                        :disabled="scope.row.account_status == '未封禁' ? true : false"> 解禁该用户 </el-button>
                                </el-button-group>
                            </template>
                        </el-table-column>
                    </el-table>
                </el-card>
            </el-main>
        </div>
    </div>
</template>
   
<script scoped>
import { get_all } from "@/api/notice.js";
import { delete_notice } from "@/api/notice.js";
import { BlockUser, UnblockUser } from '@/api/MyInfor.js';
import { GetUserStatus } from "@/api/MyInfor.js";
import { searchUserInfor } from "@/api/MyInfor.js";

const operateOptions = [
    { key: "0", display_name: "封禁" },
    { key: "1", display_name: "未封禁" },
];

export default {
    data() {
        return {
            tableData: [],
            listQuery: {
                //user_name:"",
                user_id: ""

            },
            operateOptions,
        };
    },

    created() {
        this.getlist();
    },

    methods: {
        getlist() {
            var vm = this; //全局变量
            GetUserStatus().then((res) => {
                //console.log(res);
                for (let item of res.data) {
                    //console.log(item);
                    let form = {
                        //设置添加数据的格式
                        user_id: "",
                        account_status: ""
                    };
                    form.user_id = item.user_id;
                    if (item.account_status == '0') {
                        form.account_status = "封禁";
                    }
                    else {
                        form.account_status = "未封禁";
                    }
                    vm.tableData.push(form);
                    // console.log(form);
                    // console.log(vm.table_Data);
                }
            });
        },

        getList2() {
            this.tableData = [];
            var query = JSON.stringify(this.listQuery);
            var params = JSON.parse(query);
            console.log(query);
            searchUserInfor(params).then((res) => {
                console.log(res.data);
                Object.keys(res.data).forEach((v) => {
                    let o = {};
                    o.user_id = res.data[v].user_id;
                    if (res.data[v].account_status == '0') {
                        o.account_status = "封禁";
                    }
                    else {
                        o.account_status = "未封禁";
                    }
                    //o.account_status=res.data[v].account_status;
                    console.log(o);
                    this.tableData.push(o);
                });
            });
            console.log(this.tableData);
        },

        handleFilter() {
            this.getList2();
        },
        goback() {
            this.$router.push({ path: "/Admin/AdminPage", query: { id: this.username } });
        },
        block_user(row) {
            let param = {
                user_id: row.user_id,
            };
            console.log(param);
            BlockUser(param).then(res => {
                console.log(res);
                if (res.data) {
                    alert("用户封禁成功");
                    location.reload();
                } else {
                    alert("用户封禁失败");
                }
            });
        },
        unblock_user(row) {
            let param = {
                user_id: row.user_id,
            };
            UnblockUser(param).then(res => {
                if (res.data) {
                    alert("用户解禁成功");
                    location.reload();
                } else {
                    alert("用户解禁失败");
                }
            });
        }
    },
};
</script>
  

<style>
.el-main {
    margin-left: 8%;
}

#body {
    position: relative;
    min-height: 100vh;
    height: 100%;
}

#breadcrumb {
    height: 30px;
    margin-bottom: 10px;
}
</style>