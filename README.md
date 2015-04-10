# vConnect (client application)

The vConnect client application is a C# application built with .NET 4.0. It was created by Troy Cosner and Charlie Snyder for their capstone software engineering project at Liberty University in 2015.

###The application utilizes the following external libraries:
  * [32feet.NET][1] (Bluetooth Library)
  * [NCalc][2] (Mathematical Expressions Evaluator)
  * [Json.NET][3] (JSON framework for .NET)
  
The application was developed to run on a **Windows 7** or **Windows 8** laptop using **Microsoft's Bluetooth Stack**. Outside of these requirements, the application may fail.
Consult [32feet.Net][4]'s documentation for more information.  
  
### Installation
Describe how to install here.  
  
### Usage
Describe usage here.

### Code Structure
Describe the basic classes and how they interact.

### Data Elements Currently Implemented

note: The data elements used are specified in the JSON schema on the web server.

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
  
  
[1]: https://32feet.codeplex.com/
[2]: https://ncalc.codeplex.com/
[3]: http://www.newtonsoft.com/json
[4]: https://32feet.codeplex.com/wikipage?title=Supported%20Hardware%20and%20Software