import request from '@/utils/request'

export function GetUserInfor(params) {
    /*由用户id得到用户信息 */
    return request({
      url: '/Info/getInfo',
      method: 'get',
      params:{
        ID:params.user_id
      }
    })
  }
  export function GetMyCollection(params) {
    /*得到我的收藏 */
    return request({
      url: '/Facorite/GetFacorite',
      method: 'get',
      params:{
        user_id:params.user_id
      }
    })
  }
  export function AddMyCollection(params) {
    /*添加收藏 */
    return request({
      url: '/Facorite/AddFacorite',
      method: 'post',
      params:{
        facorite_id:params.facorite_id,
        owner_id:params.owner_id,
        project_id:params.project_id,
      }
    })
  }
  export function SelectAllCollection() {
    /*查询所有收藏*/
    return request({
      url: '/Facorite/SelectAllFacorite',
      method: 'get',
    })
  }
  export function getproject(params) {
    /*查询收藏对应的项目*/
    return request({
      url: '/Facorite/GetPorject',
      method: 'get',
      params:{
        project_id:params.project_id
      }
    })
  }
  export function Ifcollect(params) {
    /*判断改用户对应项目是否收藏过*/
    return request({
      url: '/Facorite/IfCollect',
      method: 'get',
      params:{
        project_id:params.project_id,
        owner_id:params.owner_id
      }
    })
  }
  export function deletecollect(params) {
    /*删除收藏*/
    return request({
      url: '/Facorite/deletecollect',
      method: 'delete',
      params:{
        facorite_id:params.facorite_id,
      }
    })
  }
  export function BlockUser(params) {
    return request({
      url: '/User/BlockUser',
      params:{
        user_id:params.user_id,
      }
    })
  }

  export function UnblockUser(params) {
    return request({
      url: '/User/UnblockUser',
      params:{
        user_id:params.user_id,
      }
    })
  }