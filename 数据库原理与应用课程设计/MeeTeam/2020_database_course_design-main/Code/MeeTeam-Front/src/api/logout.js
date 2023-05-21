import request from '@/utils/request'

  export function logout() {
    /*注销 */
    return request({
      url: '/Logout/logout',
      method: 'get',
    })
  }