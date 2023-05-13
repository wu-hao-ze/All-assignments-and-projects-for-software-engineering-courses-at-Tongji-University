package com.mqtt.mosquitto.common;

import com.mqtt.mosquitto.entity.today;

import java.util.List;

public class HTMLmail {
    public static String HTML(List<today> todayList){
        String str="<html>\n" +
                "<head>\n" +
                "<style>\n" +
                "table,table tr th,table tr td{\n" +
                "border:1px solid #ccc;\n" +
                "}\n" +
                "table{\n" +
                "width:600px\n" +
                "border-collapse:collapse;\n" +
                "</style>\n" +
                "</head>\n" +
                "<body>\n" +
                "   <h2>这是您订阅的疫情信息!</h2>\n" +
                "   <table>\n" +
                "   <tr>\n" +
                "       <td>地区</td>\n" +
                "       <td>确诊</td>\n" +
                "       <td>治疗</td>\n" +
                "       <td>死亡</td>\n" +
                "   </tr>\n";
        for (int i=0;i< todayList.size();i++){
            today today1=todayList.get(i);
            str+="  <tr>\n" +
                    "   <td>"+today1.getArea()+"\n" +
                    "   <td>"+today1.getConfirm()+"\n" +
                    "   <td>"+today1.getHeal()+"\n" +
                    "   <td>"+today1.getDead()+"\n" +
                    "</tr>\n";
        }
        str+="<p>退订请点击http://localhost:8081/user/td?email=wyyxyy1@qq.com</p>\n" +
             "</table>\n" +
             "</body>\n" +
             "</html>\n";
        return str;
    }
}
