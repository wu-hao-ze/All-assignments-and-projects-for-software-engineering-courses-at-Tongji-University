import request from '@/utils/request'

  export function getInfo(params) {
    //获取用户信息
    return request({
      url: '/Info/getInfo',
      method: 'get',
      params:{
        ID:params.ID,
      }
    })
  }

  export function UpdatePoint(params) {
    /*创建组队需求 */
    return request({
      url: '/Info/UpdatePoint',
      method: 'post',
      params:{
        user_id:params.user_id,
      }
    })
  }
