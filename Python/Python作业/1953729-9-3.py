# -*-coding:utf-8-*-
import matplotlib.pyplot as plt
from matplotlib.font_manager import FontProperties

font = FontProperties(fname=r"C:\Windows\Fonts\simhei.ttf", size=14)
plt.bar([1, 3, 5, 7, 9], [5, 4, 8, 12, 7], label='graph1')
plt.bar([2, 4, 6, 8, 10], [4, 6, 8, 13, 15], label='graph2')

width: 0.5

plt.legend()
plt.xlabel('number')
plt.ylabel('value')
plt.title(u'测试例子——条形图', FontProperties=font)
plt.show()
