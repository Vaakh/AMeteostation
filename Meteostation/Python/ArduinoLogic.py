import ArConnector as ac
import SQLQueries as sqq
import SQLConnector as sqc
import time
import json


def processArData():
    arcon = ac.ArdConnector()
    print('Hello')
    data = arcon.get()
    if not data == None:
        tempdata = data.decode("utf-8").replace('b', '').replace('\'', '').replace('\n', '')
        prepdata = json.loads(str(tempdata))
        print(prepdata)
        scon = sqc.SQLConnector()
        currentId = scon.readFromDB(sqq.selectmaxidquery)
        print(currentId)
        if currentId is None:
            currentId = 1
        else:
            currentId = int(currentId) + 1
        print(currentId)
        query = sqq.buildInsertArQuery(currentId, prepdata["RTC_time"], prepdata["MFS_x"], prepdata["MFS_y"],
                                     prepdata["MFS_z"], prepdata["THI_t"], prepdata["THI_h"], prepdata["THO_t"],
                                     prepdata["THO_h"], prepdata["TPO_t"], prepdata["TPO_p"], prepdata["WND_s"],
                                     prepdata["WND_o"], prepdata["LLS"], prepdata["RDS"], prepdata["IRS"])
        scon.writeToDB(query)
    time.sleep(10)
