<template>
  <div class="file">
    <el-button type="primary" plain @click="outExe">生成模板</el-button>

    <el-upload
      class="upload-demo"
      drag
      multiple
      :file-list="fileList"
      :http-request="getFile"
    >
      <i class="el-icon-upload"></i>
      <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
      <div class="el-upload__tip" slot="tip">
        请先点击生成模板，填写好相应测试用例，再上传，限xls/xlsx格式，不超过30M.
      </div>
    </el-upload>
  </div>
</template>

<script>
import printExe from "@/excel/outexe.js";
import { uploadfile } from "@/api/triangletest.js";
import { dateformat } from "@/utils/dateformat.js";
export default {
  name: "ExcelFile",
  components: {},
  props: {},
  data() {
    return {
      fileList: [],
    };
  },
  computed: {},
  watch: {},
  created() {},
  mounted() {},
  methods: {
    outExe() {
      this.$confirm("此操作将导出excel文件, 是否继续?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          let tHeader = [
            "测试用例编号",
            "第一条边的值（a）",
            "第二条边的值（b）",
            "第三条边的值（c）",
            "程序预期输出",
            "实际输出",
            "程序运行信息",
            "测试结果",
            "测试时间",
          ];
          const filterVal = [
            "id",
            "A",
            "B",
            "C",
            "expectation",
            "actual",
            "info",
            "test_result",
            "test_time",
          ];
          const example = [
            {
              id: "TS1",
              A: 3,
              B: 4,
              C: 5,
              expectation: "一般三角形",
            },
          ];
          printExe("判断三角形问题模板", tHeader, filterVal, example);
        })
        .catch(() => {
          this.$message("已取消");
        });
    },
    getFile(item) {
      let formData = new FormData();
      formData.append("file", item.file);
      let config = { "Content-Type": "multipart/form-data" };
      uploadfile(formData)
        .then((res) => {
          let url = window.URL.createObjectURL(new Blob([res.data]));
          let a = document.createElement("a");
          a.style.display = "none";
          a.href = url;
          a.setAttribute(
            "download",
            "判断三角形问题测试报告 " + dateformat() + ".xls"
          );
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a);
          window.URL.revokeObjectURL(url);
        })
        .catch(() => {
          this.$message.error("Server Error");
        });
    },
  },
};
</script>

<style scoped>
.upload-demo {
  width: 80%;
  margin-top: 20px;
}
</style>
