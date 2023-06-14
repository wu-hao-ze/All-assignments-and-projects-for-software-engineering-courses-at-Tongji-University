import request from "../utils/request"

export const testsales = data => {
    return request({
        method: 'POST',
        url: '/api/sales/test',
        data
    })
}

export const uploadfile = data => {
    return request({
        method: 'POST',
        config: { 'Content-Type': 'multipart/form-data' },
        data,
        responseType: 'blob',
        url: '/api/sales/upload',
    })
}