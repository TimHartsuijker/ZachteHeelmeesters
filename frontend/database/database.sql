USE zachteheelmeester;

-- Drop tables in reverse order that they are created in the script below to avoid foreign key constraint errors
DROP TABLE IF EXISTS afspraken;
DROP TABLE IF EXISTS afdelingen;
DROP TABLE IF EXISTS medischdossierentry;
DROP TABLE IF EXISTS huisarts_patient;
DROP TABLE IF EXISTS gebruikers;
DROP TABLE IF EXISTS rollen;
DROP TABLE IF EXISTS behandelingen;

create table behandelingen (
	zorgcode varchar(50) NOT NULL,
    omschrijving varchar(255) NOT NULL,
    specialisme varchar(20) NOT NULL,
    tijdsduur INT NOT NULL,
    kosten FLOAT NOT NULL,
    PRIMARY KEY (zorgcode)
);

CREATE TABLE rollen(
    rolID INT NOT NULL,
    rolnaam VARCHAR(100) NOT NULL,
    PRIMARY KEY (rolID)
);

CREATE TABLE gebruikers(
    gebruikersID INT IDENTITY(1,1) NOT NULL,
    voornaam VARCHAR(100) NOT NULL,
    achternaam VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    wachtwoord VARCHAR(100) NOT NULL,
    Straatnaam VARCHAR(100) NOT NULL,
    Huisnummer VARCHAR(100) NOT NULL,
    Postcode VARCHAR(6) NOT NULL,
    Telefoonnummer VARCHAR(15) NOT NULL,
    rol INT NOT NULL,
    systeembeheerder BIT NOT NULL,
    FOREIGN KEY (rol) REFERENCES rollen(rolID),
    PRIMARY KEY (gebruikersID)
);

CREATE TABLE huisarts_patient (
    huisartsID INT NOT NULL,
    patientID INT NOT NULL,

    FOREIGN KEY (huisartsID) REFERENCES gebruikers(gebruikersID),
    FOREIGN KEY (patientID) REFERENCES gebruikers(gebruikersID),

    PRIMARY KEY (huisartsID, patientID)
);

CREATE TABLE medischdossierentry (
    entryID INT IDENTITY(1,1) NOT NULL,
    patientID INT NOT NULL,
    datumtijd DATETIME NOT NULL,
    -- nog geen idee wat hier verder moet komen
    FOREIGN KEY (patientID) REFERENCES gebruikers(gebruikersID),
    PRIMARY KEY (patientID, entryID)
);

CREATE TABLE afdelingen(
    afdelingID VARCHAR(50) NOT NULL,
    naam VARCHAR(100) NOT NULL,
    PRIMARY KEY (afdelingID)
);

CREATE TABLE afspraken(
    datumtijd DATETIME not null,
    behandeling VARCHAR(50) NOT NULL,
    afdeling VARCHAR(50) NOT NULL,
    arts INT NOT NULL,
    PRIMARY KEY (datumtijd, arts),
    FOREIGN KEY (arts) REFERENCES gebruikers(gebruikersID),
    FOREIGN KEY (behandeling) REFERENCES behandelingen(zorgcode),
    FOREIGN KEY (afdeling) REFERENCES afdelingen(afdelingID)
);

