import request from '@/utils/request'

  export function createprojectlist(params) {
    /*创建项目 */
    return request({
      url: '/Project/AddProject',
      method: 'post',
      params:{
        project_id:params.project_id,
        project_name:params.project_name,
        project_background:params.project_background,
        project_introduction:params.project_introduction,
        project_content:params.project_content,
        project_status:params.project_status,
        due:params.due,
        project_progress:params.project_progress,
      }
    })
  }

  export function createuser_project(params) {
    /*创建项目 */
    return request({
      url: '/User_Project/AddUser_Project',
      method: 'post',
      params:{
        project_id:params.project_id,
        user_id:params.user_id,
        duty:params.duty,
      }
    })
  }

  export function createrequirelist(params) {
    /*创建组队需求 */
    return request({
      url: '/GroupRequirement/AddGroupRequirement',
      method: 'post',
      params:{
        require_id:params.require_id,
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

  export function createevalist(params) {
    /*创建组队需求 */
    return request({
      url: '/Project_Evaluation/AddProject_Evaluation',
      method: 'post',
      params:{
        project_eva_id:params.project_eva_id,
        project_id:params.project_id,
        evaluator_id:params.evaluator_id,
        eva_content:params.eva_content,
      }
    })
  }

  export function createfacoritelist(params) {
    /*创建组队需求 */
    return request({
      url: '/Facorite/AddFacorite',
      method: 'post',
      params:{
        facorite_id:params.facorite_id,
        facorite_name:params.facorite_name,
        owner_id:params.owner_id,
      }
    })
  }