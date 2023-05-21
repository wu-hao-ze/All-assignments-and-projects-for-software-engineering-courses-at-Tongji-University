import request from '@/utils/request'

export function getProject(params) {
    /*创建组队需求 */
    return request({
        url: '/UserProject/GetProjectID',
        method: 'get',
        params: {
            ID: params.ID,
        }
    })
}

export function getProjectname(params) {
    /*创建组队需求 */
    return request({
        url: '/UserProject/GetProjectName',
        method: 'get',
        params: {
            project_id: params.project_id,
        }
    })
}

export function getfacorite(params){
    return request({
        url: '/UserProject/GetFacorite',
        method: 'get',
        params: {
            ID: params.ID,
        }
    })
}