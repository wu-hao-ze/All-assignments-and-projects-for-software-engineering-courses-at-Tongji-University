import request from "../utils/request"

export const testcash = data => {
    return request({
        method: 'POST',
        url: '/api/cash/test',
        data
    })
}

export const uploadfile = data => {
    return request({
        method: 'POST',
        config: { 'Content-Type': 'multipart/form-data' },
        data,
        responseType: 'blob',
        url: '/api/cash/upload',
    })
}