import request from '@/utils/request'

/*改变个人信息*/
export function PullInfo(params) {
  return request({
    url: '/Info/ChangeInfo',
    method: 'post',
    params: {
      ID: params.ID,
      grade: params.grade,
      gender: params.gender,
      contact_info: params.contact_info,
      institution: params.institution,
      major: params.major,
      introduction: params.introduction,
    }
  })
}