/**************************************************************************************
 * Эта программа написана для Уно. Для Мега, Дуо и Леонардро контакты будут отличаться


В файлах "cvs" столбцы разделяет знак ; а не ","!
чтение и запись данных на SD карту с использованием RTC
стоит отметить, что в этом скетче предполагается, что часы питаются от батарейки,
поэтому POW_PIN - питание карты SD

для SD
DI = MOSI = CMD
DAT0 = D0 = MISO
CLK = SCK
MOSI = pin 11
MISO = pin 12
SCLK = pin 13
**************************************************************************************/
#include <RTClib.h> // библиотека к часам
#include "stDHT.h" // библиотека к DHT
#include <Wire.h> // I2C library
#include <SparkFunMLX90614.h> // библиотека для ик-датчика
#include <Adafruit_BMP085.h> // библиотека для барометра, потом надо бы заменить на bmp180 

#define adress 0x1E // изначальный адрес для компаса, иcп. при побитовом занесении числа

RTC_DS1307 RTC; // объект RTC (DS1307 - чип, который стоит на часах)
DHT insideDHT(DHT22); // объект DHT22
DHT outsideDHT(DHT22); // объект DHT22
IRTherm therm; // объект для ик-датчика
Adafruit_BMP085 bmp; // объект для барометра

// контакты питания и выбора CS для SD-карты
const int SD_CS_PIN = 9;
const int SD_POW_PIN = 8;

// для камеры
const int CAM_PIN = 4;

// для подогрева
const int PODOGREV_PIN = 5;

// порты для датчика дождя
int aWaterPort = A1;

// порты для датчиков DHT22 
int dDHT22OUTPort = 2;
int dDHT22INPort = 3;

// порты для датчика света
int aLightPort = A2;

// переменные для датчика света
int LightLevel;
boolean night = false;

// переменные для точки росы
float t_out, h_out;
float t_in, h_in; 
float T_ros, r;
// переменные для барометра
float bmp_press, bmp_temp;

// переменные для датчиков дождя и облачности
int WaterLevel;
float infro_red;

// переменные для компаса
int x, y, z;

// пременные даты и времени
String year, month, day, hour, minute, second, time, date;


void setup() 
{
 Serial.begin(57600);
 Serial.println("Start Setup");

 pinMode (CAM_PIN, OUTPUT);
 digitalWrite (CAM_PIN, HIGH);

 pinMode (PODOGREV_PIN, OUTPUT);
 digitalWrite (PODOGREV_PIN, HIGH);
 
 pinMode(dDHT22OUTPort, INPUT);    
 digitalWrite(dDHT22OUTPort, HIGH);
 pinMode(dDHT22INPort, INPUT);    
 digitalWrite(dDHT22INPort, HIGH);
  
 pinMode(aLightPort, INPUT);
 
 pinMode(aWaterPort, INPUT);

 // инициализация Wire и RTC
 Wire.begin();
 
 RTC.begin();
 // если RTC не запущены, загрузить дату/время с компа
 if (! RTC.isrunning()){
  Serial.println(F("RTC is NOT running!"));
  RTC.adjust(DateTime(__DATE__, __TIME__));
 }
 Serial.println("Start RTC");
 
 
 // настройка I2c для компаса
 Wire.beginTransmission(adress);
 Wire.write(0x02);
 Wire.write(0x00);
 Wire.endTransmission(adress);
 Serial.println("Start Compas");
 
 therm.begin(); // инициализация ик-датчика
 therm.setUnit(TEMP_K); // задаются единицы измерения (есть Цельсии TEMP_C,
 // Кельвины TEMP_K, и Фаренгейт TEMP_F для особых ценителей)
 Serial.println("Start IK Sensor");

 Serial.println(F("Alright, finaly starting up"));
}


void loop() 
{
  Serial.println("Flag 1");
  // получить значение даты и времени и перевести в строковые значения
  DateTime datetime = RTC.now();
  year = String (datetime.year(), DEC);
  month = String (datetime.month(), DEC);
  day = String (datetime.day(), DEC);
  hour = String (datetime.hour(), DEC);
  minute = String (datetime.minute(), DEC);
  second = String (datetime.second(), DEC);
  // cобрать строку текущий даты и времени
  date = year + "/" + month + "/" + day;
  time = hour + ":" + minute + ":" + second;

  Serial.println("Flag 2");
  
  // считывание датчика давления
  bmp_temp = bmp.readTemperature();
  bmp_press = bmp.readPressure();

  Serial.println("Flag 3");
  
  // считывание DHT для точки росы
  t_out = outsideDHT.readTemperature(dDHT22OUTPort);  
  h_out = outsideDHT.readHumidity(dDHT22OUTPort);   

  Serial.println("Flag 4");

  // считывание температуры/влажности внутри
  t_in = insideDHT.readTemperature(dDHT22INPort);  
  h_in = insideDHT.readHumidity(dDHT22INPort);

  Serial.println("Flag 5");
 
  // считывание датчика света
  LightLevel = analogRead (aLightPort);

  Serial.println("Flag 6");
  
  // считывание данных с ик-датчика и датчика даждя
  infro_red = (therm.read(), 2); 
  WaterLevel = analogRead (aWaterPort);

  Serial.println("Flag 7");
  
  // чтение компаса
  Wire.beginTransmission(adress);
  Wire.write(0x03);
  Wire.endTransmission(adress);
  Wire.requestFrom(adress, 6);
  if (6 <= Wire.available()) {
   x = Wire.read()<<8; x |= Wire.read();
   y = Wire.read()<<8; y |= Wire.read();
   z = Wire.read()<<8; z |= Wire.read();
  };

 Serial.println("Flag 8");
 
 Serial.print(date); Serial.print (" "); Serial.println (time);    
 Serial.print(t_in); Serial.print(" "); Serial.print(h_in); Serial.print(" "); Serial.print(t_out); Serial.print(" "); Serial.println(h_out);
 Serial.print(bmp_temp); Serial.print(" "); Serial.println(bmp_press);
 Serial.print(LightLevel); Serial.print(" "); Serial.print(WaterLevel); Serial.print(" "); Serial.println(infro_red);
 Serial.print(x); Serial.print(" "); Serial.print(y); Serial.print(" "); Serial.println(z);

 r = (17,27*t_out)/(237,7+t_out) + log(h_out);
 T_ros = (237,7*r)/(17,27 - r);
 if (t_in <= T_ros + 2) digitalWrite (PODOGREV_PIN, LOW);
 else digitalWrite (PODOGREV_PIN, HIGH);
  
 digitalWrite (CAM_PIN, LOW);
 delay (10000);
 digitalWrite (CAM_PIN, HIGH);
 Serial.println("All Ok");
}

