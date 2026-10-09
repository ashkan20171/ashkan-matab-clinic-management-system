USE [Matab];
GO
IF OBJECT_ID(N'dbo.AppointmentSlots', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.AppointmentSlots (
  Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppointmentSlots PRIMARY KEY,
  PatientName NVARCHAR(150) NOT NULL,
  Phone NVARCHAR(40) NULL,
  AppointmentAt DATETIME2(0) NOT NULL,
  DurationMinutes INT NOT NULL CONSTRAINT DF_AppointmentSlots_Duration DEFAULT(30),
  Status NVARCHAR(20) NOT NULL CONSTRAINT DF_AppointmentSlots_Status DEFAULT(N'Scheduled'),
  Notes NVARCHAR(1000) NULL,
  CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_AppointmentSlots_Created DEFAULT(SYSDATETIME()),
  CONSTRAINT CK_AppointmentSlots_Duration CHECK (DurationMinutes BETWEEN 5 AND 240),
  CONSTRAINT CK_AppointmentSlots_Status CHECK (Status IN (N'Scheduled',N'Arrived',N'Completed',N'Cancelled'))
 );
 CREATE INDEX IX_AppointmentSlots_AppointmentAt ON dbo.AppointmentSlots(AppointmentAt);
END;
GO
