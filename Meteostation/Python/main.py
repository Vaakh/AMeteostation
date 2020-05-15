import threading
import ArduinoLogic as a
import CameraLogic as c
import SQLQueries as sqq
from time import sleep
from datetime import datetime

while True:
    sqq.ct = datetime.now()
    sqq.k = ct.timetuple()
    a.processArData()
    print("End Arduino")
    sleep(10)
    c.takePhotoWSQLData()
    sleep(10)
