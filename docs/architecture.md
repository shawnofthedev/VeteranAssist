# docs/architecture.md

# VeteranEvidenceAssist Architecture

## Overview

VeteranEvidenceAssist is a privacy-first desktop application built to help veterans organize, redact, and analyze sensitive military and medical records locally.

The application follows:
- Clean Architecture
- SOLID principles
- Local-first processing
- Interface-driven design
- Explicit user-controlled AI interactions

Primary platform:
- Windows desktop via .NET MAUI

Future platforms:
- macOS
- Linux (possible later)
- mobile (not currently prioritized)

---

# High-Level Goals

## Core Capabilities

- Import military and medical documents
- Extract text from PDFs and images
- OCR scanned records
- Detect and redact PII
- Organize evidence chronologically
- Generate AI-ready prompt packets
- Preserve provenance and traceability

---

# Core Architectural Principles

## 1. Local First

All sensitive operations should execute locally whenever possible.

Examples:
- OCR
- PII detection
- redaction
- evidence extraction
- indexing

Cloud AI integrations must always require explicit user approval.

---

## 2. Explicit Data Flow

Users must understand:
- what is stored
- what is processed
- what leaves the device

No hidden uploads or background transmissions.

---

## 3. Separation of Evidence vs AI Output

Extracted evidence is factual source material.

AI-generated content is:
- interpretive
- potentially inaccurate
- non-authoritative

The system must never merge the two invisibly.

---

# Solution Structure

```text
src/VeteranEvidenceAssist.App
src/VeteranEvidenceAssist.Core
src/VeteranEvidenceAssist.Documents
src/VeteranEvidenceAssist.Redaction
src/VeteranEvidenceAssist.AI
src/VeteranEvidenceAssist.Storage
src/VeteranEvidenceAssist.Security
tests/VeteranEvidenceAssist.Tests
```

---

# Project Responsibilities

## VeteranEvidenceAssist.App

Responsibilities:
- MAUI UI
- navigation
- view models
- dependency registration
- user workflows

Should NOT contain:
- OCR logic
- business rules
- PDF parsing
- AI provider implementations

---

## VeteranEvidenceAssist.Core

Responsibilities:
- domain models
- interfaces
- enums
- shared abstractions
- business contracts

Must remain:
- UI independent
- storage independent
- provider independent

---

## VeteranEvidenceAssist.Documents

Responsibilities:
- PDF parsing
- OCR orchestration
- image conversion
- document metadata extraction
- text coordinate extraction

Potential libraries:
- PdfPig
- Tesseract
- ImageSharp

OCR direction:

- OCR must run locally by default.
- OCR engine details stay in the Documents project behind Core abstractions.
- Temporary page images must be cleaned up after OCR completes or fails.
- OCR-derived text blocks must use `TextExtractionMethod.LocalOcr` and preserve page provenance, confidence, timestamps, and bounding boxes where available.

---

## VeteranEvidenceAssist.Redaction

Responsibilities:
- PII detection
- coordinate mapping
- redaction overlays
- permanent PDF flattening
- redaction audit data

---

## VeteranEvidenceAssist.AI

Responsibilities:
- prompt generation
- provider abstractions
- payload previewing
- AI provider implementations

Must support:
- OpenAI-compatible APIs
- local LLM endpoints
- future providers

---

## VeteranEvidenceAssist.Storage

Responsibilities:
- SQLite
- EF Core
- repositories
- import indexing
- audit logs

---

## VeteranEvidenceAssist.Security

Responsibilities:
- encryption helpers
- secure storage
- API key protection
- hashing utilities
- sensitive memory handling

---

## VeteranEvidenceAssist.Tests

Responsibilities:
- unit tests
- integration tests
- redaction validation
- OCR validation
- export validation

---

# Data Flow

## Import Workflow

```text
Document Import
    ->
File Validation
    ->
Embedded Text Extraction
    ->
OCR when needed
    ->
PII Detection
    ->
User Review
    ->
Evidence Extraction
    ->
Storage
    ->
Prompt Generation
```

---

# Domain Model Overview

## VeteranDocument

Represents an imported document.

Key metadata:
- source path
- import date
- document type
- hash
- OCR status

---

## DocumentPage

Represents a single page of a document.

Contains:
- page number
- dimensions
- extracted text blocks

---

## ExtractedTextBlock

Represents positioned text.

Contains:
- text
- coordinates
- confidence
- source page
- extraction method
- extraction timestamp

---

## PiiEntity

Represents detected sensitive information.

Contains:
- entity type
- confidence
- location
- suggested redaction bounds

---

## EvidenceItem

Represents extracted claim-relevant evidence.

Contains:
- condition
- event date
- summary
- source references

---

## PromptPacket

Represents AI-ready exportable prompts.

Contains:
- prompt template
- included evidence
- destination provider
- preview text

---

# Storage Design

## Database
SQLite via EF Core.

## File Strategy

Imported files should remain immutable.

Suggested structure:

```text
/data
    /Documents
    /redacted
    /exports
    /temp
```

Current imported PDF copies use `AppData/Documents/{DocumentId}/original.pdf`. Future OCR page images should use temporary local storage and be removed after processing.

---

# Security Design

## Encryption

Potential encrypted data:
- API keys
- cached prompts
- local embeddings
- audit metadata

Use OS-level secure storage where possible.

---

## Logging

Never log:
- raw OCR output
- SSNs
- medical details
- prompt payloads

---

# AI Provider Architecture

## Provider Interface

All providers should implement:
- capability discovery
- token estimation
- payload previewing
- prompt submission

---

## Supported Modes

### Local Only
No internet access.

### External AI
User-approved payload transmission.

---

# Future Architecture Possibilities

Potential future additions:
- local vector database
- semantic search
- embeddings
- RAG pipelines
- local LLM integration
- offline inference

These features must remain optional and privacy-first.

---

# Non-Goals

The application should NOT:
- auto-file VA claims
- estimate disability percentages
- provide legal advice
- impersonate accredited representatives
- fabricate evidence
- generate fake medical documents

---

# Design Philosophy

The software should prioritize:
- trust
- transparency
- reliability
- explainability
- user agency

When tradeoffs occur:
prefer privacy and clarity over automation.
