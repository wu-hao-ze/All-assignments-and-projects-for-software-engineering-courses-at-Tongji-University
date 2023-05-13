// page/component/new-pages/cart/cart.js
// 默认请求第一页
var numbers = 1;
var bool = true;
Page({
  data: {
    show_edit: "block",
    edit_name: "编辑",
    edit_show: "none",
    delete_icon: "../../images/delete.png",
    radio_show_Icon: "../../images/radio_show.png",
    radio_none_Icon: "../../images/radio_none.png",
    // list: [],               // 购物车列表
    // hasList: false,          // 列表是否有数据
    // 默认展示数据
    hasList: true,
    // 商品列表数据
    list: [
    ],
    // 金额
    totalPrice: 0, // 总价，初始为0
    // 全选状态
    selectAllStatus: true, // 全选状态，默认全选
    orderid: 0,
  },

  onShow() {
    wx.showToast({
      title: '加载中',
      icon: "loading",
      duration: 1000
    })

    // 价格方法
    this.count_price();
  },
  /**
   * 当前商品选中事件
   */
  selectList(e) {
    var that = this;
    // 获取选中的radio索引
    var index = e.currentTarget.dataset.index;
    // 获取到商品列表数据
    var list = that.data.list;
    // 默认全选
    that.data.selectAllStatus = true;
    // 循环数组数据，判断----选中/未选中[selected]
    list[index].selected = !list[index].selected;
    // 如果数组数据全部为selected[true],全选
    for (var i = list.length - 1; i >= 0; i--) {
      if (!list[i].selected) {
        that.data.selectAllStatus = false;
        break;
      }
    }
    // 重新渲染数据
    that.setData({
      list: list,
      selectAllStatus: that.data.selectAllStatus
    })
    // 调用计算金额方法
    that.count_price();
  },
  // 编辑
  btn_edit: function () {
    var that = this;
    if (bool) {
      that.setData({
        edit_show: "block",
        edit_name: "取消",
        show_edit: "none"
      })
      bool = false;
    } else {
      that.setData({
        edit_show: "none",
        edit_name: "编辑",
        show_edit: "block"
      })
      bool = true;
    }

  },
  /**初始化加载数据 */
  onLoad(res) {
    var countArray = JSON.parse(wx.getStorageSync("countArray"));
    console.log("countArray", countArray);
    if (countArray.length > 0) {
      this.setData({
        list: countArray,
        hasList: true
      })
      console.log(this.data.list)
    } else {
      this.setData({
        hasList: false
      })
    }
  },
  // 删除
  deletes: function (e) {
    var that = this;
    // 获取索引
    const index = e.currentTarget.dataset.index;
    // 获取商品列表数据
    let list = this.data.list;
    wx.showModal({
      title: '提示',
      content: '确认删除吗',
      success: function (res) {
        if (res.confirm) {
          // 删除索引从1
          list.splice(index, 1);
          // 页面渲染数据
          that.setData({
            list: list
          });
          // 如果数据为空
          if (!list.length) {
            that.setData({
              hasList: false
            });
          } else {
            // 调用金额渲染数据
            that.count_price();
          }
        } else {
          console.log(res);
        }
      },
      fail: function (res) {
        console.log(res);
      }
    })
  },



  /**
   * 购物车全选事件
   */
  selectAll(e) {
    // 全选ICON默认选中
    let selectAllStatus = this.data.selectAllStatus;
    // true  -----   false
    selectAllStatus = !selectAllStatus;
    // 获取商品数据
    let list = this.data.list;
    // 循环遍历判断列表中的数据是否选中
    for (let i = 0; i < list.length; i++) {
      list[i].selected = selectAllStatus;
    }
    // 页面重新渲染
    this.setData({
      selectAllStatus: selectAllStatus,
      list: list
    });
    // 计算金额方法
    this.count_price();
  },
  /**
   * 绑定加数量事件
   */
  btn_add(e) {
    // 获取点击的索引
    const index = e.currentTarget.dataset.index;
    // 获取商品数据
    let list = this.data.list;
    // 获取商品数量
    let num = list[index].num;
    // 点击递增
    num = num + 1;
    list[index].num = num;
    // 重新渲染 ---显示新的数量
    this.setData({
      list: list
    });
    // 计算金额方法
    this.count_price();
  },

  /**
   * 绑定减数量事件
   */
  btn_minus(e) {
    //   // 获取点击的索引
    const index = e.currentTarget.dataset.index;
    // const obj = e.currentTarget.dataset.obj;
    // console.log(obj);
    // 获取商品数据
    let list = this.data.list;
    // 获取商品数量
    let num = list[index].num;
    // 判断num小于等于1  return; 点击无效
    if (num <= 1) {
      return false;
    }
    // else  num大于1  点击减按钮  数量--
    num = num - 1;
    list[index].num = num;
    // 渲染页面
    this.setData({
      list: list
    });
    // 调用计算金额方法
    this.count_price();
  },
  // 提交订单
  btn_submit_order: function () {
    //     // 调起支付
    // wx.requestPayment(
    //   {
    //     'timeStamp': '',
    //     'nonceStr': '',
    //     'package': '',
    //     'signType': 'MD5',
    //     'paySign': '',
    //     'success': function (res) { },
    //     'fail': function (res) { },
    //     'complete': function (res) { }
    //   })
    // wx.showModal({
    //   title: '提示',
    //   content: '合计金额-' + that.data.totalPrice + "暂未开发",
    // })
    var that = this;
    console.log(that.data.totalPrice);
    var countArray = JSON.parse(wx.getStorageSync("countArray"));
    console.log("countArray", countArray);
    var shopid = JSON.parse(wx.getStorageSync("shopid"));
    console.log("shopid", shopid);
    wx.request({
      url: 'http://127.0.0.1:8000/api/seproject/createOrder',
      method: 'POST',
      data: {
        store_id: shopid,
        countArray: countArray
      },
      header: {
        'cookie': wx.getStorageSync('set-cookie')
      },
      success: function (res) {
        var orderid = res.data.order_id;
        that.setData({
          orderid: orderid,
        })
        console.log(that.data.orderid)
        wx.redirectTo({
          url: '../paySuccess/paySuccess?orderid=' + orderid,
        })
      }
    })
  },
  // 收藏
  btn_collert: function () {
    wx.showToast({
      title: '收藏暂未开发',
      duration: 2000
    })
  },
  /**
   * 计算总价
   */
  count_price() {
    // 获取商品列表数据
    let list = this.data.list;
    // 声明一个变量接收数组列表price
    let total = 0;
    // 循环列表得到每个数据
    for (let i = 0; i < list.length; i++) {
      // 判断选中计算价格
      if (list[i].selected) {
        // 所有价格加起来 count_money
        total += list[i].num * list[i].price;
      }
    }
    // 最后赋值到data中渲染到页面
    this.setData({
      list: list,
      totalPrice: total.toFixed(2)
    });
  },
  // 下拉刷新
  // onPullDownRefresh: function () {
  //   // 显示顶部刷新图标  
  //   wx.showNavigationBarLoading();
  //   var that = this;
  //   console.log(that.data.types_id);
  //   console.log(that.data.sel_name);
  //   wx.request({
  //     url: host + '请求后台数据地址',
  //     method: "post",
  //     data: {
  //       // 刷新显示最新数据
  //       page: 1,
  //     },
  //     success: function (res) {
  //       // console.log(res.data.data.datas);
  //       that.setData({
  //         list: res.data.data.datas
  //       })
  //     }
  //   })
  //   // 隐藏导航栏加载框  
  //   wx.hideNavigationBarLoading();
  //   // 停止下拉动作  
  //   wx.stopPullDownRefresh();
  // },
  // 加载更多
  // onReachBottom: function () {
  //   var that = this;
  //   // 显示加载图标  
  //   wx.showLoading({
  //     title: '正在加载中...',
  //   })
  //   numbers++;
  //   // 页数+1  
  //   wx.request({
  //     url: host + '后台数据地址',
  //     method: "post",
  //     data: {
  //     // 分页
  //       page: numbers,
  //     },
  //     // 请求头部  
  //     header: {
  //       'content-type': 'application/json'
  //     },
  //     success: function (res) {
  //       // 回调函数 
  //       var num = res.data.data.datas.length;
  //       // console.log(num);
  //       // 判断数据长度如果不等于0，前台展示数据，false显示暂无订单提示信息
  //       if (res.data.data.status == 0) {
  //         for (var i = 0; i < res.data.data.datas.length; i++) {
  //           that.data.list.push(res.data.data.datas[i]);
  //         }
  //         // 设置数据  
  //         that.setData({
  //           list: that.data.list
  //         })
  //       } else {
  //         wx.showToast({ title: '没有更多了', icon: 'loading', duration: 2000 })
  //       }
  //       // 隐藏加载框  
  //       wx.hideLoading();
  //     }
  //   })
  // },
})