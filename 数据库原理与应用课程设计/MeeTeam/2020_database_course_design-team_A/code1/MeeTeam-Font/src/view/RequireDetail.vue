<template>
    <div class="detaildiv">
        <el-descriptions title="" :column="2" border
            style="margin-top: 30px;margin-right: 80px;margin-bottom: 180px">
            <el-descriptions-item label="发起者" width="140">{{ originator_id }}</el-descriptions-item>
            <el-descriptions-item label="发布时间">{{ release_time }}</el-descriptions-item>
            <el-descriptions-item label="所在校区">{{ region }}</el-descriptions-item>
            <el-descriptions-item label="需求名称">{{ requirement_name }}</el-descriptions-item>
            <el-descriptions-item label="需求目的">{{ purpose }}</el-descriptions-item>
            <el-descriptions-item label="组队类型">{{ team_type }}</el-descriptions-item>
            <el-descriptions-item label="类型细则">{{ team_type_details }}</el-descriptions-item>
            <el-descriptions-item label="组队人数">{{ team_limit }}</el-descriptions-item>
            <el-descriptions-item label="组队细则">{{ details }}</el-descriptions-item>
            <el-descriptions-item label="当前状态">{{ require_status }}</el-descriptions-item>
        </el-descriptions>
        <el-steps :space="500" :active="1" finish-status="success" align-center
         style="margin-bottom: 180px">
            <el-step title="发布需求"></el-step>
            <el-step title="招募队友"></el-step>
            <el-step title="发布项目"></el-step>
        </el-steps>
        <el-button type="success" plain style="float: right;margin-right: 80px">申请加入</el-button>
        <el-button type="info" plain style="float: right;margin-right: 30px">联系发起人</el-button>
    </div>
</template>

<script>
import { id_require } from '@/api/RequireDetail'
import { detailnum } from '@/api/RequireDetail'
export default {
    name: "RequireDetail",
    props: ['re_id'],
    data() {
        return {
            detail_id: this.re_id, //存储的是接受过来的参数：项目编号
            release_time: '',
            purpose: '',
            team_type: '',
            team_limit: '',
            details: '',
            requirement_status: '',
            originator_id: '',
            project_id: '',
            region: '',
            team_type_details: '',
            requirement_name: '',
            require_status: '',
        }
    },
    mounted() {
        this.getdata();
    },
    methods: {
        goBack() {
            this.$router.push({ path: "/users/InforList" }); //返回
        },
        getdata() {
            var vm = this;
            let params = {
                require_id: this.detail_id,
            }
            id_require(params).then(function (res) {
                let item = res.data[0];
                vm.region = item.region;
                item.release_time = item.release_time.replace("\"", "").replace("\"", "");//去掉时间格式的引号
                vm.release_time = item.release_time;
                vm.purpose = item.purpose;
                vm.team_type = item.team_type;
                vm.team_limit = item.team_limit;
                vm.details = item.details;
                vm.requirement_status = item.requirement_status;
                vm.originator_id = item.originator_id;
                vm.project_id = item.project_id;//这个暂时还没有和项目连接起来
                //vm.team_type_details = item.team_type_details;
                vm.requirement_name = item.requirement_name;
                vm.require_status = item.require_status;
                let dd = item.team_type_details.split("-");
                for (let ee of dd) {
                    var ff = detailnum(Number(ee));
                    vm.team_type_details = vm.team_type_details + ff + "   ";
                }
            })
        }
    }
}
</script>

<style>
.detaildiv {
    /* position: absolute; */
    height: 100%;
    bottom: 0%;
    right: 30px;
    left: 30px;
}
</style>