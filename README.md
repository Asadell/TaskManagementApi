# ✅ Task Management API (ASP.NET Core)

**Task Management API** adalah backend REST API berbasis **ASP.NET Core (.NET 9)** yang mensimulasikan sistem **manajemen proyek & tugas (mirip Trello / Jira versi sederhana)**.

Project ini dirancang sebagai **intermediate–advanced backend project** dengan fokus pada:

* **Clean Architecture (Core vs Infrastructure)**
* **Repository Pattern (Generic)**
* **Unit of Work Pattern**
* **JWT Authentication & Authorization**
* **Complex Permission & Business Rules**

---

## 🚀 Fitur Utama

* 🔐 JWT Authentication (Register, Login)
* 👥 Manajemen User & Project Member
* 📁 Project Management (Owner, Manager, Member)
* 📌 Task Management (CRUD Task)
* 🔄 Task Status Flow (`Todo → InProgress → Done`)
* 🧠 Service Layer (Business Logic terpisah)
* 🗄️ Repository + Unit of Work Pattern
* 📄 DTO separation (Entity ≠ API Contract)

---

## 🧩 Teknologi yang Digunakan

* **ASP.NET Core 9**
* **Entity Framework Core 9**
* **PostgreSQL**
* **JWT (Json Web Token)**
* **AutoMapper**
* **BCrypt (Password Hashing)**
* **Swagger / OpenAPI**

---

## 📁 Struktur Folder

```
TaskManagementApi/
├── Controllers/
│   ├── AuthController.cs        // Register & Login
│   ├── ProjectsController.cs    // Project & Member management
│   └── TasksController.cs       // Task management
│
├── Core/
│   ├── Models/                  // Entity (Database)
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── ProjectMember.cs
│   │   └── ProjectTask.cs
│   │
│   ├── DTOs/                    // API Contract
│   │   ├── LoginDto.cs
│   │   ├── RegisterDto.cs
│   │   ├── ProjectDto.cs
│   │   └── TaskDto.cs
│   │
│   └── Interfaces/              // Abstraction Layer
│       ├── IRepository.cs
│       ├── IUserRepository.cs
│       ├── IProjectRepository.cs
│       ├── ITaskRepository.cs
│       ├── IProjectMemberRepository.cs
│       └── IUnitOfWork.cs
│
├── Infrastructure/
│   ├── Data/
│   │   └── AppDbContext.cs      // EF Core DbContext
│   │
│   └── Repositories/
│       ├── Repository.cs        // Generic Repository
│       ├── UserRepository.cs
│       ├── ProjectRepository.cs
│       ├── TaskRepository.cs
│       ├── ProjectMemberRepository.cs
│       └── UnitOfWork.cs
│
├── Services/                    // Business Logic Layer
│   ├── AuthService.cs
│   ├── ProjectService.cs
│   └── TaskService.cs
│
├── Helpers/
│   └── JwtHelper.cs             // JWT Generate & Validate
│
├── Program.cs                   // App configuration
└── appsettings.json
```

---

## 🔐 Authentication Flow

### 1️⃣ Register

```
POST /api/auth/register
```

* Membuat user baru
* Password di-hash menggunakan BCrypt
* Role default: `User`
* Response berisi **JWT Token**

---

### 2️⃣ Login

```
POST /api/auth/login
```

* Verifikasi email & password
* Generate JWT token
* Claim berisi:

  * `UserId`
  * `Email`

---

### 3️⃣ Access Protected Endpoint

Token dikirim melalui header:

```
Authorization: Bearer <JWT_TOKEN>
```

JWT akan di-validate oleh middleware ASP.NET Core.

---

## 📁 Project Management Flow

### 🆕 Create Project

```
POST /api/projects
```

* User otomatis menjadi:

  * **Owner**
  * **ProjectMember**

---

### 👥 Add Project Member

```
POST /api/projects/{projectId}/members
```

Rules:

* Hanya **Owner / Manager** yang boleh menambah member
* Tidak boleh duplicate member

Role:

* `Owner`
* `Manager`
* `Member`

---

### ❌ Remove Project Member

```
DELETE /api/projects/{projectId}/members/{userId}
```

* Owner tidak bisa dihapus
* Validasi permission dilakukan di Service layer

---

## 📌 Task Management Flow

### 🆕 Create Task

```
POST /api/tasks
```

Rules:

* User harus member project
* Status default: `Todo`

---

### 🔄 Update Task Status

```
PATCH /api/tasks/{taskId}/status
```

Status flow valid:

```
Todo → InProgress → Done
```

---

### 👤 Assign Task

```
PATCH /api/tasks/{taskId}/assign
```

* Task hanya bisa di-assign ke **member project**

---

## 🧠 Architecture & Data Flow

```
Controller
   ↓
Service (Business Rules & Validation)
   ↓
UnitOfWork
   ↓
Repository
   ↓
DbContext (EF Core)
   ↓
Database
```

Keuntungan:

* Separation of Concerns
* Mudah di-test
* Scalable untuk project besar
* Tidak ada direct DbContext di Service

---

## 🗄️ Database Relationships

* **User 1 — N ProjectMember**
* **Project 1 — N ProjectMember**
* **Project 1 — N ProjectTask**
* **User 1 — N AssignedTask**

Many-to-many antara **User** dan **Project** direpresentasikan oleh `ProjectMember`.

---

## ⚙️ Setup Project

```bash
dotnet new gitignore

dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0
dotnet add package EFCore.NamingConventions --version 9.0.0
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.6.0
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
```

---

## 📖 Swagger

Setelah menjalankan project:

```
https://localhost:{port}/swagger
```

Digunakan untuk:

* Testing endpoint
* Melihat request & response schema
* Debug authorization

---

## 🎯 Tujuan Project

Project ini dibuat untuk:

* Latihan **Clean Architecture di ASP.NET Core**
* Memahami **Repository & Unit of Work**
* Latihan permission & business rules
* Portfolio backend developer level intermediate–advanced

---

## ✨ Future Improvements

* Pagination & filtering task
* Activity log / audit trail
* Comment & attachment task
* Refresh token
* Global exception middleware

---

## 👨‍💻 Author

Dibuat sebagai **learning & portfolio project**
menggunakan **ASP.NET Core (.NET 9)**

> 💡 *“Clean architecture makes complex systems understandable.”*
