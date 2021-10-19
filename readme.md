Learn American Sign Language in VR
======================================
<!-->![screenshot](images/mason-bradford-the-fifth-force-logo.jpg)</!-->
## Introduction

This repo is for Human Computer Interaction (HCI 434). The idea behind this application is to provide an immersive learning experience for people who are looking to 
learn or practice American Sign Language (ASL). To begin, using your left hand, hold up a 1 for Learn Mode, a 2 for Practice Mode, and a 3 for Freestyle mode. To confirm your 
selection, using your right hand, hold up a thumbs up gesture at the same time as your left hand is hold your selection. 

To return to the menu, turn your left palm torwards your body and touch your index finger to your thumb!

## Known Issues
There are a few issues with the current implementation of the gesture recognition algorithm. 
1. Some signs are hard to detect or differentiate from other signs. For example, m, n, and o are all very similar. Therefore, when signing these in learn or practice modes
you might notice that the letters are progressed very quickly because the algorithm thinks you are signing the letters when you are not. This can be fixed by tweaking the 
recognition threshold and possibly rerecording the gestures to get a more unique position. 
2. Gestures that aren't static such as J and Z aren't currently recognized correctly. Currently the recognition algorithm doesn't recognize movements, only static poses. Therefore, 
letters such as J and Z had to be replaced by temporary placeholders. Try making the J movement. Somewhere in that path, the pose for J exists.

For Z, the gesture has been replaced all together temporarily. The current gesture for g is to touch your ring finger to your thumb while having all other fingers extended.
3. T also has been replaced by a temporary sign due to it's similarities to other signs. Moving forward, a way to differentiate these similar signs will need to be found. 

## Modes

### Learn Mode
In this mode you can learn the basics of the ASL alphabet. You will be shown a letter and its corresponding sign on the left. On the right you will see the letter you are currently 
signing. When you correctly sign the given letter, the application will progress to the next letter in the alphabet.

### Practice Mode
In this mode you will be tested on your ASL alphabet knowledge. On the left you will be prompted with a letter. The letter you are currently signing will be displayed on the right. 
Once you have signed the given letter, the application will progress to the next letter in the alphabet. Once you've reached the final letter, you win!

### Freestyle Mode
In this mode you can freely sign letters and the signed letter will appear on the screen. This mode is useful to test the accuracy of the recognition algorithm or just to practice at your own pace
with any letter you want. Try spelling your name!

___

## Screenshots

Below are a few in application screenshots.

![screenshot](Images/menu1.jpg)
##### Fig 1. Main Menu. #####  
---

![screenshot](Images/learn.jpg)
##### Fig 2. Learn Mode. ##### 
---

![screenshot](Images/practice.jpg) <br/>
##### Fig 3. Practice Mode. ##### 
---



___
