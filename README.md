# Travis Build Test
[![Build Status](https://travis-ci.org/dhcgn/GetPerfCountForNagios.svg?branch=master)](https://travis-ci.org/dhcgn/GetPerfCountForNagios)

# Intro

GetPerfCountForNagios is a small tool, which returns the value of one specified Performance Counter.
The format of the returned value is compatible with Nagios.

Read more:
- Nagios-Plugins: https://nagios-plugins.org/doc/guidelines.html
- Performance Counter: https://msdn.microsoft.com/en-us/library/windows/desktop/aa373083(v=vs.85).aspx

# Usage
To use this tool, parameters are needed. The Parameters and the Syntax are described in the following:

````
GetPerfCountForNagios.exe [-Name] Name [-Label] Label [-Unit] Unit 
			  [-Warning] Warning [-Critical] Critical 
			  [-Min] Min [-Max] Max

Parameter:
  -Name		Name of the Performance Counter| e.g. \Processor Information(_Total)\% Processor Time
  -Label	Label for the Result | e.g. Processor Time
  -Unit		The Unit of measuring | e.g. %
  -Warning	The warning Count of the result (Nagios warns if the result is higher as the result) | e.g. 85
  -Critical	The critical Count of the result (Nagios warns if the result is higher as the result) | e.g. 95
  -Min		The min Value of the result | e.g. 0
  -Max		The max Value of the result | e.g. 100
````

With one of the following ````[-h|\h|-?|\?|-help|\help]```` as parameter you will get a small help displayed.

You can also run this programm with powershell (like you will see in the Screenshots).

# Notes
The code includes a ````Thread.Sleep(500)````. This is necessary because some of the Performance Counter need a base value to calculate the reference value.

# Screenshot
Example Usage with Powershell:
![alt tag](https://github.com/dhcgn/GetPerfCountForNagios/blob/master/Example_use_PS.png)
