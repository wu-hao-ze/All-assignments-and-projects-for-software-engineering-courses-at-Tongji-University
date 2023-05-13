import axios from 'axios'
import { MessageBox, Message } from 'element-ui'
import store from '@/store'
import { getToken } from '@/utils/auth'

// 每次请求携带cookies信息
axios.defaults.headers.post['Content-Type']='application/x-www-form-urlencoded'
axios.defaults.withCredentials = true

// create an axios instance
const service = axios.create({
    //baseURL: process.env.VUE_APP_BASE_API, // url = base url + request url
    baseURL:'https://localhost:44338/',
    // baseURL:'http://192.168.43.153:6001/api/',这个应该是把后端部署了
    // withCredentials: true, // send cookies when cross-domain requests
    timeout: 50000, // request timeout
    //withCredentials: true//携带cookie
    async:true,
    crossDomain:true,
  })

  export default service