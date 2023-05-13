<template>
  <el-main>
    <el-upload
      class="upload-demo"
      action="https://localhost:5001/File/PostFile/"
      :data="upload_data"
      :on-preview="handlePreview"
      :on-remove="handleRemove"
      :on-success="handleSuccess"
      :before-remove="beforeRemove"
      multiple
      :limit="1"
      :on-exceed="handleExceed"
      :file-list="fileList"
    >
      <el-button size="small" type="primary">点击上传</el-button>
      <div slot="tip" class="el-upload__tip">
        只能上传jpg/png文件，且不超过500kb
      </div>
    </el-upload>
    <div>
      <el-avatar shape="square" :size="100" :fit="fit" :src="base64"></el-avatar>
    </div>
  </el-main>
</template>
<script>
import { openfile } from "@/api/file_load.js";
export default {
  data() {
    return {
      upload_data: {
        path: "head_img",
        id: "normal",
        target:'user'
      },
      fileList: [],
      base64:''
    };
  },
  created() {
    this.getimg();
  },
  methods: {
    handleRemove(file, fileList) {
      console.log(file, fileList);
    },
    handlePreview(file) {
      console.log(file);
    },
    handleSuccess() {
      location.reload();
    },
    handleExceed(files, fileList) {
      this.$message.warning(
        `当前限制选择 3 个文件，本次选择了 ${files.length} 个文件，共选择了 ${
          files.length + fileList.length
        } 个文件`
      );
    },
    beforeRemove(file, fileList) {
      return this.$confirm(`确定移除 ${file.name}？`);
    },
    getimg() {
      let params = {
        target: 'user',
        id: 'normal'
      }
      openfile(params).then((res) => {
        this.base64 = 'data:;base64,' + res.data;
      });
    },
  },
};
</script>