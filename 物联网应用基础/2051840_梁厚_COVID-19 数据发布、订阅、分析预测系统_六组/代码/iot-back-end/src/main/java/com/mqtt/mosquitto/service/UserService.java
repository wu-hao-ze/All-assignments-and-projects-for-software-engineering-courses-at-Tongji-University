package com.mqtt.mosquitto.service;

import com.mqtt.mosquitto.entity.user;
import org.springframework.stereotype.Service;


@Service
public interface UserService {
     user subscribe(String email);
     String td(String email);


}
