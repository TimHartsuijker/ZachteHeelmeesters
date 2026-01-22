-- Clear test users so they can be re-seeded with correct relationships
-- Run this script in your SQL Server database

DELETE FROM Users WHERE Email = 'gebruiker@example.com';
DELETE FROM Users WHERE Email = 'testdoctor@example.com';

-- Verify deletion
SELECT * FROM Users WHERE Email IN ('gebruiker@example.com', 'testdoctor@example.com');
-- Should return 0 rows
