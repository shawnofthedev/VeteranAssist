# SESSION_STATE

Last updated: 2026-05-19

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
  - Shows the local workspace root, document-copy directory, and JSON metadata path.
  - Can copy the local workspace root path to the clipboard without copying document contents.
  - Pre-checks selected file hashes and summarizes duplicates that reuse existing local records.
  - Shows a latest import results panel with per-file status: new imports, already-imported duplicates, or validation failures.
  - Shows clearer local intake messaging: selected PDFs are copied into app-managed local storage, source files stay unchanged, metadata stays local, and document contents are not uploaded.
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
- Added app appearance settings:
  - `AppearanceSettingsService` persists the selected mode with MAUI `Preferences`.
  - Supported modes are Light, Dark, and System Default.
  - Startup applies the saved mode through `Application.Current.UserAppTheme`.
  - Settings page includes an Appearance section with a theme picker.
  - Settings page shows the actual local workspace root, document-copy directory, and metadata index path.
  - Settings page explains that workspace paths are app-managed local storage for copies and metadata, originals are not moved or edited, and future external AI must require payload review.
  - Shared styles use theme-aware resources for cards, text, badges, callouts, and related placeholders.

### Docs

- `README.md` updated for Phase 1 status, privacy statement, non-goals, setup/build/test commands, roadmap, architecture, and screenshots placeholders.
- `docs/project-setup.md` updated for current solution layout, Phase 1 status, workspace notes, appearance setting, and commands.
- `docs/roadmap.md` added as the durable phase-by-phase product roadmap. It captures the long-term direction through document intake, OCR, PII/redaction, structured extraction, evidence workspace, prompt building, AI providers, local AI, reporting, and release polish while preserving privacy, user control, provenance, and ethical guardrails.
- `docs/ai-context/REPO_MAP.md` now includes `docs/roadmap.md`.
- `docs/adr/README.md` now includes ADR-0008 in the index.
- `README.md`, `docs/project-setup.md`, `docs/roadmap.md`, and `docs/ai-context/REPO_MAP.md` updated after workspace visibility and duplicate-import UX polish.

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

Latest verification:

- `dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj --no-restore -p:OutputPath=bin\Debug\verify\` passed after adding appearance settings.
- `dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj --no-restore` passed with 21 tests.
- Documentation-only roadmap update on 2026-05-19; no build or tests run for that change.
- `dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj --no-restore -p:OutputPath=bin\Debug\verify\` passed after workspace path and duplicate-import UX polish. Existing MAUI compiled-binding warnings remain.
- `dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj --no-restore` passed with 21 tests after workspace path and duplicate-import UX polish.
- `dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj --no-restore -p:OutputPath=bin\Debug\verify\` passed after clearer storage/privacy messaging on Documents and Settings pages. Existing MAUI compiled-binding warnings remain.
- `dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj --no-restore -p:OutputPath=bin\Debug\verify\` passed after adding per-file import results for duplicate-import UX. Existing MAUI compiled-binding warnings remain and now include the new import results list.

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
- Appearance preference is low-sensitivity UI state stored with MAUI `Preferences`, not encrypted storage.
- User roadmap input names .NET 10 as the intended primary stack, but the repo currently targets .NET 9. Treat .NET 10 as a future upgrade task until project files are intentionally changed.

## Suggested Next Steps

1. Rename `PlaceholderTextExtractionService` to `PdfPigTextExtractionService`.
2. Add compiled bindings or typed view models for MAUI list templates.
3. Add a proper document detail route registration if Shell behavior needs refinement.
4. Add local OCR in a later phase only after privacy/security review.
5. Add encrypted or SQLite-backed storage in a later phase.
6. Add tests for `DocumentDetailViewModel`.
7. Plan a .NET 10 upgrade after confirming MAUI and dependency readiness.

## Most Relevant Files

- `README.md`
- `docs/project-setup.md`
- `docs/roadmap.md`
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
- `src/VeteranEvidenceAssist.App/Pages/SettingsPage.xaml`
- `src/VeteranEvidenceAssist.App/Pages/SettingsPage.xaml.cs`
- `src/VeteranEvidenceAssist.App/Services/AppearanceSettingsService.cs`
- `src/VeteranEvidenceAssist.App/Resources/Styles/Styles.xaml`
- `src/VeteranEvidenceAssist.App/ViewModels/DocumentDetailViewModel.cs`
- `tests/VeteranEvidenceAssist.Tests/DocumentImportTests.cs`
