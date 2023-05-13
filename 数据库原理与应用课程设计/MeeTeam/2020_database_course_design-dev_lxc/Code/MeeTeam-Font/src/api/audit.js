import request from '@/utils/request'

export function audit(params) {//获取查询列表结果
    return request({
        url: '/Audit/Project_Audit',
        method: 'post',
        params: {
            project_id: params.project_id,
            admin_id: params.admin_id,
            result: params.result,
        }
    })
}