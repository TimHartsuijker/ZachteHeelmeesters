-- Create doctor_availability table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'doctor_availability')
BEGIN
    CREATE TABLE doctor_availability (
        availability_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        doctor_id INT NOT NULL,
        date_time DATETIME2 NOT NULL,
        is_available BIT NOT NULL DEFAULT 1,
        reason NVARCHAR(MAX) NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        updated_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_doctor_availability_Users FOREIGN KEY (doctor_id) REFERENCES Users(Id),
        CONSTRAINT UQ_doctor_availability_doctor_datetime UNIQUE (doctor_id, date_time)
    );
    
    PRINT 'doctor_availability table created successfully';
END
ELSE
BEGIN
    PRINT 'doctor_availability table already exists';
END
GO

-- Seed availability slots for doctor ID 2 (testdoctor@example.com) for week of 2026-01-07
DECLARE @DoctorId INT = 2;
DECLARE @SlotStart DATETIME2 = '2026-01-07T10:00:00';
DECLARE @SlotEnd DATETIME2 = '2026-01-07T15:30:00';

WHILE @SlotStart <= @SlotEnd
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM doctor_availability
        WHERE doctor_id = @DoctorId AND date_time = @SlotStart
    )
    BEGIN
        INSERT INTO doctor_availability (doctor_id, date_time, is_available, reason, created_at, updated_at)
        VALUES (@DoctorId, @SlotStart, 1, NULL, GETUTCDATE(), GETUTCDATE());
        
        PRINT 'Added slot: ' + CONVERT(VARCHAR, @SlotStart, 120);
    END;
    
    SET @SlotStart = DATEADD(MINUTE, 15, @SlotStart);
END;

PRINT 'Doctor availability slots seeded successfully';
