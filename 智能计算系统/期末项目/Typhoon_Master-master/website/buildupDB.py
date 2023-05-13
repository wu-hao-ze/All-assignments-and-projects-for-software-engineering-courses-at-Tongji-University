import os
import re
import sqlite3
import data.JTWC_reader as jtwc


def insert_typhoonhead():

    # files = os.listdir('CMA_Original_Data')

    typhoonHeaderList = jtwc.read_jtwc_folder()

    conn = sqlite3.connect('SQLiteDB/jtwc.db')

    counter = 1

    for typhoonHeader in typhoonHeaderList:
        conn.execute(
            "INSERT INTO TYPHOONHEAD(typhoon_id, typhoon_ith, typhoon_year,typhoon_name,typhoon_record_counter) "
            "VALUES(?,?,?,?,?)",
            (
                counter,
                typhoonHeader.ith,
                typhoonHeader.typhoonYear,
                typhoonHeader.typhoonName,
                typhoonHeader.typhoonRecordNum
            )
        )
        counter += 1
    conn.commit()
    conn.close()


def insert_typhoon_record():

    conn = sqlite3.connect('SQLiteDB/jtwc.db')

    typhoonHeaderList = jtwc.read_jtwc_folder()

    counter = 1

    # 整合训练集合
    for typhoonHeader in typhoonHeaderList:
        for typhoonRecord in typhoonHeader.typhoonRecords:
            conn.execute(
                "INSERT INTO TYPHOONRECORD(rid, typhoon_ith, typhoon_year,typhoon_time, totalNum, lat, long, wind) "
                "VALUES(?,?,?,?,?,?,?,?)",
                (
                    counter,
                    typhoonHeader.ith,
                    typhoonHeader.typhoonYear,
                    typhoonRecord.typhoonTime,
                    typhoonRecord.totalNum,
                    typhoonRecord.lat,
                    typhoonRecord.long,
                    typhoonRecord.wind,
                )
            )
            counter += 1
    conn.commit()
    conn.close()

insert_typhoonhead()
insert_typhoon_record()