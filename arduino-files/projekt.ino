void setup() {
   Serial.begin(9600);
   /*
   for (int pin = 3; pin <= 13; pin++) {
     pinMode(pin, OUTPUT);
   }
   */
}

void loop() {
  if (Serial.available() > 0) { 
    char command = Serial.read(); 
    Serial.println(command);

    switch(command){
      case 'a':
        digitalWrite(3, HIGH);
        break;
      case 'b':
        digitalWrite(4, HIGH);
        break;
      case 'c':
        digitalWrite(5, HIGH);
        break;
      case 'd':
        digitalWrite(6, HIGH);
        break;
      case 'x':
        digitalWrite(7, HIGH);
        break;
      case 'q':
        digitalWrite(8, HIGH);
        break;
      case 'w':
        digitalWrite(9, HIGH);
        break;
      case 'e':
        digitalWrite(10, HIGH);
        break;
      case 'r':
        digitalWrite(11, HIGH);
        break;
      case 't':
        digitalWrite(12, HIGH);
        break;
      case 'y':
        digitalWrite(13, HIGH);
        break;
       default:
        break;
    }
    
  } else {
    for (int pin = 3; pin <= 13; pin++) {
      digitalWrite(pin, LOW);  // Wyłączenie wszystkich pinów
    }
  }
  /*
  Serial.print("Pin 3: " + 
  String(digitalRead(3)) + 
  " Pin 4: " + 
  String(digitalRead(4)) + 
  " Pin 5: " + 
  String(digitalRead(5)) + 
  " Pin 6: " + 
  String(digitalRead(6)) + 
  " Pin 7: " + 
  String(digitalRead(7)) + 
  " Pin 8: " + 
  String(digitalRead(8)) + 
  " Pin 9: " + 
  String(digitalRead(9)) + 
  " Pin 10: " + 
  String(digitalRead(10)) + 
  " Pin 11: " + 
  String(digitalRead(11)) + 
  " Pin 12: " + 
  String(digitalRead(12)) + 
  " Pin 13: " + 
  String(digitalRead(13)) + 
  "\n");
  */

}
