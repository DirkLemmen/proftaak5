
#include <Charliplexing.h>
#include "Myfont.h"

void displayText(char* txt);
void turnOnAllLeds();

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

  turnOnAllLeds();
}

int strLen = 0;
#define strMaxLen 10
char str[strMaxLen]; // buffer for storing input from user

int brightness = 127; // current lamp brightness (0 - 127)
bool manualBrightness = false; // is the brightness being manually controlled

bool isFlickering = false; // is the lamp flickering
long long flickerStamp = 0;
bool flickerOn = true;

bool showingNewValue = false; // showing new brightness in text
long long newValueStamp = 0;
char newValueStr[strMaxLen];

// display text on the lamp. 3 characters max.
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

// keep brightness between 0 and 127.
inline void keepBrightnessBetweenBounds() {
  if (brightness > 127) brightness = 127;
  if (brightness < 0) brightness = 0;
}

// handle user input for patterns.
void handlePatternChange() {
  if (str[0] != '!' && str[0] != '#') return;

  if (strcmp(str, "#BLNK") == 0) {
    isFlickering = true;
    flickerOn = true;
    flickerStamp = millis();
  }
  if (strcmp(str, "!BLNK") == 0) {
    isFlickering = false;
  }
}

// handle user input for manual brightness changes.
void handleBrightnessChange() {
  if (str[0] == '!' || str[0] == '#') return;
  
  manualBrightness = true;
  int newBrightness = fast_atoi(str);
  
  if (strcmp(str, "-1") == 0) {
    manualBrightness = false;
  }
  else {
    brightness = newBrightness;
    keepBrightnessBetweenBounds();
    
    newValueStamp = millis();
    showingNewValue = true;
    
    int percentage = map(brightness, 0, 127, 0, 100); // calculate percentage to show on lamp.
    sprintf(newValueStr, "%d", percentage);
    LedSign::Clear();

    LedSign::SetBrightness(brightness);
  }
}

// Turn on all leds.
void turnOnAllLeds() {
  for (int row = 0; row < DISPLAY_ROWS; row++)
    for (int col = 0; col < DISPLAY_COLS; col++)
      LedSign::Set(col, row, 1);
}

void loop()
{ 
  // if the brightness is not manually controlled, calculate brightness based on input from LDR.
  if (!manualBrightness) {
    int ldrVal = analogRead(A3);
    brightness = map(ldrVal, 100, 1000, 127, 0);
    keepBrightnessBetweenBounds();
  }

  // handle data retrieved from user.
  if (Serial.available() > 0) {
    char ch = Serial.read();
  
    if (ch == '\n') {
      handleBrightnessChange();
      handlePatternChange();
      
      memset(str, 0, strMaxLen);
      strLen = 0;
    }
    else {
      str[strLen++] = ch;
    }
  }

  // show text on screen if value recently changed.
  if (showingNewValue) { 
    displayText(newValueStr);
    if (millis() - newValueStamp > 1000) {
      showingNewValue = false;
      turnOnAllLeds();
    }
  }

  // either flicker or display a constant brightness.
  if (isFlickering) {
    if (millis() - flickerStamp < 300) {
      LedSign::SetBrightness(flickerOn ? brightness : 0);        
    }
    else {
      flickerStamp = millis();
      flickerOn = !flickerOn;
    }       
  } else {
    LedSign::SetBrightness(brightness);
  }
}
