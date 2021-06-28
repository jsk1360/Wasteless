# Wasteless

Wasteless is a project which contains tools to predict and reduce waste in foodservice.

## Getting Started

### Dependencies

  - .NET 5 and .NET Core 3.1
  - Python 3
  - Sql Server

### Installing

  - Setup the database either by using the [backup-file](Data/DBBackup/WastelessDemo.bak) or the
    [database-project](Data/DB/)

  - Set the connection string `DefaultConnection` in project Web
    
    Now you should be able to run the Web-app part of the application.
    
    To be able to get new predictions you need to setup some functionality through Azure Functions. Read more
    [here](#AzureFunctions).

### Executing program

  - Cd into `Web/Wasteless` folder
  - Restore `dotnet tools`

<!-- end list -->

``` sh
dotnet tool restore
```

  - Run the migrations

<!-- end list -->

``` sh
dotnet ef database update
```

  - Run the app

<!-- end list -->

``` sh
dotnet run
```

## Directory Contents

### AzureFunctions

This folder contains three [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview):

  - DatabaseUpdaters - PredictionsUpdate (C\#)
  - DatabaseUpdaters - WeatherInfoUpdate (C\#)
  - PredictionsApi (Python)

PredictionsApi is an HTTP trigger. It returns a prediction of waste by given date.

DatabaseUpdaters are scheduled functions which update the underlying database used by the Web UI and PredictionsApi.

PredictionsUpdate update the prediction by calling the the
PredictionsApi.  
WeatherInfoUpdate updates weather which is used by the PredictionsApi.

### Data

1.  Db
    
    This is a Sql Server Database Project which contains everything used by the PredictionsApi and Web UI.

2.  DbBackup
    
    This contains a Sql Server backup -file which you can use to initialize the database.

3.  ETL
    
    This contains Sql Server Integration Services -project used to load the database with existing data.

4.  PowerBI
    
    This contains the PowerBI -report which is embedded in the Web UI.

5.  TestData
    
    This contains Excel-files which you can use in combination with SSIS -project.

### Web

This is the Web-project used to display predictions and to feed new waste amounts for the predictions. It's a .NET 5.0
C# backend with a React frontend.

## License

This project is licensed under the MIT License - see the LICENSE file for details
