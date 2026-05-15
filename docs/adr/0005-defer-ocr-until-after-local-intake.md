# ADR-0005: Defer OCR Until After Local Intake

## Status

Accepted

## Context

Many veteran records are scanned PDFs or image-heavy records. OCR will be important, but it introduces additional complexity:

- OCR engine selection.
- Local model/runtime dependencies.
- Accuracy and confidence handling.
- Performance and memory use.
- Potential handling of handwritten or low-quality scans.
- Privacy-sensitive caching of extracted text.

Phase 1 is focused on safe local intake and embedded text extraction.

## Decision

OCR is deferred until after Phase 1 local intake.

Current Phase 1 behavior:

- PDFs with meaningful embedded text are marked `EmbeddedTextExtracted`.
- PDFs with pages but little or no embedded text are marked `OcrNeeded`.
- No OCR engine is invoked.
- The UI shows an OCR-needed warning in document review.

## Consequences

Positive:

- Keeps Phase 1 smaller, safer, and testable.
- Makes scanned/no-text PDFs visible to users without pretending extraction succeeded.
- Avoids adding OCR dependencies before privacy/performance design is ready.

Tradeoffs:

- Scanned documents cannot yet be searched or reviewed as text.
- Users may need to wait for a later phase for full scanned-record support.

## Notes

Future OCR must remain local by default. Confidence and provenance must be persisted with OCR text.
