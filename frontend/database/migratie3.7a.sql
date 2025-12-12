USE zachteheelmeester;

-- Insert all roles (only if they don't exist)
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 1)
    INSERT INTO rollen (rolID, rolnaam) VALUES (1, 'patiënt');
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 2)
    INSERT INTO rollen (rolID, rolnaam) VALUES (2, 'huisarts');
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 3)
    INSERT INTO rollen (rolID, rolnaam) VALUES (3, 'specialist');
IF NOT EXISTS (SELECT 1 FROM rollen WHERE rolID = 4)
    INSERT INTO rollen (rolID, rolnaam) VALUES (4, 'admin');

-- Insert test users (only if they don't exist)
-- Password is 'Test123!' (you should hash this in production)
IF NOT EXISTS (SELECT 1 FROM gebruikers WHERE email = 'patient@test.nl')
    INSERT INTO gebruikers (
        voornaam, 
        achternaam, 
        email, 
        wachtwoord, 
        Straatnaam, 
        Huisnummer, 
        Postcode, 
        Telefoonnummer, 
        rol
    ) VALUES (
        'Jan',
        'Testpersoon',
        'patient@test.nl',
        'Test123!',
        'Teststraat',
        '123',
        '1234AB',
        '0612345678',
        1
    );

IF NOT EXISTS (SELECT 1 FROM gebruikers WHERE email = 'huisarts@test.nl')
    INSERT INTO gebruikers (
        voornaam, 
        achternaam, 
        email, 
        wachtwoord, 
        Straatnaam, 
        Huisnummer, 
        Postcode, 
        Telefoonnummer, 
        rol
    ) VALUES (
        'Emma',
        'Dokter',
        'huisarts@test.nl',
        'Test123!',
        'Dokterslaan',
        '45',
        '5678CD',
        '0687654321',
        2
    );
