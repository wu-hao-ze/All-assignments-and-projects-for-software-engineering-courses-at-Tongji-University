package com.mqtt.mosquitto.common;


import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class Result<T> {

    private boolean success;
    private int code;
    private String msg;
    private T data;

    public static <T> Result<T> success(T data){
        return new Result<>(true,200,"success",data);
    }
    public static <T> Result<T> fail(int code, String msg){
        return new Result<>(true,code,msg,null);
    }

}

