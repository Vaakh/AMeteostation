from time import sleep
from datetime import datetime
from sh import gphoto2 as gp
import signal, os, subprocess
import SQLQueries as sqq
import SQLConnector as sqc

# Kill ghoto2 procees that
# starts every connection and doesn't
# allow work with camera

def killgphoto2Process():
    p = subprocess.Popen(['ps', '-A'], stdout = subprocess.PIPE)
    out, err = p.communicate()

    # Search for the line with gp process
    for line in out.splitlines():
        if b'gvfsd-gphoto2' in line:
            # Kill the procees
            pid = int(line.split(None, 1)[0])
            os.kill(pid, signal.SIGKILL)


shot_date = datetime.now().strftime("%Y-%m-%d")
shot_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
picID = "MeteoShots"

saveinsdcardCommand = ["--set-config", "capturetarget=1"]
clearCommand = ["--folder", "/store_00020001/DCIM/100CANON", \
                "-R", "--delete-all-files"]
triggerCommand = ["--trigger-capture"]
downloadCommand = ["--get-all-files"]

autoisoCommand = ["--set-config-value", "/main/imgsettings/iso=Auto"]
nightisoCommand = ["--set-config-value", "/main/imgsettings/iso=3200"]

dayshutterspeedCommand = ["--set-config-value", "/main/capturesettings/shutterspeed=1/5000"]
sunsetshutterspeedCommand = ["--set-config-value", "/main/capturesettings/shutterspeed=1/100"]
twilightshutterspeedCommand = ["--set-config-value", "/main/capturesettings/shutterspeed=1"]
nightshutterspeedCommand = ["--set-config-value", "/main/capturesettings/shutterspeed=30"]


folder_name = shot_date + picID
save_location = "/home/pi/Pictures/meteophoto/" + folder_name

def createSaveFolder():
    try:
        os.makedirs(save_location)
    except:
        print("Failed to create new directory: directory already exists")
    os.chdir(save_location)

def captureImages():
    gp(triggerCommand)
    sleep(40)
    gp(downloadCommand)
    sleep(3)
    gp(clearCommand)


def renameFiles(ID):
    for filename in os.listdir("."):
        if len(filename) < 13:
            if filename.endswith(".JPG"):
                os.rename(filename, (datetime.now().strftime("%Y-%m-%d %H:%M:%S") + ID + ".JPG"))
                print("Renamed the JPG")
            elif filename.endswith(".CR2"):
                os.rename(filename, (datetime.now().strftime("%Y-%m-%d %H:%M:%S") + ID + ".CR2"))
                print("Renamed the CR2")

def takePhotoWSQLData():
    killgphoto2Process()
    gp(saveinsdcardCommand)
    gp(clearCommand)
    createSaveFolder()

    scon = sqc.SQLConnector()
    currentid = scon.readFromDB(sqq.selectmaxidquery)
    query = sqq.selectLLSquery(currentid)
    LLS = scon.readFromDB(query)
    if LLS is not None:
        print("Current LLS: " + str(LLS))
        if int(LLS) <= 200:
            gp(autoisoCommand)
            gp(dayshutterspeedCommand)
        elif int(LLS) > 200 and int(LLS) <= 640:
            gp(autoisoCommand)
            gp(sunsetshutterspeedCommand)
        elif int(LLS) > 640 and int(LLS) < 940:
            gp(autoisoCommand)
            gp(twilightshutterspeedCommand)
        else:
            gp(autoisoCommand)
            gp(nightshutterspeedCommand)
    
        captureImages()
        renameFiles(picID)
    
        sqlpathtophoto = (save_location + shot_time + picID + ".JPG")
        query = sqq.buildInsertCamQuery(sqlpathtophoto, currentid)
        scon.writeToDB(query)
        print("Camera end\n")

def takePhotoWSQLDataTest():
    killgphoto2Process()
    gp(clearCommand)
    createSaveFolder()

    scon = sqc.SQLConnector()
    currentid = scon.readFromDB(sqq.selectmaxidquery)
    print(currentid)
    query = sqq.selectLLSquery(currentid)
    LLS = scon.readFromDB(query)
    if LLS < 200:
        gp(autoisoCommand)
        gp(dayshutterspeedCommand)
    elif LLS > 200 and LLS < 400:
        gp(autoisoCommand)
        gp(dayshutterspeedCommand)
    else:
        gp(autoisoCommand)
        gp(dayshutterspeedCommand)

    captureImages()
    renameFiles(picID)

    sqlpathtophoto = (save_location + shot_time + picID + ".JPG")
    query = sqq.buildInsertCamQuery(sqlpathtophoto, currentid)
    scon.writeToDB(query)

def takeTestPhoto():
    killgphoto2Process()
    gp(clearCommand)
    gp(autoisoCommand)
    gp(twilightshutterspeedCommand)
    createSaveFolder()
    captureImages()
    renameFiles(picID)

#takeTestPhoto() 
