# AMeteostatoin
It is software for my astronomical meteostation (that's why "A" in the name) 
- Software contains: 
  - Module for Arduino Uno, that intended for sensor's data collection; 
  - Module for Rasberri PI, that collect data from Arduino and proccess it; 
  - Module for Raspberri PI, that control Canon's EOS camera and process photos; 
  - Module for Raspberri PI, that respinsible for server and web-site to presenting data to the user; 
- Now I have realisation for first 3 points, but processing have some bugs. How I'm working on it. 

TO DO: 
- rewrite "else" at CanonAPI.cs (CheckError (362), ReportError (427))
