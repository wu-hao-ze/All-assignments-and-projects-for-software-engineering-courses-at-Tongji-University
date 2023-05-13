import request from '@/utils/request'

export function getRe(params) {
    /*获取用户关系 */
    return request({
        url: '/Follow/getRe',
        method: 'get',
        params: {
            MyID: params.MyID,
            ID: params.ID
        }
    })
}

export function pullRe(params) {
    /*创建组队需求 */
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