# vConnect (client application)

The vConnect client application is a C# application built with .NET 4.0. It was created by Troy Cosner and Charlie Snyder for their capstone software engineering project at Liberty University in 2015.

[vConnect Client Video][5]

###The application utilizes the following external libraries:
  * [32feet.NET][1] (Bluetooth Library)
  * [NCalc][2] (Mathematical Expressions Evaluator)
  * [Json.NET][3] (JSON framework for .NET)
  
The application was developed to run on a **Windows 7** or **Windows 8** laptop using **Microsoft's Bluetooth Stack**. Outside of these requirements, the application may fail.
Consult [32feet.Net][4]'s documentation for more information.  
  
### Installation
vConnect contains an installer that installs the vConnect application, a monitor application, and its required files in a directory. 

  1. Install vConnect.msi.
  2. Insert OBDLink LX module into the vehicle's OBDII port located underneath the driver-side dash board.
  3. On Windows laptop that has vConnect installed, go to Control Panel &rarr; Hardware and Sound &rarr; Devices and Printers.
  4. On the OBDLink LX module, press the BT Pair Button, located on the face of the module.
  5. Click Add a device in the Devices and Printers window. 
  6. Select OBDLink LX, and click the Next Button. (Note that the OBDLink LX module will only be discoverable for two minutes after pressing the BT Pair Button. If your laptop does not detect the device, press the BT Pair Button again)
  7. A window will be displayed with a PIN for the OBDLink LX module. Record this number and Click the Connect Button.
  8. Run C:\vConnect\vConnect.exe.
  9. Click through the message boxes stating that no server connection data was found, and that no OBDII connection info was detected.
  10. Double click on the vConnect Icon (check mark) located in the Windows tool bar to open the vConnect UI.
  11. Click the Set Pin Button.
  12. Enter the VIN you recorded in step 7, and click the OK Button.
  13. Click the Select OBDII Device Button.
  14. Select the OBDLink LX module, and click the Next Button. The Device Status Label should change to "Connected", and the Bluetooth Device ID should read "OBDLink LX".
  15. Click the Update Schema Button.
  16. Click the Configure Server Address Button.
  17. Enter the address of the server that vConnect's database is being stored on, and click the OK Button. (vconnect-danieladams456.rhcloud.com)
  18. Click the Configure Port Button.
  19. Enter the port number of the database server to be using to connect, and click the OK Button. (Port 80) 
  20. Click the Start Button.
 
If Configured correctly, the Polling status will read "Polling", and the Server Status will "Connected" if the laptop currently has internet access. 

NOTE: This installation process must be repeated if you wish to use a different OBDLink LX module/laptop pair. However, once you successfully complete this installation process for a OBDLink LX module/laptop pair, you can switch vehicles simply by inserting the OBDLink LX module into another vehicle's OBDII port.
  
### Usage
After installation, execute vConnect.exe.

If vConnect was correctly configured during installation, it will start-up automatically when Windows powers on, and will automatically connect to the paired OBDLink LX module and begin polling data.

To change vConnect's configuration settings, open up the Settings UI by double-clicking on the vConnect icon (green checkmark) in the Windows Taskbar.

  * To Stop Polling: Press the Stop Button.
  * To Start Polling: Press the Start Button. If you receive an error due to no schema file being detected, click the Update Schema Button and then click the Start Button.
  * To change the server connection information: Click the Configure Server Address Button and/or Configure Port Button, and enter the desired server address and port number.
  * To Connect to a New OBDII Module: 
 * If Poll Status is currently Polling, click the Stop Button.
 * Click the Disconnect BT Device Button if the Device Status reads Connected.
 * Remove the old OBDII Module from the vehicle's OBDII port.
 * Perform steps 2-7, 12-14.
  * To Update Schema: Click the Update Schema Button.
  * To close the settings UI: Click the Close Button.
  * To view help options: Click the Help Button.
  * To view the error log: Navigate to C:\vConnect, and open the error.log file.
  * To view the event log: Navigate to C:\vConnect, and open the event.log file.

The latest version of vConnect utilizes a Monitor application in order to ensure that the application continues to run. Whenever any situations cause the program to terminate, the monitor will restart it and begin polling for data.

NOTE: The buttons and labels on the vConnect Settings UI are all self-explanatory. However, for the Packets Sent label, it will be incremented if no error codes are detected, even though no message was actually sent to the server. This is because vConnect "successfully" detected that there were no error detected.
### Code Structure
![alt text](vConnect/Images/classDiagram.jpg)

### Architecture
![alt text](vConnect/Images/architecture.png)

### Data Elements Currently Implemented
| Element Name | Description | Unit |
| ------------ | ----------- | ---- |
| VIN | Vehicle Identification Number | Characters |
| vehicle_speed | Current Speed of Vehicle | km/h |
| engine_rpm | Current RPM of Vehicle | rpm |
| run_time_since_start | Time Engine Has Been Running | seconds |
| fuel_level | Percent of Tank Full | percent |
| oil_temp | Engine Oil Temperature | degrees Celsius |
| accel_pos | Relative Accelerator Position | Percent |
| dist_with_MIL | Distance Driven With Check-Engine Light On | km |
note: The data elements used are specified in the JSON schema on the web server.  
  
[1]: https://32feet.codeplex.com/
[2]: https://ncalc.codeplex.com/
[3]: http://www.newtonsoft.com/json
[4]: https://32feet.codeplex.com/wikipage?title=Supported%20Hardware%20and%20Software
[5]: https://www.youtube.com/watch?v=cx5EbRd3sHs