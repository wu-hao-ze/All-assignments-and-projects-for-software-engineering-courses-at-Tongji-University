package com.mqtt.mosquitto.publish;

import lombok.extern.slf4j.Slf4j;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.persist.MemoryPersistence;

/*
 * @ClassName
 * @Decription TOO mqtt服务器，发布主题
 * @Author HanniOvO
 */
@Slf4j
public class PublishSample {

    //定义一个发布方法
        public  void pub() {

            //主题名
            String topic = "mqtt/test";
            //消息内容
            String content = "send email to user";
            //QoS服务质量等级
            int qos = 1;
            //访问服务器地址
            String broker = "tcp://gz2vip.91tunnel.com:10047";
            //账号
            String userName = "wyyxyy1";
            //密码
            String password = "135135";
            String clientId = "pubClient";
            // 内存存储
            MemoryPersistence persistence = new MemoryPersistence();

            try {
                // 创建客户端
                MqttClient sampleClient = new MqttClient(broker, clientId, persistence);
                // 创建链接参数
                MqttConnectOptions connOpts = new MqttConnectOptions();
                // 在重新启动和重新连接时记住状态
                connOpts.setCleanSession(false);
                // 设置连接的用户名
                connOpts.setUserName(userName);
                connOpts.setPassword(password.toCharArray());
                // 建立连接
                sampleClient.connect(connOpts);
                // 创建消息
                MqttMessage message = new MqttMessage(content.getBytes());
                // 设置消息的服务质量
                message.setQos(qos);
                // 发布消息
                sampleClient.publish(topic, message);
                // 断开连接
                sampleClient.disconnect();
                // 关闭客户端
                sampleClient.close();

            }
            //异常抛出
            catch (MqttException m) {
               log.error("reason " + m.getReasonCode());
                log.error("msg " + m.getMessage());
                log.error("loc " + m.getLocalizedMessage());
                log.error("cause " + m.getCause());
                log.error("excep " + m);
                m.printStackTrace();
            }
        }
}
