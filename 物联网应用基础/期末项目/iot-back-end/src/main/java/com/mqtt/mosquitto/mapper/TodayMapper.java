package com.mqtt.mosquitto.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.mqtt.mosquitto.entity.today;
import org.apache.ibatis.annotations.Insert;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

import java.util.List;

@Mapper
public interface TodayMapper extends BaseMapper<today> {
    @Insert("<script>" +
            "REPLACE INTO today(confirm,heal,dead,area)VALUES" +
            "<foreach collection='todayList' item='today'   separator=','> " +
            "(#{today.confirm},#{today.heal},#{today.dead},#{today.area})" +
            "</foreach>" +
            "</script>")//批量建议手写sql
    boolean insertBatch(@Param("todayList") List<today> todayList);
}
