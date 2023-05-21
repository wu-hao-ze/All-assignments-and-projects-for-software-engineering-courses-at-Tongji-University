import request from '@/utils/request'

export function openfile(params) {
    return request({
        url: '/File/OpenFile',
        method: 'get',
        params: {
            target: params.target,
            id: params.id
        }
    })
}

export function findimg(params) {
    return request({
        url: 'File/OpenFile',
        method: 'get',
        params: {
            target: "project",
            id: params.id
        }
    })
}

export function copyimg(params) {
    return request({
        url: 'File/CopyFile',
        method: 'POST',
        params: {
            project_id: params.project_id,
            filename: params.filename
        }
    })
}