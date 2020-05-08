import threading
import ArduinoLogic as a
import CameraLogic as c
from time import sleep


while True:
    a.processArData()
    print("End Arduino")
    sleep(10)
    c.takePhotoWSQLDataTest()
    sleep(10)
