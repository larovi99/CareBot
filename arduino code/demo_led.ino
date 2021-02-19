const int yellowLed = 10;
const int redLed = 12;
const int greenLed = 8;

char incomingdata = '0';
void setup() {  
    Serial.begin(9600);
    pinMode(yellowLed, OUTPUT);
    pinMode(redLed, OUTPUT);
    pinMode(greenLed, OUTPUT);
}  
void loop() {  
    incomingdata = Serial.read(); {  
        switch (incomingdata)
        {
          case 'B':
          digitalWrite(yellowLed, HIGH);
          break;

          case 'R':
          digitalWrite(redLed, HIGH);
          break;

          case 'G':
          digitalWrite(greenLed, HIGH);
          break;

          case 'Z':
          digitalWrite(yellowLed, LOW);
          break;

          case 'X':
          digitalWrite(redLed, LOW);
          break;

          case 'C':
          digitalWrite(greenLed, LOW);
          break;

          case 'V':
          {
            digitalWrite(yellowLed, HIGH);
            digitalWrite(redLed, HIGH);
            digitalWrite(greenLed, HIGH);
            break;
          }

          case 'M':
          {
            digitalWrite(yellowLed, LOW);
            digitalWrite(redLed, LOW);
            digitalWrite(greenLed, LOW);
            break;
          }

          default:
          break;
        }
    }      
}  
