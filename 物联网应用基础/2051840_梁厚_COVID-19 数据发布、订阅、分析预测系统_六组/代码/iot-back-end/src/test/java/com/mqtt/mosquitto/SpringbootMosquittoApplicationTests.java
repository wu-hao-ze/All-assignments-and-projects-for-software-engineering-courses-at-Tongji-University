package com.mqtt.mosquitto;

import com.mqtt.mosquitto.publish.PublishSample;
import com.mqtt.mosquitto.subscribe.SubscribeSample;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.junit4.SpringRunner;

@RunWith(SpringRunner.class)
@SpringBootTest
public class SpringbootMosquittoApplicationTests {

	@Test
	public void contextLoads() throws MqttException {
		SubscribeSample subscribeSample=new SubscribeSample();
		subscribeSample.sub();

		PublishSample publishSample=new PublishSample();
		publishSample.pub();
	}

}
