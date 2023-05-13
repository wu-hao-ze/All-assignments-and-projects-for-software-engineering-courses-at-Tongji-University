f = open('hlm.txt', 'rb')
s1 = 0
s2 = 0
str1 = f.read()
s1 = str1.count('林黛玉')
s2 = str1.count('贾宝玉')
f.close()
