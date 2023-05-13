<template>
    <div id="bigbox">
        <div>
            <el-row :gutter="10">
                <el-col :span="12">
                    <el-card class="box-card">
                        <template #header>
                            <div class="card-header">
                                <span>hi, {{  user_name  }} !</span>
                                <!--span标签用于组合行内的元素-->
                            </div>
                        </template>
                        <template class="test">
                            <el-calendar v-model="value" />
                        </template>
                    </el-card>
                </el-col>

                <el-col :span="12">
                    <el-card class="box-card">
                        <template #header>
                            <div class="card-header">
                                <span>近期公告速览</span>
                                <!--span标签用于组合行内的元素-->
                                <el-button type=primary class="button" text @click="Newroutes">For More</el-button>
                            </div>
                        </template>
                        <template>
                            <el-table :data="tableData" style="width: 100%">
                                <el-table-column prop="title" label="公告标题">
                                </el-table-column>
                                <el-table-column prop="operate_time" label="发布时间" align="right">
                                </el-table-column>
                            </el-table>
                        </template>

                    </el-card>
                </el-col>
            </el-row>
        </div>

        <br>
        <h3 style="text-align:left">近期热点项目</h3>

        <div class="light_index">
            <!--{{ bannerheight }} 用于测试高度-->
            <!--跑马灯-->
            <template>
                <el-carousel :interval="4000" type="card" :height="bannerheight + 'px'">
                    <el-carousel-item v-for="item in imageList" :key="item.id">
                        <img ref="bannerheight" :src="item.idView" class="image" @load="imgLoad" style="width:100%">
                    </el-carousel-item>
                </el-carousel>
            </template>
        </div>

    </div>
</template>


<script>
import global_msg from '../../utils/global.js'
import { getInfo } from '@/api/getinfo'
import { ref } from 'vue'
import { get_all } from '@/api/notice.js'
import { openfile } from "@/api/file_load.js"

export default {
    name: '_index',
    data() {
        this.tableData = [];
        get_all().then(res => {
            Object.keys(res.data).forEach(v => {
                let o = {};
                if (v < 6) {
                    o.notice_id = res.data[v].notice_id;
                    o.title = res.data[v].notice_title;
                    o.admin_id = res.data[v].admin_id;
                    o.operate_time = res.data[v].operate_time;
                    if (res.data[v].operate_type) {
                        this.tableData.push(o);
                    }
                }
            });
        });

        return {
            user_name: '',
            user_id: '',
            bannerheight: '',
            imageList: [],
                            /*
                { id: 0, idView: require("@/assets/l1.jpg") },
                { id: 1, idView: require("@/assets/l2.jpg") },
                { id: 2, idView: require("@/assets/l4.jpg") },
                { id: 3, idView: require("@/assets/l1.jpg") },
                { id: 4, idView: require("@/assets/l2.jpg") },
                { id: 5, idView: require("@/assets/l4.jpg") },
                */
            value: new Date()
        };
    },

    created() {
        this.getid();
        this.getname();
        this.getimg();
    },

    mounted() {
        this.imgLoad();
        window.addEventListener('resize', () => {
            this.bannerheight = this.$refs.bannerheight[0].height
            this.imgLoad();
        }, false)
    },

    methods: {
        getimg() {
            for (let i = 0; i < 6; i++) {
                let param = {
                    target: "project",
                    id: i
                }
                openfile(param).then((res) => {
                    if (res.data) {
                        this.imageList.push({ id: param.id, idView: 'data:;base64,' + res.data});
                    }
                })
            }

            console.log(this.imageList);
        },

        getid() {
            this.user_id = global_msg.nowuserid; //这里用全局默认id值，后续可能需要更改？
            console.log(this.user_id);
        },

        getname() {
            let param = {
                ID: this.user_id
            };

            getInfo(param).then((res) => {
                if (res.data) {
                    console.log(param);
                    console.log(res);
                    this.user_name = res.data[0].user_name;
                    console.log(this.user_name);
                }
                else {
                    alert("info error");
                }
            })

        },

        Newroutes() {
            this.$router.push("/users/NoticeList"); //这里左侧的颜色显示有点问题，看看之后解决一下
        },

        imgLoad() {
            this.$nextTick(() => {
                this.bannerheight = this.$refs.bannerheight[0].height
                console.log(this.$refs.bannerheight[0].height);
            })
        }
    }
}
</script>


<style>
#bigbox {
    margin-left: 8%;
}

.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.text {
    font-size: 14px;
}

.item {
    margin-bottom: 20px;
}

.box-card {
    width: 450px;
    height: 500px;
}
</style>

<style>
.test /deep/ .el-calendar-table .el-calendar-day {
    height: 50%;
    width: 100%;
}
</style>

<style>
._index {
    background: #c8cfd8;
    width: 80%;
    text-align: center;
    margin-left: 142px;
    width: 80.6%;

}

.el-carousel__item h3 {
    color: #475669;
    font-size: 14px;
    opacity: 0.75;
    line-height: 200px;
    margin: 0;
}

.el-carousel__item:nth-child(2n) {
    background-color: #99a9bf;
}

.el-carousel__item:nth-child(2n+1) {
    background-color: #d3dce6;
}
</style>

