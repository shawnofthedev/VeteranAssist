# ADR-0006: Defer Redaction and Export Implementation

## Status

Accepted

## Context

Redaction is a high-risk feature. Incomplete redaction can expose sensitive military, medical, and personal information. Visual overlays alone are not acceptable because hidden PDF text may remain recoverable.

Export workflows also carry risk because they produce files users may share externally.

## Decision

Real redaction and export generation are deferred until after local intake, extraction, and review workflows are stable.

Current behavior:

- Redaction pages are UI placeholders.
- Export pages are UI placeholders.
- No redacted PDFs are generated.
- No export files are generated.
- Original imported files are never modified.

Future redaction requirements:

- Users must review and approve redactions.
- Redacted PDFs must permanently remove underlying content.
- Redacted PDFs must be flattened.
- Redaction actions should be auditable.
- Audit logs must avoid storing raw sensitive values.

## Consequences

Positive:

- Avoids shipping unsafe redaction behavior.
- Keeps current work focused on reliable local intake and provenance.
- Makes future redaction implementation easier to test against clear requirements.

Tradeoffs:

- Users cannot yet produce redacted copies.
- Export workflows remain non-functional placeholders.

## Notes

Before implementing redaction, add tests for hidden text removal, metadata cleanup, OCR layer handling, export integrity, and coordinate accuracy.
