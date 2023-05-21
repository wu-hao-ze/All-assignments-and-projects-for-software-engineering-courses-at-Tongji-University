import request from '@/utils/request'

export function get_project(params) {
    /*根据id获取单条公告内容*/
    return request({
        url: '/Project/get_by_id',
        method: 'get',
        params: {
            id: params.id
        }
    })
}

export function get_username(params) {
    /*根据id获取项目相关的id和duty*/
    return request({
        url: '/User_Project/get_by_id',
        method: 'get',
        params: {
            pid: params.pid,
            uid: params.uid
        }
    })
}

export function get_leader(params) {
    /*根据id获取项目的发布者*/
    return request({
        url: '/User_Project/get_leader',
        method: 'get',
        params: {
            id: params.id
        }
    })
}

export function get_member(params) {
    /*根据id获取项目相关的id和duty*/
    return request({
        url: '/User_Project/get_member',
        method: 'get',
        params: {
            pid: params.pid,
            duty: params.duty
        }
    })
}

export function get_require(params) {
    /*根据id获取单条公告内容*/
    return request({
        url: '/GroupRequirement/get_by_id',
        method: 'get',
        params: {
            id: params.id
        }
    })
}

export function get_eva() {
    /*获取评论*/
    return request({
        url: '/Project_Evaluation/SelectAllProject_Evaluation',
        method: 'get',
    })
}


export function detailnum(num) {
    if (num === 2)
        return "创新创业";
    else if (num === 3)
        return "数学建模";
    else if (num === 4)
        return "学科竞赛";
    else if (num === 5)
        return "其他竞赛";
    else if (num === 7)
        return "新生院";
    else if (num === 8)
        return "建筑与城市规划学院";
    else if (num === 9)
        return "土木工程学院";
    else if (num === 10)
        return "机械与能源工程学院";
    else if (num === 11)
        return "经济与管理学院";
    else if (num === 12)
        return "环境科学与工程学院";
    else if (num === 13)
        return "材料科学与工程学院";
    else if (num === 14)
        return "电子与信息工程学院";
    else if (num === 15)
        return "人文学院";
    else if (num === 16)
        return "汽车学院";
    else if (num === 17)
        return "软件学院";
    else if (num === 18)
        return "其他学院";
}