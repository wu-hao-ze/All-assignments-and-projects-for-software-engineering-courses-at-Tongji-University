<template>
  <div>
    <el-container>
      <el-header>
        <div style="color: gold">
          <i class="el-icon-search"></i>
          Image Search
          <el-button
            v-on:click="showFavorites"
            style="float: right; margin-top: 12px"
            type="success"
            plain
            >查看收藏夹</el-button
          >
        </div>
      </el-header>
      <el-main>
        <el-card class="box-card">
          <el-upload
            class="upload-demo"
            action="http://47.115.33.176:5000/imgUpload"
            :on-preview="handlePreview"
            :on-remove="handleRemove"
            :on-success="handleSuccess"
            :file-list="fileList"
            list-type="picture">
            <el-button size="small" type="primary"
              >点击上传图片搜索 <i class="el-icon-upload"></i
            ></el-button>
            <div slot="tip" class="el-upload__tip">
              只能上传jpg/png文件，且不超过500kb
            </div>
          </el-upload>
        </el-card>
        <div v-if="showResult">
          <h4>
            共搜索到 <span>{{ imageList.length }}</span> 张图片
          </h4>
          <el-row :gutter="60">
            <el-col :span="8" v-for="item in imageList" :key="item">
              <el-card :body-style="{ padding: '8px' }">
                <img :src="item" class="image" />
                <div style="padding: 14px">
                  <span>与 {{ file.name }} 相似的图片</span>
                  <div class="bottom clearfix">
                    <time class="time">{{
                      item.slice(item.indexOf("im"))
                    }}</time>
                    <el-button
                      type="warning"
                      class="button"
                      v-on:click="handleFavorites(item)"
                      icon="el-icon-star-off"
                      circle></el-button>
                  </div>
                </div>
              </el-card>
            </el-col>
          </el-row>
        </div>
        <div v-if="showFavorite">
          <h4>
            共收藏有 <span>{{ favorites.length }}</span> 张图片
          </h4>
          <el-row :gutter="60">
            <el-col :span="8" v-for="item in favorites" :key="item">
              <el-card :body-style="{ padding: '8px' }">
                <img :src="item" class="image" />
                <div style="padding: 14px">
                  <span>我的收藏</span>
                  <div class="bottom clearfix">
                    <time class="time">{{
                      item.slice(item.indexOf("im"))
                    }}</time>
                    <el-button
                      type="warning"
                      @click="handleFavorites($event, item)"
                      class="button"
                      icon="el-icon-delete"
                      circle></el-button>
                  </div>
                </div>
              </el-card>
            </el-col>
          </el-row>
        </div>
      </el-main>
    </el-container>
  </div>
</template>
<script>
export default {
  mounted() {},
  data() {
    return {
      showResult: false,
      showFavorite: false,
      num: 9,
      currentDate: new Date(),
      file: {},
      fileList: [],
      imageList: [],
      favorites: [],
    }
  },
  methods: {
    handleRemove(file, fileList) {
      //console.log(file, fileList);
    },
    handlePreview(file) {
      //console.log(file);
    },
    handleSuccess(response, file, fileList) {
      //console.log(response, file, fileList);
      this.imageList = response
      for (let i = 0; i < this.imageList.length; i++) {
        this.imageList[i] =
          "http://47.115.33.176:5000" + this.imageList[i].replace("\\", "/")
      }
      console.log(this.imageList)
      this.file = file
      this.showResult = true
      this.showFavorite = false
    },
    handleFavorites(event, url) {
      console.log(url)
      if (event.target.style.color != "red") {
        event.target.style.color = "red"
      } else {
        event.target.style.color = "white"
      }
      let key = url.slice(url.indexOf("im"))
      if (localStorage.getItem(key) == null) {
        localStorage.setItem(key, url)
      } else {
        localStorage.removeItem(key)
      }
    },
    showFavorites() {
      this.favorites = []
      for (let i = 0; i < localStorage.length; i++) {
        let key = localStorage.key(i) //获取本地存储的Key
        let url = localStorage.getItem(key)
        if (url.startsWith("http")) {
          this.favorites.push(url.replace("result", "dataset"))
        }
      }
      console.log(this.favorites)
      this.showResult = false
      this.showFavorite = true
    },
  },
}
</script>
<style scoped>
.box-card {
  margin-bottom: 5vh;
}
.time {
  font-size: 13px;
  color: #999;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
}

.button {
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
  clear: both;
}

.el-header {
  background-color: #143761;
  color: #f1ecec;
  text-align: center;
  line-height: 60px;
}
</style>
