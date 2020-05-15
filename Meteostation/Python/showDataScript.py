import mysql.connector
from mysql.connector import errorcode
import SQLQueries as sq
import csv


def readIDFromDB(msgquery):
    try:
        conn = mysql.connector.connect(host=sq.server, user=sq.user,
                                      password=sq.password, database=sq.database)
        cursor = conn.cursor(buffered=True)
        cursor.execute(msgquery)
        msgs = cursor.fetchone()
        cursor.close()
        conn.close()
        return msgs[0]
    except mysql.connector.Error as err:
        print(err)

def readFromDB(msgquery):
    try:
        conn = mysql.connector.connect(host=sq.server, user=sq.user,
                                      password=sq.password, database=sq.database)
        cursor = conn.cursor(buffered=True)
        cursor.execute(msgquery)
        msgs = cursor.fetchone()
        cursor.close()
        conn.close()
        return msgs
    except mysql.connector.Error as err:
        print(err)
        

date = input('Enter date you want to get(YYYY/MM/DD):')
FILENAME = "/home/pi/" + date + ".csv"
maxID = int(readIDFromDB(selectMaxIDquery(date)))
for i in range (1, maxID, 1):
    t = readFromDB(sq.selectIDDatequery(i, date))
    with open(FILENAME, "a", newline="") as file:
        writer = csv.writer(file)
        writer.writerow(t)
