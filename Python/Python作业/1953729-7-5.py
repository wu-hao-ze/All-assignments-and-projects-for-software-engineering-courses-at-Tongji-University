f1 = open('1.txt', 'rb')
f2 = open('2.txt', 'rb')
str1 = f1.read()
str2 = f2.read()
str3 = str1 + str2
f3 = open('3.txt', 'wb')
f3.write(str3)
f1.close()
f2.close()
f3.close()
