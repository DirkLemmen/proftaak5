
#include <Charliplexing.h>
#include <BLEduino.h>
#include "Myfont.h"


BLEduino BLE;

void displayNewBrightness(String value);

void setup()
{
  LedSign::Init();

  pinMode(A3, INPUT);
  Serial.begin(9600);

  BLE.begin(); //Initialize BLE object
  BLE.sendCommand(COMMAND_RESET); //Start advertising BLEduino

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
   if(BLE.available(FIRMATA_READ)){
    
        //Read packet
        //The parameter is also optional here.  
        //BLE.read() will return the last packet received, regardless of pipe.
        BLEPacket packet = BLE.read(FIRMATA_READ);

        //Parse packet
        //uint8_t length = packet.length; //not used here
        uint8_t * data = packet.data;
        /************************************
        Packet Structure for LED data
        ||    0    |     1    |      2    ||
        || LED Pin | Not Used | Pin State ||
        Using firmata pipe
        *************************************/

        //Parse LED data
        byte led_pin = data[0];
        byte led_state = data[2];

        pinMode(led_pin, OUTPUT);
        digitalWrite(led_pin, led_state);

    }
    
  if (!manualValue) {
    int ldrVal = analogRead(A3);
    brightness = map(ldrVal, 100, 1000, 127, 0);
    if (brightness > 127) brightness = 127;
    if (brightness < 0) brightness = 0;
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
