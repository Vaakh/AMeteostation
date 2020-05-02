#include <HIDKeyboard.h>

HIDKeyboard keyboard;

int LLSPort = A1;

void setup() {
  pinMode(LLSPort, INPUT);
  keyboard.begin(); // Start communication
  delay(2000); // Wait for device to be found as a keyboard
}

void loop() {

  int LLS = 0;
  LLS = analogRead(LLSPort);
  if (LLS > 100) { 
    keyboard.pressKey(' ');
    keyboard.releaseKey();
    delay(100);
    }
  

}
