<template>
    <div>
        <el-header>
            <span class="container" @click="goback">
                <div class="button-wrapper">
                    <svg width="120" height="42">
                        <rect class="rectangle" width="120" height="42" />
                    </svg>
                    <div class="btn">
                        返回
                    </div>
                </div>
            </span>用户注册
        </el-header>
        <el-main id="con">
            <el-form :model="ruleForm" :rules="rules" ref="ruleForm" label-width="100px" class="demo-ruleForm">
                <el-form-item label="名称" prop="user_name">
                    <el-input v-model="ruleForm.user_name"></el-input>
                </el-form-item>
                <el-form-item label="密码" prop="pass">
                    <el-input type="password" v-model="ruleForm.pass" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="确认密码" prop="checkPass">
                    <el-input type="password" v-model="ruleForm.checkPass" autocomplete="off"></el-input>
                </el-form-item>
                <el-form-item label="性别" prop="gender">
                    <el-select v-model="ruleForm.gender" placeholder="请选择您的性別">
                        <el-option label="女" value='0'></el-option>
                        <el-option label="男" value="1"></el-option>
                    </el-select>
                </el-form-item>
                <el-form-item prop="contact_info" label="邮箱" :rules="[
                    { required: true, message: '请输入邮箱地址', trigger: 'blur' },
                    { type: 'email', message: '请输入正确的邮箱地址', trigger: ['blur', 'change'] }
                ]">
                    <el-input v-model="ruleForm.contact_info"></el-input>
                </el-form-item>
                <el-form-item label="所在学院" prop="institution">
                    <el-input v-model="ruleForm.institution"></el-input>
                </el-form-item>
                <el-form-item label="专业" prop="major">
                    <el-input v-model="ruleForm.major"></el-input>
                </el-form-item>
                <el-form-item label="自我介绍" prop="introduction">
                    <el-input type="textarea" v-model="ruleForm.introduction"></el-input>
                </el-form-item>
                <el-form-item label="头像">
                    <div slot="tip" class="el-upload__tip">
                        只能上传jpg/png文件，且不超过500kb
                    </div>
                    <el-upload class="upload-demo" action="https://localhost:5001/File/PostFile/" :data="upload_data"
                        :on-preview="handlePreview" :on-remove="handleRemove" :on-success="handleSuccess"
                        :before-remove="beforeRemove" multiple :limit="1" :on-exceed="handleExceed"
                        :file-list="fileList">
                        <el-button size="small" type="primary">点击上传</el-button>
                    </el-upload>
                    <div>
                        <el-avatar shape="square" :size="100" :fit="fit" :src="base64"></el-avatar>
                    </div>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="submitForm('ruleForm')">提交</el-button>
                    <el-button @click="resetForm('ruleForm')">重置</el-button>
                </el-form-item>
            </el-form>
        </el-main>
    </div>
</template>

<script>
import global_msg from '../../utils/global.js'
import { adduser } from '@/api/login.js'
import { isUseruni } from '@/api/login.js'
import { openfile } from "@/api/file_load.js";
export default {
    name: 'AddUser',
    data() {
        var nameunique = (rule, value, callback) => {
            if (value === '') {
                callback(new Error('请输入您的用户名'));
            } else {
                let params = {
                    user_id: value,
                }
                isUseruni(params).then(function (res) {
                    if (res.data === false) {
                        callback(new Error('此用户名已经被使用过'));
                    } else {
                        callback();
                    }
                })
            }
        };
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
            upload_data: {
                path: "head_img",
                id: "1",
                target: 'project'
            },
            fileList: [],
            base64: '',
            ruleForm: {
                user_name: '',
                pass: '',
                checkPass: '',
                age: '', //0是女生，1是男生
                gender: '',
                contact_info: '',
                institution: '',
                major: '',
                introduction: '',
            },
            rules: {
                user_name: [
                    { required: true, message: '请输入您的用户名', trigger: 'blur' },
                    { min: 2, max: 18, message: '长度在 2 到 18 个字符', trigger: 'blur' },
                    { validator: nameunique, trigger: 'blur' }
                ],
                pass: [
                    { validator: validatePass, trigger: 'blur' }
                ],
                checkPass: [
                    { validator: validatePass2, trigger: 'blur' }
                ],
                gender: [
                    { required: true, message: '请选择性別', trigger: 'change' }
                ],
                institution: [
                    { required: true, message: '请输入您所在的学院', trigger: 'blur' },
                    { min: 3, max: 5, message: '长度在 3 到 5 个字符', trigger: 'blur' }
                ],
                major: [
                    { required: true, message: '请输入您的专业', trigger: 'blur' },
                    { min: 3, max: 5, message: '长度在 3 到 5 个字符', trigger: 'blur' }
                ],
                introduction: [
                    { required: true, message: '请输入您的自我介绍', trigger: 'blur' },
                    { min: 3, max: 100, message: '长度在 3 到 100 个字符', trigger: 'blur' }
                ]
            }
        };
    },
    created() {
    this.getimg();
  },
    methods: {
        goback() {
            this.$router.push({ path: "/login" }); //返回
        },
        submitForm(formName) {
            this.$refs[formName].validate((valid) => {
                if (valid) {
                    console.log('submit!');
                } else {
                    console.log('error submit!!');
                    return false;
                }
            });
            let vm = this;
            let param = {
                user_id: this.ruleForm.user_name,//把用户id和用户名都设成用户名
                password: this.ruleForm.pass,
                user_name: this.ruleForm.user_name,
                gender: this.ruleForm.gender,
                contact_info: this.ruleForm.contact_info,
                institution: this.ruleForm.institution,
                major: this.ruleForm.major,
                introduction: this.ruleForm.introduction
            }
            adduser(param).then(function (res) {
                if (res.data === false) {
                    vm.$message.error("提交失败");
                    vm.resetForm(formName);
                } else {
                    vm.$message.success("提交成功");
                    vm.$router.push({ path: "/login" }); //接下来进入到哪个路由
                }
            })
        },
        resetForm(formName) {
            this.$refs[formName].resetFields();
        }
    }
}
</script>
<style scoped>
.el-header,
.el-footer {
    font-weight: 900;
    font-size: 24px;
    background-color: #B3C0D1;
    color: #333;
    text-align: center;
    line-height: 60px;
}

#con {
    height: 100%;
    width: 100%;
    position: fixed;
    margin-left: 0%;
    background-color: #ecf0f5;
    color: #333;
    text-align: left;
    line-height: 220px;
}

#home {
    width: 100%;
    min-height: 100vh;
    background-color: #ecf0f5;
    background-size: 100% 100%;
}

.demo-ruleForm>>>.el-form-item__label

/*标题的字体大小 */
    {
    font-size: 18px;
}

.container {
    display: inline;
    float: left;
    display: inline-block;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 40px;
    height: 48px;
}

.button-wrapper {
    display: inline-block;
    position: relative;
    width: 60px;
    height: 30px;
    text-align: center;
}

.rectangle {
    stroke-width: 8px;
    stroke: #ecf0f5;
    fill: transparent;
    /* Core part of the animation */
    stroke-dasharray: 200 500;
    stroke-dashoffset: -372;
    /* 偏移负数，虚线整体右移动了372个单位 */
}

.btn {
    color: white;
    font-size: 24px;
    letter-spacing: 6px;
    position: relative;
    top: -72px;
    left: 30px;
}

@keyframes extend {
    to {
        stroke-dasharray: 600;
        /* 属性用于创建虚线： */
        stroke-dashoffset: 0;
        stroke-width: 4;
        /* 属性定义了一条线，文本或元素轮廓厚度： */
    }
}

.button-wrapper:hover .rectangle {
    animation: 0.5s extend linear forwards;
}
</style>