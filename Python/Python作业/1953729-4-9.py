s = list("一二三四五六")
for z in range(5, 7):
    for x in range(1, 6):
        for y in range(1, 6):
            if x < y < z:
                print("x在周", s[x - 1], ' ', "y在周", s[y - 1], ' ', "z在周", s[z - 1], sep="")
