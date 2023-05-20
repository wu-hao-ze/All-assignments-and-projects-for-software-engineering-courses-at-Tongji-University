//基于axios封装的请求模块
import axios from 'axios'

const request = axios.create({

    //    baseURL:'http://121.199.46.150:5000'

    baseURL: 'http://localhost:5000'
})



export default request