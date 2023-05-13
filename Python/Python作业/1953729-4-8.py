a, n = eval(input("请输入a和n的值："))
temp = 0
sum1 = 0
print("Sn=", end="")
for i in range(1, n + 1):
    temp = temp * 10 + a
    sum1 += temp
    print(temp, "+", sep="", end="")
print('\b', '=', sum1, sep="")
