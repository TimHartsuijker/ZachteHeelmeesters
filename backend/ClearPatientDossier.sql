-- Clear medical dossier entries for a specific patient
-- WARNING: This deletes data! Use only for testing purposes.

-- Replace with the patient email you want to clear
DECLARE @PatientEmail NVARCHAR(255) = 'gebruiker@example.com';

-- Get the patient ID
DECLARE @PatientId INT;
SELECT @PatientId = Id FROM Users WHERE Email = @PatientEmail;

-- Delete medical record files first (FK constraint)
DELETE FROM MedicalRecordFiles 
WHERE MedicalRecordEntryId IN (
    SELECT Id FROM MedicalRecordEntries WHERE PatientId = @PatientId
);

-- Delete medical record entries
DELETE FROM MedicalRecordEntries WHERE PatientId = @PatientId;

-- Verify
SELECT 
    u.Email,
    COUNT(mre.Id) AS RemainingEntries
FROM Users u
LEFT JOIN MedicalRecordEntries mre ON u.Id = mre.PatientId
WHERE u.Email = @PatientEmail
GROUP BY u.Email;

PRINT 'Medical dossier cleared for ' + @PatientEmail;
