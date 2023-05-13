import numpy as np


def multi_matrix(matrix1, matrix2):
    if len(matrix1[0]) == len(matrix2):
        res = [[0] * len(matrix2[0]) for i in range(len(matrix1))]
        for i in range(len(matrix1)):
            for j in range(len(matrix2[0])):
                for k in range(len(matrix2)):
                    res[i][j] += matrix1[i][k] * matrix2[k][j]
        return res
    return '输入矩阵有误！'


if _name_ == '_main_':
    matrix = np.random.rand(10, 5) * 2 - 1
    print("原矩阵：")
    print(matrix)
tran_matrix2 = [[row[i] for row in matrix] for i in range(len(matrix[0]))]
print("转置矩阵:", tran_matrix2)
dot_matrix2 = multi_matrix(matrix, tran_matrix2)
print("矩阵相乘：")
print(dot_matrix2)

