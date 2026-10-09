# Matab | Clinic Management — maintenance release

## Environment
- Windows 10/11, Visual Studio 2022 with .NET desktop development workload
- .NET Framework 4.8 Developer Pack
- SQL Server database named `Matab` (database backup/schema is **not** provided)

## Changes
- Preserved the original WinForms forms and solution.
- Centralized the legacy SQL Server connection string in `Matab/app.config`.
- Parameterized login SQL to prevent injection, added connection error feedback, and disposed connection/command.
- Bundled the two legacy DLLs supplied in the original archive under `Matab/lib` and adjusted their reference paths.
- Removed machine-specific Stimulsoft reference paths in favor of `Matab/lib` (DLLs are **not included** in the original upload).

## Important limitations
This is a targeted source-level maintenance update, **not** a verified clean build or a completed modernization. The original Stimulsoft 2010 assemblies and compatible licenses must be supplied in `Matab/lib` to compile reporting code. The existing login schema stores plaintext passwords; do not deploy with real patient data until the database and all password workflows have been migrated to salted password hashing. Other forms may still contain unsafe dynamic SQL and need a full audit. A database backup, schema and integration testing are necessary before deployment.

## Build
Open `Matab.sln` in Visual Studio 2022, restore missing proprietary Stimulsoft DLLs, set connection string in `Matab/app.config`, and build for x86 as configured in the project. Do not put actual patient data or secrets into the repository.
