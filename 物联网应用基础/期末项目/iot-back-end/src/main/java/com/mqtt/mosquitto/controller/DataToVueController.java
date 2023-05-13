package com.mqtt.mosquitto.controller;

import com.mqtt.mosquitto.common.JsonResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.List;

@RestController
@RequestMapping("data")
public class DataToVueController {
    @GetMapping("")
    public JsonResult CovidDataToVue()throws Exception{
        final String fileData;
        try {
            String fileName = "D:\\RiverFiles\\mqtt-message.txt";
            Path path = Paths.get(fileName);
            byte[] bytes = Files.readAllBytes(path);
            List<String> allLines = Files.readAllLines(path, StandardCharsets.UTF_8);
            fileData = String.join(",", allLines);
        }catch (Exception e){
            return JsonResult.isError(10001,"未知错误");
        }
        return JsonResult.isOk(fileData);
    }
}
