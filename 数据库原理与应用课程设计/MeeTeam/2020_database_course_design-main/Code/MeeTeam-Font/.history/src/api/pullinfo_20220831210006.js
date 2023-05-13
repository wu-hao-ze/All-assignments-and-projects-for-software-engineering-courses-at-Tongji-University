import request from '@/utils/request'

  export function PullInfo(params) {
    /*改变个人信息*/
    return request({
      url: '/Info/PullInfo',
      method: 'get',
      params:{
        ID:params.ID,
        gender:params.gender,
        phone_number:params.contact_info,
        institution:params.institution,
        major:params.major,
        introduction:params.introduction,
        grade:params.grade,
      }
    })
  }