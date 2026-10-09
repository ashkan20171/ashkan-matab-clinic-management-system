USE [Matab];
GO
-- Additive, rerunnable migration. Does not modify legacy patient or visit tables.
IF OBJECT_ID(N'dbo.PatientPrescriptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PatientPrescriptions
    (
        PrescriptionId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PatientPrescriptions PRIMARY KEY,
        PatientCode INT NOT NULL,
        PrescribedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_PatientPrescriptions_PrescribedAtUtc DEFAULT SYSUTCDATETIME(),
        MedicationName NVARCHAR(250) NOT NULL,
        Directions NVARCHAR(2000) NULL,
        Prescriber NVARCHAR(200) NOT NULL,
        CONSTRAINT FK_PatientPrescriptions_Patient FOREIGN KEY (PatientCode) REFERENCES dbo.Parvandeh(CodeP)
    );
    CREATE INDEX IX_PatientPrescriptions_Patient ON dbo.PatientPrescriptions(PatientCode, PrescriptionId DESC);
END;
GO
