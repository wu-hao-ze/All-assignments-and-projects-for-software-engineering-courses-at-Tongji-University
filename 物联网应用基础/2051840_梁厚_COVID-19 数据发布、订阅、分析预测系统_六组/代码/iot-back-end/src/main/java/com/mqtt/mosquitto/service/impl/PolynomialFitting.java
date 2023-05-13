package com.mqtt.mosquitto.service.impl;

import org.springframework.stereotype.Service;

import java.util.Arrays;

public class PolynomialFitting {

    // 多项式次数
    private Integer n;
    // 拟合后的系数
    private final double[] coefficients;
    // 预测的数据点

    private Integer[] predictedData;

    // 把数组拆分成k份
    public static Integer[][] split(Integer[] data, Integer k) {
        Integer n = data.length;

        // 拆分的大小
        int size = n / k;

        // 拆分的数组
        Integer[][] splits = new Integer[k][];

        // 拆分数据
        Integer index = 0;
        for (int i = 0; i < k; i++) {
            // 最后一份可能不满
            int splitSize = (i == k - 1) ? n - index : size;
            splits[i] = new Integer[splitSize];

            for (int j = 0; j < splitSize; j++) {
                splits[i][j] = data[index];
                index++;
            }
        }

        return splits;
    }

    // 确定最佳的多项式次数
    public static Integer determinePolynomialOrder(Integer[] x, Integer[] y, Integer k) {
        // 交叉验证评估结果
        double[] evaluationResults = new double[k];

        // 分割数据
        Integer[][] xSplits = split(x, 2);
        Integer[][] ySplits = split(y, 2);

        // 构建训练数据和评估数据
        Integer[] xTrain = xSplits[0];
        Integer[] yTrain = ySplits[0];
        Integer[] xEval = xSplits[1];
        Integer[] yEval = ySplits[1];

        // 对每个次数的多项式进行拟合
        for (int order = 1; order <= k; order++) {
            // 拟合多项式
            PolynomialFitting polyfit = new PolynomialFitting(order);
            polyfit.fit(xTrain, yTrain);
            // 计算预测误差
            double error = 0;
            for (int j = 0; j < xEval.length; j++) {
                Integer predicted = polyfit.predict(xEval[j]);
                error += (predicted - yEval[j]) * (predicted - yEval[j]);
            }
            // 计算评估结果
            evaluationResults[order - 1] += error / xEval.length;
        }
        // 找到误差最小的评估结果
        double maxEvaluationResult = Double.POSITIVE_INFINITY;
        int bestOrder = 0;
        for (int i = 0; i < evaluationResults.length; i++) {
            if (evaluationResults[i] < maxEvaluationResult) {
                maxEvaluationResult = evaluationResults[i];
                bestOrder = i + 1;
            }
        }
        //System.out.prIntegerln(bestOrder);
        // 返回最佳的多项式次数
        return bestOrder;
    }

    public PolynomialFitting(Integer n) {
        this.n = n;
        this.coefficients = new double[n + 1];
        Arrays.fill(this.coefficients, 0);
        this.predictedData = null;
    }

    // 计算系数
    public void fit(Integer[] x, Integer[] y) {
        int m = x.length;
        double[] tmpX = new double[2 * n + 1];

        // 计算X的0次幂到2n次幂
        for (int i = 0; i < 2 * n + 1; i++) {
            for (Integer integer : x) {
                tmpX[i] += Math.pow(integer, i);
            }
        }

        double[][] X = new double[n + 1][n + 2];
        for (Integer i = 0; i <= n; i++) {
            for (Integer j = 0; j <= n; j++) {
                X[i][j] = tmpX[i + j];
            }
        }

        double[] Y = new double[n + 1];
        for (int i = 0; i <= n; i++) {
            for (int j = 0; j < m; j++) {
                Y[i] += Math.pow(x[j], i) * y[j];
            }
        }

        for (int i = 0; i <= n; i++) {
            X[i][n + 1] = Y[i];
        }

        n = n + 1;
        // 将方程组化为上三角矩阵
        for (int i = 0; i < n; i++) {
            for (int j = i + 1; j < n; j++) {
                if (X[i][i] < X[j][i]) {
                    for (int k = 0; k <= n; k++) {
                        double temp = X[i][k];
                        X[i][k] = X[j][k];
                        X[j][k] = temp;
                    }
                }
            }
        }

        // 逐行消元
        for (int i = 0; i < n - 1; i++) {
            for (int j = i + 1; j < n; j++) {
                double temp = X[j][i] / X[i][i];
                for (int k = 0; k <= n; k++) {
                    X[j][k] -= temp * X[i][k];
                }
            }
        }

        // 逐行求解
        for (int i = n - 1; i >= 0; i--) {
            double temp = 0;
            for (int j = i + 1; j < n; j++) {
                temp += X[i][j] * coefficients[j];
            }
            coefficients[i] = (X[i][n] - temp) / X[i][i];
        }
    }

    // 预测数据

    public void predict(Integer[] x) {
        this.predictedData = new Integer[x.length];
        for (int i = 0; i < x.length; i++) {
            int y = 0;
            for (int j = 0; j < n; j++) {
                y += (int)(coefficients[j] * Math.pow(x[i], j));
            }
            predictedData[i] = y;
        }
    }

    public Integer predict(Integer x) {
        this.predictedData = new Integer[1];
        this.predictedData[0] = 0;
        for (int i = 0; i < n; i++) {
            this.predictedData[0] += (int)(coefficients[i] * Math.pow(x, i));
        }
        return this.predictedData[0];
    }

    // 获取预测的数据
    public Integer[] getPredictedData() {
        return this.predictedData;
    }

    public String polyFit(Integer[] y) {
        // 训练数据
        //double[] x = {1, 2, 3, 4, 5};
        //double[] y = {1, 4, 9, 16, 25};
        Integer[] x = new Integer[y.length];
        for(int i = 0; i < y.length; i++)
            x[i] = i + 1;
        // 确定最佳的多项式次数
        Integer bestOrder = determinePolynomialOrder(x, y, 10);
        // 创建一个多项式拟合类
        PolynomialFitting polyFit = new PolynomialFitting(bestOrder);
        // 计算系数
        polyFit.fit(x, y);

        // 预测结果（预测未来的五组数据）
        Integer[] z = new Integer[5];
        for(int i = 0; i < 5; i++)
            z[i] = x.length + i + 1;
        polyFit.predict(z);
        for(int i = 0; i < z.length; i++)
            System.out.println(polyFit.getPredictedData()[i]);
        return Arrays.toString(polyFit.predictedData);
    }

}
