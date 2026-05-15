# ADR-0003: Phase 1 JSON Metadata Storage

## Status

Accepted

## Context

The architecture documentation identifies SQLite/EF Core as the intended long-term local database direction. Phase 1, however, is focused on safe local document intake, hashing, embedded text extraction, and review workflow validation.

The data model is still evolving, especially around extraction status, OCR, redaction decisions, provenance, and audit metadata.

## Decision

For Phase 1, document metadata will be persisted locally as JSON using `JsonLocalStorageService`.

Current persisted metadata includes:

- Document ID
- Original file name
- Local workspace file path
- Document type
- SHA-256 hash
- Import timestamp
- Extraction status
- OCR-required flag
- Extracted text preview
- Redaction status placeholder
- Extracted text character count
- Page metadata
- Extracted text blocks

Imported files are copied into the local workspace under `AppData/Documents/{DocumentId}/original.pdf`.

Duplicate imports are detected by SHA-256 hash. If a matching metadata record exists and its workspace copy still exists, the existing record is reused instead of writing duplicate metadata.

## Consequences

Positive:

- Simple to inspect and test during early development.
- Avoids premature database schema/migration work while the model is changing.
- Keeps Phase 1 focused on safe local intake.

Tradeoffs:

- JSON metadata is not suitable as the final storage layer for larger datasets.
- It is not encrypted at rest yet.
- Concurrent writes are protected only by an in-process semaphore.
- Future migration to SQLite/EF Core will be needed.

## Notes

Future storage work should revisit encryption, database migrations, indexing, audit logs, and secure handling of sensitive fields.
