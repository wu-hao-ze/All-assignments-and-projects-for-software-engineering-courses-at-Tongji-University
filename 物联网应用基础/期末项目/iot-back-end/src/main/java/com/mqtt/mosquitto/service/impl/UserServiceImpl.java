package com.mqtt.mosquitto.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.baomidou.mybatisplus.core.conditions.update.UpdateWrapper;
import com.mqtt.mosquitto.entity.user;
import com.mqtt.mosquitto.mapper.UserMapper;
import com.mqtt.mosquitto.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;


@Service
public class UserServiceImpl implements UserService {
    @Autowired
    UserMapper userMapper;
    public user subscribe(String email){
        user user1=new user(email,1);
        try{
            QueryWrapper queryWrapper=new QueryWrapper();
            queryWrapper.eq("email",email);
            if(userMapper.selectList(queryWrapper)!=null){
                UpdateWrapper updateWrapper =new UpdateWrapper();
                updateWrapper.set("subscribe","1");
                updateWrapper.eq("email",email);
                if (userMapper.update(null,updateWrapper)==0){
                    throw new RuntimeException("订阅失败!");
                }
            }
            else{
                userMapper.insert(user1);
            }
        }catch (Exception e){
            System.out.println(e.getMessage());
            return null;
        }
        return user1;
    }
    public String td(String email){
        try{
            user user1=new user();
            user1.setEmail(email);
            user1.setSubscribe(0);
            UpdateWrapper updateWrapper =new UpdateWrapper();
            updateWrapper.set("subscribe","0");
            updateWrapper.eq("email",email);
            if (userMapper.update(null,updateWrapper)==0){
                throw new RuntimeException("退订失败!");
            }
        }catch (Exception e){
            return e.getMessage();
        }
        return "退订成功";
    }

}
