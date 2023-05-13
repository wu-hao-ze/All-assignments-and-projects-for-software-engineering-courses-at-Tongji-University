import request from '@/utils/request'

// export function getusername(params) {
//   /*组队需求的表格*/
//   return request({
//     url: '/Login/GetUsername',
//     method: 'get',
//     params: {
//       user_id: params.user_id,
//     }
//   })
// }

// export function getuserid(params) {
//   return request({
//     url: '/Login/GetUserid',
//     method: 'get',
//     params: {
//       user_name: params.user_name,
//     }
//   })
// }
export function login(params) {
  /*登陆 */
  return request({
    url: '/Login/login',
    method: 'get',
    params: {
      user_id: params.user,
      password: params.pass
    }
  })
}
export function adminlogin(params) {
  /*登陆 */
  return request({
    url: '/Login/adminlogin',
    method: 'get',
    params: {
      admin_id: params.user,
      password: params.pass
    }
  })
}
export function adduser(params) {
  /*用户注册*/
  return request({
    url: '/Login/addnormal',
    method: 'post',
    params: {
      user_id: params.user_id,
      password: params.password,
      user_name: params.user_name,
      gender: params.gender,
      contact_info: params.contact_info,
      institution: params.institution,
      major: params.major,
      introduction: params.introduction
    }
  })
}
export function getuser() {
  return request({
    url: '/Login/SelectAlluser',
    method: 'get',
  })
}

export function isUseruni(params) {
  /*组队需求的表格*/
  return request({
    url: '/Login/IsUserUni',
    method: 'get',
    params: {
      user_id: params.user_id,
    }
  })
}

export function SendMail(params) {
  /*需要发送验证码*/
  return request({
    url: '/Login/SendMailUseZj',
    method: 'get',
    params: {
      aa: params.mail_id,
    }
  })
}

export function ChangePass(params) {
  /*修改密码*/
  return request({
    url: '/Login/UpdateUser',
    method: 'post',
    params: {
      user_id: params.user_id,
      password: params.password,
    }
  })
}

export function IftrueMail(params) {
  /*验证邮箱用户是否正确*/
  return request({
    url: '/Login/Iftruemail',
    method: 'get',
    params: {
      user_id: params.user_id,
      contact_info: params.contact_info,
    }
  })
}



