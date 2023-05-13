s = input("输入表达式：")
n = len(s)
i = 0
sum1 = 0
sum2 = 0
for i in range(0, n):
    if s[i] == '(':
        sum1 += 1
    if s[i] == ')':
        sum2 += 1
if sum1 > sum2:
    print("左括号多于右括号")
elif sum1 < sum2:
    print("左括号少于右括号")
else:
    print("左括号等于右括号")
