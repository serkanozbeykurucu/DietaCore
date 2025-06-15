# 🥗 DietaCore API

<div align="center">

![DietaCore Logo](https://img.shields.io/badge/DietaCore-Nutrition%20Management-green?style=for-the-badge&logo=leaf)

**A comprehensive nutrition and diet management system built with .NET Core**

[![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white)](#)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=flat-square&logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](LICENSE)
[![Architecture](https://img.shields.io/badge/Architecture-Layered-orange?style=flat-square)]()
[![Postgres](https://img.shields.io/badge/Postgres-%23316192.svg?logo=postgresql&logoColor=white)](#)

</div>

---

## 📋 Table of Contents

- [🎯 About](#-about)
- [✨ Features](#-features)
- [🛠️ Technologies Used](#️-technologies-used)
- [🏗️ Architecture](#️-architecture)
- [🗄️ Database Schema](#️-database-schema)
- [🚀 Getting Started](#-getting-started)
- [📡 API Endpoints](#-api-endpoints)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

---

## 🎯 About

**DietaCore** is a modern, scalable nutrition management API designed to facilitate seamless interactions between dietitians, clients, and administrators. Built with clean architecture principles and industry best practices, it provides comprehensive diet planning, meal management, and progress tracking capabilities.

The system enables dietitians to manage their clients effectively, create personalized diet plans, track progress, and maintain detailed meal records while providing clients with easy access to their nutrition programs.

---

## ✨ Features

### 🔐 **Authentication & Authorization**

- **User Registration & Login** with email confirmation
- **Role-based Access Control** (Admin, Dietitian, Client)
- **Password Reset** functionality
- **JWT Token-based** authentication

### 👥 **User Management**

- **Multi-role System**: Admins, Dietitians, and Clients
- **Profile Management** for all user types
- **Client-Dietitian Assignment** system

### 🍽️ **Diet & Meal Management**

- **Personalized Diet Plans** creation and management
- **Detailed Meal Planning** with nutritional information
- **Flexible Meal Scheduling** and organization

### 📊 **Progress Tracking**

- **Client Progress Monitoring** with detailed metrics
- **Progress Summary Reports** for quick insights
- **Historical Data Tracking** for long-term analysis

### 🏥 **Administrative Control**

- **Complete User Management** (Create, Read, Update, Delete)
- **System-wide Oversight** of all operations
- **Client-Dietitian Relationship** management

---

## 🛠️ Technologies Used

<table>
<tr>
<td>

**Backend Framework**

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core

</td>
<td>

**Architecture & Patterns**

- Layered Architecture
- SOLID Principles
- DRY & KISS Principles
- Repository Pattern

</td>
</tr>
<tr>
<td>

**Validation & Security**

- FluentValidation
- JWT Authentication
- Role-based Authorization
- Data Annotations

</td>
<td>

**DevOps & Deployment**

- Docker & Docker Compose
- RESTful API Design
- Swagger/OpenAPI Documentation

</td>
</tr>
</table>

---

## 🏗️ Architecture

DietaCore follows a **clean layered architecture** approach:

```
📁 DietaCore Solution
├── 🌐 DietaCore.Api          # Presentation Layer (Controllers, Middleware)
├── 💼 DietaCore.Business     # Business Logic Layer (Services, Validators)
├── 🗄️ DietaCore.DataAccess   # Data Access Layer (Repositories, DbContext)
├── 📦 DietaCore.Dto          # Data Transfer Objects
├── 🏗️ DietaCore.Entities     # Domain Entities
└── 🔧 DietaCore.Shared       # Shared Components (Constants, Responses)
```

### Key Architectural Benefits:

- **Separation of Concerns**: Each layer has a distinct responsibility
- **Maintainability**: Easy to modify and extend
- **Testability**: Loosely coupled components for better testing
- **Scalability**: Modular design supports system growth

---

## 🗄️ Database Schema

The DietaCore system uses a comprehensive relational database schema designed to handle user management, authentication, diet planning, meal management, and progress tracking efficiently.

### 📊 Entity Relationship Diagram

![Database Schema](DietaCore-DbDiagram.png)

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- PSQL Server or compatible database

### Installation

#### 🐳 Using Docker (Recommended)

```bash
# Clone the repository
git clone https://github.com/serkanozbeykurucu/DietaCore.git
cd dietacore

# Run with Docker Compose
docker-compose up -d
```

#### 🛠️ Manual Setup

```bash
# Clone the repository
git clone https://github.com/serkanozbeykurucu/DietaCore.git
cd dietacore

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the application
dotnet run --project DietaCore.Api
```

### Configuration

Update `appsettings.json` with your database connection string and JWT settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  },
  "JwtSettings": {
    "SecretKey": "Your_Secret_Key_Here",
    "Issuer": "DietaCoreAPI",
    "Audience": "DietaCoreUsers"
  }
}
```

---

## 📡 API Endpoints

### 🔐 Authentication Endpoints

| Method | Endpoint                   | Description                | Access |
| ------ | -------------------------- | -------------------------- | ------ |
| `POST` | `/api/Auth/Register`       | Register new user account  | Public |
| `POST` | `/api/Auth/Login`          | User authentication        | Public |
| `POST` | `/api/Auth/ConfirmEmail`   | Confirm user email address | Public |
| `POST` | `/api/Auth/ForgotPassword` | Request password reset     | Public |
| `POST` | `/api/Auth/ResetPassword`  | Reset user password        | Public |

### 👑 Admin Management Endpoints

| Method   | Endpoint                               | Description                    | Access |
| -------- | -------------------------------------- | ------------------------------ | ------ |
| `GET`    | `/api/Admin/GetAllDietitians`          | Retrieve all dietitians        | Admin  |
| `GET`    | `/api/Admin/GetDietitianById`          | Get specific dietitian details | Admin  |
| `POST`   | `/api/Admin/CreateDietitian`           | Create new dietitian account   | Admin  |
| `PUT`    | `/api/Admin/UpdateDietitian`           | Update dietitian information   | Admin  |
| `DELETE` | `/api/Admin/DeleteDietitian`           | Remove dietitian from system   | Admin  |
| `GET`    | `/api/Admin/GetAllClients`             | Retrieve all client records    | Admin  |
| `GET`    | `/api/Admin/GetClientById`             | Get specific client details    | Admin  |
| `POST`   | `/api/Admin/CreateClient`              | Create new client account      | Admin  |
| `PUT`    | `/api/Admin/UpdateClient`              | Update client information      | Admin  |
| `DELETE` | `/api/Admin/DeleteClient`              | Remove client from system      | Admin  |
| `POST`   | `/api/Admin/AssignClientToDietitian`   | Assign client to dietitian     | Admin  |
| `POST`   | `/api/Admin/RemoveClientFromDietitian` | Remove client assignment       | Admin  |

### 🥼 Dietitian Management Endpoints

| Method   | Endpoint                            | Description                             | Access           |
| -------- | ----------------------------------- | --------------------------------------- | ---------------- |
| `GET`    | `/api/Dietitian/GetProfile`         | Get dietitian profile information       | Dietitian, Admin |
| `GET`    | `/api/Dietitian/GetClients`         | Get assigned clients list               | Dietitian, Admin |
| `GET`    | `/api/Dietitian/GetClientById`      | Get specific client details             | Dietitian, Admin |
| `POST`   | `/api/Dietitian/CreateClient`       | Create new client under dietitian       | Dietitian, Admin |
| `PUT`    | `/api/Dietitian/UpdateClient`       | Update client information               | Dietitian, Admin |
| `GET`    | `/api/Dietitian/GetDietPlans`       | Get all diet plans created by dietitian | Dietitian, Admin |
| `GET`    | `/api/Dietitian/GetDietPlanById`    | Get specific diet plan details          | Dietitian, Admin |
| `GET`    | `/api/Dietitian/GetClientDietPlans` | Get diet plans for specific client      | Dietitian, Admin |
| `POST`   | `/api/Dietitian/CreateDietPlan`     | Create new diet plan                    | Dietitian, Admin |
| `PUT`    | `/api/Dietitian/UpdateDietPlan`     | Update existing diet plan               | Dietitian, Admin |
| `DELETE` | `/api/Dietitian/DeleteDietPlan`     | Delete diet plan                        | Dietitian, Admin |

### 👤 Client Access Endpoints

| Method | Endpoint                           | Description                      | Access        |
| ------ | ---------------------------------- | -------------------------------- | ------------- |
| `GET`  | `/api/Client/GetProfile`           | Get client profile information   | Client, Admin |
| `GET`  | `/api/Client/GetDietPlans`         | Get assigned diet plans          | Client, Admin |
| `GET`  | `/api/Client/GetDietPlanById`      | Get specific diet plan details   | Client, Admin |
| `GET`  | `/api/Client/GetMealsByDietPlanId` | Get meals for specific diet plan | Client, Admin |

### 🍽️ Meal Management Endpoints

| Method   | Endpoint                           | Description                    | Access           |
| -------- | ---------------------------------- | ------------------------------ | ---------------- |
| `GET`    | `/api/Meal/GetByIdForDietitian`    | Get meal details for dietitian | Dietitian, Admin |
| `GET`    | `/api/Meal/GetByDietPlanId`        | Get meals by diet plan         | Dietitian, Admin |
| `POST`   | `/api/Meal/CreateByDietitianAsync` | Create new meal entry          | Dietitian, Admin |
| `PUT`    | `/api/Meal/UpdateByDietitianAsync` | Update existing meal           | Dietitian, Admin |
| `DELETE` | `/api/Meal/DeleteByDietitian`      | Delete meal entry              | Dietitian, Admin |

### 📊 Progress Tracking Endpoints

| Method   | Endpoint                       | Description                 | Access           |
| -------- | ------------------------------ | --------------------------- | ---------------- |
| `GET`    | `/api/Progress/GetProgress`    | Get client progress history | Dietitian, Admin |
| `GET`    | `/api/Progress/GetSummary`     | Get progress summary report | Dietitian, Admin |
| `POST`   | `/api/Progress/CreateProgress` | Record new progress entry   | Dietitian, Admin |
| `PUT`    | `/api/Progress/UpdateProgress` | Update progress record      | Dietitian, Admin |
| `DELETE` | `/api/Progress/DeleteProgress` | Delete progress entry       | Dietitian, Admin |

---

## 💻 API Usage Examples

### 🔐 Authentication Examples

#### Register New User

```http
POST /api/Auth/Register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "phoneNumber": "+1234567890"
}
```

**Response:**

```json
{
  "data": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "role": "Client",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-12-15T10:30:00Z"
  },
  "responseCode": 200,
  "message": "Registration successful"
}
```

#### User Login

```http
POST /api/Auth/Login
{
  "email": "fatmaguru@dietacore.com",
  "password": "123456888."
}
```

**Response:**

```json
{
  "data": {
    "id": 2,
    "firstName": "Fatma",
    "lastName": "Guru",
    "email": "fatmaguru@dietacore.com",
    "role": "Dietitian",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-12-15T10:30:00Z"
  },
  "responseCode": 200,
  "message": "Login successful"
}
```

### 👑 Admin Operations Examples

#### Create New Dietitian

```http
POST /api/Admin/CreateDietitian
Authorization: Bearer {admin_token}
{
  "firstName": "Dr. Sarah",
  "lastName": "Wilson",
  "email": "sarah.wilson@dietacore.com",
  "password": "DietitianPass123!",
  "phoneNumber": "+1987654321",
  "specialization": "Sports Nutrition",
  "licenseNumber": "DN123456",
  "education": "PhD in Nutrition Science",
  "biography": "Experienced sports nutritionist with 10+ years of practice"
}
```

#### Create New Client

```http
POST /api/Admin/CreateClient
Authorization: Bearer {admin_token}
{
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@example.com",
  "password": "ClientPass123!",
  "phoneNumber": "+1555666777",
  "dietitianId": 2,
  "dateOfBirth": "1990-05-15T00:00:00Z",
  "gender": "Female",
  "height": 165.5,
  "initialWeight": 70.0,
  "medicalConditions": "Diabetes Type 2",
  "allergies": "Gluten, Nuts"
}
```

#### Assign Client to Dietitian

```http
POST /api/Admin/AssignClientToDietitian?clientId=1&dietitianId=2
Authorization: Bearer {admin_token}
```

### 🥼 Dietitian Operations Examples

#### Create Diet Plan

```http
POST /api/Dietitian/CreateDietPlan
Authorization: Bearer {dietitian_token}
{
  "title": "Weight Loss Plan - Alice Johnson",
  "description": "12-week structured weight loss program with balanced nutrition",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-03-31T00:00:00Z",
  "initialWeight": 70.0,
  "targetWeight": 65.0,
  "clientId": 1
}
```

#### Update Client Information

```http
PUT /api/Dietitian/UpdateClient
Authorization: Bearer {dietitian_token}
{
  "id": 1,
  "firstName": "Alice",
  "lastName": "Johnson",
  "email": "alice.johnson@example.com",
  "phoneNumber": "+1555666777",
  "dietitianId": 2,
  "dateOfBirth": "1990-05-15T00:00:00Z",
  "gender": "Female",
  "height": 165.5,
  "currentWeight": 68.5,
  "medicalConditions": "Diabetes Type 2 - Controlled",
  "allergies": "Gluten, Tree Nuts"
}
```

### 🍽️ Meal Management Examples

#### Create New Meal

```http
POST /api/Meal/CreateByDietitianAsync
Authorization: Bearer {dietitian_token}
{
  "title": "Healthy Breakfast",
  "startTime": "07:00:00",
  "endTime": "08:00:00",
  "description": "Protein-rich breakfast to start the day",
  "contents": "2 eggs, 1 slice whole grain toast, 1/2 avocado, herbal tea",
  "calories": 350,
  "proteins": 18.5,
  "carbohydrates": 25.0,
  "fats": 22.0,
  "dietPlanId": 1
}
```

#### Update Existing Meal

```http
PUT /api/Meal/UpdateByDietitianAsync
Authorization: Bearer {dietitian_token}
{
  "id": 1,
  "title": "Enhanced Breakfast",
  "startTime": "07:00:00",
  "endTime": "08:30:00",
  "description": "Protein-rich breakfast with added fiber",
  "contents": "2 eggs, 1 slice whole grain toast, 1/2 avocado, 1 tbsp chia seeds, herbal tea",
  "calories": 380,
  "proteins": 20.0,
  "carbohydrates": 28.0,
  "fats": 24.0
}
```

### 📊 Progress Tracking Examples

#### Record Client Progress

```http
POST /api/Progress/CreateProgress
Authorization: Bearer {dietitian_token}
{
  "clientId": 1,
  "weight": 68.5,
  "bodyFat": 22.5,
  "muscle": 45.2,
  "waist": 78.0,
  "notes": "Great progress! Client is following the diet plan consistently.",
  "date": "2024-01-15T00:00:00Z"
}
```

**Response:**

```json
{
  "data": {
    "id": 1,
    "clientId": 1,
    "weight": 68.5,
    "bodyFat": 22.5,
    "muscle": 45.2,
    "waist": 78.0,
    "notes": "Great progress! Client is following the diet plan consistently.",
    "date": "2024-01-15T00:00:00Z"
  },
  "responseCode": 200,
  "message": "Progress recorded successfully"
}
```

#### Get Progress Summary

```http
GET /api/Progress/GetSummary?clientId=1
Authorization: Bearer {dietitian_token}
```

**Response:**

```json
{
  "data": {
    "clientId": 1,
    "clientName": "Alice Johnson",
    "startWeight": 70.0,
    "currentWeight": 68.5,
    "targetWeight": 65.0,
    "weightLoss": 1.5,
    "recent": [
      {
        "id": 1,
        "clientId": 1,
        "weight": 68.5,
        "bodyFat": 22.5,
        "muscle": 45.2,
        "waist": 78.0,
        "notes": "Great progress!",
        "date": "2024-01-15T00:00:00Z"
      }
    ]
  },
  "responseCode": 200,
  "message": "Progress summary retrieved successfully"
}
```

### 👤 Client Access Examples

#### Get Client Profile

```http
GET /api/Client/GetProfile
Authorization: Bearer {client_token}
```

**Response:**

```json
{
  "data": {
    "id": 1,
    "firstName": "Alice",
    "lastName": "Johnson",
    "email": "alice.johnson@example.com",
    "phoneNumber": "+1555666777",
    "dietitianId": 2,
    "dietitianName": "Dr. Sarah Wilson",
    "dateOfBirth": "1990-05-15T00:00:00Z",
    "age": 33,
    "gender": "Female",
    "height": 165.5,
    "initialWeight": 70.0,
    "currentWeight": 68.5,
    "medicalConditions": "Diabetes Type 2 - Controlled",
    "allergies": "Gluten, Tree Nuts",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-15T00:00:00Z"
  },
  "responseCode": 200,
  "message": "Profile retrieved successfully"
}
```

#### Get Client's Diet Plans

```http
GET /api/Client/GetDietPlans
Authorization: Bearer {client_token}
```

### 🔧 Common Response Format

All API responses follow this consistent format:

```json
{
  "data": [
    {
      "id": 1,
      "title": "Kilo Verme Planı Güncellemesi",
      "description": "Ayşe Yılmaz için 4 haftalık düşük karbonhidrat diyet planı, güncellenmiş hedef kilolar.",
      "startDate": "2025-06-17T00:00:00Z",
      "endDate": "2025-07-15T00:00:00Z",
      "initialWeight": 68.5,
      "targetWeight": 62.0,
      "clientId": 1,
      "clientName": "Ayşe Yılmaz",
      "createdByDietitianId": 1,
      "dietitianName": "Dt. Fatma Guru",
      "meals": [
        {
          "id": 1,
          "title": "Kahvaltı",
          "startTime": "08:00:00",
          "endTime": "08:30:00",
          "description": "Düşük karbonhidratlı sağlıklı kahvaltı, yumurta ve peynir içerir.",
          "contents": "2 haşlanmış yumurta, 1 dilim tam buğday ekmeği, 1 avuç ceviz, 1 dilim beyaz peynir, 1 fincan yeşil çay.",
          "calories": 350,
          "proteins": 20,
          "carbohydrates": 30,
          "fats": 18,
          "dietPlanId": 1,
          "createdAt": "2025-06-14T12:24:32.972486Z",
          "updatedAt": "2025-06-14T12:28:23.368123Z"
        }
      ]
    }
  ],
  "responseCode": 200,
  "message": "Operation completed successfully"
}
```

**Error Response Example:**

```json
{
  "data": null,
  "responseCode": 500,
  "message": "Somethink went wrong!"
}
```

### 🔑 Authentication Headers

For protected endpoints, include the JWT token in the Authorization header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

---

## 🤝 Contributing

We welcome contributions to DietaCore! Here's how you can help:

### 🚀 Quick Start for Contributors

1. **Fork the Repository**

   ```bash
   git clone https://github.com/serkanozbeykurucu/DietaCore.git
   cd dietacore
   ```

2. **Create a Feature Branch**

   ```bash
   git checkout -b feature/amazing-feature
   ```

3. **Make Your Changes**

   - Follow the existing code style and architecture
   - Add appropriate tests for new functionality
   - Update documentation as needed

4. **Commit Your Changes**

   ```bash
   git commit -m 'Add amazing feature'
   ```

5. **Push to Your Branch**

   ```bash
   git push origin feature/amazing-feature
   ```

6. **Open a Pull Request**
   - Provide a clear description of your changes
   - Reference any related issues
   - Ensure all tests pass

### 📋 Contribution Guidelines

- **Code Style**: Follow C# coding conventions and existing project patterns
- **Architecture**: Maintain the layered architecture principles
- **Testing**: Include unit tests for new functionality
- **Documentation**: Update README and code comments as needed
- **Validation**: Use FluentValidation for input validation
- **Security**: Ensure proper authorization and authentication

### 🐛 Reporting Issues

- Use GitHub Issues to report bugs or suggest features
- Provide detailed descriptions and steps to reproduce
- Include environment information when reporting bugs

---

## 📄 License

🇬🇧 (English) This project is licensed under the MIT License - see the LICENSE file for details.

---

<div align="center">

**Made with ❤️ for better nutrition management**

⭐ Star this repo if you find it helpful!

[🐛 Report Bug](../../issues) • [✨ Request Feature](../../issues) • [🤝 Contribute](../../pulls)

</div>
