# Patient Dossier Implementation - US2.12

## Overview
This implementation provides a comprehensive patient dossier system where:
- **Patients** can view their complete medical history with uploaded documents (AC2.12.1, AC2.12.2)
- **Documents** can be filtered by category and date range (AC2.12.3)
- **Patients** have read-only access to their dossier (AC2.14.4)
- **Doctors** can upload medical documents (Word, PDF, images, etc.) to patient dossiers

## Architecture

### Backend (.NET/C#)
- **Storage Method**: Files are stored as binary data (`varbinary(max)`) in SQL Server database
  - Following the Stack Overflow approach: https://stackoverflow.com/questions/21259677/how-to-store-word-documents-in-sql-server-2008
  - This approach is better for:
    - Simplified backup (single database backup includes all files)
    - Better security (no file system access needed)
    - Easier deployment and portability
    - Transaction consistency

- **Model Updates**:
  - `MedicalRecordFile.cs`: Changed from `FilePath` to `FileContent` (byte[])
  - Added `Category` and `Description` fields for better organization

- **New Controller**: `MedicalDossierController.cs`
  - `GET /api/MedicalDossier/patient/{patientId}` - Get all patient files with filtering
  - `GET /api/MedicalDossier/patient/{patientId}/categories` - Get available categories
  - `GET /api/MedicalDossier/file/{fileId}` - Download a specific file
  - `POST /api/MedicalDossier/upload` - Upload a new file (doctors)
  - `DELETE /api/MedicalDossier/file/{fileId}` - Delete a file (doctors)

- **Migration**: `UpdateMedicalRecordFileForBinaryStorage`
  - Drops `FilePath` column
  - Adds `FileContent` (varbinary(max))
  - Adds `Category` and `Description` columns

### Frontend (Vue.js)
- **Patient View**: `MedicalDossier.vue`
  - Displays all medical documents in card format
  - Filter by category (dropdown populated from backend)
  - Filter by date range (from/to dates)
  - Download files with one click
  - Shows file metadata (uploader, date, size, category)
  - Read-only interface (no edit/delete buttons for patients)

- **Doctor View**: `DoctorUpload.vue`
  - Upload documents to patient dossiers
  - Drag-and-drop file upload
  - Category selection (required)
  - Optional description field
  - File type and size validation

- **Component**: `FileUpload.vue`
  - Reusable file upload component
  - Supports drag-and-drop
  - Shows file preview before upload
  - Progress indication during upload

## Setup Instructions

### 1. Backend Setup

#### Apply Database Migration
```bash
cd backend
dotnet ef database update
```

This will:
- Drop the old `FilePath` column
- Add `FileContent` column (varbinary(max))
- Add `Category` and `Description` columns

#### Run the Backend
```bash
cd backend
dotnet run
```

The backend will start on:
- HTTPS: https://localhost:7240
- HTTP: http://localhost:5016

Swagger UI available at: https://localhost:7240/swagger

### 2. Frontend Setup

#### Install Dependencies (if not already done)
```bash
cd frontend
npm install
```

#### Run the Frontend
```bash
cd frontend
npm run dev
```

The frontend will start on: http://localhost:5173

## Routes

- `/dossier` - Patient view of medical dossier (AC2.12.1)
- `/doctor/upload` - Doctor upload interface

## Acceptance Criteria Coverage

✅ **AC2.12.1**: "De patiënt kan in het portaal het medisch dossier openen."
- Route `/dossier` provides access to the medical dossier
- Clear header and navigation

✅ **AC2.12.2**: "De patiënt kan zijn volledige medische geschiedenis altijd terugzien."
- All files are fetched from the database and displayed
- Historical data is preserved and always accessible
- Files are sorted by date (most recent first)

✅ **AC2.12.3**: "De gegevens zijn overzichtelijk gestructureerd per datum of categorie en kunnen gefilterd worden."
- Files displayed in card format with clear date and category
- Filter by category (dropdown with all available categories)
- Filter by date range (from/to inputs)
- Filters can be applied and reset

✅ **AC2.14.4**: "De patiënt kan het medisch dossier alleen inzien, niet aanpassen."
- Patient view has no edit or delete buttons
- Only download functionality available
- Upload interface is separate (doctor-only route)

## File Upload Specifications

### Supported File Types
- PDF documents (.pdf)
- Word documents (.doc, .docx)
- Images (.jpg, .jpeg, .png)
- Text files (.txt)

### File Size Limit
- Maximum: 10 MB per file

### Categories
1. Röntgenfoto
2. Labresultaten
3. Ontslagbrief
4. Verwijsbrief
5. Medicatielijst
6. Behandelplan
7. Overige

## API Examples

### Get Patient Dossier (with filters)
```
GET /api/MedicalDossier/patient/1?category=Röntgenfoto&fromDate=2025-01-01&toDate=2025-12-31
```

### Upload File
```
POST /api/MedicalDossier/upload
Content-Type: multipart/form-data

{
  file: [binary],
  patientId: 1,
  doctorId: 2,
  appointmentId: 1,
  category: "Labresultaten",
  description: "Bloedonderzoek resultaten"
}
```

### Download File
```
GET /api/MedicalDossier/file/5
```

## Security Considerations

### Current Implementation
- Files stored in database with binary data
- CORS configured for localhost development

### TODO for Production
- [ ] Add authentication/authorization
- [ ] Verify user has permission to access specific patient dossiers
- [ ] Add audit logging for file access
- [ ] Implement rate limiting for downloads
- [ ] Add virus scanning for uploaded files
- [ ] Use HTTPS only in production
- [ ] Configure CORS for production domains

## Testing

### Manual Testing Steps

#### Patient View Testing
1. Navigate to `/dossier`
2. Verify all files are displayed
3. Test category filter
4. Test date range filter
5. Download a file and verify it opens correctly
6. Verify no edit/delete buttons are present

#### Doctor Upload Testing
1. Navigate to `/doctor/upload`
2. Try uploading different file types
3. Verify category selection is required
4. Test drag-and-drop functionality
5. Verify file appears in patient dossier after upload

## Known Limitations

1. **Authentication**: Currently uses hardcoded patient/doctor IDs
   - TODO: Integrate with authentication system

2. **File Size**: Large files may impact database performance
   - Consider implementing a file size warning
   - For very large files (>50MB), consider cloud storage (Azure Blob)

3. **Performance**: Loading many files may be slow
   - Implement pagination if needed
   - Consider lazy loading for file content

## Database Schema

```sql
MedicalRecordFiles
├── Id (int, PK)
├── MedicalRecordEntryId (int, FK)
├── FileName (nvarchar)
├── FileContent (varbinary(max)) -- Binary file data
├── ContentType (nvarchar)
├── FileSize (bigint)
├── UploadedAt (datetime2)
├── Category (nvarchar, nullable)
└── Description (nvarchar, nullable)
```

## Troubleshooting

### Migration Issues
If migration fails, try:
```bash
dotnet ef migrations remove
dotnet ef migrations add UpdateMedicalRecordFileForBinaryStorage
dotnet ef database update
```

### CORS Errors
Ensure backend Program.cs has:
```csharp
builder.Services.AddCors(options => { ... });
app.UseCors("AllowFrontend");
```

### Frontend Can't Connect to Backend
1. Verify backend is running on https://localhost:7240
2. Check API_BASE_URL in Vue files matches backend URL
3. Accept SSL certificate if prompted

## Future Enhancements

1. **File Preview**: Add in-browser preview for PDFs and images
2. **Versioning**: Track file versions and changes
3. **Sharing**: Allow patients to share files with other healthcare providers
4. **Search**: Full-text search across file descriptions
5. **Notifications**: Notify patients when new files are added
6. **Bulk Upload**: Allow doctors to upload multiple files at once
7. **Cloud Storage**: Migrate to Azure Blob Storage for better scalability
