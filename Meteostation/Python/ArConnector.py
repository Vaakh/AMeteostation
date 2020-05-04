from SerialPorts import serial_ports, curr_speed
import serial

class ArdConnector:
    def __init__(self):
        self.speed = []
        self.port = []
        self.port.append(serial_ports())
        self.speed.append(curr_speed)
        self.current_port = str(self.port[0]).replace('[', '').replace(']', '').replace('\'', '')
        print(self.current_port)

    def connect(self):
        try:
            self.current_port = serial.Serial(self.current_port, curr_speed)
            return True
        except Exception as e:
            print(e)
            return False

    def send(self, msg):
        if self.connect():
            self.current_port.write(msg)
            self.current_port.close()

    def get(self):
        msg = None
        if self.connect():
            if self.current_port:
                msg = self.current_port.read_until()
                self.current_port.flushInput()
                self.current_port.close()
        return msg
