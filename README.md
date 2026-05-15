# VeteranEvidenceAssist

> This application is designed to assist veterans in organizing and reviewing their own records.
> It is NOT a VA-accredited representative, legal service, or medical diagnostic tool.

## Overview

VeteranEvidenceAssist is a privacy-first desktop application for organizing sensitive military and medical records, reviewing extracted document text, preparing redaction workflows, building evidence timelines, and creating user-reviewed prompt packets.

The project is Windows-first and built with .NET MAUI using a Clean Architecture-style solution split across app, core domain, documents, redaction, AI, storage, and security projects.

## Why This Exists

Veterans often need to review large sets of service, VA, and private medical records. This project exists to help users keep those records organized, trace facts back to source documents, and stay in control of what is copied, exported, or shared.

The app is intended to support personal record review and organization. It does not represent users, file claims, diagnose conditions, estimate ratings, or promise outcomes.

## Privacy First

VeteranEvidenceAssist is designed around local-first processing and explicit user control.

- Documents should be processed locally by default.
- Imported files should remain unchanged.
- Sensitive document content must not be uploaded silently.
- Prompt payloads must be previewed before anything leaves the device.
- AI-generated content must remain separate from extracted factual evidence.

Current UI messaging makes the core promise visible: nothing leaves this device without user review and approval.

## Current Status

Current development phase: Phase 1 Local Document Intake complete.

The application currently has a MAUI workflow for selecting local PDFs, copying them into the app workspace, hashing imported copies, detecting duplicate imports by hash, persisting metadata, extracting embedded PDF text when present, and reviewing document details locally.

PDFs with little or no embedded text are marked as OCR-needed for a future phase. OCR, true redaction, export generation, claim-specific recommendations, and cloud AI provider calls are not implemented.

## Features Implemented

- .NET 9 / .NET MAUI Windows desktop solution.
- Shell navigation for Dashboard, Documents, Review, Redactions, Evidence Timeline, Prompt Builder, Exports, and Settings.
- Lightweight design system with color tokens, spacing, typography, cards, badges, and callouts.
- Privacy-first workflow pages with clear local-control messaging.
- Documents page import workflow for local PDF selection.
- Split document review page with document metadata, extraction status, OCR-needed warning, and extracted text preview.
- Redaction review wireframe with mock PII suggestions, confidence indicators, approve/reject placeholders, and export safety warning.
- Prompt builder wireframe with selected evidence, template selector placeholder, payload preview, and copy prompt placeholder.
- Settings wireframe for local workspace, privacy/security, AI providers, and export preferences.
- Local PDF import service groundwork:
  - PDF path validation.
  - Copies imported files into local workspace.
  - SHA-256 file hashing.
  - JSON metadata persistence.
  - Embedded PDF text extraction with PdfPig.
  - OCR-needed status for PDFs with little or no embedded text.
  - Hash-based duplicate import reuse.
- Unit tests for file hashing, import validation, original-file immutability, duplicate import handling, metadata persistence, extraction status, no-text PDF detection, and core domain behavior.

## Planned Roadmap

Phase 0: Project scaffold and core domain contracts.

Phase 0.5: UX/wireframe foundation, navigation, design tokens, workflow clarity, and privacy messaging.

Phase 1: Stable local document import, PDF handling, file hashing, duplicate detection, metadata persistence, embedded PDF text extraction, OCR-needed detection, and document detail review. Complete.

Phase 2: Local OCR, safer extraction confidence handling, evidence review workflows, and source navigation.

Phase 3: PII detection, user-reviewed redaction decisions, audit metadata, and permanent flattened redacted PDF export.

Phase 4: Evidence timeline refinement, packet exports, prompt templates, and reviewed clipboard/export workflows.

Phase 5: Optional user-configured AI providers or local LLM integrations with explicit payload preview and approval.

## Architecture

The solution follows a local-first Clean Architecture direction:

```text
src/VeteranEvidenceAssist.App         MAUI UI, navigation, workflow screens
src/VeteranEvidenceAssist.Core        Domain models, enums, interfaces, value objects
src/VeteranEvidenceAssist.Documents   Local document import and PDF text extraction
src/VeteranEvidenceAssist.Redaction   PII/redaction abstractions and future implementation
src/VeteranEvidenceAssist.AI          Prompt generation abstractions and local prompt workflows
src/VeteranEvidenceAssist.Storage     Local metadata persistence and repositories
src/VeteranEvidenceAssist.Security    Hashing/encryption/security services
tests/VeteranEvidenceAssist.Tests     Unit and workflow tests
```

Key architectural rules:

- App layer depends on abstractions and UI-specific workflow code.
- Core stays UI- and storage-independent.
- Document parsing stays isolated in the Documents project.
- AI provider implementations must remain swappable and user-controlled.
- Security-sensitive storage and credential behavior should remain centralized.

## Screenshots

Screenshots are not committed yet.

Placeholder screens currently available in the running MAUI app:

- Dashboard privacy and workflow overview.
- Documents import queue and local workspace wireframe.
- Document Review split viewer/text panel wireframe.
- Redactions review and export safety warning wireframe.
- Evidence Timeline source-linked evidence wireframe.
- Prompt Builder payload preview wireframe.
- Exports and Settings wireframes.

TODO: Add sanitized screenshots from the mock-data wireframes once the visual layout stabilizes.

## Local Development

Prerequisites:

- .NET 9 SDK.
- .NET MAUI workload for Windows desktop development.
- Windows 10 19041 or later target support.

Restore and build:

```powershell
dotnet restore VeteranEvidenceAssist.sln
dotnet build VeteranEvidenceAssist.sln
```

Run tests:

```powershell
dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj
```

Run tests without rebuilding after a successful build:

```powershell
dotnet test tests\VeteranEvidenceAssist.Tests\VeteranEvidenceAssist.Tests.csproj --no-build
```

Build the app project directly:

```powershell
dotnet build src\VeteranEvidenceAssist.App\VeteranEvidenceAssist.App.csproj
```

Run the Windows MAUI app from Visual Studio or with the appropriate MAUI Windows launch profile.

## Security Philosophy

Security is a primary product requirement because the app handles highly sensitive records.

- Never log raw document text, SSNs, medical details, or prompt payloads.
- Never store API keys in source control.
- Prefer OS credential stores for secrets.
- Keep imported originals immutable.
- Require review before exports or external AI transmission.
- Redacted PDFs must permanently remove underlying content, not merely draw visual overlays.
- When uncertain, prefer privacy, transparency, and user control.

## Non-Goals / Disclaimer

VeteranEvidenceAssist is:

- Not a VA-accredited representative.
- Not legal advice.
- Not medical advice.
- Not a medical diagnostic tool.
- Not automatic claim filing.
- Not a disability rating estimator.
- Not a system for fabricating evidence, nexus letters, or claim narratives.

The app is intended to help users organize and review their own records. Users should consult qualified professionals for legal, medical, or VA representation questions.

## Contributing

Contributors should follow `AGENTS.md` and the docs in `docs/`.

Important contribution rules:

- Preserve local-first behavior.
- Do not add silent uploads or hidden telemetry.
- Keep AI-generated text separate from extracted source facts.
- Add tests for security-sensitive code.
- Document redaction, storage, and AI transmission decisions clearly.

## License

License not yet selected.

TODO: Add an explicit license before external distribution.
