import request from '@/utils/request'

/*获取项目表格*/
export function getlistInfor() {
  return request({
    url: '/Project/SelectAllProject',
    method: 'get',
  })
}

export function getrequireInfor() {
  return request({
    url: '/GroupRequirement/SelectAllGroupRequirement',
    method: 'get',
  })
}