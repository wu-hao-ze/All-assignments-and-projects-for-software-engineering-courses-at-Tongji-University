import request from '@/utils/request'

//获取查询列表结果
export function fetchList(query) {
    return request({
        url: '/Project/project_query',
        method: 'get',
        params: {
            s: query
        }
    })
}