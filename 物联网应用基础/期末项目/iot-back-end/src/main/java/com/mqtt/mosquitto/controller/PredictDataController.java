package com.mqtt.mosquitto.controller;

import com.mqtt.mosquitto.common.JsonResult;
import com.mqtt.mosquitto.service.impl.PolynomialFitting;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;

@Configuration
@RestController
@RequestMapping("predict")
public class PredictDataController {
    @Bean
    PolynomialFitting polynomialFitting(){
        return new PolynomialFitting(30);
    }
    
    @PostMapping("predict/{num_array}")
    public JsonResult PredictData(@PathVariable String num_array){
        final String preData;
        //传入类似“1,2,1,34,56”的String
        //这里将String转成Integer[]
        List<Integer> dataList = Arrays.stream(num_array.split(",")).map(Integer::parseInt).collect(Collectors.toList());
        Integer[] dataArray = new Integer[30];
        for(int i = 0; i < dataList.size();i++){
            dataArray[i] = dataList.get(i);
        }
        //调用预测类返回预测数据
        preData = polynomialFitting().polyFit(dataArray);
        /*
        try {
            //传入类似“1,2,1,34,56”的String
            //这里将String转成Integer[]
            List<Integer> dataList = Arrays.stream(num_array.split(",")).map(Integer::parseInt).collect(Collectors.toList());
            Integer[] dataArray = new Integer[0];
            for(int i = 0; i < dataList.size();i++){
                dataArray[i] = dataList.get(i);
            }
            //调用预测类返回预测数据
            PolynomialFitting polynomialFitting = null;
            preData = polynomialFitting.polyFit(dataArray);
        }catch (Exception e){
            return JsonResult.isError(10001,"未知错误");
        }*/
        return JsonResult.isOk(preData);
    }
}
