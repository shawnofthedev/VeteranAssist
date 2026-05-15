# ADR-0008: Document Repository and Workspace Layout

## Status

Accepted

## Context

Phase 1 document intake needs a stable contract for listing, retrieving, saving, and deduplicating imported documents. The first implementation used `ILocalStorageService` directly, which mixed document repository behavior with broader local storage responsibilities.

The import workflow also needs predictable per-document storage so future OCR, redaction, and audit workflows can safely attach generated files to a document without modifying the original import copy.

## Decision

Introduce `IDocumentRepository` in the Core project for document metadata persistence.

The JSON storage implementation now backs both `IDocumentRepository` and the legacy `ILocalStorageService` contract during Phase 1.

Imported PDFs are stored under:

```text
AppData/Documents/{DocumentId}/original.pdf
```

Metadata persists the original filename, stored path, SHA-256 hash, UTC import timestamp, extraction status, OCR-required flag, extracted text preview, document type, redaction status placeholder, page metadata, and extracted text blocks.

## Consequences

Positive:

- Keeps document persistence testable and swappable.
- Gives duplicate detection a repository-level hash lookup.
- Creates a per-document workspace suitable for future OCR/redaction artifacts.
- Preserves original imported files unchanged.

Tradeoffs:

- JSON remains an interim Phase 1 persistence strategy.
- Existing metadata created with the older `imports/{guid}.pdf` layout is not migrated automatically yet.
- Future SQLite/encryption work should keep the repository contract stable where practical.
