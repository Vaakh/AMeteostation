from datetime import datetime

server = 'localhost'
user = 'Meteostation'
database = 'datatest'
password = 'OpenSourse!123'

ct = datetime.now()
k = ct.timetuple()
tableName = str(k[0]) + "/" + str(k[1]) + "/" + str(k[2])

createtablequery = "CREATE TABLE `datatest`.`" + tableName + "` ( `DataID` INT NOT NULL, `RTC_time` TIME NULL, " \
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
                                                                                             "`DataID` ASC) VISIBLE); " \
                                                                                             "INSERT INTO " \
                                                                                             "`datatest`.`" + \
                                                                                tableName + "` (`DataID`) VALUES('0'); "

selectmaxidquery: str = "SELECT MAX(`DataID`) FROM `datatest`.`" + tableName + "`;"

def buildInsertQuery(currentId, RTC_time, MFS_x, MFS_y, MFS_z,
                     THI_t, THI_h, THO_t, THO_h, TPO_t, TPO_p,
                     WND_s, WND_o, LLS, RDS, IRS):
    query = "INSERT INTO `datatest`.`" + tableName + "` (`DataID`, `RTC_time`, `MFS_x`, `MFS_y`, `MFS_z`, `THI_t`, " \
                                                     "`THI_h`, " + "`THO_t`, `THO_h`, `TPO_t`, `TPO_p`, `WND_s`, " \
                                                                   "`WND_o`, `LLS`, `RDS`, `IRS`) " + "VALUES('" + \
                                        str(currentId) + "', '" + str(RTC_time).replace(',', '.') + "', " \
                                        "'" + str(MFS_x).replace(',', '.') + "', '" + str(MFS_y).replace(',', '.') + "', " \
                                        "'" + str(MFS_z).replace(',', '.') + "', '" + str(THI_t).replace(',', '.') + "', " \
                                        "'" + str(THI_h).replace(',', '.') + "', '" + str(THO_t).replace(',', '.') + "', " \
                                        "'" + str(THO_h).replace(',', '.') + "', '" + str(TPO_t).replace(',', '.') + "', " \
                                        "'" + str(TPO_p).replace(',', '.') + "', '" + str(WND_s).replace(',', '.') + "', " \
                                        "'" + str(WND_o).replace(',', '.') + "', '" + str(LLS) + "', " \
                                        "'" + str(RDS) + "', '" + str(IRS) + "');"
    return query
