#include <Wire.h> //I2C lib

#include <DHT.h> //to THO and THI sensors
#include <DHT_U.h>

#include <Adafruit_Sensor.h> //general library for all Adafruit sensors

#include <Adafruit_MLX90614.h> //IRS

#include <Adafruit_HMC5883_U.h> //MFS

#include <Adafruit_BMP085_U.h> //TPO

#include <Time.h> //for RTC
#include <TimeLib.h>

#include <DS1307RTC.h> //for RTC

Adafruit_HMC5883_Unified mag = Adafruit_HMC5883_Unified(12345);
bool magBegin;

Adafruit_MLX90614 mlx = Adafruit_MLX90614();
bool mlxBegin;

Adafruit_BMP085_Unified bmp = Adafruit_BMP085_Unified(10085);
bool bmpBegin;

void setup() {
  Serial.begin(9600);
  magBegin = mag.begin();
  mlxBegin = mlx.begin();
  bmpBegin = bmp.begin();
}

void loop() {
  float magX = 404;
  float magY = 404;
  float magZ = 404;
  
  float IRS = 404;

  float TPO_t;
  float TPO_p;
  
  if (magBegin) {
    /* Get a new sensor event */ 
    sensors_event_t event; 
    mag.getEvent(&event);
    magX = event.magnetic.x;
    magY = event.magnetic.y;
    magZ = event.magnetic.z;
  }
  else {
    magBegin = mag.begin();
  }

  if (mlxBegin) {
    IRS = mlx.readObjectTempC();
  }
  else {
    mlxBegin = mlx.begin();
  }

  if (bmpBegin) {
    sensors_event_t event;
    bmp.getEvent(&event);
    bmp.getTemperature(&TPO_t);
    bmp.getPressure(&TPO_p);
  }
  else {
    bmpBegin = bmp.begin();
  }

  

}
