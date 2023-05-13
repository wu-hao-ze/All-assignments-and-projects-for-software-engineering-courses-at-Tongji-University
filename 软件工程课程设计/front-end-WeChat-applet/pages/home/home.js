var app = new getApp();
Page({

  /**
   * 页面的初始数据
   */
  data: {
    // 门店数据
    shopid: 0,
    shopList: [],
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    //调用models组件
    var userInfo = wx.getStorageSync("userinfo");
    if (!userInfo) {
      var modelLogo = this.selectComponent("#Models");
      modelLogo.getShow();
      return;
    }
    this.setData({
      nickName: userInfo.nickName,
      avatarUrl: userInfo.avatarUrl
    })
    //登录交互
    wx.login({
      success(res) {
        if (res.code) {
          wx.request({
            success(res) {
              if (res.header['set-cookie'] != '') {
                wx.setStorageSync('set-cookie', res.header['set-cookie'])
              }
            },
            url: 'http://127.0.0.1:8000/api/seproject/getOpenid',
            data: {
              code: res.code
            },
            header: {
              'cookie': wx.getStorageSync('set-cookie')
            }
          })
        } else {
          console.log('登录失败！' + res.errMsg)
        }
      }
    })
    var that = this
    wx.request({
      url: 'http://127.0.0.1:8000/api/seproject/getStoreInfo',
      success: function (res) {
        that.setData({
          shopList: res.data.shoplist,
        })
      }
    })
  },
  /**进入店铺 */
  getOpenShop(e) {
    var shopid = e.currentTarget.dataset.item.id;
    this.setData({
      shopid: shopid
    })
    console.log(this.data.shopid)
    wx.navigateTo({
      url: '../food/food?shopid=' + shopid,
    })
  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  }
})