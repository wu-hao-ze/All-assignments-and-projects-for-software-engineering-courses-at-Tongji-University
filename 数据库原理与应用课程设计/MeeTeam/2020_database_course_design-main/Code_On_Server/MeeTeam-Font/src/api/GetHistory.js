import request from '@/utils/request'

export function gethistory(params) {
  return request({
    url: '/History/Gethistory',
    method: 'get',
    params: {
      user_id: params.user_id
    }
  })
}