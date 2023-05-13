import request from '@/utils/request'

export function deleteproject(params) {
    return request({
      url: '/Project/deleteProject',
      method: 'delete',
      params:{
        project_id:params.project_id
      }
    })
  }

      /*删除组队需求 */
  export function deleteapply(params) {
      return request({
        url: '/User_Project/DeleteApply',
        method: 'delete',
        params:{
          pid:params.pid,
          uid:params.uid
        }
      })
    }
  

  export function Audit(params) {
        return request({
          url: '/User_Project/Audit',
          method: 'post',
          params:{
            pid:params.pid,
            uid:params.uid,
            dy:params.dy
          }
        })
  }


  export function UpdateProgress(params) {
    return request({
      url: '/Project/UpdateProgress',
      method: 'post',
      params:{
        project_id:params.project_id,
        project_progress:params.project_progress,
      }
    })
  }


  export function Changeprojectlist(params) {
    return request({
      url: '/Project/Changeprojectlist',
      method: 'post',
      params:{
        project_id:params.project_id,
        project_name:params.project_name,
        project_background:params.project_background,
        project_introduction:params.project_introduction,
        project_content:params.project_content,
        due:params.due,
      }
    })
  }

  
  export function Changerequirelist(params) {
    return request({
      url: '/GroupRequirement/Changerequirelist',
      method: 'post',
      params:{
        purpose:params.purpose,
        team_type:params.team_type,
        team_limit:params.team_limit,
        details:params.details,       
        require_status:params.require_status,
        project_id:params.project_id,
        region:params.region,
        team_type_details:params.team_type_detail,
      }
    })
  }
