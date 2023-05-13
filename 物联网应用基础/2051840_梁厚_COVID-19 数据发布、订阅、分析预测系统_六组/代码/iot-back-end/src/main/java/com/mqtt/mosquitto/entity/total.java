package com.mqtt.mosquitto.entity;

import com.baomidou.mybatisplus.annotation.TableName;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
@TableName("total")
public class total {
    int confirm;
    int dead;
    int heal;
    String area;
}
