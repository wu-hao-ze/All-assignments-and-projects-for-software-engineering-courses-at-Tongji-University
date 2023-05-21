import request from '@/utils/request'

  export function getlistInfor() {
    /*项目表格*/
    return request({
      url: '/Project/SelectAllProject',
      method: 'get',
    })
  }

  export function getrequireInfor() {
    /*项目表格*/
    return request({
      url: '/GroupRequirement/SelectAllGroupRequirement',
      method: 'get',
    })
  }