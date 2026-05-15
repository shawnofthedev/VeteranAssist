# ADR-0001: Local-first Privacy Model

## Status

Accepted

## Context

VeteranEvidenceAssist handles highly sensitive military, VA, and medical records. These documents may contain Social Security numbers, VA file numbers, diagnoses, addresses, phone numbers, service records, and other protected personal information.

The app is intended to help users organize and review their own records. It is not a VA representative, legal service, medical diagnostic tool, claim filing system, or rating estimator.

## Decision

VeteranEvidenceAssist will use a local-first privacy model.

Core rules:

- Do not upload documents silently.
- Do not send document text to cloud AI services automatically.
- Do not add hidden telemetry containing document contents.
- Let users review any outgoing payload before it leaves the device.
- Prefer local processing for import, hashing, text extraction, PII detection, OCR, redaction, and evidence organization.
- Keep AI-generated content separate from extracted factual evidence.

## Consequences

Positive:

- Protects users from accidental disclosure of sensitive records.
- Keeps trust and user control central to the product.
- Makes future cloud integrations explicit and optional.

Tradeoffs:

- Some features may be slower or more complex because they must run locally.
- Cloud-based convenience features require additional review, consent UI, and security work.
- Contributors must treat all imported records as sensitive even in development.

## Notes

This ADR applies to all future phases. Later AI provider support must preserve explicit payload preview and user approval.
