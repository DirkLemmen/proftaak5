
#include <Charliplexing.h>
#include <BLEduino.h>
#include "Myfont.h"


BLEduino BLE;

void displayText(char* txt);

void setup()
{
  LedSign::Init();

  pinMode(A3, INPUT);
  Serial.begin(9600);

  displayText(".");
  delay(300);
  displayText("..");
  delay(300);
  displayText("...");
  delay(300);
}

int strLen = 0;
#define strMaxLen 10
char str[strMaxLen];

int brightness = 127;
bool manualValue = false;
bool showingNewValue = false;
long long newValueStamp = 0;
char newValueStr[strMaxLen];

void displayText(char* txt) {
  int len = strlen(txt);
  if (len > 3) len = 3;
  int xoff=3-len;
  for(int j=0; j<len; j++){
      Myfont::Draw(xoff + j*4, txt[j]);
  }
}

// https://stackoverflow.com/questions/16826422/c-most-efficient-way-to-convert-string-to-int-faster-than-atoi
int fast_atoi(const char * str)
{
    int val = 0;
    while( *str ) {
        val = val*10 + (*str++ - '0');
    }
    return val;
}

inline void keepBrightnessBetweenBounds() {
  if (brightness > 127) brightness = 127;
  if (brightness < 0) brightness = 0;
}

void handleBrightnessChange() {
  manualValue = true;
      int newBrightness = fast_atoi(str);

      if (strcmp(str, "-1") == 0) {
        manualValue = false;
      }
      else {
        brightness = newBrightness;
        keepBrightnessBetweenBounds();

        newValueStamp = millis();
        showingNewValue = true;

        int percentage = map(brightness, 0, 127, 0, 100);
        //sprintf(newValueStr, "%d", percentage);
        sprintf(newValueStr, "%s", str);
        LedSign::Clear();
      }

}

void loop()
{      
  if (!manualValue) {
    int ldrVal = analogRead(A3);
    brightness = map(ldrVal, 100, 1000, 127, 0);
    keepBrightnessBetweenBounds();
  }
  
  if (Serial.available() > 0) {
    char ch = Serial.read();
  
    if (ch == '\n') {
      
      memset(str, 0, strMaxLen);
      strLen = 0;
    }
    else {
      str[strLen++] = ch;
    }
  }
  
  LedSign::SetBrightness(brightness);

  if (showingNewValue) { 
    displayText(newValueStr);
    if (millis() - newValueStamp > 1000) {
      showingNewValue = false;
    }
  } else {
    for (int row = 0; row < DISPLAY_ROWS; row++)
      for (int col = 0; col < DISPLAY_COLS; col++)
        LedSign::Set(col, row, 1);
  }
}
