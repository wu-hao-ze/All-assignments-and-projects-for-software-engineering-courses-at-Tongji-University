package com.mqtt.mosquitto.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.mqtt.mosquitto.entity.total;
import org.apache.ibatis.annotations.Insert;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

import java.util.List;

@Mapper
public interface TotalMapper extends BaseMapper<total> {
    @Insert("<script>" +
            "REPLACE INTO total(confirm,heal,dead,area)VALUES" +
            "<foreach collection='totalList' item='total'   separator=','> " +
            "(#{total.confirm},#{total.heal},#{total.dead},#{total.area})" +
            "</foreach> " +
            "</script>")//批量建议手写sql
    boolean insertBatch(@Param("totalList") List<total> totalList);
}
