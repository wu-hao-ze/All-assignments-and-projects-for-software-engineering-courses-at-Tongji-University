package com.mqtt.mosquitto.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.mqtt.mosquitto.entity.user;
import org.apache.ibatis.annotations.Mapper;

@Mapper
public interface UserMapper extends BaseMapper<user> {

}
