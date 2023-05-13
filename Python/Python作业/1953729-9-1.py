import matplotlib
import matplotlib.pyplot as plt  # 导入pyplot子库
import numpy as np

plt.figure(figsize=(6, 4))  # 绘图区大小 为 6*4 英寸
matplotlib.rcParams['font.family'] = 'YouYuan'
matplotlib.rcParams['font.size'] = 12
a = np.arange(-1, 1, 0.001)
plt.plot(a, a * a, 'b--')
plt.xlabel('横轴：x', color='green')
plt.ylabel('纵轴：y=x^2', color='red')
plt.axis([-1, 1, 0, 1])
plt.show()
