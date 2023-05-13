import request from '@/utils/request'

  export function getInfo(params) {
    /*创建组队需求 */
    return request({
      url: '/Info/getInfo',
      method: 'get',
      params:{
        ID:params.ID,
      }
    })
  }