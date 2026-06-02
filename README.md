# Finance Tracker

A personal finance management application built with ASP.NET Core 8 that helps users track income, expenses, budgets, and financial goals.

## Features

### User Management

* User registration and authentication
* ASP.NET Core Identity integration
* Secure password management
* User profile management

### Transaction Management

* Record income and expenses
* Categorize transactions
* Transaction history tracking
* Wallet-based transaction management

### Budget Management

* Create budgets by category
* Weekly, monthly, and yearly budget periods
* Budget spending tracking
* Budget threshold alerts
* Budget performance monitoring

### Wallet Management

* Multiple wallet support
* Bank account tracking
* Cash wallet management
* Credit card tracking
* Investment account tracking
* Real-time balance updates

### Financial Analytics

* Spending tracking by category
* Budget utilization monitoring
* Income versus expense analysis
* Savings goal tracking

---

## Architecture

The solution follows a clean architecture pattern:

```text
FinanceTracker
│
├── FinanceTracker.Presentation
│   ├── MVC Controllers
│   ├── Razor Views
│   └── User Interface
│
├── FinanceTracker.Application
│   ├── DTOs
│   ├── Service Interfaces
│   └── Business Contracts
│
├── FinanceTracker.Domain
│   ├── Entities
│   ├── Enums
│   └── Core Business Models
│
└── FinanceTracker.Infrastructure
    ├── Entity Framework Core
    ├── Repositories
    ├── Services
    └── Database Context
```

---

## Technologies Used

### Backend

* ASP.NET Core 8
* C#
* Entity Framework Core
* ASP.NET Identity

### Database

* Microsoft SQL Server

### Frontend

* Razor Views
* MVC Pattern
* Bootstrap

### Development Tools

* Visual Studio 2022
* .NET 8 SDK

---

## Domain Models

### User

Represents an application user and includes:

* Full Name
* Email Address
* Preferred Currency
* Savings Target
* Account Status

### Wallet

Represents a financial account:

* Bank Account
* Cash Wallet
* Credit Card
* Investment Account

### Transaction

Stores financial activities:

* Income
* Expense
* Category
* Description
* Transaction Date

### Budget

Tracks spending limits:

* Category-based budgets
* Weekly budgets
* Monthly budgets
* Yearly budgets

### Recurring Transaction

Supports recurring financial activities:

* Daily
* Weekly
* Bi-Weekly
* Monthly
* Quarterly
* Yearly

---

## Project Structure

```text
FinanceTracker/
│
├── FinanceTracker.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Wallet.cs
│   │   ├── Transaction.cs
│   │   ├── Budget.cs
│   │   └── RecurringTransaction.cs
│
├── FinanceTracker.Application/
│   ├── DTOs/
│   └── Interfaces/
│
├── FinanceTracker.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   └── Services/
│
└── FinanceTracker.Presentation/
    ├── Controllers/
    ├── Views/
    └── wwwroot/
```

---

## Getting Started

### Prerequisites

Install the following:

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 or later

### Clone the Repository

```bash
git clone https://github.com/yourusername/FinanceTracker.git
cd FinanceTracker
```

### Configure Database

Update the connection string in:

```json
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FinanceTrackerDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Apply Migrations

```bash
dotnet ef database update
```

### Run the Application

```bash
dotnet run --project FinanceTracker.Presentation
```

Or launch the solution using Visual Studio.

---

## Default Categories

### Expense Categories

* Food & Dining
* Transportation
* Utilities
* Entertainment
* Shopping
* Healthcare
* Education
* Other

### Income Categories

* Salary
* Freelance
* Investment
* Bonus
* Other

---

## Security

The application uses:

* ASP.NET Core Identity
* Secure password hashing
* Authentication cookies
* Authorization policies
* Entity Framework Core data protection

---

## Future Enhancements

* Dashboard analytics
* Charts and reports
* CSV/Excel export
* Recurring transaction automation
* Email notifications
* Mobile-responsive enhancements
* Savings goal recommendations
* Multi-currency support
* REST API endpoints

---

## License

This project is licensed under the MIT License.

---

## Author

 

**Daberechukwu Ibeakanma**

Software Engineer | Backend Developer | ASP.NET Core Developer

  

Developed as a personal finance management system using ASP.NET Core and Entity Framework Core.
