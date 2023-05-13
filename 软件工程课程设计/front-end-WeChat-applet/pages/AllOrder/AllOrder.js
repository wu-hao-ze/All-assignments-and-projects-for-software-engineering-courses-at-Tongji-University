// pages/AllOrder/AllOrder.js
Page({
  /**
   * 页面的初始数据
   */
  data: {
    orderList: [],
  },

  getComment(e) {
    var shopcomment = JSON.stringify(e.currentTarget.dataset.item);
    wx.setStorageSync("shopcomment", shopcomment);
    if (!e.currentTarget.dataset.item.orderComment) {
      wx.navigateTo({
        url: '../comment/comment'
      })
    }
    else {
      wx.navigateTo({
        url: '../check/check'
      })
    }
  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {
    var that = this
    wx.request({
      url: 'http://127.0.0.1:8000/api/seproject/getAllOrders',
      header: {
        'cookie': wx.getStorageSync('set-cookie')
      },
      success: function (res) {
        console.log(res.cookie)
        that.setData({
          orderList: res.data.data,
        })
      }
    })
  },
})