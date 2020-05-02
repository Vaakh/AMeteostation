from SerialPort import serial_ports, curr_speed
import serial


class ArduinoConnector:
    def __init__(self):
        self.port.addItems(serial_ports())
        self.speed.addItems(curr_speed)
        self.current_port = None

    def connect(self):
        try:
            self.current_port = serial.Serial(self.port[0], curr_speed)
        except Exception as e:
            print(e)

    def send(self, msg):
        if self.current_port:
            self.current_port.write(msg)

    def get(self):
        msg = None
        if self.current_port:
            if self.current_port:
                msg = self.current_port.read_until()
                self.current_port.flushInput()
        return msg

