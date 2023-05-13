/**
 * > 组件名:mask-alert
 * > 调用方式第一步:JSON引入"model":"../components/model/model"
 * > 调用方式第二步:页面引入<model></model>
0 */
Component({
  properties: {
    modalStart: {
      type: Boolean,
      value: false
    },
  },
  /**
   * 注意:diffid[1:成功中奖弹窗;2:未中奖弹窗;]
   */
  data: {
    logo: "../../images/logos.png"
  },
  methods: {
    /**
     * 取消事件
     */
    getCance: function () {
      console.log("getCance");
      this.setData({
        modalStart: false
      })

    },
    getse() {
      this.setData({
        modalStart: false,
      })
    },
    /**
     * 组件中传值
     */
    getOenMask(res) {
      wx.setStorageSync("userinfo", res.detail.userInfo);
      var sessionKey = wx.getStorageSync("sessionKey");
      console.log("res", res.detail.userInfo);
      return;
    },
    // 显示
    getShow(res) {
      console.log("获取==>", res);
      this.setData({
        modalStart: true
      })
    },
  }
})