x = float(input("上网总时间为："))
if x < 10:
    y = 30
elif 10 <= x < 50:
    y = 30 + 2.5 * (x - 10)
elif 50 <= x <= 60:
    y = 130 + 2 * (x - 50)
else:
    y = 150
print("上网费用为：", y, "元")
