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

The project prioritizes **graph-based analytics instead of raw tables**:

- 📈 **Line Chart – Wallet Balance History**
  - Displays continuous wallet balance changes
  - X-axis progresses by transaction order
  - Y-axis represents TDC balance

- 📊 **Bar Chart – Leaderboard**
  - Shows top 3 students based on earned TDC
  - 🏆 1st Place: Gold  
  - 🥈 2nd Place: Silver  
  - 🥉 3rd Place: Bronze  

---

## 🧱 Tech Stack

- **Backend:** ASP.NET Core (.NET 8)
- **Frontend:** Razor Pages / MVC
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** Role-based (Admin / Student)
- **UI Design:** Custom modern dashboard design
- **Charts:** Lightweight JavaScript chart components

---

## 🗂 Project Structure

teduWallet/
│
├── Controllers/
├── Models/
├── Services/
├── Views/
├── wwwroot/
│ ├── css/
│ ├── js/
│
├── App_Data/
├── schema.sql
├── appsettings.json
├── Program.cs
└── teduWallet.csproj

---

## 🧑‍💻 User Roles

### Student
- View available tasks
- Apply for tasks
- Earn and spend TDC
- View wallet balance and transaction history
- Compete on the leaderboard

### Admin
- Manage users
- Create and edit tasks
- Create and manage rewards
- Approve or reject task submissions
- Monitor system activity

---

## ⚙️ Setup Instructions

1. Clone the repository
   ```bash
   git clone https://github.com/your-username/teduWallet.git
Configure the database connection in appsettings.json
Apply the database schema
schema.sql
Run the project
dotnet run
Open in browser:
https://localhost:xxxx
🎯 Purpose
TEDU Wallet was developed as an academic project to demonstrate:
Role-based system design
Transaction-based wallet logic
Data visualization with charts
Clean dashboard UI/UX principles
Real-world campus reward scenarios
👥 Contributors
Mehmet Karayel
Elif Seden Yurtseven
Berre Yazgı
Namık Batın Gambaz
📄 License
This project is intended for educational purposes.

---

If you want, I can next:
- Add **screenshots section**
- Optimize README for **grading rubric**
- Create a **short demo description**
- Write a **GitHub project description (About section)**

Just tell me 👍
