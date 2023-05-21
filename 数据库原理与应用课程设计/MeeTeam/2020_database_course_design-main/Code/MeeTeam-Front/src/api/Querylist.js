import request from '@/utils/request'

export function fetchList(query) {//获取查询列表结果
    return request({
        url: '/Project/project_query',
        method: 'get',
        params: {
            s: query
        }
    })
}