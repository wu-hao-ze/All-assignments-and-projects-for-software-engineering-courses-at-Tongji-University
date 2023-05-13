<template>
  <div id="app">
    <div v-if="$route.meta.keepAlive">
        <navTop></navTop>
        <router-view></router-view>
    </div>
    <router-view v-if="!$route.meta.keepAlive"></router-view>
  </div>
</template>

<script>
import navTop from '@/components/common/nav.vue';
export default {
  name:"commonNav",
  components:{
     navTop
  }
}
</script>