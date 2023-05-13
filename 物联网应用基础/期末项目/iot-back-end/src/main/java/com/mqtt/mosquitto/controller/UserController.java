package com.mqtt.mosquitto.controller;

import com.mqtt.mosquitto.common.Result;
import com.mqtt.mosquitto.entity.user;
import com.mqtt.mosquitto.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("user")
public class UserController {
    @Autowired
    UserService userService;
    @GetMapping("")
    public Result<user> subscribe(String email){
        user re=null;
        try {
            re=userService.subscribe(email);
        }catch (Exception e){
            return Result.fail(10001,"订阅失败，未知错误");
        }
        if (re==null)
            return Result.fail(10001,"订阅失败");
        return Result.success(re);
    }
    @GetMapping("td")
    public Result<String> td(String email){
        String re=null;
        try {
            re=userService.td(email);
        }catch (Exception e){
            return Result.fail(10001,"td失败");
        }
        if (re==null){
            return Result.fail(10001,"td失败");
        }
        return Result.success(re);
    }
}
