package com.mqtt.mosquitto.schedule;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import com.mqtt.mosquitto.common.SpringUtils;
import com.mqtt.mosquitto.entity.today;
import com.mqtt.mosquitto.entity.total;
import com.mqtt.mosquitto.service.CovidService;
import org.jsoup.Jsoup;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Configuration;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.scheduling.annotation.Scheduled;

import java.io.IOException;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Configuration      //1.主要用于标记配置类，兼备Component的效果。
@EnableScheduling   // 2.开启定时任务

public class updateData {
    private ApplicationContext applicationContext = SpringUtils.getApplicationContext();
    CovidService covidService=applicationContext.getBean(CovidService.class);
    @Scheduled(cron = "0 0 12 * * ?")
    private void updateData() throws IOException {
        System.err.println("执行静态定时任务时间: " + LocalDateTime.now());
        String resultBody = Jsoup.connect("https://c.m.163.com/ug/api/wuhan/app/data/list-total")
                .ignoreContentType(true)
                .execute().body();
        List<total> totalList=new ArrayList<>();
        List<today> todayList=new ArrayList<>();
        //2 转换成Object类型
        JSONObject jsonObject = JSON.parseObject(resultBody);
        //3 获取data部分
        JSONObject data = jsonObject.getJSONObject("data");

        JSONArray countryList=data.getJSONArray("areaTree");
        JSONObject china=countryList.getJSONObject(2);
        JSONObject chinaTotal=china.getJSONObject("total");
        JSONObject chinaToday=china.getJSONObject("today");
        totalList.add(new total(
                chinaTotal.getInteger("confirm"),
                chinaTotal.getInteger("dead"),
                chinaTotal.getInteger("heal"),
                "中国"));
        todayList.add(new today(
                chinaToday.getInteger("confirm"),
                chinaToday.getInteger("dead"),
                chinaToday.getInteger("heal"),
                "中国"));
        JSONArray areaList=china.getJSONArray("children");
        for (int i=0;i<areaList.size();i++){
            JSONObject json= areaList.getJSONObject(i);
            JSONObject todayJson=json.getJSONObject("today");
            JSONObject totalJson=json.getJSONObject("total");
            totalList.add(new total(
                    totalJson.getInteger("confirm"),
                    totalJson.getInteger("dead"),
                    totalJson.getInteger("heal"),
                    json.getString("name")));
            todayList.add(new today(
                    todayJson.getInteger("confirm"),
                    todayJson.getInteger("dead"),
                    todayJson.getInteger("heal"),
                    json.getString("name")));
        }

        covidService.saveData(totalList,todayList);

    }

}
