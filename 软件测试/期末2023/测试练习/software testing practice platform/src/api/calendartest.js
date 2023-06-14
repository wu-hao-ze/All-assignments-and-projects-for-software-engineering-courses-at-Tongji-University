import request from "../utils/request"

export const testcalendar = data => {
  return request({
    method: 'POST',
    url: '/api/calendar/test',
    data
  })
}

export const uploadfile = data => {
  return request({
    method: 'POST',
    config: { 'Content-Type': 'multipart/form-data' },
    data,
    responseType: 'blob',
    url: '/api/calendar/upload',
  })
}