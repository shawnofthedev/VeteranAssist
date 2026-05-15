# AGENTS.md

# VeteranEvidenceAssist - AI Contributor Guide

## Project Purpose

VeteranEvidenceAssist is a privacy-first desktop application designed to help veterans organize sensitive military and medical documents, redact personally identifiable information (PII), extract evidence timelines, and prepare AI-ready summaries/prompts.

The application is NOT:
- a VA representative
- a law firm
- a medical diagnostic tool
- an automated claim filing system
- a disability rating estimator

The application exists to help users organize and understand their own records while maintaining control of their data.

---

# Core Principles

## Privacy First
This project must prioritize local-first processing and user control.

Rules:
- Never upload documents silently.
- Never send document contents to cloud AI automatically.
- Always allow users to review outgoing payloads.
- Default to offline/local processing whenever possible.
- Treat all imported records as highly sensitive.

Examples of sensitive data:
- DD-214
- VA records
- Service treatment records
- Medical diagnoses
- Social Security Numbers
- VA file numbers
- Addresses
- Phone numbers

---

# Security Requirements

## Required
- Encrypt sensitive local storage where feasible.
- Avoid storing secrets in plaintext.
- Prefer secure OS credential/key stores.
- Log security-sensitive actions.
- Flatten exported redacted PDFs permanently.

## Never
- Fake redaction overlays.
- Store API keys in source control.
- Hardcode secrets.
- Send telemetry containing document contents.
- Add analytics that expose user medical data.

---

# Legal / Ethical Constraints

AI contributors must avoid implementing features that:
- provide legal representation
- imply VA accreditation
- guarantee claim outcomes
- estimate disability ratings
- automatically recommend fraudulent claims
- fabricate medical evidence
- generate fake nexus letters

Allowed:
- summarization
- organization
- timeline extraction
- prompt generation
- evidence categorization
- user-directed AI assistance

All AI-generated content should remain clearly separate from extracted factual evidence.

---

# Architectural Guidelines

## Preferred Architecture
- Clean Architecture
- SOLID principles
- Dependency Injection
- Interface-driven services
- Unit-testable business logic

## Solution Structure

```text
VeteranEvidenceAssist.App
VeteranEvidenceAssist.Core
VeteranEvidenceAssist.Documents
VeteranEvidenceAssist.Redaction
VeteranEvidenceAssist.AI
VeteranEvidenceAssist.Storage
VeteranEvidenceAssist.Security
VeteranEvidenceAssist.Tests
```

## Dependency Rules

- App layer depends on abstractions only.
- Core layer should avoid UI dependencies.
- Document parsing must be isolated.
- AI provider implementations must be swappable.
- Security logic should remain centralized.

---

# Coding Standards

## General
- Prefer readability over cleverness.
- Use async/await correctly.
- Avoid static mutable state.
- Favor composition over inheritance.
- Keep methods small and focused.

## Logging
- Never log raw document contents.
- Never log PII values directly.
- Sanitize exceptions where appropriate.

## Exceptions
- Fail safely.
- Surface user-friendly errors.
- Avoid swallowing exceptions silently.

---

# Data Extraction Rules

Extracted evidence should preserve provenance:
- source document
- page number
- extraction confidence
- extraction timestamp

AI-generated summaries must never overwrite original extracted evidence.

---

# Redaction Rules

Redaction is a high-risk feature.

Requirements:
- Users must review redactions before export.
- Exported PDFs must have data removed permanently.
- Redaction metadata should be auditable.
- Original files should remain untouched unless explicitly replaced.

---

# AI Integration Rules

## Allowed Providers
- Local LLMs
- OpenAI-compatible providers
- User-configured external APIs

## Requirements
- Show users exactly what text leaves the device.
- Require explicit user action before transmission.
- Separate prompts from raw evidence when possible.

## Never
- Automatically upload entire documents.
- Automatically send user medical records to cloud services.
- Claim AI outputs are medically or legally authoritative.

---

# UX Principles

The app should feel:
- trustworthy
- calm
- transparent
- privacy-focused
- professional

Avoid:
- aggressive automation
- dark patterns
- fear-based messaging
- exaggerated marketing language

---

# MVP Priorities

Priority order:
1. Stable document import
2. Reliable OCR
3. Safe PII detection
4. Secure redaction
5. Evidence organization
6. Prompt generation
7. AI integrations

---

# Testing Expectations

Required tests:
- PII detection
- Redaction validation
- PDF export integrity
- Evidence extraction
- Encryption utilities

Avoid merging untested security-sensitive code.

---

# Performance Goals

Target:
- Responsive local desktop experience
- Efficient processing of large PDFs
- Minimal memory spikes during OCR

---

# Future Possibilities

Potential future support:
- local vector search
- semantic evidence linking
- offline local LLM integration
- physician discussion packets
- VSO export packets

These features must still follow all privacy and legal constraints above.

---

# Contributor Expectations

AI contributors should:
- ask for clarification when requirements are ambiguous
- prefer safer implementations
- avoid overengineering early MVP phases
- maintain clear comments and TODOs
- document security-sensitive decisions

When uncertain:
prefer privacy, transparency, and user control.

---

# AI Continuity Rules

These rules keep future AI-assisted sessions grounded in engineering state rather than long conversation history.

## Before Starting New Work

Read:

- `AGENTS.md`
- `README.md`
- `docs/ai-context/SESSION_STATE.md`
- `docs/ai-context/REPO_MAP.md`
- relevant ADRs in `docs/adr/`

Also read the docs directly relevant to the requested work:

- `docs/architecture.md`
- `docs/security.md`
- `docs/redaction-spec.md`
- `docs/project-setup.md`

## When Completing Meaningful Work

Update or create:

- `docs/ai-context/SESSION_STATE.md`
- `docs/ai-context/REPO_MAP.md` when repository structure, workflows, or important files change
- `docs/adr/xxxx-title.md` when an architectural, privacy, storage, provider, redaction, or security decision is made

Use lowercase, zero-padded ADR filenames like:

```text
docs/adr/0007-example-title.md
```

## What To Capture

Do not create long conversation summaries. Capture engineering state only:

- what changed
- current branch/focus
- known issues
- next recommended tasks
- important decisions
- verification performed
- relevant files touched

## What Not To Capture

Do not include:

- raw document contents
- PII
- medical details from user records
- secrets or API keys
- long chat transcripts
- speculative product promises

When uncertain, preserve concise state that helps the next contributor continue safely.

