import request from '@/utils/request'

/*创建项目 */
export function createprojectlist(params) {
  return request({
    url: '/Project/AddProject',
    method: 'post',
    params: {
      project_id: params.project_id,
      project_name: params.project_name,
      project_background: params.project_background,
      project_introduction: params.project_introduction,
      project_content: params.project_content,
      project_status: params.project_status,
      due: params.due,
      project_progress: params.project_progress,
    }
  })
}