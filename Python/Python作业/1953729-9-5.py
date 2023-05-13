import matplotlib.pyplot as plt

score = [0.16, 0.28, 0.26, 0.14, 0.1, 0.06]
plt.rcParams['font.sans-serif'] = ['SimHei']
plt.title("消费支出")
labels = ['学习用品', '日常用品', '伙食费', '通讯费', '娱乐费', '其他开支']
explode = (0.2, 0, 0, 0, 0, 0)
plt.axis("equal")
plt.pie(score, explode=explode, autopct='%4.1f%%', labels=labels)
# autopct='% 控制数据标记的格式化显示 显示 1 位小数
plt.legend(bbox_to_anchor=(1, 0.6))
plt.show()
