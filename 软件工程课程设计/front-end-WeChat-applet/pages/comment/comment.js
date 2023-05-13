// pages/comment/comment.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    shopcomment: {},
    default_score: 0,
    score: 0,
    score_text_arr: ['用餐体验极不愉快', '用餐不如意', '用餐体验一般', '用餐体验较好', 'oh，这是多么美妙的餐馆'],
    score_text: "",
    score_img_arr: [],
    is_upload: false,
    time: null,
    text: ""
  },
  _default_score: function (tauch_score = 0) {
    var score_img = [];
    var score = 0;
    for (let i = 0; i < 5; i++) {
      if (i < tauch_score) {
        score_img[i] = "../../images/star_on.png"
        score = i;
      } else {
        score_img[i] = "../../images/star_off.png"
      }
    }
    this.setData({
      score_img_arr: score_img,
      score_text: this.data.score_text_arr[score]
    });
  },

  onScore: function (e) {
    var score = e.currentTarget.dataset.score;
    this._default_score(score);
    this.setData({
      score: score
    })
  },

  //上传评论
  onSubmit: function (e) {
    wx.showLoading({
      title: '评价上传中',
    })
    var that = this;
    var is = false;
    var flag = false;
    var index = 0;
    this.setData({
      text: e.detail.value.text
    })
    wx.request({
      url: 'http://127.0.0.1:8000/api/seproject/addComment',
      method: "POST",
      header: {
        'cookie': wx.getStorageSync('set-cookie')
      },
      data: {
        order_id: that.data.shopcomment.order_id,
        user_text: that.data.text,
        user_score: that.data.score
      },
      success: res => {
        wx.hideLoading();
        setTimeout(function () {
          wx.switchTab({
            url: '../AllOrder/AllOrder'
          })
        }, 1000)
      }
    });

  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var shopcomment = JSON.parse(wx.getStorageSync("shopcomment"));
    this.setData({
      shopcomment: shopcomment,
    })
    this._default_score(this.data.default_score);
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