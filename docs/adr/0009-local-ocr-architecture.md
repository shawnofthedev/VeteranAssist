# ADR-0009: Local OCR Architecture

## Status

Accepted

## Context

Phase 1 imports PDFs, extracts embedded PDF text with PdfPig, and marks scanned-like PDFs as OCR-needed when little or no embedded text is found. The next phase needs OCR support for scanned PDFs and images without weakening the project's privacy model.

OCR will process highly sensitive military, VA, and medical records. It must preserve provenance for later PII detection, redaction review, evidence extraction, and prompt generation.

## Decision

OCR will be implemented as a local-first Documents-layer capability behind Core abstractions.

The Core project already owns the key contracts and output model:

- `ITextExtractionService` extracts text for a whole `VeteranDocument`.
- `IOcrService` extracts text for an individual `DocumentPage`.
- `ExtractedTextBlock` stores extracted text, extraction method, confidence, optional source range, bounding box JSON, and extraction timestamp.
- `TextExtractionMethod.LocalOcr` identifies OCR-derived text.

The Documents project will own OCR orchestration and engine-specific implementation details. The App project will only trigger and display OCR state through abstractions.

The first OCR implementation should:

- Run locally by default.
- Never upload document images or OCR text.
- Convert PDF pages to temporary local page images only when needed.
- Delete temporary page images after OCR completes or fails.
- Preserve source document ID, page number, confidence, extraction method, extraction timestamp, and bounding boxes where the engine provides them.
- Mark OCR failures safely without deleting the original imported PDF copy.
- Avoid logging raw OCR text, PII, medical details, or full file paths unless explicitly sanitized.

Dependency selection is deferred until implementation. Candidate engines may include Tesseract or another local OCR library, but the selected dependency must be reviewed for licensing, platform support, maintenance status, model/data-file requirements, and packaging impact.

## Consequences

Positive:

- Keeps OCR local and user-controlled.
- Preserves Clean Architecture boundaries.
- Allows the OCR engine to be swapped without changing App workflows.
- Produces provenance-rich text blocks for future redaction and evidence workflows.
- Avoids committing to an OCR dependency before reviewing platform and packaging tradeoffs.

Tradeoffs:

- OCR implementation remains deferred until a follow-up development task.
- Page image rendering/conversion will need a separate design and tests.
- OCR may require native binaries or trained-data files, which can complicate packaging.
- Confidence and bounding-box quality will vary by engine and document quality.

## Follow-up Work

- Add tests for OCR-needed document selection and OCR result persistence.
- Decide PDF page rendering/image conversion approach.
- Select and document the OCR engine dependency.
- Implement a local OCR service that returns `ExtractedTextBlock` values with `TextExtractionMethod.LocalOcr`.
- Add failure-path tests proving temporary files are cleaned up and originals remain unchanged.
