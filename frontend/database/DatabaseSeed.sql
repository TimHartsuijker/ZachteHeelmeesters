USE zachteheelmeester;
GO

-- Seed roles
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 1)
    INSERT INTO rollen (rolID, rolnaam) VALUES (1, 'patiënt');
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 2)
    INSERT INTO rollen (rolID, rolnaam) VALUES (2, 'huisarts');
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 3)
    INSERT INTO rollen (rolID, rolnaam) VALUES (3, 'arts');

-- Huisarts gebruiker
IF NOT EXISTS (SELECT 1 FROM gebruikers WHERE email = 'huisarts.jansen@example.nl')
BEGIN
    INSERT INTO gebruikers (
        voornaam, achternaam, email, wachtwoord,
        Straatnaam, Huisnummer, Postcode, Telefoonnummer,
        rol, systeembeheerder
    ) VALUES (
        'Jan', 'Jansen', 'huisarts.jansen@example.nl', 'hashed_pw_demo',
        'Dorpsstraat', '12A', '1234AB', '0612345678',
        2, 0
    );
END

-- Patiënt gebruiker
IF NOT EXISTS (SELECT 1 FROM gebruikers WHERE email = 'patiënt.devries@example.nl')
BEGIN
    INSERT INTO gebruikers (
        voornaam, achternaam, email, wachtwoord,
        Straatnaam, Huisnummer, Postcode, Telefoonnummer,
        rol, systeembeheerder
    ) VALUES (
        'Emma', 'de Vries', 'patiënt.devries@example.nl', 'hashed_pw_demo',
        'Lindelaan', '45', '5678CD', '0687654321',
        1, 0
    );
END

-- Ophalen IDs om relatie te leggen
DECLARE @huisartsID INT, @patientID INT;
SELECT @huisartsID = gebruikersID FROM gebruikers WHERE email = 'huisarts.jansen@example.nl';
SELECT @patientID  = gebruikersID FROM gebruikers WHERE email = 'patiënt.devries@example.nl';

-- Koppel huisarts aan patiënt
IF NOT EXISTS (
    SELECT 1 FROM huisarts_patient WHERE huisartsID = @huisartsID AND patientID = @patientID
)
BEGIN
    INSERT INTO huisarts_patient (huisartsID, patientID) VALUES (@huisartsID, @patientID);
END

-- Optioneel: afdeling + behandeling voor demo
IF NOT EXISTS (SELECT 1 FROM afdelingen WHERE afdelingID = 'ALG')
    INSERT INTO afdelingen (afdelingID, naam) VALUES ('ALG', 'Algemene Zorg');

IF NOT EXISTS (SELECT 1 FROM behandelingen WHERE zorgcode = 'Z001')
    INSERT INTO behandelingen (zorgcode, omschrijving, specialisme, tijdsduur, kosten)
    VALUES ('Z001', 'Intake consult', 'ALG', 30, 45.00);
GO
