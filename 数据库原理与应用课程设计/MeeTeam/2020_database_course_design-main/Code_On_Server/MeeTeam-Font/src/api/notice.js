import request from '@/utils/request'

/*根据id获取单条公告内容*/
export function get_notice(params) {
    return request({
        url: '/notice/get_by_id',
        method: 'get',
        params: {
            id: params.id
        }
    })
}


    /*根据id获取单条公告内容*/
export function get_all() {
    return request({
        url: '/notice/get_all',
        method: 'get',
    })
}

export function create_notice(params) {
    /*创建新公告*/
    return request({
        url: '/notice/post',
        method: 'post',
        params: { not: params }
    })
}

export function delete_notice(params) {
    return request({
        url: '/notice/delete_by_id',
        method: 'delete',
        params: {
            id: params.id
        }
    })
}

export function fetchList(query) {//获取查询列表结果
    return request({
        url: '/Notice/notice_query',
        method: 'get',
        params: {
            s: query
        }
    })
}