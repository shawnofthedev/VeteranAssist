# REPO_MAP

This map gives contributors and future chat sessions a quick orientation to the VeteranEvidenceAssist repository.

## Repository Root

```text
VeteranAssist/
  AGENTS.md
  README.md
  VeteranEvidenceAssist.sln
  docs/
  src/
  tests/
```

Key root files:

- `AGENTS.md`: contributor rules, privacy requirements, architecture expectations, redaction constraints, and legal/ethical boundaries.
- `README.md`: project overview, current status, roadmap, local development, security philosophy, and non-goals.
- `VeteranEvidenceAssist.sln`: .NET solution file.

## Documentation

```text
docs/
  architecture.md
  project-setup.md
  redaction-spec.md
  security.md
  ai-context/
  adr/
```

Docs:

- `docs/architecture.md`: Clean Architecture direction, project responsibilities, data flow, and non-goals.
- `docs/security.md`: threat model, storage/security expectations, logging rules, API key rules, and export security.
- `docs/redaction-spec.md`: redaction requirements, user review workflow, permanent removal requirement, and future validation needs.
- `docs/project-setup.md`: prerequisites, build/test commands, current phase, workspace notes, and scaffold reference.

AI continuity:

```text
docs/ai-context/
  SESSION_STATE.md
  REPO_MAP.md
```

- `docs/ai-context/SESSION_STATE.md`: current engineering state for handoff between chat windows.
- `docs/ai-context/REPO_MAP.md`: this file.

ADRs:

```text
docs/adr/
  README.md
  0001-local-first-privacy-model.md
  0002-clean-architecture-solution-boundaries.md
  0003-phase-1-json-metadata-storage.md
  0004-pdfpig-embedded-text-extraction.md
  0005-defer-ocr-until-after-local-intake.md
  0006-defer-redaction-and-export.md
  0007-hash-based-duplicate-import-reuse.md
  0008-document-repository-and-workspace-layout.md
```

Use ADRs to understand why major decisions were made.

## Source Projects

```text
src/
  VeteranEvidenceAssist.App/
  VeteranEvidenceAssist.Core/
  VeteranEvidenceAssist.Documents/
  VeteranEvidenceAssist.Redaction/
  VeteranEvidenceAssist.AI/
  VeteranEvidenceAssist.Storage/
  VeteranEvidenceAssist.Security/
```

## App Project

Path:

```text
src/VeteranEvidenceAssist.App/
```

Responsibility:

- .NET MAUI UI.
- Shell navigation.
- Pages and workflow orchestration.
- View models/projections for UI display.
- Dependency registration in `MauiProgram.cs`.

Important files:

```text
src/VeteranEvidenceAssist.App/
  App.xaml
  App.xaml.cs
  AppShell.xaml
  AppShell.xaml.cs
  MauiProgram.cs
  GlobalUsings.cs
  Pages/
  Resources/Styles/
  Services/
  ViewModels/
```

Pages:

```text
Pages/
  DashboardPage.xaml
  DocumentsPage.xaml
  DocumentsPage.xaml.cs
  DocumentReviewPage.xaml
  DocumentReviewPage.xaml.cs
  RedactionReviewPage.xaml
  EvidenceTimelinePage.xaml
  PromptBuilderPage.xaml
  ExportsPage.xaml
  SettingsPage.xaml
  ImportDocumentsPage.xaml
```

Current active document intake flow:

- `DocumentsPage.xaml`
- `DocumentsPage.xaml.cs`
- `DocumentReviewPage.xaml`
- `DocumentReviewPage.xaml.cs`
- `ViewModels/DocumentDetailViewModel.cs`
- `Services/AppearanceSettingsService.cs`

Settings:

- `SettingsPage.xaml` includes local workspace, privacy/security, appearance, AI provider, and export preference sections.
- Appearance supports Light, Dark, and System Default modes and is persisted locally with MAUI `Preferences`.
- `App.xaml.cs` applies the saved appearance mode on startup through `Application.Current.UserAppTheme`.

Notes:

- `ImportDocumentsPage` still exists from earlier work, but the active Shell route uses `DocumentsPage`.
- PDF rendering is still a placeholder.
- Do not put PDF parsing, OCR, redaction, or AI provider logic in the App project.

## Core Project

Path:

```text
src/VeteranEvidenceAssist.Core/
```

Responsibility:

- Domain models.
- Enums.
- Interfaces.
- Value objects.
- UI-independent business contracts.

Important folders:

```text
Models/
Enums/
Interfaces/
Options/
ValueObjects/
```

Important files:

```text
Models/VeteranDocument.cs
Models/DocumentPage.cs
Models/ExtractedTextBlock.cs
Models/PiiEntity.cs
Models/RedactionDecision.cs
Models/EvidenceItem.cs
Models/EvidenceTimeline.cs
Enums/DocumentExtractionStatus.cs
Enums/TextExtractionMethod.cs
Interfaces/IDocumentImportService.cs
Interfaces/ITextExtractionService.cs
Interfaces/ILocalStorageService.cs
Interfaces/IDocumentRepository.cs
Interfaces/IFileHashService.cs
Options/LocalWorkspaceOptions.cs
```

Current Phase 1 metadata:

- `VeteranDocument.ExtractionStatus`
- `VeteranDocument.ExtractedTextCharacterCount`
- `VeteranDocument.RequiresOcr`
- `VeteranDocument.ExtractedTextPreview`
- `VeteranDocument.RedactionStatus`
- `DocumentPage.PageNumber`
- `ExtractedTextBlock.DocumentPageId`
- `ExtractedTextBlock.ExtractionMethod`

## Documents Project

Path:

```text
src/VeteranEvidenceAssist.Documents/
```

Responsibility:

- Local document import.
- PDF parsing.
- Embedded text extraction.
- Future OCR orchestration.

Important files:

```text
Services/PlaceholderDocumentImportService.cs
Services/PlaceholderTextExtractionService.cs
VeteranEvidenceAssist.Documents.csproj
```

Current behavior:

- Validates PDF input.
- Copies file into local workspace under `Documents/{DocumentId}/original.pdf`.
- Uses `IFileHashService` for SHA-256 hashing.
- Uses PdfPig for embedded PDF text extraction.
- Creates page metadata and extracted text blocks.
- Marks text-based PDFs as `EmbeddedTextExtracted`.
- Marks no-text/scanned-like PDFs as `OcrNeeded`.
- Reuses an existing import through `IDocumentRepository.FindBySha256HashAsync` when a selected PDF has the same SHA-256 hash and the workspace copy still exists.
- Does not OCR.
- Does not upload anything.

Note:

`LocalDocumentImportService` is the active import service. `PlaceholderDocumentImportService` remains as a compatibility wrapper in the same file. `PlaceholderTextExtractionService` should be renamed in a future cleanup.

## Storage Project

Path:

```text
src/VeteranEvidenceAssist.Storage/
```

Responsibility:

- Local metadata persistence.
- Future SQLite/EF Core storage.
- Audit log repository implementations.

Important files:

```text
Repositories/JsonLocalStorageService.cs
Repositories/InMemoryLocalStorageService.cs
Repositories/InMemoryAuditLogService.cs
Data/LocalStorageOptions.cs
```

Current behavior:

- `JsonLocalStorageService` stores document metadata locally as JSON.
- Metadata includes document records, pages, extraction status, and text blocks.
- Evidence timeline persistence is not implemented beyond placeholder behavior.

Future direction:

- SQLite/EF Core once model stabilizes.
- Encryption for sensitive persisted fields where feasible.
- Better audit metadata.

## Security Project

Path:

```text
src/VeteranEvidenceAssist.Security/
```

Responsibility:

- Hashing.
- Encryption helpers.
- Future secure storage / OS credential integration.

Important files:

```text
Services/Sha256FileHashService.cs
Services/PlaceholderEncryptionService.cs
```

Current behavior:

- `Sha256FileHashService` computes SHA-256 locally.
- `PlaceholderEncryptionService` exists as a placeholder.

Rules:

- Do not store API keys in source control.
- Do not log secrets, raw document text, SSNs, or medical details.

## Redaction Project

Path:

```text
src/VeteranEvidenceAssist.Redaction/
```

Responsibility:

- Future PII detection.
- Future redaction review.
- Future permanent flattened redacted exports.

Important files:

```text
Services/PlaceholderPiiDetectionService.cs
Services/PlaceholderRedactionService.cs
```

Current behavior:

- Placeholder only.
- No real redaction/export.

Critical future rule:

- Visual overlays are not sufficient. Redaction must permanently remove underlying content.

## AI Project

Path:

```text
src/VeteranEvidenceAssist.AI/
```

Responsibility:

- Prompt generation abstractions.
- Future provider integration.
- Future payload previewing.

Important files:

```text
Services/LocalPromptGenerationService.cs
Services/PlaceholderEvidenceExtractionService.cs
```

Current behavior:

- Local placeholder/prompt-generation groundwork only.
- No cloud AI calls.

Rules:

- Never send document contents to AI providers automatically.
- Show the exact outgoing payload before user-approved transmission.

## Tests

Path:

```text
tests/VeteranEvidenceAssist.Tests/
```

Important files:

```text
DomainModelTests.cs
DocumentImportTests.cs
VeteranEvidenceAssist.Tests.csproj
```

Current coverage includes:

- Domain defaults.
- Source reference preservation.
- SHA-256 hashing.
- PDF import validation.
- Original-file immutability.
- Workspace copy behavior.
- Metadata persistence.
- Metadata round-trip with pages and text blocks.
- Embedded PDF text extraction.
- OCR-needed detection for no-text PDFs.
- Duplicate import behavior.
- Stale duplicate metadata with missing workspace copy.
- Corrupt PDF cleanup/no partial metadata persistence.

Run tests:

```powershell
dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

## Local Workspace

The app uses a local workspace rooted at the MAUI app data directory with a `data` folder.

Suggested structure:

```text
data/
  Documents/
  metadata/
  redacted/
  exports/
  temp/
```

Current Phase 1 usage:

- `AppData/Documents/{DocumentId}/original.pdf`: copied PDFs.
- `metadata/`: JSON document metadata.

Original files must remain unchanged.

## Main Workflows

### Import PDF

Entry point:

```text
DocumentsPage.xaml.cs
```

Flow:

```text
User chooses PDFs
  -> IDocumentImportService.ImportAsync
  -> Validate PDF
  -> Hash selected file locally
  -> Reuse existing record if hash already exists with a valid workspace copy
  -> Copy into AppData/Documents/{DocumentId}/original.pdf
  -> Hash copied file for persisted metadata
  -> Extract embedded text locally
  -> Determine extraction status
  -> Save metadata locally
  -> Refresh Documents list
```

### Review Document

Entry point:

```text
DocumentReviewPage.xaml.cs
```

Flow:

```text
User selects imported document
  -> Shell navigates to document-review with documentId
  -> Load document metadata from IDocumentRepository
  -> Project to DocumentDetailViewModel
  -> Show file details, hash, page count, extraction status, OCR warning, text preview
```

## Common Commands

Restore:

```powershell
dotnet restore VeteranEvidenceAssist.sln
```

Build full solution:

```powershell
dotnet build VeteranEvidenceAssist.sln
```

Run tests:

```powershell
dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

Build MAUI app to alternate output when the running app locks normal output DLLs:

```powershell
dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj --no-restore -p:OutputPath=bin\Debug\verify\
```

## Current Known Issues

- MAUI compiled-binding warnings remain for CollectionView templates.
- Running MAUI app can lock build output DLLs.
- PDF page rendering is still a placeholder.
- OCR is deferred.
- Redaction and export are placeholders.
- JSON metadata is not final storage.
- Placeholder service names should be cleaned up.

## Safe Places To Extend Next

- Add tests for `DocumentDetailViewModel`.
- Rename placeholder services to production names.
- Add duplicate-import warning UX.
- Add workspace path display.
- Add typed `x:DataType` bindings to reduce MAUI warnings.
- Add local OCR only after privacy/performance design review.
