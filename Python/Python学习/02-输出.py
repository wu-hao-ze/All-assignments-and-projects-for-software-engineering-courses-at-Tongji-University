"""

%c                   字符
%s                   字符串
%d                   有符号的十进制整数
%u                   无符号的十进制整数
%f                   浮点数
%o                   八进制整数
%x                   十六进制整数
%X                   十六进制整数
%e                   科学计数法
%E                   科学计数法

"""

print('十六进制数%x' % 15)
print('十六进制数%X' % 15)
weight = 75.5
print('我的体重是%f公斤' % weight)
print('我的体重是%.2f公斤' % weight)  # 小数点后面保留2位小数
print('我的体重是%16f公斤' % weight)  # %16f是数据宽度 超出原样输出，且默认右对齐
print('我的体重是%-4d公斤' % 75)  # 左对齐
print('我的体重是%04d公斤' % 75)  # 用0补全空位
print('我的名字是%s,明年%d岁了,体重是%.2f公斤,学号是%7d' % ('whz', 18, weight, 1953729))
print('我的名字是%s,明年%s岁了,体重是%.2f公斤,学号是%7s' % ('whz', 18, weight, 1953729))
print(f'我的名字是{"whz"},明年{18}岁了,体重是{weight}公斤,学号是{1953729}')
print('\n就这')
print('\t长留\bsadjfaslf')
# print函数自带换行结束
print(end='\n')
print('sdfsdafsadfdasf', end='\t')
print('吴浩泽', end='')
print('闫秋宏')
# 如果想在字符串中输出\，则要使用\\来输出\，因为\自带转义
# %-s %-d是左对齐
print('123\\')

x = 1234.567
s = 'pythonOk'
n = 1234567
print('x={1:12.2f} s={2:10s} n={0:10d}'.format(n, x, s))
# 对于format数据格式来讲，字符串默认是左对齐，数值是右对齐
