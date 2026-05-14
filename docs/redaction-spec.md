# docs/redaction-spec.md

# VeteranEvidenceAssist Redaction Specification

## Overview

This document defines the redaction requirements and expectations for VeteranEvidenceAssist.

Redaction is considered a high-risk feature because incomplete redaction can expose sensitive military and medical information.

---

# Redaction Goals

The system must:
- detect likely PII
- assist user review
- permanently remove redacted data
- preserve document readability
- maintain auditability

The system must NOT:
- silently alter originals
- rely solely on visual overlays
- imply perfect detection accuracy

---

# Supported PII Types

Initial MVP targets:

- Social Security Numbers
- dates of birth
- phone numbers
- addresses
- emails
- VA file numbers
- service numbers

Future support may include:
- dependent names
- physician identifiers
- signatures
- barcode regions

---

# Detection Pipeline

## Layer 1 - Pattern Detection

Examples:
- regex
- format matching
- keyword adjacency

---

## Layer 2 - Contextual Detection

Future enhancement:
- local NLP/NER models
- confidence scoring
- contextual classification

---

# Coordinate Preservation

All extracted text should preserve:
- page number
- bounding rectangle
- OCR confidence
- extraction source

This enables precise redaction placement.

---

# User Review Workflow

Required flow:

```text
Import
    ->
Detect PII
    ->
Highlight Suggestions
    ->
User Review
    ->
Approve / Reject
    ->
Export Redacted Copy
```

---

# Redaction Rendering Requirements

## Critical Requirement

Redaction must permanently remove underlying text.

Not acceptable:
- black rectangles only
- annotation overlays only
- hidden layers preserving text

---

# PDF Export Requirements

Redacted exports should:
- flatten content
- remove hidden OCR text where appropriate
- avoid retaining recoverable metadata
- preserve pagination

---

# Original File Policy

Original imports:
- remain unchanged
- remain immutable
- should never be overwritten automatically

---

# Audit Requirements

Redaction actions should record:
- timestamp
- user action
- document ID
- redaction type
- export action

Audit logs should NOT store raw sensitive values.

---

# Confidence Scoring

PII detections should include:
- confidence score
- detection source
- suggested category

Users must be able to override suggestions.

---

# UX Requirements

The UI should:
- clearly distinguish suggested vs approved redactions
- allow zooming and inspection
- avoid clutter
- provide clear warnings before export

---

# OCR Considerations

Scanned records may contain:
- skewed text
- handwriting
- low resolution
- artifacts

The system should:
- gracefully handle uncertainty
- expose confidence levels
- avoid pretending extraction is perfect

---

# Future Enhancements

Potential future improvements:
- image region redaction
- handwriting detection
- signature detection
- table-aware redaction
- local ML-based PII detection

---

# Failure Handling

If the system is uncertain:
- warn the user
- avoid automatic removal
- preserve manual review opportunities

---

# Testing Requirements

Required validation tests:
- hidden text removal
- metadata cleanup
- OCR layer verification
- export integrity
- coordinate accuracy

---

# Redaction Philosophy

Redaction should prioritize:
- safety
- permanence
- transparency
- user verification

When uncertain:
prefer over-warning rather than silent failure.
