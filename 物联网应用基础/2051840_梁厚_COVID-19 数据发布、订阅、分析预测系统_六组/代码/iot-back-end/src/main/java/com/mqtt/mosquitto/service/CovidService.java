package com.mqtt.mosquitto.service;

import com.mqtt.mosquitto.entity.today;
import com.mqtt.mosquitto.entity.total;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public interface CovidService {
    boolean saveData(List<total> totalList,List<today> todayList);
    void sendEmail();
    total totalData(String area);
    today todayData(String area);
}
