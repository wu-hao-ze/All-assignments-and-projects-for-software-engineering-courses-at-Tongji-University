import request from '@/utils/request'

//获取查询列表结果
export function audit(params) {
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


export function recommend(param) {
    return request({
        url: 'Project/Recommend_Project',
        method: 'post',
        params: {
            project_id: param.project_id,
        }
    })
}
