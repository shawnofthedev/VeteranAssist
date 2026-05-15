# ADR-0002: Clean Architecture Solution Boundaries

## Status

Accepted

## Context

The project is a .NET 9 / .NET MAUI desktop application with multiple responsibilities: UI workflows, document parsing, metadata storage, hashing/security, redaction, prompt generation, and future AI provider integration.

Sensitive operations should be testable and isolated from UI concerns.

## Decision

VeteranEvidenceAssist will follow Clean Architecture-style project boundaries:

```text
src/VeteranEvidenceAssist.App         MAUI UI, navigation, workflow screens, view models
src/VeteranEvidenceAssist.Core        Domain models, enums, interfaces, value objects
src/VeteranEvidenceAssist.Documents   Local document import and PDF text extraction
src/VeteranEvidenceAssist.Redaction   PII/redaction abstractions and future implementation
src/VeteranEvidenceAssist.AI          Prompt generation abstractions and future providers
src/VeteranEvidenceAssist.Storage     Local metadata persistence and repositories
src/VeteranEvidenceAssist.Security    Hashing, encryption, secure storage helpers
tests/VeteranEvidenceAssist.Tests     Unit and workflow tests
```

Rules:

- Core must remain UI-independent.
- Document parsing must remain outside the App project.
- Security-sensitive operations should remain centralized and testable.
- AI provider implementations must remain swappable and user-controlled.
- The App project should orchestrate workflows through interfaces where practical.

## Consequences

Positive:

- Easier to test import, storage, hashing, and extraction without launching MAUI.
- Keeps high-risk features like redaction and AI transmission isolated.
- Makes future replacement of storage or PDF/OCR implementation easier.

Tradeoffs:

- More projects and interfaces than a small single-project app.
- Early-phase code may contain simple implementations behind abstractions.

## Notes

Some service names currently include `Placeholder` even though Phase 1 behavior is real. A future cleanup should rename those services without changing boundaries.
