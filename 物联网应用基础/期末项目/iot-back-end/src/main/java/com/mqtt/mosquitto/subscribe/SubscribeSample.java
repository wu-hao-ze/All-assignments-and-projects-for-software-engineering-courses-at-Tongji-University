package com.mqtt.mosquitto.subscribe;


import com.mqtt.mosquitto.common.SpringUtils;

import com.mqtt.mosquitto.service.CovidService;

import lombok.extern.slf4j.Slf4j;

import org.eclipse.paho.client.mqttv3.*;
import org.eclipse.paho.client.mqttv3.persist.MemoryPersistence;
import org.springframework.context.ApplicationContext;

import java.io.FileWriter;
import java.io.IOException;

/*
 * @ClassName
 * @Decription TOO mqtt,服务器订阅消息
 * @Author HanniOvO
 */
@Slf4j
public class SubscribeSample {

    //定义一个订阅方法
    private ApplicationContext applicationContext = SpringUtils.getApplicationContext();
    private CovidService covidService=applicationContext.getBean(CovidService.class);




    public void sub() throws MqttException {
        System.out.println("链接MQTT");
        //System.out.println(queryList("wyyxyy1@qq.com"));
        //收到的消息
        MqttMessage thisMes;
        //服务器主机
        String HOST = "tcp://gz2vip.91tunnel.com:10047";
        //主机
        String TOPIC = "mqtt/test";
        //QoS服务质量等级
        int qos = 1;
        //订阅Id
        String clientid = "subClient";
        //用户名
        String userName = "wyyxyy1";
        //密码
        String passWord = "135135";
        try {
            // host为主机名，test为clientid即连接MQTT的客户端ID，一般以客户端唯一标识符表示，MemoryPersistence设置clientid的保存形式，默认为以内存保存
            MqttClient client = new MqttClient(HOST, clientid, new MemoryPersistence());
            // MQTT的连接设置
            MqttConnectOptions options = new MqttConnectOptions();
            // 设置是否清空session,这里如果设置为false表示服务器会保留客户端的连接记录，这里设置为true表示每次连接到服务器都以新的身份连接
            options.setCleanSession(true);
            // 设置连接的用户名
            options.setUserName(userName);
            // 设置连接的密码
            options.setPassword(passWord.toCharArray());
            // 设置超时时间 单位为秒
            options.setConnectionTimeout(10);
            // 设置会话心跳时间 单位为秒 服务器会每隔1.5*20秒的时间向客户端发送个消息判断客户端是否在线，但这个方法并没有重连的机制
            options.setKeepAliveInterval(20);
            // 设置回调函数
            client.setCallback(new MqttCallback() {

                public void connectionLost(Throwable cause) {
                    System.out.println("connectionLost...连接主机");
                }

                public void messageArrived(String topic, MqttMessage message) throws Exception {
                    System.out.println("topic...主题:"+topic);
                    System.out.println("Qos:"+message.getQos());
                    String thisMes = "message content...消息：:"+new String(message.getPayload());
                    System.out.println(thisMes);
                    try{
                        /*File file = new File("D:\\RiverFiles\\mqtt-message.txt");
                        //if file doesnt exists, then create it
                        if(!file.exists()){
                            file.createNewFile();
                        }*/
                        //true = append file
                        FileWriter fileWritter = new FileWriter("D:\\RiverFiles\\mqtt-message.txt",true);
                        fileWritter.append(thisMes);
                        fileWritter.close();
                    }catch(IOException e){
                        e.printStackTrace();
                    }
                    covidService.sendEmail();

                }

                public void deliveryComplete(IMqttDeliveryToken token) {
                    System.out.println("deliveryComplete---------"+ token.isComplete());
                }

            });
            client.connect(options);
            //订阅消息
            client.subscribe(TOPIC, qos);
        } catch (Exception e) {
            e.printStackTrace();
        }

    }
}
