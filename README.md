# Human Navigation Online Beta

This is the beta version of Human Navigation Online that allows crowdsourced HRI experiments and competitions.

This Unity project is based on the competition software of Human Navigation task in the RoboCup@Home Simulation.  
https://github.com/RoboCupatHomeSim/human-navigation-unity

This project requires the Photon server and Oculus. Please see the following page for setting up the Photon server.  
http://www.sigverse.org/wiki/en/index.php?%28HSR%29Cleanup%20Task%20using%20Cloud%20and%20VR

---
## Prerequisites

- OS: Windows 10
- Unity version: 2018.4.11f1

---
## How to Use

### Build
1. Create a "Build" folder in this project folder.
2. Open this project with Unity.
3. Click [File]-[Build Settings].
4. Click [Build].
5. Select the "Build" folder.
6. Type a file name (e.g. HumanNavigation) and save the file.


### Modify Configuration

#### Human Navi Configuration
1. Copy the "SIGVerseConfig/HumanNavi/sample/HumanNaviConfig.json" to the "SIGVerseConfig/HumanNavi/" folder.
1. Open the config file.
1. Change "photonServerIP" (See the SIGVerse wiki page for more information.)
1. (For only the cloud server side) Change the value of "photonServerMachine" to "true".

#### SIGVerse Configuration (For only cloud server side)
1. Open the "SIGVerseConfig/SIGVerseConfig.json" file.
1. Type the IP address of ROS to "Rosbridge IP".


### Execution

#### Preparation
1. Copy the "SIGVerseConfig" and "TTS" folders into the "Build" folder.
1. Copy the "Build" folder to the another machine.
1. Modify the configuration files in each machine.

#### Execution procedure
1. Execute robot controllers on ROS side.
1. Execute the "HumanNavigation.exe" in the "Build" folder on both local and server machines.
1. Press the "Session Start" button on the local machine (for VR)
1. Press the "Session Start" button on the server machine (for robot)

---
## License

This project is licensed under the SIGVerse License - see the LICENSE.txt file for details.
