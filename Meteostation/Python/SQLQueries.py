from datetime import datetime

server = 'localhost'
user = 'Meteostation'
database = 'meteo_db'
password = 'OpenSourse!123'

ct = datetime.now()
k = ct.timetuple()
insert_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
tableName = str(k[0]) + "/" + str(k[1]) + "/" + str(k[2])

createtablequery = "CREATE TABLE `meteo_db`.`" + tableName + "` ( `DataID` INT NOT NULL, `RTC_time` TIME NULL, " \
                                                             "`MFS_x` FLOAT NULL, `MFS_y` FLOAT NULL, `MFS_z` FLOAT " \
                                                             "NULL, " + "`THI_t` FLOAT NULL, `THI_h` FLOAT NULL, " \
                                                                        "`THO_t` FLOAT NULL, `THO_h` FLOAT NULL, " \
                                                                        "`TPO_t` FLOAT NULL, `TPO_p` FLOAT NULL, " \
                                                                        "" + "`WND_s` FLOAT NULL, `WND_o` FLOAT NULL, " \
                                                                             "`LLS` INT NULL, `RDS` INT NULL, " \
                                                                             "`IRS` FLOAT NULL, `photo_path` VARCHAR(" \
                                                                             "120) NULL, " + "PRIMARY KEY(`DataID`), " \
                                                                                             "UNIQUE INDEX " \
                                                                                             "`DataID_UNIQUE` (" \
                                                                                             "`DataID` ASC)); " \
                                                                                             "INSERT INTO " \
                                                                                             "`datatest`.`" + \
                                                                                tableName + "` (`DataID`) VALUES('0'); "

selectmaxidquery: str = "SELECT MAX(`DataID`) FROM `meteo_db`.`" + tableName + "`;"

def buildInsertArQuery(currentId, RTC_time, MFS_x, MFS_y, MFS_z,
                     THI_t, THI_h, THO_t, THO_h, TPO_t, TPO_p,
                     WND_s, WND_o, LLS, RDS, IRS):
    query = "INSERT INTO `meteo_db`.`" + tableName + "` (`DataID`, `RTC_time`, `MFS_x`, `MFS_y`, `MFS_z`, `THI_t`, " \
                                                     "`THI_h`, " + "`THO_t`, `THO_h`, `TPO_t`, `TPO_p`, `WND_s`, " \
                                                                   "`WND_o`, `LLS`, `RDS`, `IRS`) " + "VALUES('" + \
                                        str(currentId) + "', '" + str(insert_time).replace(',', '.') + "', " \
                                        "'" + str(MFS_x).replace(',', '.') + "', '" + str(MFS_y).replace(',', '.') + "', " \
                                        "'" + str(MFS_z).replace(',', '.') + "', '" + str(THI_t).replace(',', '.') + "', " \
                                        "'" + str(THI_h).replace(',', '.') + "', '" + str(THO_t).replace(',', '.') + "', " \
                                        "'" + str(THO_h).replace(',', '.') + "', '" + str(TPO_t).replace(',', '.') + "', " \
                                        "'" + str(TPO_p).replace(',', '.') + "', '" + str(WND_s).replace(',', '.') + "', " \
                                        "'" + str(WND_o).replace(',', '.') + "', '" + str(LLS) + "', " \
                                        "'" + str(RDS) + "', '" + str(IRS) + "');"
    return query

def selectLLSquery(dataid):
    query = "SELECT `LLS` FROM `meteo_db`.`" + tableName +"` WHERE `DataID` = " + str(dataid) + ";"
    return query

def buildInsertCamQuery(pathToPhoto, dataid):
    query = "UPDATE `meteo_db`.`" + tableName + "` SET `photo_path` = " + "'" + str(pathToPhoto) + "' WHERE `DataID` = " + str(dataid) + ";"
    return query


selectallquery: str = "SELECT * FROM `meteo_db`.`2020/5/13`;"

def selectMaxIDquery(date) 
    msg = "SELECT MAX(`DataID`) FROM `meteo_db`.`" + str(date) + "`;"
    return msg

def selectIDDatequery(dataid, date):
    query = "SELECT * FROM `meteo_db`.`" + str(date) + "` WHERE `DataID` = " + str(dataid) + ";"
    return query
