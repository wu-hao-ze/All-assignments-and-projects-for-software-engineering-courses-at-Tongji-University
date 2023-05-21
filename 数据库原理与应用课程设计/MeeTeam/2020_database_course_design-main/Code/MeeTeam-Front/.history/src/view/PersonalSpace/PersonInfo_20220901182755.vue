<template>
    <div id="PersonInfo">
        <div style="text-align: center;height: 50px">
         个人信息
         </div>
          <el-form  label-width="80px" class="info">
                <el-form-item label="ID" prop="ID">
                 <el-input type="text" v-bind:value="ID" aria-disabled="true" autocomplete="off"></el-input>
                </el-form-item> 

                <el-form-item label="姓名" prop="name">
                 <el-input type="text" v-bind:value="name" aria-disabled="true" autocomplete="off"></el-input>
                </el-form-item>  

                <el-form-item label="注册时间" prop="RegisterTime">
                 <el-input type="text" v-bind:value="RegisterTime" aria-disabled="true" autocomplete="off"></el-input>
                </el-form-item>  

                <el-form-item label="性别">
                    <el-select v-model="gender.value" placeholder="请选择你的性别">
                     <el-option label="男" value="男"></el-option>
                     <el-option label="女" value="女"></el-option>
                    </el-select>
                </el-form-item>

                <el-form-item label="电话号码" prop="phone_number">
                 <el-input type="text" v-model="phone_number" autocomplete="off"></el-input>
                </el-form-item>

                <el-form-item label="就读院校" prop="institution">
                 <el-input type="text" v-model="institution" autocomplete="off"></el-input>
                </el-form-item> 

                <el-form-item label="专业" prop="major">
                 <el-input type="text" v-model="major" autocomplete="off"></el-input>
                </el-form-item>

                <el-form-item label="个人简介" prop="introduction">
                 <el-input type="text" v-model="introduction" autocomplete="off"></el-input>
                </el-form-item>  
                
                <el-form-item label="积分" prop="point">
                 <el-input type="text" v-bind:value="point" aria-disabled="true" autocomplete="off"></el-input>
                </el-form-item>  

                <el-form-item>
                 <el-button type="primary" @click="submitForm()">确定</el-button>
                 <el-button @click="resetForm()">重置</el-button>
                </el-form-item>
          </el-form>
    </div>
</template>

<script>
import global_msg from '../../utils/global.js'
import { getInfo } from '@/api/getinfo.js'
import { PullInfo } from '@/api/pullinfo.js'
export default {
    name: 'MeeTeamFontPesonInfo',

    data() {
        return {
            ID:"",
            name:"",
            RegisterTime:"",//注册时间
            gender:[{value:'1',label:'男'},{value:'0',label:'女'}],//性别
            phone_number:"",//联系方式
            institution:"",//就读院校
            major:"",//专业
            introduction:"",//个人简介
            point:""//个人积分
        };
    },
    mounted(){
        // this.$store.commit('updateId','')//修改vuex中的id
        this.ID=global_msg.ID;
          let vm = this;
            let param = {
                ID:this.ID
            }
        getInfo(param).then(function(res){
        //    console.log(res)
        //    console.log(res.isexist)
            if(res.data){
       //         console.log(vm)
                vm.name=res.data.user_name
                vm.RegisterTime=res.data.register_time
                vm.point=res.data.point
            }
            else{
                vm.resetForm()
            }
        })
    },
    methods: {
        resetForm() {//重置表单
           this.phone_number=''
           this.institution=''
           this.introduction=''
           this.major=''
        },

        submitForm(){
            let vm = this;
            let param = {
                ID:vm.ID,
                gender:this.gender,
                phone_number:this.phone_number,
                institution:this.institution,
                major:this.major,
                introduction:this.introduction,
            }
            console.log(pa)
             PullInfo(param).then(function(res){
           //     console.log(res)
                if(res.data)
                {
                    vm.$router.push({path: "/users"}); //回到个人首页
                }
                else{
                    vm.resetForm()
                }
             })
        }
    },
};
</script>

<style scoped>
    #PersonInfo{
    width: 900px;
    height: 700px;
    background: #e5e9f2;
    position: absolute;
    left: 60%;
    top: 30%;
    margin-left: -550px;
    margin-top: -170px;
    border-radius: 5px;
    padding-top: 40px;
    padding-right: 50px;
  }
</style>