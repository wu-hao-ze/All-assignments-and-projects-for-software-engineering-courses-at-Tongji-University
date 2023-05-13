import request from '@/utils/request'

/*由用户id得到用户信息 */
export function GetUserInfor(params) {
  return request({
    url: '/Info/getInfo',
    method: 'get',
    params: {
      ID: params.user_id
    }
  })
}

/*由用户id得到用户信息 */
export function searchUserInfor(params) {
  return request({
    url: '/Info/searchInfo',
    method: 'get',
    params: {
      ID: params.user_id
    }
  })
}

/*得到我的收藏 */
export function GetMyCollection(params) {
  return request({
    url: '/Facorite/GetFacorite',
    method: 'get',
    params: {
      user_id: params.user_id
    }
  })
}


/*添加收藏 */
export function AddMyCollection(params) {
  return request({
    url: '/Facorite/AddFacorite',
    method: 'post',
    params: {
      facorite_id: params.facorite_id,
      owner_id: params.owner_id,
      project_id: params.project_id,
    }
  })
}


/*查询所有收藏*/
export function SelectAllCollection() {
  return request({
    url: '/Facorite/SelectAllFacorite',
    method: 'get',
  })
}


/*查询收藏对应的项目*/
export function getproject(params) {
  return request({
    url: '/Facorite/GetPorject',
    method: 'get',
    params: {
      project_id: params.project_id
    }
  })
}


/*判断改用户对应项目是否收藏过*/
export function Ifcollect(params) {
  return request({
    url: '/Facorite/IfCollect',
    method: 'get',
    params: {
      project_id: params.project_id,
      owner_id: params.owner_id
    }
  })
}


/*删除收藏*/
export function deletecollect(params) {
  return request({
    url: '/Facorite/deletecollect',
    method: 'delete',
    params: {
      facorite_id: params.facorite_id,
    }
  })
}


/*返回参与过的所有过审的项目*/
export function my_project(params) {
  return request({
    url: '/Project/my_project',
    method: 'GET',
    params: {
      user_id: params.user_id
    }
  })
}


/*
*以下为封禁功能用到
*/
export function BlockUser(params) {
  return request({
    url: '/User/BlockUser',
    method: 'post',
    params: {
      user_id: params.user_id,
    }
  })
}


export function UnblockUser(params) {
  return request({
    url: '/User/UnblockUser',
    method: 'post',
    params: {
      user_id: params.user_id,
    }
  })
}


export function GetUserStatus() {
  return request({
    url: '/User/SelectAllStatus',
    method: 'get',
  })
}
