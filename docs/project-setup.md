# VeteranEvidenceAssist Project Setup

## Solution creation commands

```powershell
dotnet new sln -n VeteranEvidenceAssist
dotnet new maui -n VeteranEvidenceAssist.App
dotnet new classlib -n VeteranEvidenceAssist.Core
dotnet new classlib -n VeteranEvidenceAssist.Documents
dotnet new classlib -n VeteranEvidenceAssist.Redaction
dotnet new classlib -n VeteranEvidenceAssist.AI
dotnet new classlib -n VeteranEvidenceAssist.Storage
dotnet new classlib -n VeteranEvidenceAssist.Security
dotnet new xunit -n VeteranEvidenceAssist.Tests

dotnet sln VeteranEvidenceAssist.sln add `
  VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj `
  VeteranEvidenceAssist.Core\VeteranEvidenceAssist.Core.csproj `
  VeteranEvidenceAssist.Documents\VeteranEvidenceAssist.Documents.csproj `
  VeteranEvidenceAssist.Redaction\VeteranEvidenceAssist.Redaction.csproj `
  VeteranEvidenceAssist.AI\VeteranEvidenceAssist.AI.csproj `
  VeteranEvidenceAssist.Storage\VeteranEvidenceAssist.Storage.csproj `
  VeteranEvidenceAssist.Security\VeteranEvidenceAssist.Security.csproj `
  VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

## Recommended folder structure

```text
VeteranEvidenceAssist.App/
  Pages/
  Resources/
  Platforms/
VeteranEvidenceAssist.Core/
  Models/
  Services/
VeteranEvidenceAssist.Documents/
  Services/
VeteranEvidenceAssist.Redaction/
  Services/
VeteranEvidenceAssist.AI/
  Services/
VeteranEvidenceAssist.Storage/
  Services/
VeteranEvidenceAssist.Security/
  Services/
VeteranEvidenceAssist.Tests/
docs/
```

## Later-phase TODOs

- Add local OCR and embedded PDF text extraction.
- Add tested PII detectors before enabling export workflows.
- Implement true PDF redaction that removes underlying content and flattens exported files.
- Encrypt sensitive persisted fields and use OS credential storage for secrets.
- Add migrations once the EF Core model stabilizes.
- Keep prompt building copy-first and review-first. Do not add silent cloud AI calls.
- Keep factual evidence separate from AI-generated interpretation.
