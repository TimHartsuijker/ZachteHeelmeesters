# Quick Start Guide - Patient Dossier

## 🚀 Start the Application

### 1. Start Backend
```bash
cd backend
dotnet run
```
✅ Backend running on: https://localhost:7240
✅ Swagger UI: https://localhost:7240/swagger

### 2. Start Frontend
```bash
cd frontend
npm run dev
```
✅ Frontend running on: http://localhost:5173

## 📱 Access the Application

### Patient View (Read-Only Dossier)
🔗 http://localhost:5173/dossier

**Features:**
- View all medical documents
- Filter by category
- Filter by date range
- Download files
- Read-only access (no editing)

### Doctor View (Upload Documents)
🔗 http://localhost:5173/doctor/upload

**Features:**
- Upload medical documents
- Drag-and-drop files
- Select category
- Add description
- File validation

## 📋 Acceptance Criteria Met

✅ AC2.12.1 - Patient can open medical dossier
✅ AC2.12.2 - Patient can view complete medical history
✅ AC2.12.3 - Data structured and filterable
✅ AC2.14.4 - Read-only patient access

## 🧪 Quick Test

1. **Upload a file** (Doctor view):
   - Go to http://localhost:5173/doctor/upload
   - Upload a PDF, Word doc, or image
   - Select a category
   - Click "Document Uploaden"

2. **View the file** (Patient view):
   - Go to http://localhost:5173/dossier
   - See your uploaded file
   - Try the filters
   - Download the file

## 📁 File Requirements

- **Types**: PDF, Word, JPG, PNG, TXT
- **Max Size**: 10 MB
- **Categories**: Röntgenfoto, Labresultaten, Ontslagbrief, etc.

## 🔧 Troubleshooting

### Backend won't start
- Check connection string in `backend/appsettings.json`
- Ensure SQL Server LocalDB is running
- Run: `dotnet ef database update`

### Frontend won't start
- Run: `npm install` in frontend folder
- Check Node.js is installed
- Clear npm cache: `npm cache clean --force`

### Can't connect to API
- Verify backend is running on port 7240
- Check browser accepts SSL certificate
- Verify CORS is enabled in Program.cs

## 📖 Full Documentation

For complete details, see:
- `IMPLEMENTATION_SUMMARY.md` - Implementation overview
- `PATIENT_DOSSIER_README.md` - Detailed technical documentation

---
**Ready to use!** 🎉
