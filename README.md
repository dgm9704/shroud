# shroud
[![.NET Core](https://github.com/dgm9704/shroud/workflows/.NET%20Core/badge.svg)](https://github.com/dgm9704/shroud/actions?query=workflow%3A%22.NET+Core%22)
## Diwen.BofCrypt

.NET Library with very simple functionality for creating a reporting package for sending to Finanssivalvonta / Financial Supervisory Authority (FIN-FSA)

### nuget
[![nuget](https://img.shields.io/nuget/v/Diwen.BofCrypt.svg)](https://www.nuget.org/packages/Diwen.BofCrypt/)

### usage 
- acquire public key in xml from FIN-FSA (or use the one included test project)
- generate XBRL report and XML header file according to instructions from FIN-FSA
- ` ReportPackage.Create(publickeyxmlfile, reportpackagezipfile, xbrlreportfile, headerxmlfile); `

### requirements
.NET Standard 2.0

### license
GNU Lesser General Public License v3.0

## Diwen.BofCrypt.Tests
Test project that contains simple example code

### requirements
.NET Core 3.1

### licence
Free Public License 1.0.0
