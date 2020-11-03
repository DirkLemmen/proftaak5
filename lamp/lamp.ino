
#include <Charliplexing.h>
#include "Myfont.h"

void displayNewBrightness(String value);

void setup()
{
  LedSign::Init();

  pinMode(A3, INPUT);
  Serial.begin(9600);

  displayNewBrightness("100");
}

int strLen = 0;
#define strMaxLen 10
char str[strMaxLen];

int brightness = 127;
bool manualValue = false;

void displayNewBrightness(String value) {
  unsigned char* txt = (unsigned char*)(value.c_str());
  int len = value.length();
  int xoff=0;
  for(int j=0; j<len; j++){
      Myfont::Draw(xoff + j*4, txt[j]);
  }
  delay(3000);
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

void loop()
{      
  if (!manualValue) {
    int ldrVal = analogRead(A3);
    brightness = map(ldrVal, 300, 1000, 127, 0);
  }
  
  if (Serial.available() > 0) {
    char ch = Serial.read();
  
    if (ch == '\n') {
      manualValue = true;
      int newBrightness = fast_atoi(str);
      if (newBrightness == -1) {
        manualValue = false;  
      }
      else {
        if (newBrightness > 127) newBrightness = 127;
        if (newBrightness < 0) newBrightness = 0;
      
        brightness = newBrightness;
        //displayNewBrightness(str);
      }

      memset(str, 0, strMaxLen);
      strLen = 0;
    }
    else {
      str[strLen++] = ch;
    }
  }
  
  LedSign::SetBrightness(brightness);
  
  for (int row = 0; row < DISPLAY_ROWS; row++)
    for (int col = 0; col < DISPLAY_COLS; col++)
      LedSign::Set(col, row, 1);
}
