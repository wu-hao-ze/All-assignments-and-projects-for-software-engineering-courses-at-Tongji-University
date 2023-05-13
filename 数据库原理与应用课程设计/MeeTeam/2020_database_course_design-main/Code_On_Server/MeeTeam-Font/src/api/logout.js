import request from '@/utils/request'

/*注销 */
export function logout() {
  return request({
    url: '/Logout/logout',
    method: 'get',
  })
}