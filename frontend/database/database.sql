DROP DATABASE IF EXISTS zachteheelmeester;

CREATE database zachteheelmeester;

USE zachteheelmeester;

drop table IF EXISTS behandelingen;

create table behandelingen (
	zorgcode varchar(50) NOT NULL,
    omschrijving varchar(255) NOT NULL,
    specialisme varchar(20) NOT NULL,
    tijdsduur int(2) NOT NULL,
    kosten FLOAT NOT NULL,
    PRIMARY KEY (zorgcode)
);

DROP TABLE IF EXISTS rollen;

CREATE TABLE rollen(
    rolID VARCHAR(50) NOT NULL,
    rolnaam VARCHAR(100) NOT NULL,
    PRIMARY KEY (rolID)
);

DROP TABLE IF EXISTS gebruikers;

CREATE TABLE gebruikers(
    email VARCHAR(100) NOT NULL,
    wachtwoord VARCHAR(100) NOT NULL,
    rol VARCHAR(50) NOT NULL,
    FOREIGN KEY (rol) REFERENCES rollen(rolID),
    PRIMARY KEY (email)
);

DROP TABLE IF EXISTS afdelingen;

CREATE TABLE afdelingen(
afdelingID VARCHAR(50) NOT NULL,
naam VARCHAR(100) NOT NULL,
PRIMARY KEY (afdelingID)
);

DROP TABLE IF EXISTS afspraken;

CREATE TABLE afspraken(
datumtijd DATETIME not null,
behandeling VARCHAR(50) NOT NULL,
afdeling VARCHAR(50) NOT NULL,
FOREIGN KEY (behandeling) REFERENCES behandelingen(zorgcode),
FOREIGN KEY (afdeling) REFERENCES afdelingen(afdelingID)
);