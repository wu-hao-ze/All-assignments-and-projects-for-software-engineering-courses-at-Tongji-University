package com.mqtt.mosquitto.controller;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import com.mqtt.mosquitto.common.Result;
import com.mqtt.mosquitto.entity.today;
import com.mqtt.mosquitto.entity.total;
import com.mqtt.mosquitto.publish.PublishSample;
import com.mqtt.mosquitto.service.CovidService;
import com.mqtt.mosquitto.subscribe.SubscribeSample;
import org.jsoup.Jsoup;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

@RestController
@RequestMapping("covid")
public class CovidDataController {
    @Autowired
    CovidService covidService;
    @GetMapping("")
    public Result<String> test(){
        try {
            SubscribeSample subscribeSample=new SubscribeSample();
            subscribeSample.sub();
        }catch (Exception e){
            System.out.println(e);
        }

        PublishSample publishSample=new PublishSample();
        publishSample.pub();
        return Result.success("成功");
    }
    @GetMapping("data")
    public Result<String> data() throws IOException {
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
        if (covidService.saveData(totalList,todayList)){
        return Result.success("success");
        }
        return Result.fail(10001,"error");
    }
    @GetMapping("today")
    public Result<today> today(String area){
        return Result.success(covidService.todayData(area));
    }
    @GetMapping("total")
    public Result<total> total(String area){
        return Result.success(covidService.totalData(area));
    }
}
