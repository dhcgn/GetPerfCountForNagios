[![Build Status](https://travis-ci.org/dhcgn/GetPerfCountForNagios.svg?branch=master)](https://travis-ci.org/dhcgn/GetPerfCountForNagios)

# Intro
Get Performance Counter as Nagios Performance Data.

Read more:

- Nagios Performance Data: https://nagios-plugins.org/doc/guidelines.html#AEN200 
- Performance Counter: https://msdn.microsoft.com/en-us/library/windows/desktop/aa373083(v=vs.85).aspx

# Screenshot

***Todo***

# Usage

````
Syntax: GetPerfCountForNagios.exe [-Name] Name [-Label] Label [-Unit] Unit 
                                  [-Warning] Warning [-Critical] Critical 
                                  [-Min] Min [-Max] Max [-h|\h|-?|\?|-help|\help]

Options:
    -Name          Name of the Performance Counter| e.g. \Processor Information(_Total)\% Processor Time
    -Label         Label for the Result | e.g. Processor Time
    -Unit          The Unit of measuring | e.g. %
    -Warning       The warning Count of the result (Nagios warns if the result is higher as the result) | e.g. 85
    -Critical      The critical Count of the result (Nagios warns if the result is higher as the result) | e.g. 95
    -Min           The min Value of the result | e.g. 0
    -Max           The max Value of the result | e.g. 100
````

## Performance Counter

You can get the information about the Performance Counter by execute ``Get-Counter -ListSet *`` in Powershell.

Common Performance Counter are:
- ``\Processor Information(_Total)\% Processor Time``
- ``Web Service(_Total)\Current Connections``