class SweetPotato(object):
    def __init__(self):
        self.cook_time = 0
        self.cook_state = '生的'
        self.condiments = []

    def cook(self, time):
        """烤地瓜的方法"""
        self.cook_time += time
        if 0 <= self.cook_time < 3:
            self.cook_state = '生的'
        elif 3 <= self.cook_time < 5:
            self.cook_state = '半生不熟'
        elif 5 <= self.cook_time < 8:
            self.cook_state = '熟了'
        elif self.cook_time >= 8:
            self.cook_state = '烤糊了'

    def add_condiments(self, condiment):
        """添加调料"""
        self.condiments.append(condiment)

    def __str__(self):
        return f'这个地瓜烤了{self.cook_time}分钟，状态是{self.cook_state}，添加的调料有{self.condiments}'


potato1 = SweetPotato()
print(potato1)
potato1.cook(2)
potato1.add_condiments('酱油')
print(potato1)
potato1.cook(2)
print(potato1)


class Furniture(object):
    def __init__(self, name, area):
        self.name = name
        self.area = area


class Home(object):
    def __init__(self, address, area):
        self.address = address
        self.area = area
        self.free_area = area
        self.furniture = []

    def add_furniture(self, item):
        """容纳家具"""
        if self.free_area >= item.area:
            self.furniture.append(item.name)
            self.free_area -= item.area
        else:
            print('家具太大，剩余面积不足')

    def __str__(self):
        return f'房子坐落于{self.address}，占地面积{self.area}，' \
               f'剩余面积是{self.free_area}，家具有{self.furniture}'


bed = Furniture('双人床', 60)
sofa = Furniture('沙发', 10)
home1 = Home('北京', 100)
print(home1)
home1.add_furniture(bed)
print(home1)
home1.add_furniture(sofa)
print(home1)
home1.add_furniture(bed)
