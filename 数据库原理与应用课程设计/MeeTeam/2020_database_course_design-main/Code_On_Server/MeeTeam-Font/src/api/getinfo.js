import request from '@/utils/request'

/*获取用户信息 */
export function getInfo(params) {
  return request({
    url: '/Info/getInfo',
    method: 'get',
    params: {
      ID: params.ID,
    }
  })
}


/*更新积分*/
export function UpdatePoint(params) {
  return request({
    url: '/Info/UpdatePoint',
    method: 'post',
    params: {
      user_id: params.user_id,
    }
  })
}