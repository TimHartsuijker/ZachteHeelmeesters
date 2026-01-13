-- Clear test medical data to allow reseeding
DELETE FROM MedicalRecordFiles;
DELETE FROM MedicalRecordEntries;
DELETE FROM Appointments;
DELETE FROM Referrals;
DELETE FROM Treatments;
DELETE FROM Specialisms;
DELETE FROM Departments;

PRINT 'Test data cleared. Restart the backend to reseed.';
