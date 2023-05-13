class Rect(object):
    def __init__(self, length, width):
        self.length = length
        self.width = width

    def count(self):
        zhouchang = (self.length + self.width) * 2
        print(zhouchang)
        return zhouchang

    def area(self):
        mianji = (self.length * self.width)
        print(mianji)
        return mianji

    def Sum(self, t):
        he = self.area() + t.area()
        print(he)


r1 = Rect(3, 7)
r2 = Rect(4, 5)
r1.Sum(r2)
