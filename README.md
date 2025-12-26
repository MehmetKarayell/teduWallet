# 🎓 TEDU Wallet

TEDU Wallet is a **reward-based digital wallet and task management system** designed for university students.  
Students earn **TDC (TEDU Digital Coins)** by completing campus tasks, track their wallet activity, and compete on a leaderboard.  
Administrators manage users, tasks, rewards, and approvals through a dedicated admin panel.

---

## 🚀 Features

### 👩‍🎓 Student Dashboard
- View total wallet balance (TDC)
- Browse and apply for campus tasks
- Track **transaction history** (earned & spent coins)
- **Leaderboard with bar chart visualization (Top 3 students)**
- **Wallet balance history displayed as a line chart**
- Modern, responsive dashboard UI

### 🛠 Admin Dashboard
- User management
- Task creation and management
- Reward creation and management
- Task approval workflow
- System overview and operational metrics

---

## 📊 Data Visualization

Instead of only tables, the project includes charts:

- 📈 **Line Chart – Wallet Balance History**
  - Continuous wallet balance changes
  - X-axis progresses by transaction order
  - Y-axis shows TDC balance

- 📊 **Bar Chart – Leaderboard**
  - Top 3 students based on weekly earned coins (descending)
  - 🏆 1st: Gold, 🥈 2nd: Silver, 🥉 3rd: Bronze

---

## 🧱 Tech Stack

- **Backend:** ASP.NET Core (.NET 8)
- **Frontend:** MVC (Razor Views)
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Auth:** Role-based (Admin / Student)
- **Charts:** JavaScript chart components (see libraries below)

---

## 📚 Libraries Used

### ✅ NuGet Packages (Backend)
The project uses common .NET libraries such as:
- **Microsoft.EntityFrameworkCore** (ORM)
- **Microsoft.EntityFrameworkCore.SqlServer** (SQL Server provider)
- **Microsoft.EntityFrameworkCore.Tools** (migrations / tooling)
- **Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation** *(optional, if enabled)*

> Exact versions can be verified in the .csproj file.

### ✅ Frontend Libraries
- **Bootstrap** (UI / layout)
- **Bootstrap Icons** (icons)
- **Chart library** (for bar + line charts, e.g., Chart.js)

> Exact references can be verified under: wwwroot/ (or layout .cshtml).

---

## ✅ Step-by-Step: How to Run the Application

### 1) Prerequisites
Make sure you have:
- **.NET SDK 8.0+**
- **SQL Server** (LocalDB / Docker / Remote SQL Server)
- (Optional) **SSMS** or **Azure Data Studio** to inspect DB

Check .NET:

dotnet --version

---

### 2) Clone the Repository

git clone https://github.com/<your-username>/teduWallet.git
cd teduWallet

---

### 3) Configure Database Connection

Open appsettings.json and set your SQL Server connection string.

Example:

"ConnectionStrings": {
  "CampusCoinDb": "Server=YOUR_SERVER;Database=CAMPUSCOIN;User Id=YOUR_USER;Password=YOUR_PASS;TrustServerCertificate=True;"
}


> Use the same key name that your Program.cs reads (example: CampusCoinDb or DefaultConnection).

---

### 4) Create / Initialize Database

Choose **ONE** method depending on your project setup:

#### ✅ Option A — Using SQL Script (schema.sql)

1. Open SQL Server (SSMS / Azure Data Studio)
2. Create the database (if needed)
3. Run schema.sql

This will create tables/views needed by the project.

#### ✅ Option B — Using EF Core Migrations (if migrations exist)

dotnet tool install --global dotnet-ef
dotnet ef database update

---

### 5) Restore & Run

dotnet restore
dotnet build
dotnet run

---

### 6) Open in Browser

After running, the terminal prints something like:

* https://localhost:xxxx
* http://localhost:xxxx

Open that URL in your browser.

---

## 🗂 Project Structure

teduWallet/
├── Controllers/
├── Models/
├── Services/
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
├── schema.sql
├── appsettings.json
├── Program.cs
└── teduWallet.csproj

---

## 🧑‍💻 User Roles

### Student

* View tasks, apply for tasks
* Earn/spend TDC
* See wallet and transaction history
* View leaderboard charts

### Admin

* Manage users, tasks, rewards
* Approve submissions
* Monitor system overview

---

## 👥 Contributors

* Mehmet Karayel
* Elif Seden Yurtseven
* Berre Yazgı
* Namık Batın Gambaz

---

## 📄 License
This project is intended for educational purposes.

```

