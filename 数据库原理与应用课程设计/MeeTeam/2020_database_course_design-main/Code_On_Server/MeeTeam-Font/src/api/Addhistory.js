import request from '@/utils/request'

export function addviewhistory(params) {
    return request({
        url: '/User/AddViewHistory',
        method: 'post',
        params: {
            user_id: params.user_id,
            project_id: params.project_id,
        }
    })
}