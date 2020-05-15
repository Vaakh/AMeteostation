#include <Wire.h> //I2C lib

#include <ArduinoJson.h>

#include <DHT.h> //to THO and THI sensors
#include <DHT_U.h>

#include <Adafruit_Sensor.h> //general library for all Adafruit sensors

#include <Adafruit_MLX90614.h> //IRS

#include <Adafruit_HMC5883_U.h> //MFS

#include <Adafruit_BMP085_U.h> //TPO

//#include <Time.h> //for RTC
//#include <TimeLib.h>

#include <RTClib.h> //for RTC

#define THOPIN 8
#define THIPIN 7
#define DHTTYPE    DHT22

Adafruit_HMC5883_Unified mag = Adafruit_HMC5883_Unified(12345);
bool magBegin;

Adafruit_MLX90614 mlx = Adafruit_MLX90614();
bool mlxBegin;

Adafruit_BMP085_Unified bmp = Adafruit_BMP085_Unified(10085);
bool bmpBegin;

RTC_DS1307 RTC; //RTC object where DS1307 - chip, that installed in clock
String year, month, day, hour, minute, second, time, date;
bool rtcBegin;

DHT_Unified thi(THIPIN, DHTTYPE);
bool thiBegin;

DHT_Unified tho(THOPIN, DHTTYPE);
bool thoBegin;

int LLSPort = A3;

int RDSPort = A2;

void setup() {

  pinMode(LLSPort, INPUT);
  pinMode(RDSPort, INPUT);
  Serial.begin(9600);
  Wire.begin();
  magBegin = mag.begin();
  mlxBegin = mlx.begin();
  bmpBegin = bmp.begin();
  rtcBegin = RTC.begin();
  if (rtcBegin){
    RTC.adjust(DateTime(__DATE__, __TIME__));
  }
  thi.begin();
  tho.begin();

}

void loop() {
  
  float magX = 404;
  float magY = 404;
  float magZ = 404;
  
  float IRS = 404;

  float TPO_t = 404;
  float TPO_p = 404404;

  float THI_t = 404;
  float THI_h = 404;

  float THO_t = 404;
  float THO_h = 404;

  int LLS = 404404;

  int RDS = 404404;

  StaticJsonDocument<200> data;
  
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

  if (rtcBegin) {
    // get date/time values and mve it to string
    DateTime datetime = RTC.now();
    year = String (datetime.year(), DEC);
    month = String (datetime.month(), DEC);
    day = String (datetime.day(), DEC);
    hour = String (datetime.hour(), DEC);
    minute = String (datetime.minute(), DEC);
    second = String (datetime.second(), DEC);
    // build date/time strings
    date = year + "/" + month + "/" + day;
    time = hour + ":" + minute + ":" + second;
  }
  else {
    rtcBegin = RTC.begin();
    if (rtcBegin){
      RTC.adjust(DateTime(__DATE__, __TIME__)); 
    }
  }


  // Get temperature event
  sensors_event_t event;
  thi.temperature().getEvent(&event);
  if (isnan(event.temperature)) {
    thi.begin();
  }
  else {
    THI_t = event.temperature;
  }
  // Get humidity event
  thi.humidity().getEvent(&event);
  if (isnan(event.relative_humidity)) {
    thi.begin();
  }
  else {
    THI_h = event.relative_humidity;
  }


  tho.temperature().getEvent(&event);
  if (isnan(event.temperature)) {
    tho.begin();
  }
  else {
    THO_t = event.temperature;
  }
  // Get humidity event
  tho.humidity().getEvent(&event);
  if (isnan(event.relative_humidity)) {
    tho.begin();
  }
  else {
    THO_h = event.relative_humidity;
  }

  LLS = analogRead(LLSPort);

  RDS = analogRead(RDSPort);
  
  data["Data_From_Arduino"] = 0;
  data["RTC_date"] = date;
  data["RTC_time"] = time;
  data["MFS_x"] = magX;
  data["MFS_y"] = magY;
  data["MFS_z"] = magZ;
  data["THI_t"] = THI_t;
  data["THI_h"] = THI_h;
  data["THO_t"] = THO_t;
  data["THO_h"] = THO_h;
  data["TPO_t"] = TPO_t;
  data["TPO_p"] = TPO_p;
  data["WND_s"] = 404;
  data["WND_o"] = 404;
  data["LLS"] = LLS;
  data["RDS"] = RDS;
  data["IRS"] = IRS;

  serializeJson(data, Serial);
  Serial.print("\n");

  delay(1000);

  //String data = date + " " + time + " " + magX + " " + magY + " " + magZ + " " + IRS + " " + TPO_t + " " + TPO_p + " " + THI_t + " " + THI_h + " " + THO_t + " " + THO_h + " " + LLS + " " + RDS; 
  
  //Serial.println(data);

}
