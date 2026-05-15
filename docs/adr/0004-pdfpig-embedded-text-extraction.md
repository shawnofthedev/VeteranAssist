# ADR-0004: PdfPig for Embedded PDF Text Extraction

## Status

Accepted

## Context

Phase 1 requires basic text extraction from text-based PDFs without OCR and without cloud services. The application needs local PDF parsing that can read embedded text and page metadata.

Suggested package: UglyToad.PdfPig.

## Decision

Use PdfPig for local embedded PDF text extraction in Phase 1.

Current behavior:

- Open the local workspace copy of the PDF.
- Read pages with PdfPig.
- Preserve page number and page dimensions.
- Extract embedded text using PdfPig text extraction utilities.
- Store extracted text blocks with references to their source page.
- Do not OCR scanned PDFs.
- Do not upload any PDF content.

## Consequences

Positive:

- Keeps extraction local.
- Provides enough capability for text-based PDFs in Phase 1.
- Supports tests that generate text-based PDFs locally.
- Fits the Documents project boundary.

Tradeoffs:

- Text ordering may vary for complex PDFs.
- Scanned PDFs or image-only pages will not yield text.
- Coordinate-level extraction may need refinement for future redaction.

## Notes

OCR and advanced layout analysis should be evaluated separately in a future phase, with privacy and performance review.
