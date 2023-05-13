num = input("请输入18位身份证号:")
print("身份证号为：")
print(num)
print("出生年月日为：")
s = num[6:10] + '年' + num[10:12] + '月' + num[12:14] + '日'
print(s)
