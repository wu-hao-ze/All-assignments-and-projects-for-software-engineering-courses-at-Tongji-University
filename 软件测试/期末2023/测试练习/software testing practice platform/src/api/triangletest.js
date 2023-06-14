import request from "../utils/request"

export const testtriangle = data => {
    return request({
        method: 'POST',
        url: '/api/triangle/test',
        data
    })
}

export const uploadfile = data => {
    return request({
        method: 'POST',
        config: { 'Content-Type': 'multipart/form-data' },
        data,
        responseType: 'blob',
        url: '/api/triangle/upload',
    })
}