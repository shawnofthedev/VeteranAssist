# VeteranEvidenceAssist Project Setup

This document captures the current local development setup for the Windows-first .NET MAUI solution.

## Prerequisites

- .NET 9 SDK.
- .NET MAUI workload for Windows desktop development.
- Windows 10 19041 or later target support.
- Visual Studio with MAUI tooling is recommended for running/debugging the desktop app.

Check installed workloads:

```powershell
dotnet workload list
```

Install MAUI workload if needed:

```powershell
dotnet workload install maui
```

## Current Solution Layout

```powershell
VeteranEvidenceAssist.sln
src\VeteranEvidenceAssist.App
src\VeteranEvidenceAssist.Core
src\VeteranEvidenceAssist.Documents
src\VeteranEvidenceAssist.Redaction
src\VeteranEvidenceAssist.AI
src\VeteranEvidenceAssist.Storage
src\VeteranEvidenceAssist.Security
tests\VeteranEvidenceAssist.Tests
docs
```

## Restore, Build, and Test

Restore packages:

```powershell
dotnet restore VeteranEvidenceAssist.sln
```

Build the full solution:

```powershell
dotnet build VeteranEvidenceAssist.sln
```

Run unit tests:

```powershell
dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

Build the MAUI app project:

```powershell
dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj
```

## Current Development Phase

Current phase: Phase 0.5 UX/Wireframes with Phase 1 local import/text extraction groundwork.

Implemented at this stage:

- MAUI Shell navigation foundation.
- Privacy-first wireframe pages for the major workflows.
- Shared color, spacing, typography, card, badge, and callout styles.
- Local PDF import service groundwork.
- SHA-256 hashing for imported files.
- JSON metadata persistence.
- Embedded PDF text extraction when available.
- Tests for domain models and local import behavior.

Not implemented yet:

- OCR for scanned PDFs.
- Real PII detection.
- True permanent PDF redaction.
- Export generation.
- Cloud AI calls.
- Claim recommendation, legal advice, medical advice, or rating estimation.

## Workspace and Storage Notes

The app is designed to use local application storage. Imported files should be copied into a local workspace and originals should remain unchanged.

```text
data/
  imports/
  metadata/
  redacted/
  exports/
  temp/
```

Current metadata persistence is JSON-backed for early development. The architecture docs still identify SQLite/EF Core as the intended later storage direction once the model stabilizes.

## Solution Creation Reference

These were the original scaffold commands, adjusted for the current `src/` and `tests/` layout:

```powershell
dotnet new sln -n VeteranEvidenceAssist
dotnet new maui -n VeteranEvidenceAssist.App -o src\VeteranEvidenceAssist.App
dotnet new classlib -n VeteranEvidenceAssist.Core -o src\VeteranEvidenceAssist.Core
dotnet new classlib -n VeteranEvidenceAssist.Documents -o src\VeteranEvidenceAssist.Documents
dotnet new classlib -n VeteranEvidenceAssist.Redaction -o src\VeteranEvidenceAssist.Redaction
dotnet new classlib -n VeteranEvidenceAssist.AI -o src\VeteranEvidenceAssist.AI
dotnet new classlib -n VeteranEvidenceAssist.Storage -o src\VeteranEvidenceAssist.Storage
dotnet new classlib -n VeteranEvidenceAssist.Security -o src\VeteranEvidenceAssist.Security
dotnet new xunit -n VeteranEvidenceAssist.Tests -o tests\VeteranEvidenceAssist.Tests

dotnet sln VeteranEvidenceAssist.sln add `
  src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj `
  src\VeteranEvidenceAssist.Core\VeteranEvidenceAssist.Core.csproj `
  src\VeteranEvidenceAssist.Documents\VeteranEvidenceAssist.Documents.csproj `
  src\VeteranEvidenceAssist.Redaction\VeteranEvidenceAssist.Redaction.csproj `
  src\VeteranEvidenceAssist.AI\VeteranEvidenceAssist.AI.csproj `
  src\VeteranEvidenceAssist.Storage\VeteranEvidenceAssist.Storage.csproj `
  src\VeteranEvidenceAssist.Security\VeteranEvidenceAssist.Security.csproj `
  tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

## Later-phase TODOs

- Add local OCR for scanned PDFs.
- Add tested PII detectors before enabling export workflows.
- Implement true PDF redaction that removes underlying content and flattens exported files.
- Encrypt sensitive persisted fields and use OS credential storage for secrets.
- Add migrations once the EF Core model stabilizes.
- Keep prompt building copy-first and review-first. Do not add silent cloud AI calls.
- Keep factual evidence separate from AI-generated interpretation.
