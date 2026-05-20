# ADR-0010: Tesseract as First Local OCR Engine

## Status

Accepted

## Context

ADR-0009 established the local OCR architecture but intentionally deferred selecting an OCR engine. The project now has a `LocalOcrService` skeleton and a swappable `ILocalOcrEngine`, but the default engine remains `NoOpLocalOcrEngine`.

The first concrete OCR engine must support the project's privacy-first requirements:

- Local/offline processing by default.
- No document uploads.
- No raw OCR text in logs.
- Testable failure handling.
- Clear packaging story for Windows-first MAUI.

Primary references reviewed:

- Tesseract official site: https://tesseractocr.org/
- Tesseract main repository: https://github.com/tesseract-ocr/tesseract
- Tesseract installation docs: https://tesseract-ocr.github.io/tessdoc/Installation.html
- Charles Weld .NET wrapper: https://github.com/charlesw/tesseract

## Decision

Use Tesseract as the first planned local OCR engine, but do not install an OCR package yet.

Tesseract is the preferred first engine because:

- It runs locally/offline.
- The core engine is licensed under Apache 2.0.
- It is mature, widely used, and actively documented.
- It supports common image inputs such as PNG, JPEG, and TIFF.
- It supports many languages through trained data files.
- It has a known .NET wrapper option.

Implementation should target the existing `ILocalOcrEngine` abstraction. The concrete Tesseract implementation should be added only after the PDF page rendering and packaging approach is decided.

The implementation path should be:

1. Add PDF page image rendering/conversion with tests for temporary file cleanup.
2. Decide how `tessdata` files are discovered, bundled, or user-configured.
3. Decide whether to use a .NET wrapper or invoke a local Tesseract executable.
4. Add the concrete Tesseract-backed `ILocalOcrEngine`.
5. Keep `NoOpLocalOcrEngine` available as the fallback until OCR is fully configured.

## Wrapper vs CLI Direction

The first implementation should prefer a .NET wrapper only if native binary loading, package maintenance, and Windows packaging are acceptable after a spike.

If wrapper packaging is brittle, prefer invoking a user-installed local Tesseract executable through a narrow adapter. That adapter must avoid logging document paths or OCR text and must sanitize errors before surfacing them.

## Consequences

Positive:

- Keeps OCR local and compatible with the project privacy model.
- Avoids cloud OCR services entirely for the first implementation.
- Keeps the concrete engine swappable through `ILocalOcrEngine`.
- Defers risky package installation until page rendering and packaging are clear.

Tradeoffs:

- Tesseract does not read PDFs directly; pages must be rendered to images first.
- OCR quality depends heavily on image quality and preprocessing.
- `tessdata` files must be packaged, configured, or located reliably.
- Native binaries or a local executable may complicate deployment.
- Handwriting and low-quality scans may remain unreliable.

## Follow-up Work

- Add a page image rendering service for OCR input.
- Add temporary file cleanup tests around rendered page images.
- Spike a Tesseract wrapper and CLI adapter before choosing the final integration style.
- Document `tessdata` storage and configuration.
- Add user-facing OCR unavailable/configuration status before exposing OCR actions.
