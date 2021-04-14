Wasteless is a project which contains tools to predict and reduce waste in foodservice. 

# Content

## AzureFunctions

This folder contains three [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview):
* DatabaseUpdaters - PredictionsUpdate (C#)
* DatabaseUpdaters - WeatherInfoUpdate (C#)
* PredictionsApi (Python)

PredictionsApi is a HTTP trigger. It returns a prediction of waste by given date.

DatabaseUpdaters are scheduled functions which update the underlying database used by the Web UI and PredictionsApi.

PredictionsUpdate update the prediction by calling the the PredictionsApi.  
WeatherInfoUpdate updates weather which is used by the PredictionsApi.

## Data

### Db

This is a Sql Server Database Project which contains everything used by the PredictionsApi and Web UI.

### DbBackup

This contains a Sql Server backup -file which you can use to initialize the database.

### ETL

This contains Sql Server Integration Services -project used to load the database with existing data.

### PowerBI

This contains the PowerBI -report which is embedded in the Web UI.

### TestData

This contains Excel-files which you can use in combination with SSIS -project.

## Web

This is the Web-project used to display predictions and to feed new waste amounts for the predictions. It's a .NET 5.0 C# backend with a React frontend.

# How it works

Users input waste amounts daily in the Web UI. Azure Functions are scheduled to run every night and this is how we get new and more precise predictions.
