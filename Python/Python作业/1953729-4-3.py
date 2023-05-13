s = input("请输入句子：")
w = s.split()
i = 0
k = 0
t = 0
for w[i] in w:
    if k < len(w[i]):
        k = len(w[i])
        t = i
print("最长的单词为", w[t], "长度为", k)
