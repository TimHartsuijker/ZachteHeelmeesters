-- 4.20-migration.sql
-- Doctor Availability Calendar Migration
-- This migration adds a calendar system for doctors to manage their availability
-- Time slots are 15-minute increments

-- Create DoctorAvailability table to store doctor availability/unavailability
CREATE TABLE doctor_availability (
    availability_id INT IDENTITY(1,1) NOT NULL,
    doctor_id INT NOT NULL,
    date_time DATETIME NOT NULL,  -- Start time of the 15-minute slot (always on 0, 15, 30, or 45 minute mark)
    is_available BIT NOT NULL DEFAULT 1,  -- 1 = available, 0 = unavailable
    reason VARCHAR(255) NULL,  -- Optional reason for unavailability (e.g., "lunch break", "vacation", "off work")
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    updated_at DATETIME NOT NULL DEFAULT GETDATE(),
    
    FOREIGN KEY (doctor_id) REFERENCES gebruikers(gebruikersID),
    PRIMARY KEY (availability_id),
    UNIQUE (doctor_id, date_time)  -- One entry per doctor per time slot
);

-- Create index for efficient querying of availability by doctor and date range
CREATE INDEX idx_doctor_availability_doctor_date 
ON doctor_availability(doctor_id, date_time);

-- Add a check constraint to ensure times are on 15-minute boundaries
ALTER TABLE doctor_availability
ADD CONSTRAINT ck_fifteen_minute_slots 
CHECK (DATEPART(MINUTE, date_time) % 15 = 0);
