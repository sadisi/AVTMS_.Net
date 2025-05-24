# AVTMS (Automated Vehicle Tracking and Monitoring System) - ASP.NET Core MVC

This project is the system of an Intelligent Vehicle Tracking and Violation Management System, which also includes Vehicle Management and User Management functionalities. Developed using ASP.NET Core MVC, Entity Framework Core, and MySQL, it serves as the web-based administrative and data-processing component of a comprehensive real-time vehicle monitoring platform.
The backend is designed to work seamlessly with a separate Python-based Machine Learning model that utilizes YOLOv10 and PaddleOCR to detect vehicles and extract license plate numbers from live CCTV or uploaded footage.
## 🚗 Features

- 🔐 Admin & Auth User & Base User & Employee Management (ASP.NET Core Identity)
- 🧾 Vehicle Owner Registration and Records and Management
- 🚙 Vehicle registration and Records and Management and Details & Notes Management (with Encrypted Data Fields)
- 🎥 Live CCTV and Uploaded Video Analysis Integration
- 🚘 Vehicle Detection via YOLOv10 
- 🔍 Number Plate Recognition using PaddleOCR
- 📨 Violation Alerts and Email Notification System
- 🔐 Encrypted Data Storage for Sensitive Fields

## 🛠️ Technologies Used

- ASP.NET Core MVC (.NET 7)
- MySQL
- Entity Framework Core
- ASP.NET Identity for Authentication
- YOLOv10 (for vehicle detection)
- PaddleOCR (for license plate recognition)
- Python Integration via Script Execution
- Bootstrap 5 for UI
- AJAX & Modals for Dynamic User Experience
- YOLOv10 + PaddleOCR (in Python - see ML repo)

## 🔧 Setup Instructions

### Prerequisites

- Visual Studio 2022 or later
- .NET SDK 7.0+
- MySQL Server
- Python 3.10+
- Clone the ML model repo: [AVTMS-ML-Model](https://github.com/sadisi/AVTMS-ML-Model)

### Steps

1. Clone the repo:
   ```bash
   git clone https://github.com/sadisi/AVTMS_.Net



## After Colning Repo 

1. Setup DB (Update connection string in appsettings.json):
   
        "ConnectionStrings": {
                 "DefaultConnection": "server=localhost;database=avtms_db;user=root;password=yourpassword;"
           }


3.  Run EF migrations (Type Below command in Visual Studio terminal )

                Add-Migration "DB setup"



4. Run the project

        Click Run button
    Or Below Command
   
        dotnet run

   

