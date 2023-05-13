import request from '@/utils/request'

/*获取用户关系 */
export function getRe(params) {
    return request({
        url: '/Follow/getRe',
        method: 'get',
        params: {
            MyID: params.MyID,
            ID: params.ID
        }
    })
}

/*创建组队需求 */
export function pullRe(params) {
    return request({
        url: '/Follow/pullRe',
        method: 'get',
        params: {
            MyID: params.MyID,
            ID: params.ID,
            relationship: params.relationship
        }
    })
}