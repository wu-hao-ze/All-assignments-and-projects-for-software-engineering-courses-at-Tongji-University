from flask import Flask,render_template, request
import pandas as pd
from geopy.distance import great_circle
import math
import os

app = Flask(__name__)


@app.route('/')
def index():
    datasources= os.listdir('data/')
    for i in range(len(datasources)):
        datasources[i] = datasources[i].split('.')[0]
    return render_template('index.html', datasources=datasources)


@app.route('/getTyphoonYear')
def getTyphoonYear():
    datasource = request.args.get('datasource')
    result = ''
    if datasource=='请选择数据源':
        result = ''
    elif len(datasource) != 0:
        data = pd.read_csv('data/' + datasource + '.csv')
        years = data['YEAR'].unique()
        for i in range(len(years)):
            result += str(years[i])
            if i is not (len(years)-1) :
                result += ','
        # print(result)
    else:
        result = '数据源为空'
    return result


@app.route('/getTyphoonName')
def getTyphoonName():
    datasource = request.args.get('datasource')
    year = request.args.get('year')
    result = ''
    if datasource=='请选择数据源' or year=='请选择年份':
        result = ''
    elif len(datasource) == 0:
        result = '数据源为空'
    elif len(year) == 0:    
        result = '年份为空'
    else:
        data = pd.read_csv('data/' + datasource + '.csv')
        keys = data['KEY'].unique()
        year_keys = [v for i, v in enumerate(keys) if any(s in v for s in [str(year)])]
        cn_name = data[data['KEY'].isin(year_keys)][['KEY','CN_NAME']].copy()
        cn_name.drop_duplicates(inplace=True)
        cn_name.reset_index(drop=True, inplace=True)
        for i in range(len(year_keys)):
            result += str(year_keys[i]) + ',' + cn_name.at[i, 'CN_NAME']
            if i is not (len(year_keys)-1) :
                result += ';'
        # print(result)
    return result


@app.route('/getTyphoonRecord')
def getTyphoonRecord():
    datasource = request.args.get('datasource')
    year = request.args.get('year')
    key = request.args.get('name')
    result = ''
    if datasource=='请选择数据源' or year=='请选择年份' or key=='请选择台风':
        result = ''
    elif len(datasource) == 0:
        result = '数据源为空'
    elif len(year) == 0:    
        result = '年份为空'
    elif len(key) == 0:
        result = '台风名为空'
    else:
        data = pd.read_csv('data/' + datasource + '.csv')
        data = data[data['KEY']==key].copy()
        #data['LAT'] = data['LAT'].map(lambda x:x*0.1)
        #data['LONG'] = data['LONG'].map(lambda x:x*0.1)
        data.reset_index(drop=True, inplace=True)
        # 从第八个起报点开始返回台风路径，而不是从开始返回
        for i in range(8, len(data)):
            time = str(data.at[i, 'MONTH']).zfill(2)+str(data.at[i, 'DAY']).zfill(2)+str(data.at[i, 'HOUR']).zfill(2)
            result += str(data.at[i, 'LAT']) + ',' + str(data.at[i, 'LONG']) + ',' + time+ ',' + str(data.at[i, 'I'])
            if i is not (len(data)-1) :
                result += ';'
        print(result)
    return result


@app.route('/getPredict')
def getPredict():
    datasource = request.args.get('datasource')
    year = request.args.get('year')
    key = request.args.get('name')
    result = ''
    if datasource=='请选择数据源' or year=='请选择年份' or key=='请选择台风':
        result = ''
    elif len(datasource) == 0:
        result = '数据源为空'
    elif len(year) == 0:
        result = '年份为空'
    elif len(key) == 0:
        result = '台风名为空'
    else:
        track_csv = pd.read_csv('predict/' + datasource + '_track.csv')
        intensity_csv = pd.read_csv('predict/' + datasource + '_intensity.csv')
        assert track_csv.shape[0] == intensity_csv.shape[0]

        track_csv = track_csv[track_csv['KEY'] == key].copy()
        intensity_csv = intensity_csv[intensity_csv['KEY'] == key].copy()

        track_csv.reset_index(drop=True, inplace=True)
        intensity_csv.reset_index(drop=True, inplace=True)
        # track_csv.at[i, 'TIME'])[4:] 只返回MMDDHH，不返回YYYY
        for i in range(len(track_csv)):
            result += str(track_csv.at[i, 'PRED_LAT']) + ',' + \
                      str(track_csv.at[i, 'PRED_LONG']) + ',' + \
                      str(track_csv.at[i, 'TIME'])[4:] + ',' +\
                      str(int(intensity_csv.at[i, 'I'])) + ';'
        # 计算平均经度误差、平均纬度、平均距离误差
        long_err = []
        lat_err = []
        dist_err = []
        intensity_mae = 0
        for i in range(len(track_csv)):
            long_err.append(math.fabs(track_csv.at[i, 'LONG'] - track_csv.at[i, 'PRED_LONG']))
            lat_err.append(math.fabs(track_csv.at[i, 'LAT'] - track_csv.at[i, 'PRED_LAT']))
            dist_err.append(great_circle(
                    (track_csv.at[i, 'LAT'], track_csv.at[i, 'LONG']),
                    (track_csv.at[i, 'PRED_LAT'], track_csv.at[i, 'PRED_LONG'])
                ).kilometers
             )
            intensity_mae += math.fabs(intensity_csv.at[i, 'WND'] - intensity_csv.at[i, 'PRED_WND'])
        result += str(round(sum(long_err)/len(long_err), 2)) + ';'
        result += str(round(sum(lat_err)/len(lat_err), 2)) + ';'
        result += str(round(sum(dist_err)/len(dist_err), 2)) + ';'
        result += str(round(intensity_mae/len(dist_err), 2)) + ';'
        print(result)
    return result


@app.route('/getBaseline')
def getBaseline():
    datasource = request.args.get('datasource')
    year = request.args.get('year')
    key = request.args.get('name')
    result = ''
    if datasource=='请选择数据源' or year=='请选择年份' or key=='请选择台风':
        result = ''
    elif len(datasource) == 0:
        result = '数据源为空'
    elif len(year) == 0:
        result = '年份为空'
    elif len(key) == 0:
        result = '台风名为空'
    else:
        track_csv = pd.read_csv('predict/' + datasource + '_baseline_track.csv')
        intensity_csv = pd.read_csv('predict/' + datasource + '_baseline_intensity.csv')
        assert track_csv.shape[0] == intensity_csv.shape[0]

        track_csv = track_csv[track_csv['KEY'] == key].copy()
        intensity_csv = intensity_csv[intensity_csv['KEY'] == key].copy()

        track_csv.reset_index(drop=True, inplace=True)
        intensity_csv.reset_index(drop=True, inplace=True)
        # track_csv.at[i, 'TIME'])[4:] 只返回MMDDHH，不返回YYYY
        for i in range(len(track_csv)):
            result += str(track_csv.at[i, 'PRED_LAT']) + ',' + \
                      str(track_csv.at[i, 'PRED_LONG']) + ',' + \
                      str(track_csv.at[i, 'TIME'])[4:] + ',' +\
                      str(int(intensity_csv.at[i, 'I'])) + ';'
        # 计算平均经度误差、平均纬度、平均距离误差
        long_err = []
        lat_err = []
        dist_err = []
        intensity_mae = 0
        for i in range(len(track_csv)):
            long_err.append(math.fabs(track_csv.at[i, 'LONG'] - track_csv.at[i, 'PRED_LONG']))
            lat_err.append(math.fabs(track_csv.at[i, 'LAT'] - track_csv.at[i, 'PRED_LAT']))
            dist_err.append(great_circle(
                    (track_csv.at[i, 'LAT'], track_csv.at[i, 'LONG']),
                    (track_csv.at[i, 'PRED_LAT'], track_csv.at[i, 'PRED_LONG'])
                ).kilometers
             )
            intensity_mae += math.fabs(intensity_csv.at[i, 'WND'] - intensity_csv.at[i, 'PRED_WND'])
        result += str(round(sum(long_err)/len(long_err), 2)) + ';'
        result += str(round(sum(lat_err)/len(lat_err), 2)) + ';'
        result += str(round(sum(dist_err)/len(dist_err), 2)) + ';'
        result += str(round(intensity_mae/len(long_err), 2)) + ';'
        print(result)
    return result


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080) 
