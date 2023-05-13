<template>
    <el-main>
        <div id="breadcrumb">
            <el-breadcrumb separator-class="el-icon-arrow-right">
                <el-breadcrumb-item :to="{ path: '/users/UserPageContent' }">首页</el-breadcrumb-item>
                <el-breadcrumb-item>项目管理</el-breadcrumb-item>
                <el-breadcrumb-item>平台项目列表</el-breadcrumb-item>
            </el-breadcrumb>
        </div>

        <template>
            <div>
                <el-row :glutter="10">
                    <el-col :span="6" v-for="(o, index) in tabledata" :key="index" :offset="2">
                        <div style="margin-top:15px">
                            <!--这一层div的作用是什么-->
                            <el-card :body-style="{ padding: '0px' }" shadow="hover">
                                <img :src=o.project_img class="image" />
                                <div class="text">
                                    {{  o.project_name  }}<br />
                                    发起时间：{{  o.create_time  }}<br />
                                    项目进度：{{  o.project_progress  }}<br />
                                    当前是否有组队需求：{{o.project_status}}<br/><br/>
                                    <template>
                                        <el-button type="success" plain size="mini" style="float:right"
                                            @click="handleLook(index, o.row)">了解更多</el-button>
                                    </template>
                                    <br>
                                </div>
                            </el-card>
                        </div>
                        <br><br>
                    </el-col>
                </el-row>
            </div>
        </template>
    </el-main>
</template>

<script>
import { ref } from 'vue'
import global_msg from '../../utils/global.js'
import { getlistInfor } from '@/api/Inforlist.js'
import { openfile } from "@/api/file_load.js";

export default {
    name: 'InforList',
    data() {
        return {
            tabledata: [],
            current_page: 1,
            total: null,
            pagesize: 8
        }
    },
    methods: {
        /*
                refresh() {
            location.reload();
        },
        */


        gettable() {
            getlistInfor().then(res => {
                let vm = this;
                global_msg.projectnum = res.data.length;//改变全局requirenum
                // console.log(res);
                // console.log(res.data.length);
                // console.log(res.data[0]);
                // console.log(res.data[0].details);
                for (let item of res.data) {
                    let form = {//设置添加数据的格式
                        project_name: '',
                        project_introduction: '',
                        project_progress: '',
                        create_time: '',
                        project_status: '',
                        project_id: '',
                        project_img: ''
                    }

                    form.project_name = item.project_name;
                    form.project_introduction = item.project_introduction;
                    form.project_progress = item.project_progress;
                    form.create_time = item.create_time;
                    form.create_time = form.create_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
                    form.project_status = item.project_status;
                    form.project_id = item.project_id;

                    let param = {
                        target: "project",
                        id: form.project_id
                    }

                    openfile(param).then((res) => {
                        if (res.data) {
                            form.project_img = 'data:;base64,' + res.data;
                        }
                    })

                    vm.tabledata.push(form);
                    // console.log(form);
                    // console.log(vm.tableData);
                }
            }).catch((res) => {
                console.log(res);
            })
        },

        handleLook(index, row) {//进入项目详情页面
            var project_id = this.tabledata[index].project_id;
            console.log(index, row);
            this.$router.push({ path: "/users/ProjectDetail", query: { p_id: project_id } });
        }
    },

    mounted() {
        this.gettable();
    },

}
</script>

    
<style>
.button {
    padding: 0;
    min-height: auto;
}

.image {
    width: 100%;
    display: block;
}

.text {
    padding: 16px;
    line-height: 30px;
}

/*
这里的后两个是对于card自适应高度的一种可能可行的解决方案
*/
.el-row {
    margin-bottom: 20px;
    display: flex;
    flex-wrap: wrap;
}

.el-row .el-card {
    min-width: 100%;
    height: 100%;
    margin-right: 20px;
    transition: all .5s;
}


</style>

<style scoped>
.time {
    font-size: 13px;
    color: #999;
}

.bottom {
    margin-top: 13px;
    line-height: 12px;
    padding: 0;
    float: right;
}

.image {
    width: 100%;
    display: block;
}

.clearfix:before,
.clearfix:after {
    display: table;
    content: "";
}

.clearfix:after {
    clear: both
}
</style>
