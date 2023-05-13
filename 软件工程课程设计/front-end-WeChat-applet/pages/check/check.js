// pages/check/check.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    shopcomment: {},
    text: "",
    score: 0,
    time: "",
    store_text: ""
  },
  back: function (e) {
    wx.switchTab({
      url: '../AllOrder/AllOrder'
    })
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var shopcomment = JSON.parse(wx.getStorageSync("shopcomment"));
    this.setData({
      shopcomment: shopcomment,
    })
    console.log(shopcomment)
    var that = this
    wx.request({
      url: 'http://127.0.0.1:8000/api/seproject/getComment',
      method: "GET",
      data: {
        order_id: that.data.shopcomment.order_id,
      },
      success: function (res) {
        console.log(res)
        that.setData({
          text: res.data.data.user_text,
          score: res.data.data.user_score,
          time: res.data.data.user_time,
          store_text: res.data.data.store_text
        })
      }
    })
  },
})