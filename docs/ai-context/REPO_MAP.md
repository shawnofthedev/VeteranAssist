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
  roadmap.md
  security.md
  ai-context/
  adr/
```

Docs:

- `docs/architecture.md`: Clean Architecture direction, project responsibilities, data flow, and non-goals.
- `docs/security.md`: threat model, storage/security expectations, logging rules, API key rules, and export security.
- `docs/redaction-spec.md`: redaction requirements, user review workflow, permanent removal requirement, and future validation needs.
- `docs/project-setup.md`: prerequisites, build/test commands, current phase, workspace notes, and scaffold reference.
- `docs/roadmap.md`: phase-by-phase product roadmap, current status, priorities, and guardrails for future work.

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
  0009-local-ocr-architecture.md
  0010-tesseract-as-first-local-ocr-engine.md
```

Use ADRs to understand why major decisions were made.

ADR-0009 records the local OCR architecture direction: OCR stays local by default, engine-specific code belongs in Documents, temporary page images must be cleaned up, and OCR text blocks should preserve page provenance, confidence, timestamps, and bounding boxes where available.

ADR-0010 selects Tesseract as the first planned local OCR engine while deferring package installation until PDF page rendering, wrapper-vs-CLI integration, and `tessdata` packaging/configuration are decided.

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
- `ViewModels/DocumentListItem.cs`
- `ViewModels/ImportResultItem.cs`
- `Services/AppearanceSettingsService.cs`

Settings:

- `SettingsPage.xaml` includes local workspace, privacy/security, appearance, AI provider, and export preference sections.
- Local workspace settings show the current app data root, document-copy directory, and JSON metadata path.
- Appearance supports Light, Dark, and System Default modes and is persisted locally with MAUI `Preferences`.
- `App.xaml.cs` applies the saved appearance mode on startup through `Application.Current.UserAppTheme`.

Notes:

- `AppShell.xaml.cs` owns route/query constants for document review navigation.
- `ImportDocumentsPage` still exists from earlier work, but the active Shell route uses `DocumentsPage`.
- CollectionView templates on active review/import pages use typed `x:DataType` bindings.
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
Services/LocalDocumentImportService.cs
Services/LocalOcrService.cs
Services/ILocalOcrEngine.cs
Services/NoOpLocalOcrEngine.cs
Services/PdfPigTextExtractionService.cs
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
- `DocumentsPage` pre-checks selected file hashes so the import summary can tell users which selections reused existing local records.
- `LocalOcrService` exists as an OCR orchestration skeleton behind `IOcrService`.
- `NoOpLocalOcrEngine` is the default engine placeholder; no real OCR dependency is installed.
- Tesseract is the first planned OCR engine, but no Tesseract package, binary, or trained data is installed yet.

Note:

`LocalDocumentImportService` is the active import service. `PdfPigTextExtractionService` performs real embedded PDF text extraction only; OCR remains deferred.

Future OCR direction:

- Implement the concrete OCR engine behind `ILocalOcrEngine`.
- Mark OCR-derived text as `TextExtractionMethod.LocalOcr`.
- Keep temporary page images in local temp storage and clean them after processing.
- Do not log raw OCR text or sensitive document contents.

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
- Local OCR service skeleton behavior and cancellation.

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
  -> DocumentsPage pre-checks hash for duplicate UX
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
  -> Shell navigates to document review through AppShell route/query constants
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

- Running MAUI app can lock build output DLLs.
- PDF page rendering is still a placeholder.
- OCR engine implementation is deferred; only the local OCR service skeleton exists.
- Redaction and export are placeholders.
- JSON metadata is not final storage.
- Remaining placeholder services are in later-phase Redaction, AI, and Security areas.

## Safe Places To Extend Next

- Add tests for `DocumentDetailViewModel`.
- Select a local OCR engine dependency only after licensing, platform, packaging, and privacy review.
- Add PDF page image rendering/conversion for OCR with temporary file cleanup tests.
