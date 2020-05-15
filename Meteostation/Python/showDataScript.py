import mysql.connector
from mysql.connector import errorcode
import SQLQueries as sq
import csv

FILENAME = "/home/pi/data.csv"

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
        

maxID = int(readIDFromDB(sq.selectmaxidqueryTEST))
for i in range (1, maxID, 1):
    t = readFromDB(sq.selectIDquery(i))
    with open(FILENAME, "a", newline="") as file:
        writer = csv.writer(file)
        writer.writerow(t)
