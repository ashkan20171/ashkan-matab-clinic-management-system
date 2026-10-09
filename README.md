<div align="center">

# 🩺 Ashkan Matab
### Modernizing Clinic Operations, One Workflow at a Time

**A bilingual Windows desktop clinic management system built with C#, WinForms, and Microsoft SQL Server.**

[![C#](https://img.shields.io/badge/C%23-Desktop-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.8-5C2D91?style=for-the-badge)](https://dotnet.microsoft.com/download/dotnet-framework/net48)
[![SQL Server](https://img.shields.io/badge/SQL_Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Visual Studio](https://img.shields.io/badge/Visual_Studio-2022-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)](https://visualstudio.microsoft.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078D4?style=for-the-badge&logo=windows&logoColor=white)](https://www.microsoft.com/windows)

**Patient Records · Appointment Scheduling · Clinical Workflows · Persian / English UI · Legacy Modernization**

</div>

---

## ✨ The idea

Healthcare administration needs dependable workflows and clear interfaces. **Ashkan Matab** is a personal software engineering portfolio project that evolves an existing C# Windows Forms application into a more maintainable and usable clinic-management experience.

Rather than claiming a complete rewrite, the project demonstrates the real-world engineering challenge of **improving a legacy desktop application while preserving its existing business logic, database integration, and Designer-based forms**.

> **Portfolio status:** Under active development. This is **not** a production-certified healthcare system, and Windows end-to-end build, security and compliance testing have not yet been completed.

## 🚀 What it covers

| Area | Capabilities |
| :--- | :--- |
| 👥 **Patient management** | Patient record screens and SQL Server-backed data operations |
| 📅 **Appointments** | Booking, lookup, editing, deletion and date-related validation |
| 📊 **Clinic dashboard** | Overview cards, appointment table and navigation to clinical workflows |
| 📝 **Clinical documents** | Medical records, prescriptions and medical certificate-related modules |
| 🌐 **Localization** | Persian interface with RTL behavior and English with LTR behavior; legacy-screen coverage is still being improved |
| 🎨 **Desktop UX** | Visual refinements, keyboard access, focused-input styling, contextual tooltips and table readability |
| 🛡️ **Safer data operations** | Parameterized queries in modernized paths, scoped SQL connections and deletion confirmations in selected workflows |

## 🖼️ Interface preview

> Add genuine screenshots from a successful Windows run to `docs/screenshots/`. Avoid mockups that could be mistaken for implemented features.

| Dashboard | Patient management | Appointments |
| :---: | :---: | :---: |
| _Screenshot coming soon_ | _Screenshot coming soon_ | _Screenshot coming soon_ |

## 🧰 Technology stack

| Concern | Technology |
| :--- | :--- |
| Language | C# |
| Desktop framework | Windows Forms, .NET Framework 4.8 |
| Legacy UI components | DotNetBar (third-party dependency) |
| Data access | ADO.NET / `System.Data.SqlClient` |
| Database | Microsoft SQL Server |
| IDE | Visual Studio 2022 |
| Localization | Persian (RTL) and English (LTR) |
| Target operating system | Windows |

## 🏗️ Project structure

```text
Ashkan-Matab/
├── Matab.sln
├── Matab/
│   ├── Matab.csproj
│   ├── Program.cs
│   ├── ClinicDashboard.cs
│   ├── ClinicLanguage.cs
│   ├── ClinicEntryVisuals.cs
│   ├── DatabaseSettings.cs
│   ├── AppointmentScheduler.cs
│   ├── ElectronicMedicalRecordForm.cs
│   ├── PrescriptionManager.cs
│   ├── Reporting/
│   ├── *.Designer.cs
│   └── app.config
├── Database/
│   ├── Matab_Schema.sql
│   ├── Stage11_AppointmentSlots.sql
│   ├── Stage12_ElectronicMedicalRecords.sql
│   ├── Stage13_Prescriptions.sql
│   └── Initialize-Matab.ps1
└── README.md
```

This structure reflects the existing application; not every module follows a separate layered architecture yet.

## ⚡ Getting started

### Requirements

- Windows 10/11
- Visual Studio 2022 with **.NET desktop development** workload
- .NET Framework **4.8 Developer Pack**
- SQL Server / SQL Server Express and optionally SQL Server Management Studio
- Licensed/available DotNetBar assemblies required by the legacy project

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/ashkan20171/ashkan-matab-clinic-management-system.git
   cd ashkan-matab-clinic-management-system
   ```
2. Open `Matab.sln` in Visual Studio 2022.
3. Restore or resolve any missing third-party references. **Do not commit commercial DotNetBar binaries unless redistribution is permitted.**
4. Review `Database/Matab_Schema.sql` and the staged SQL scripts before running them against a **development-only** SQL Server instance. Back up any existing database first. Schema setup has not been validated as a fully reproducible one-click installation.
5. Configure the `MatabDatabase` connection string in `Matab/app.config` for your local SQL Server environment. Never commit credentials or secrets.
6. Select the appropriate solution configuration, then **Build → Rebuild Solution** and run.
7. If you encounter legacy project issues, consult `BUILD_FIX_GUIDE_FA.md`.

> **Important:** A ready-to-use database with real records is not distributed. Use synthetic sample data only.

## 🌍 Localization and UX

The interface is designed for **Persian (right-to-left)** and **English (left-to-right)** usage. Modernized screens adapt captions and layout direction; some legacy WinForms/DotNetBar controls still need additional localization and live-switch testing.

The UI work emphasizes a calm navy-and-teal palette, readable forms, safer destructive actions, and efficient keyboard navigation.

## 🔐 Privacy and responsible development

Healthcare information is sensitive. This repository is intended for demonstration and learning, **not** real patient data or live clinic deployment. Before any production use, the application would need an independent security and privacy review, access controls, audit trails, encryption and secret management, backup/restore testing, reliability validation, and an assessment of applicable regulations such as GDPR where relevant.

No claim of GDPR compliance, medical-device certification, or production readiness is made.

## 🧪 Quality and current limitations

- Windows compilation and full end-to-end workflows have **not** been independently verified for the latest stage.
- Automated tests and continuous integration are not yet in place.
- Some legacy forms still mix presentation logic and SQL operations.
- The SQL scripts require review and environment-specific configuration.
- Full RTL/LTR coverage and accessibility validation remain in progress.
- External DotNetBar licensing and distribution must be checked before public release.

## 🗺️ Roadmap

- [ ] Reproducible SQL Server development database and synthetic seed data
- [ ] Structured data access and clearer separation of responsibilities
- [ ] Authentication hardening, roles and permissions
- [ ] Audit logs and privacy-by-design controls
- [ ] Comprehensive RTL/LTR coverage across legacy screens
- [ ] Automated tests and Windows CI build
- [ ] Responsive form layouts and visual regression review
- [ ] Backup/restore documentation and operational diagnostics

## 💡 Engineering value

**For software teams in Germany and across Europe**, this project illustrates experience with:

- Maintaining and incrementally modernizing **legacy .NET desktop systems**
- Integrating **C#, ADO.NET and SQL Server** in business workflows
- Balancing **backward compatibility, UI/UX improvements and data safety**
- Supporting **multilingual applications and RTL/LTR interfaces**
- Communicating implementation status, technical debt and delivery risks transparently

## 👨‍💻 Author

**Ashkan Motaei**  
Software Developer · C# / .NET · Desktop & Web Applications  
GitHub: [@ashkan20171](https://github.com/ashkan20171)

## 📜 License

**No open-source license is currently granted.** If you intend to allow reuse, add an explicit `LICENSE` file after reviewing third-party component terms.

---

<div align="center">

**Built with C# • Evolving with care • Designed for clearer clinic workflows**

⭐ If you find the engineering approach interesting, consider starring the repository.

</div>
