"""
需求：用户输入当前目录下任意文件名，程序完成对该文件的备份功能，例如：test[备份].txt
"""
old_name = input('请输入您要备份的文件名：')
index = old_name.rfind('.')
print(index)
if index <= 0:
    print('输入错误')
    exit()
new_name = old_name[:index] + '[备份]' + old_name[index:]
print(new_name)
old_f = open(old_name, 'rb')
new_f = open(new_name, 'wb')
while 1:
    con = old_f.read(1024)
    if len(con) == 0:
        break
    new_f.write(con)
old_f.close()
new_f.close()
