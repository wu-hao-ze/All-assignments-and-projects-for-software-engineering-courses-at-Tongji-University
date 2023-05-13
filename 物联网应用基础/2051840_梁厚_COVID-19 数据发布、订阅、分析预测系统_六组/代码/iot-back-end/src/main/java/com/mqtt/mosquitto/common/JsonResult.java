package com.mqtt.mosquitto.common;

public class JsonResult {
    private Integer code;//状态码
    private Object data;//数据
    private String msg;//信息

    /**
     * 正确时返回的信息
     */
    public static JsonResult isOk(Object data) {
        return new JsonResult(200,data,"success");
    }

    /**
     * 错误时返回的信息
     */
    public static JsonResult isError(Integer code,String msg) {
        return new JsonResult(code,null,msg);
    }

    public JsonResult() {
        super();
    }
    public JsonResult(Integer code, Object data, String msg) {
        super();
        this.code = code;
        this.data = data;
        this.msg = msg;
    }

    public Integer getCode() {
        return code;
    }

    public void setCode(Integer code) {
        this.code = code;
    }

    public Object getData() {
        return data;
    }

    public void setData(Object data) {
        this.data = data;
    }

    public String getMsg() {
        return msg;
    }

    public void setMsg(String msg) {
        this.msg = msg;
    }
}
