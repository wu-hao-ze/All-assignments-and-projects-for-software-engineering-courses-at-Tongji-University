Page({
  data: {
    shopBase: {
      shopicon: "../../images/shopicon.png",
      shopAddIcon: "../../images/add-icon.png",
      shopMinuteIcon: "../../images/minute.png"
    },
    goods: [],
    cutab: 0,
    scrollTop: 100,
    foodCounts: 0,
    totalPrice: 0,// 总价格
    totalCount: 0, // 总商品数
    carArray: [],
    fold: true,
    toView: "",
    parentIndex: 0,
    shopid: 0
  },
  selectMenu: function (e) {
    var index = e.currentTarget.dataset.itemIndex;
    this.setData({
      toView: 'order' + index,
      cutab: index
    })
    console.log(this.data.toView);
  },
  //移除商品
  decreaseCart: function (e) {
    var index = e.currentTarget.dataset.itemIndex;
    var parentIndex = e.currentTarget.dataset.parentindex;
    this.data.goods.forEach((good) => {
      good.foods.forEach((food) => {
        if (food.Count > 0) {
          this.data.goods[parentIndex].foods[index].Count--
          var mark = 'a' + index + 'b' + parentIndex
          var id = this.data.goods[parentIndex].foods[index].id;
          var price = this.data.goods[parentIndex].foods[index].price;
          var num = this.data.goods[parentIndex].foods[index].Count;
          var obj = { price: price, num: num, mark: mark, id: id };
          var carArray1 = this.data.carArray.filter(item => item.mark != mark);
          carArray1.push(obj);
          console.log(carArray1);
          this.setData({
            carArray: carArray1,
            goods: this.data.goods
          })
          this.calTotalPrice()
        }
      })
    })
  },
  // 滑动切换tab 
  onGoodsScroll: function (e) {
    console.log(e.detail.scrollTop);

    var scale = e.detail.scrollWidth / 570,
      scrollTop = e.detail.scrollTop / scale,
      h = 0,
      classifySeleted,
      len = this.data.goods.length;
    this.data.goods.forEach(function (classify, i) {
      var _h = 70 + classify.foods.length * (46 * 3 + 20 * 2);
      if (scrollTop >= h - 100 / scale) {
        classifySeleted = classify.foods[i].id;
        console.log(classifySeleted);
      }
      h += _h;
    });
    this.setData({
      cutab: classifySeleted
    });
  },
  minueCart(e) {
    console.log(e);
    var index = e.currentTarget.dataset.itemIndex;
    var parentIndex = e.currentTarget.dataset.parentindex;
    var mark = 'a' + index + 'b' + parentIndex;
    var id = e.currentTarget.dataset.id;
    var shopname = e.currentTarget.dataset.shopname;
    var shaopdesc = e.currentTarget.dataset.shopdesc;
    var icon = e.currentTarget.dataset.icon;
    var price = this.data.goods[parentIndex].foods[index].price;
    var num = this.data.goods[parentIndex].foods[index].Count;
    if (num <= 0) {
      return false;
    }
    // else  num大于1  点击减按钮  数量--
    num = num - 1;
    this.data.goods[parentIndex].foods[index].Count = num;
    var obj = { price: price, num: num, mark: mark, icon: icon, selected: true, shopname: shopname, shaopdesc: shaopdesc, id: id };
    var carArray1 = this.data.carArray.filter(item => item.mark != mark)
    if (num > 0) {
      carArray1.push(obj)
    }
    console.log(carArray1);
    this.setData({
      carArray: carArray1,
      goods: this.data.goods
    })
    this.calTotalPrice();

  },
  //添加到购物车
  addCart(e) {
    var index = e.currentTarget.dataset.itemIndex;
    var parentIndex = e.currentTarget.dataset.parentindex;
    this.data.goods[parentIndex].foods[index].Count++;
    var mark = 'a' + index + 'b' + parentIndex;
    var id = e.currentTarget.dataset.id;
    var shopname = e.currentTarget.dataset.shopname;
    var shaopdesc = e.currentTarget.dataset.shopdesc;
    var price = this.data.goods[parentIndex].foods[index].price;
    var icon = e.currentTarget.dataset.icon;
    var num = this.data.goods[parentIndex].foods[index].Count;
    if (num > 99) {
      wx.showModal({
        title: '提示',
        content: '亲，最多选购99件哦！',
      })
      return;
    }
    var obj = { price: price, num: num, mark: mark, icon: icon, selected: true, shopname: shopname, shaopdesc: shaopdesc, id: id };
    var carArray1 = this.data.carArray.filter(item => item.mark != mark)
    carArray1.push(obj)
    console.log(carArray1);
    this.setData({
      carArray: carArray1,
      goods: this.data.goods
    })
    this.calTotalPrice();

  },
  //计算总价
  calTotalPrice: function () {
    var carArray = this.data.carArray;
    var totalPrice = 0;
    var totalCount = 0;
    for (var i = 0; i < carArray.length; i++) {
      totalPrice += carArray[i].price * carArray[i].num;
      totalCount += carArray[i].num
    }
    this.setData({
      totalPrice: totalPrice,
      totalCount: totalCount,
    });
  },
  onLoad: function (options) {
    console.log(options.shopid)
    var that = this
    wx.request({
      url: 'http://127.0.0.1:8000/api/seproject/getDishInfo',
      data: {
        store_id: options.shopid
      },
      header: {
        'cookie': wx.getStorageSync('Set-Cookie')
      },
      success: function (res) {
        console.log(res.data.goods)
        that.setData({
          goods: res.data.goods,
          shopid: options.shopid
        })
      }
    })
  },
  onReady: function () {
    // 页面渲染完成
  },
  onShow: function () {
    // 页面显示
  },
  // 打开购物车页面
  getOpenShop() {
    var countArray = JSON.stringify(this.data.carArray);
    var shopid = JSON.stringify(this.data.shopid);
    wx.setStorageSync("countArray", countArray);
    wx.setStorageSync('shopid', shopid)
    wx.navigateTo({
      url: '../order/order',
    })
  },
  onUnload: function () {
    // 页面关闭
  }
})
