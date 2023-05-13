import request from '@/utils/request'

export function GetUserInfor(params) {
    /*由用户id得到用户信息 */
    return request({
      url: '/MyInforController/GetUserInfor',
      method: 'get',
      params:{
        user_id:params.user_id
      }
    })
  }
