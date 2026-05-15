# SESSION_STATE

Last updated: 2026-05-14

## Project

Repository: `https://github.com/shawnofthedev/VeteranAssist`

Local path used in this session: `C:\Users\svota\source\repos\VeteranAssist`

Project: VeteranEvidenceAssist, a privacy-first .NET 9 / .NET MAUI Windows desktop app for helping veterans organize and review their own military and medical records.

Important disclaimer:

This application is designed to assist veterans in organizing and reviewing their own records. It is not a VA-accredited representative, legal service, medical diagnostic tool, automatic claim filing system, or disability rating estimator.

## Active Phase

Current phase: Phase 1, Local Document Intake complete.

Current goal:

Maintain the completed safe local PDF intake milestone and prepare for the next phase.

## Hard Constraints

- Privacy-first and local-first.
- Never modify original imported files.
- Never upload documents or extracted text.
- Do not add cloud AI/provider calls yet.
- Do not add OCR yet.
- Do not add real redaction/export yet.
- Do not add legal, medical, claim recommendation, or VA rating logic.
- Do not log raw document text or PII.
- Preserve provenance for extracted text with document/page references.
- Keep Clean Architecture boundaries:
  - Core: domain models, enums, interfaces.
  - Documents: local PDF import/text extraction.
  - Storage: local metadata persistence.
  - Security: hashing/encryption/security services.
  - App: MAUI UI/workflow/view models.

## Implemented So Far

### Core

- Domain models include `VeteranDocument`, `DocumentPage`, and `ExtractedTextBlock`.
- `VeteranDocument` now persists:
  - `OriginalFileName`
  - `LocalFilePath`
  - `DocumentType`
  - `Sha256Hash`
  - `ImportedAt`
  - `ExtractionStatus`
  - `ExtractedTextCharacterCount`
  - `Pages`
- Added `DocumentExtractionStatus` enum:
  - `Unknown`
  - `EmbeddedTextExtracted`
  - `OcrNeeded`
  - `NoTextFound`
  - `ExtractionFailed`
- Added `IFileHashService`.
- Added `IDocumentRepository` for document metadata save/get/list/hash lookup.

### Security

- Added `Sha256FileHashService`.
- Hashing is local and testable.

### Documents

- `LocalDocumentImportService` performs the real Phase 1 local import work. `PlaceholderDocumentImportService` remains as a compatibility wrapper.
- Import behavior:
  - Validates PDF path.
  - Copies PDF into local workspace under `AppData/Documents/{DocumentId}/original.pdf`.
  - Leaves original file untouched.
  - Computes SHA-256 hash on the copied file.
  - Reuses an existing metadata record when the selected PDF hash is already imported and the workspace copy still exists.
  - Extracts embedded text via `PlaceholderTextExtractionService`.
  - Persists pages and extracted text blocks.
  - Persists extracted text preview and sets extraction status:
    - `EmbeddedTextExtracted` when meaningful embedded text is found.
    - `OcrNeeded` when pages exist but little/no embedded text exists.
  - Sets `RequiresOcr` when extraction status is OCR-needed.
  - Cleans up copied workspace file if PDF extraction fails before metadata persistence.
- `PlaceholderTextExtractionService` uses PdfPig for embedded text extraction only.
- No OCR is implemented.

### Storage

- `JsonLocalStorageService` persists document metadata locally as JSON and implements `IDocumentRepository`.
- Current metadata path comes from `LocalWorkspaceOptions`.
- SQLite/EF Core remains a later intended direction, but JSON is current Phase 1 persistence.

### App

- MAUI Shell navigation includes:
  - Dashboard
  - Documents
  - Review
  - Redactions
  - Evidence Timeline
  - Prompt Builder
  - Exports
  - Settings
- `DocumentsPage`:
  - Lets user select one or more local PDFs.
  - Imports PDFs through `IDocumentImportService`.
  - Lists imported documents from `IDocumentRepository`.
  - Shows page count, extraction status, short hash, and import date.
  - Navigates to document review using Shell route `//document-review`.
  - Navigation failures are caught and shown as a safe status message.
- `DocumentReviewPage`:
  - Loads selected document by `documentId`.
  - Shows file name, import date, short hash, page count, extraction status.
  - Shows extracted text preview grouped by source page.
  - Shows OCR-needed warning when status is `OcrNeeded`.
  - PDF rendering is still a placeholder.
- Added `DocumentDetailViewModel` in App for review-page projection.

### Docs

- `README.md` updated for Phase 1 status, privacy statement, non-goals, setup/build/test commands, roadmap, architecture, and screenshots placeholders.
- `docs/project-setup.md` updated for current solution layout, Phase 1 status, workspace notes, and commands.

## Tests

Current test project: `tests\VeteranEvidenceAssist.Tests`

Current test count observed: 21 passing.

Covered behaviors include:

- File hash generation.
- Import copies PDF into workspace.
- Import does not mutate original file bytes.
- Non-PDF import rejection.
- Empty/whitespace path rejection.
- Missing PDF rejection.
- Workspace directory/metadata creation.
- Corrupt PDF does not persist partial metadata and cleans up copied file.
- Metadata persistence across storage instances.
- Metadata round-trip for pages and text blocks.
- Document listing order by newest import first.
- Text-based PDF embedded text extraction.
- Text-based PDF status is `EmbeddedTextExtracted`.
- Blank/no-text PDF creates page metadata without OCR.
- Blank/no-text PDF status is `OcrNeeded`.
- Duplicate imports reuse an existing record when the workspace copy still exists.
- Stale duplicate metadata with a missing workspace copy allows a new import.
- Repository lookup by SHA-256 hash.
- Synthetic extraction failure cleanup/no partial metadata persistence.

Useful commands:

```powershell
dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

When the MAUI app is running and locking output DLLs, use a temporary output folder for compile checks:

```powershell
dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj --no-restore -p:OutputPath=bin\Debug\verify\
```

This compile check passed after the latest changes. It may leave generated build output under `bin\Debug\verify`.

## Known Warnings / Issues

- MAUI XAML compiled-binding warnings remain for CollectionView templates because `x:DataType` is not set.
- The app may lock output DLLs while running, causing normal solution builds to fail until the app is closed.
- `PlaceholderTextExtractionService` name is still misleading because it contains real embedded PDF text extraction behavior.
- PDF page rendering is not implemented; Document Review uses a placeholder viewer.
- OCR is intentionally not implemented.
- Redaction/export workflows are still placeholders.
- Storage is JSON-backed and not encrypted; future work should revisit SQLite and encryption.
- `ImportDocumentsPage` still exists from earlier work but the active Shell route uses `DocumentsPage`.

## Suggested Next Steps

1. Rename `PlaceholderTextExtractionService` to `PdfPigTextExtractionService`.
2. Add compiled bindings or typed view models for MAUI list templates.
3. Add a proper document detail route registration if Shell behavior needs refinement.
4. Add a user-visible local workspace path display.
5. Add stronger per-file duplicate import UX.
6. Add local OCR in a later phase only after privacy/security review.
7. Add encrypted or SQLite-backed storage in a later phase.
8. Add tests for `DocumentDetailViewModel`.

## Most Relevant Files

- `README.md`
- `docs/project-setup.md`
- `AGENTS.md`
- `src/VeteranEvidenceAssist.Core/Models/VeteranDocument.cs`
- `src/VeteranEvidenceAssist.Core/Enums/DocumentExtractionStatus.cs`
- `src/VeteranEvidenceAssist.Core/Interfaces/IFileHashService.cs`
- `src/VeteranEvidenceAssist.Core/Interfaces/IDocumentRepository.cs`
- `src/VeteranEvidenceAssist.Documents/Services/PlaceholderDocumentImportService.cs`
- `src/VeteranEvidenceAssist.Documents/Services/PlaceholderTextExtractionService.cs`
- `src/VeteranEvidenceAssist.Storage/Repositories/JsonLocalStorageService.cs`
- `src/VeteranEvidenceAssist.Security/Services/Sha256FileHashService.cs`
- `src/VeteranEvidenceAssist.App/Pages/DocumentsPage.xaml`
- `src/VeteranEvidenceAssist.App/Pages/DocumentsPage.xaml.cs`
- `src/VeteranEvidenceAssist.App/Pages/DocumentReviewPage.xaml`
- `src/VeteranEvidenceAssist.App/Pages/DocumentReviewPage.xaml.cs`
- `src/VeteranEvidenceAssist.App/ViewModels/DocumentDetailViewModel.cs`
- `tests/VeteranEvidenceAssist.Tests/DocumentImportTests.cs`
