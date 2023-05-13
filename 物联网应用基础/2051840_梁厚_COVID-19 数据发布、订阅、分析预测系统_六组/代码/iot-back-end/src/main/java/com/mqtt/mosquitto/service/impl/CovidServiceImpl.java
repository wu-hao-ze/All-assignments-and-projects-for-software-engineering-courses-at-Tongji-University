package com.mqtt.mosquitto.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.mqtt.mosquitto.common.HTMLmail;
import com.mqtt.mosquitto.entity.today;
import com.mqtt.mosquitto.entity.total;
import com.mqtt.mosquitto.entity.user;
import com.mqtt.mosquitto.mapper.TodayMapper;
import com.mqtt.mosquitto.mapper.TotalMapper;
import com.mqtt.mosquitto.mapper.UserMapper;
import com.mqtt.mosquitto.service.CovidService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.stereotype.Service;

import javax.mail.MessagingException;
import javax.mail.internet.MimeMessage;
import java.util.ArrayList;
import java.util.List;

@Service
public class CovidServiceImpl implements CovidService {
    @Autowired
    TodayMapper todayMapper;
    @Autowired
    TotalMapper totalMapper;
    @Autowired
    UserMapper userMapper;
    @Autowired
    JavaMailSenderImpl mailSender;
    public boolean saveData(List<total> totalList, List<today> todayList){
        boolean re=false;
        boolean re1=false;
        try{
            re=todayMapper.insertBatch(todayList);
        }catch (Exception e){
            System.out.println(e);
        }
        try {
        re1=totalMapper.insertBatch(totalList);
        }catch (Exception e){
            System.out.println(e);
        }
        return re&&re1;
    }
    public void sendEmail(){
        //System.out.println("222");
        List<today> todayList=todayMapper.selectList(null);
        String message= HTMLmail.HTML(todayList);
        QueryWrapper queryWrapper=new QueryWrapper();
        queryWrapper.eq("subscribe","1");
        List<user>users=new ArrayList<>();
        users=userMapper.selectList(queryWrapper);
        System.out.println("用户列表"+users);
        users.forEach((user)->{
            MimeMessage mimeMessage = mailSender.createMimeMessage();
            MimeMessageHelper helper;
            try {
                helper = new MimeMessageHelper(mimeMessage, true);

                helper.setSubject("疫情信息速递");

                helper.setText(message,true);
                helper.setTo(user.getEmail());
                helper.setFrom("417695971@qq.com");
            } catch (MessagingException err) {
                System.out.println(err.getMessage());
                throw new RuntimeException(err);
            }
            mailSender.send(mimeMessage);
        });
    }
    public today todayData(String area){
        QueryWrapper<today> queryWrapper=new QueryWrapper<>();
        queryWrapper.eq("area",area);
        return todayMapper.selectOne(queryWrapper);
    }
    public total totalData(String area){
        QueryWrapper<total> queryWrapper=new QueryWrapper<>();
        queryWrapper.eq("area",area);
        return totalMapper.selectOne(queryWrapper);
    }
}
