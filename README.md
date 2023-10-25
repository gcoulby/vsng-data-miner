This project aims to extract variables from a running Virtual Sailor NG process for the purposes of extracting real-time data from the simulator. 

This finds a boat's name in a specific memory location that has 11, 4-byte float values precedining it and extracts those floats. 

Example: 
```log
Found the boat named Adriatic at RAM address: 0x7FF6AD169B9B
Value: 0xA8DF873F: 1.061513 - Lat
Value: 0x30644842: 50.09784 - Lon
Value: 0xCB15B0C0: -5.5026603 - ??
Value: 0x8C43AA43: 340.5277 - Set Heading
Value: 0x15E34041: 12.05544 - STW
Value: 0xD2E34041: 12.05562 - SOG
Value: 0x783EAA43: 340.48804 - Act Heading
Value: 0xF8FFF943: 499.99976 - RPM
Value: 0xE80BFA3C: 0.030523255 - ??
Value: 0xD00BD242: 105.02307 - ??
Value: 0x4D42EB41: 29.407373 - ??
```

I am still on the hunt for more variables in the absence of a suitable SDK/API within the game.