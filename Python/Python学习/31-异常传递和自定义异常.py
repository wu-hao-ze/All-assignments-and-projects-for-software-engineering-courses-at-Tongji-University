import time

try:
    f = open('test.txt')
    try:
        while 1:
            content = f.readline()
            if len(content) == 0:
                break
            time.sleep(2)
            print(content)
    except:
        # 在命令提示符下如果按下ctrl+c可以中止
        '''
        在对应的目录下cmd中使用python 对应的py文件，就可以运行
        '''
        print('意外中止')
    finally:
        f.close()
        print('关闭文件')
except:
    print('没有这个文件')

'''
自定义异常的作用就是将不满足程序逻辑的情况抛出异常，简单来讲就是报错，报的是逻辑错误
抛出自定义异常的语法就是raise 异常类对象
'''
'''
需求：密码长度不足，则报异常，用户输入密码，如果输入的长度不足3位，则报错，即抛出自定义异常并捕获该异常
'''


# 自定义异常类，继承Exception
class ShortInputError(Exception):
    def __init__(self, length, min_len):
        self.length = length
        self.min_len = min_len

    def __str__(self):
        return f'输入的长度是{self.length},不能少于{self.min_len}个字符'


def main():
    try:
        con = input('请输入密码：')
        if len(con) < 3:
            raise ShortInputError(len(con), 3)
    except Exception as result:
        print(result)
    else:
        print('密码输入完成')


main()
