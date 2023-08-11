Read Me
=======

In scenarios where your application is running under a user account with restricted access to the system and system resources (such as running in an IIS application), DynamicPDF Converter for .NET will require a Windows service to perform conversions of some file types. The DynamicPDF.Converter.dll assembly will automatically look for the service and use it when it is available.

Installing the DynamicPDF Converter Service
===========================================

1. Uninstall any older version of the "DynamicPDF Converter" services prior to installing this service.
2. The "DPDFConverterService" folder contains the following files:
    * DynamicPDF.Converter.Service.exe
    * DynamicPDF.Converter.Service.exe.config
    * ReadMe.txt
3. Copy or move these files to a location on your system where they will reside permanently.
4. Copy or move the DynamicPDF.Converter.dll file present in the "bin" folder to the new file location. "DynamicPDF.Converter.Service.exe" and "DynamicPDF.Converter.dll" files needs to be present in one folder, or else the "DynamicPDF.Converter.Service.exe" installation will fail with the following message "Could not load file or assembly 'DynamicPDF.Converter'".
5. Open a command prompt as Admin (Run as administrator)
6. Navigate to the new file location and run the following command to install the DynamicPDF Converter Windows service:
      DynamicPDF.Converter.Service.exe /r
7. A Pop-Up appear asking for user account credential form which user the service will be installed and run
8. Enter the user account and credentials of a user with Admin privileges and that can open office documents on the system


We also recommend logging onto the system with the user account used for the service and opening a Word and Excel document to and making sure all one time dialogs are clicked through and accepted.

Uninstalling the DynamicPDF Converter Service
=============================================

1. Open a command prompt as Admin (Run as administrator)
2. Navigate to the folder where you have installed the service and run the following command to uninstall the DynamicPDF Converter windows service     
      DynamicPDF.Converter.Service.exe /u