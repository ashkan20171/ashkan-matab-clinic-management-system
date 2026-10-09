USE [Matab];
GO
IF OBJECT_ID(N'dbo.ClinicalNotes', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.ClinicalNotes (
  NoteId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ClinicalNotes PRIMARY KEY,
  PatientCode INT NOT NULL,
  EncounterDate DATETIME2(0) NOT NULL CONSTRAINT DF_ClinicalNotes_EncounterDate DEFAULT SYSUTCDATETIME(),
  ChiefComplaint NVARCHAR(1000) NULL,
  MedicalHistory NVARCHAR(MAX) NULL,
  Allergies NVARCHAR(2000) NULL,
  Medications NVARCHAR(2000) NULL,
  Assessment NVARCHAR(MAX) NULL,
  TreatmentPlan NVARCHAR(MAX) NULL,
  CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_ClinicalNotes_CreatedAt DEFAULT SYSUTCDATETIME(),
  CONSTRAINT FK_ClinicalNotes_Parvandeh FOREIGN KEY (PatientCode) REFERENCES dbo.Parvandeh(CodeP)
 );
 CREATE INDEX IX_ClinicalNotes_PatientDate ON dbo.ClinicalNotes(PatientCode, EncounterDate DESC);
END;
GO
