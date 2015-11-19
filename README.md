
Excelsis WebApi
==========

Prerequisites
-------------

### ASP.NET 5 beta 8
Excelsis WebApi uses ASP.NET 5 beta 8. If you haven't already, you need to [download](https://www.microsoft.com/en-us/download/details.aspx?id=49442) and install the upgrade. 

If you are running Visual Studio 2015, the files you need are probably DotNetVersionManager-x64.msi and WebToolsExtensionsVS14.msi. Read the [Install Instructions](https://www.microsoft.com/en-us/download/details.aspx?id=49442&fa43d42b-25b5-4a42-fe9b-1634f450f5ee=True) on the download page for more information.

After you have downloaded and installed ASP.NET 5 beta 8 and the WebToolsExtensionsvs14.msi you will need to run the sql script.
The sql script can be found in the Database folder inside the project "Lisa.Excelsis.WebApi\src\Lisa.Excelsis.WebApi\Database", the name of the file is "testDataScript.sql". 
Open testDataScript.sql and run the script, this will create a database called ExcelsisDb and the tables with test data.
