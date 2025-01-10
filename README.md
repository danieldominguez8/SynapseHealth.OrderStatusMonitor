# SynapseHealth Order Status Monitor
## Overview

The SynapseHealth Order Status Monitor is a console application designed to fetch medical equipment orders, process the orders, alert the customer, and update medical equipment orders. It utilizes various services to handle HTTP requests, logging, and alerting.

## Features

- Fetch medical equipment orders from a remote server.
- Process orders and send alerts for delivered items.
- Update order status on the server.
- Log information and errors using Serilog.

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022

## Getting Started

### Clone the Repository
Via Terminal
- git clone https://github.com/your-repo/synapsehealth-order-status-monitor.git cd synapsehealth-order-status-monitor

Via visual studio 2022 
- clone the repository using the link https://github.com/your-repo/synapsehealth-order-status-monitor.git

### Build the Solution

- Open the solution in Visual Studio 2022 and build the project.

### (Optional) Setup JSON Server
#### Navigate to ConsoleApp directory via Terminal
- cd .\source\repos\SynapseHealth.OrderStatusMonitor\SynapseHealth.OrderStatusMonitor.ConsoleApp
#### Start JSON Server
- npx json-server --watch db.json --port 3000
#### Uncomment/Comment Lines for URL
- [AlertService.cs](#alertservice.cs-context) Comment out line 37 and uncomment line 38
- [OrderService.cs](#orderservice.cs-context) Comment out line 32 and uncomment line 33
- [UpdateService.cs](#updateservice.cs-context) Comment out line 32 and uncomment line 33
- If application is run with JSON Server, updates to Orders and added Alerts will be viewed in db.json

### Run the Application

- Run the application using Visual Studio or the .NET CLI:
- dotnet run --project SynapseHealth.OrderStatusMonitor.ConsoleApp

## Project Structure

### Program.cs

- The entry point of the application. It configures services, sets up logging, and initiates the order processing workflow.

[Program.cs](#program.cs-context)

### Services

#### AlertService.cs

- Handles sending alerts for delivered items.

[AlertService.cs](#alertservice.cs-context)

#### HttpClientService.cs

- Provides methods for making HTTP GET, POST, and PUT requests.

[HttpClientService.cs](#httpclientservice.cs-context)

#### OrderService.cs

- Fetches and processes medical equipment orders.

[OrderService.cs](#orderservice.cs-context)

#### UpdateService.cs

- Updates the status of medical equipment orders on the server.

[UpdateService.cs](#updateservice.cs-context)

## Configuration

### Logging

- Logging is configured using Serilog. Logs are written to the console and a file located at `./SynapseHealth.OrderStatusMonitor.ConsoleApp/bin/Debug/net8.0/logs`.

### HTTP Endpoints

#### Mock Ednpoints
- Orders: `https://dandav8.mockmaster.io/synapsehealthmockapi/orders`
- Alerts: `https://dandav8.mockmaster.io/synapsehealthmockapi/alerts`
#### JSON-Server Endpoints
- Orders: `http://localhost:3000/orders`
- Alerts: `http://localhost:3000/alerts`

## Testing

### Test Projects

The solution includes test projects to ensure the functionality of the application. These tests cover various aspects of the services and their interactions.

#### Running Tests

You can run the tests using Visual Studio Test Explorer or the .NET CLI:
dotnet test

#### Test Files

- [AlertServiceTests.cs](#alertservicetests.cs-context): Tests the `AlertService` class.
- [HttpClientServiceTests.cs](#httpclientservicetests.cs-context): Tests the `HttpClientService` class.
- [OrderServiceTests.cs](#orderservicetests.cs-context): Tests the `OrderService` class.
- [UpdateServiceTests.cs](#updateservicetests.cs-context): Tests the `UpdateService` class.

By following these steps, you should be able to set up, run, and test the SynapseHealth Order Status Monitor application.
