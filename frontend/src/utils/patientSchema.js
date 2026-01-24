export const createPatientSkeleton = (data = {}) => {
  return {
    patientID: data.patientID || 0,
    voornaam: data.voornaam || '',
    achternaam: data.achternaam || '',
    email: data.email || '',
    straatnaam: data.straatnaam || '',
    huisnummer: data.huisnummer || '',
    postcode: data.postcode || '',
    telefoonnummer: data.telefoonnummer || '',
    huisartspraktijk: data.huisartspraktijk || 'Niet bekend',
    huisartsnaam: data.huisartsnaam || 'Niet bekend',
    bsn: data.bsn || '',
    geboortedatum: data.geboortedatum || '',
    geslacht: data.geslacht || ''
  };
};