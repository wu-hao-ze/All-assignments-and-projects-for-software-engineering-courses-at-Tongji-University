<template>
    <div id="contain">
        <div id="pass-container">
            <div style="text-align: center;height: 50px;font-size: 25px">
                找回密码
            </div>
            <el-form :model="ruleForm" status-icon :rules="rules" ref="ruleForm" label-width="100px"
                class="demo-ruleForm" label-position="right" v-show="ifnext">
                <el-form-item label="账号" prop="user">
                    <el-input type="text" v-model="user" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="邮箱" prop="mail">
                    <el-input type="text" v-model="mail" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="验证码" prop="word">
                    <el-input v-model="word" style="width:170px;margin-right:15px;"></el-input>
                    <el-button v-if="isSend" @click="sendMsg">发送验证码</el-button>
                    <el-button v-else>{{  num  }}重新获取</el-button>
                </el-form-item>
                <el-button type="primary" round @click="firststep" style="width:100px">返回登录</el-button>
                <el-button type="primary" round @click="nextstep" style="width:100px">下一步</el-button>
            </el-form>
            <el-form :model="ruleForm" status-icon :rules="rules" ref="ruleForm" label-width="100px"
                class="demo-ruleForm" label-position="right" v-show="!ifnext">
                <el-form-item label="账号" prop="user">
                    <el-input type="text" v-model="user"></el-input>
                </el-form-item>
                <!-- <el-form-item label="邮箱" prop="mail">
                    <el-input type="text" v-model="mail"></el-input>
                </el-form-item> -->
                <el-form-item label="密码" prop="pass">
                    <el-input type="password" v-model="ruleForm.pass" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="确认密码" prop="checkPass">
                    <el-input type="password" v-model="ruleForm.checkPass" autocomplete="off"></el-input>
                </el-form-item>
                <el-button type="primary" round @click="finishstep" style="width:100px">完成修改</el-button>
            </el-form>
        </div>
    </div>

</template>

<script>
import { IftrueMail } from '@/api/login.js'
import { SendMail } from '@/api/login.js'
import { ChangePass } from '@/api/login.js'
export default {
    name: 'ForgetPass',
    data() {
        var validatePass = (rule, value, callback) => {
            if (value === '') {
                callback(new Error('请输入密码'));
            } else {
                if (this.ruleForm.checkPass !== '') {
                    this.$refs.ruleForm.validateField('checkPass');
                }
                callback();
            }
        };
        var validatePass2 = (rule, value, callback) => {
            if (value === '') {
                callback(new Error('请再次输入密码'));
            } else if (value !== this.ruleForm.pass) {
                callback(new Error('两次输入密码不一致!'));
            } else {
                callback();
            }
        };
        return {
            ruleForm: {
                pass: '',
                checkPass: '',
            },
            rules: {
                pass: [
                    { validator: validatePass, trigger: 'blur' }
                ],
                checkPass: [
                    { validator: validatePass2, trigger: 'blur' }
                ]
            },
            user: "",
            mail: "",
            word: "",
            isSend: true,                //发送验证码
            num: 60,
            rightword: "",
            ifnext: true,
        };
    },
    methods: {
        sendMsg() {
            let vm = this;
            let pa = {
                user_id: this.user,
                contact_info: this.mail
            }
            IftrueMail(pa).then(function (res) {
                if (res.data === true) {//邮箱密码正确
                    vm.isSend = false
                    let timer = setInterval(() => {//倒计时
                        vm.num--;
                        if (vm.num <= 0) {
                            vm.isSend = true
                            clearInterval(timer)
                            vm.num = 60
                        }
                    }, 1000)
                    let pa2 = {
                        mail_id: vm.mail
                    }
                    SendMail(pa2).then(function (res2) {
                        vm.rightword = res2.data;//获得正确验证码
                        alert(vm.rightword)
                    })
                }
                else {
                    vm.$message.error("邮箱或用户名不正确");
                }
            })
        },
        nextstep() {
            if (Number(this.word) === Number(this.rightword)) {
                if(this.word===null)
                this.$message.error()
                this.ifnext = false;
            }
            else {
                this.$message.error("验证码不正确");
            }
        },
        finishstep() {
            let vm=this;
            let pa3 = {
                user_id: this.user,
                password: this.ruleForm.pass,
            }
            ChangePass(pa3).then(function (res) {
                if (res.data === true) {
                    vm.$message.success("修改成功");
                    vm.$router.push({ path: "/login" }); //接下来进入到哪个路由
                }
                else{
                    vm.$message.error("修改失败");
                }
            })
        },
        firststep(){
            this.$router.push({ path: "/login" }); //接下来进入到哪个路由
        }
    }
}
</script>

<style>
#contain {
    background-image: url("@/assets/bg3.png");
    margin: 0;
    padding: 0;
    width: 100%;
    height: 100%;
    position: fixed;
    background-size: 100% 100%;
}

#pass-container {
    width: 400px;
    height: 290px;
    background: rgba(190, 180, 180, 0.5);
    /*背景颜色为黑色，透明度为0.8*/
    position: absolute;
    /*绝对定位 */
    left: 50%;
    top: 50%;
    margin-left: -220px;
    margin-top: -170px;
    border-radius: 5px;
    padding-top: 40px;
    padding-right: 40px;
    padding-left: 10px;
    font-weight: 700;
}
</style>