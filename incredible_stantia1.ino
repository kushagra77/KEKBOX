// C++ code
//
/*
  Ping Sensor

  This sketch reads a PING))) ultrasonic
  rangefinder and prints on the serial monitor
  if the distanced to closest object is unsafe.
  To do this, it sends a pulse to the sensor to 
  initiate a reading, then listens for a pulse to return.  
  The length of the returning pulse is proportional to the
  distance of the object from the sensor.
	
  Used code from: http://www.arduino.cc/en/Tutorial/Ping
*/

#define ms_to_cm  0.01723

int cm1 = 0 , cm2 = 0, cm3 = 0;

long readUltrasonicDistance(int triggerPin, int echoPin)
{
  pinMode(triggerPin, OUTPUT);  // Clear the trigger
  digitalWrite(triggerPin, LOW);
  delayMicroseconds(2);
  // Sets the trigger pin to HIGH state for 10 microseconds
  digitalWrite(triggerPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(triggerPin, LOW);
  pinMode(echoPin, INPUT);
  // Reads the echo pin, and returns the sound wave travel time in cm
  return ms_to_cm * pulseIn(echoPin, HIGH);
}

void setup()
{
  Serial.begin(9600);

}

void loop()
{
  // measure the ping time in cm
  cm1 = readUltrasonicDistance(7, 7);
  cm2 = readUltrasonicDistance(8, 8);
  cm3 = readUltrasonicDistance(4, 4);
  // moves robot away to maintain safe distance
  if (cm1 < 7) {
  	Serial.print("1 TOO CLOSE\n");
  }
  if (cm2 < 7) {
  	Serial.print("2 TOO CLOSE\n");
  }
  if (cm3 < 7) {
  	Serial.print("3 TOO CLOSE\n");
  }
  delay(50); // Wait for 50 millisecond(s)
}