# VeteranEvidenceAssist

VeteranEvidenceAssist is a privacy-first, local-first desktop app scaffold for organizing sensitive military and medical documents, reviewing likely PII, preparing redacted exports, building evidence timelines, and creating user-reviewed prompts.

The app is not a VA representative, law firm, medical diagnostic tool, claim filing system, rating estimator, or automatic claim recommendation engine.

## Phase 0 Scope

- .NET 9 / .NET MAUI solution structure under `src/` and `tests/`
- Core domain models, enums, value objects, and service interfaces
- Placeholder services for import, extraction, PII detection, redaction, evidence extraction, storage, prompt generation, and encryption
- Basic MAUI Shell navigation
- Documentation for architecture, security, and redaction constraints

## Build

```powershell
dotnet restore VeteranEvidenceAssist.sln
dotnet build VeteranEvidenceAssist.sln
dotnet build tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

TODO Phase 1: Replace placeholders with local document/OCR/storage implementations while preserving explicit user review and no silent cloud transmission.
