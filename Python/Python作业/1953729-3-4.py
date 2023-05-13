x, y, z = eval(input("请输入三个数："))
if x > y:
    x, y = y, x
if y > z:
    if x < z:
        y, z = z, y
    else:
        y, z = z, y
        x, y = y, x
print("从小到大的顺序为： %f < %f < %f" % (x, y, z))
