import mysql.connector
from mysql.connector import errorcode
import SQLQueries as sq
from time import sleep

class SQLConnector:
    def __init__(self):
        self.conn = None
        self.maintable = sq.tableName

    def connect(self):
        try:
            self.conn = mysql.connector.connect(host=sq.server, user=sq.user,
                                                password=sq.password, database=sq.database)
            return True
        except mysql.connector.Error as err:
            if err.errno == errorcode.ER_ACCESS_DENIED_ERROR:
                print("Something is wrong with your user name or password")
            elif err.errno == errorcode.ER_BAD_DB_ERROR:
                print("Database does not exist")
            else:
                print(err)
            return False

    def checkTableExists(self, table_name):
        if self.connect():
            cursor = self.conn.cursor()
            cursor.execute("""
                SELECT COUNT(*)
                FROM information_schema.tables
                WHERE table_name = '{0}'
                """.format(table_name.replace('\'', '\'\'')))
            if cursor.fetchone()[0] == 1:
                cursor.close()
                self.conn.close()
                return True

            cursor.close()
            self.conn.close()
        return False

    def writeToDB(self, msgquery):
        try:
            if not self.checkTableExists(self.maintable):
                if self.connect():
                    cursor = self.conn.cursor()
                    cursor.execute(sq.createtablequery)
                    cursor.close()
                    self.conn.close()

            if self.connect():
                cursor = self.conn.cursor()
                cursor.execute(msgquery)
                self.conn.commit()
                cursor.close()
                self.conn.close()
        except mysql.connector.Error as err:
            print(err)

    def readFromDB(self, msgquery):
        try:
            if not self.checkTableExists(self.maintable):
                if self.connect():
                    cursor = self.conn.cursor()
                    cursor.execute(sq.createtablequery)
                    cursor.close()
                    self.conn.close()

            sleep(1)
            if self.connect():
                cursor = self.conn.cursor(buffered=True)
                cursor.execute(msgquery)
                msg = cursor.fetchone()
                cursor.close()
                self.conn.close()
            try:
                return msg[0]
            except:
                return "None"
        except mysql.connector.Error as err:
            print(err)