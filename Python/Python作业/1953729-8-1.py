class Building(object):
    def __init__(self, length, width, floor, price):
        self.length = length
        self.width = width
        self.floor = floor
        self.price = price

    def area(self):
        self.mianji = self.length * self.width
        return self.mianji

    def total(self):
        self.zongjia = self.area() * self.floor * self.price
        return self.zongjia


length, width, floor, price = eval(input('请输入楼的长，宽，层数，每平米单价：'))
loufang = Building(length, width, floor, price)
print(loufang.area())
print(loufang.total())
