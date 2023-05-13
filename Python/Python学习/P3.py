import random


class Color:
    def __init__(self, r, g, b):
        self.r = r
        self.g = g
        self.b = b

    def gray(self):
        return self.r * 0.299 + self.g * 0.587 + self.b * 0.114

    def disp(self, n):
        print(f'像素{n}:({self.r},{self.g},{self.b})')
        print(f'对应的灰度值：{self.gray()}')


color1 = Color(random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))
color1.disp(1)
color2 = Color(random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))
color2.disp(2)
color3 = Color(random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))
color3.disp(3)
color4 = Color(random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))
color4.disp(4)
color5 = Color(random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))
color5.disp(5)
