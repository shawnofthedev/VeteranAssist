# docs/security.md

# VeteranEvidenceAssist Security Guidelines

## Overview

VeteranEvidenceAssist processes highly sensitive military and medical records.

Security is a primary feature of the application, not an afterthought.

This document defines baseline security expectations for all contributors.

---

# Threat Model

Potential sensitive information includes:
- Social Security Numbers
- VA file numbers
- service records
- medical diagnoses
- addresses
- phone numbers
- emails
- military history

Potential risks:
- accidental cloud uploads
- insecure local storage
- leaked logs
- incomplete redaction
- malicious plugins
- prompt leakage
- insecure exports

---

# Core Security Principles

## 1. Local First

Sensitive processing should remain local whenever feasible.

Examples:
- OCR
- PII detection
- evidence extraction
- indexing
- redaction

---

## 2. Explicit Consent

Users must knowingly approve:
- AI payload transmission
- exports
- cloud integrations

No silent background uploads.

---

## 3. Least Privilege

Components should only access:
- the data they require
- the services they need

---

# Data Classification

## Highly Sensitive
- DD-214
- medical records
- VA correspondence
- SSNs
- service numbers

## Sensitive
- prompts
- extracted timelines
- provider settings

## Low Sensitivity
- UI preferences
- non-identifying settings

---

# Storage Security

## Database

SQLite databases should:
- support encryption where feasible
- avoid storing raw secrets
- minimize duplicated sensitive text

---

## File Storage

Imported documents:
- should remain immutable
- should not be modified in-place

Temporary files:
- should be cleaned automatically
- should avoid persistent storage when possible

---

# API Keys

## Requirements

API keys:
- must never be committed
- must never appear in logs
- should use secure OS credential storage

Preferred:
- Windows Credential Manager
- macOS Keychain

---

# Logging Rules

## Never Log
- raw OCR text
- SSNs
- prompt payloads
- medical details
- extracted evidence contents

## Allowed Logging
- operation status
- performance metrics
- sanitized exceptions

---

# Redaction Security

## Critical Requirement

Visual overlays alone are NOT acceptable.

Redacted exports must:
- permanently remove underlying text
- flatten the output
- prevent text recovery

---

# AI Provider Security

## Requirements

Before sending data externally:
- show payload preview
- show provider name
- require user approval

---

## Never
- auto-send entire documents
- transmit hidden metadata silently
- retain prompts unnecessarily

---

# Memory Handling

Where feasible:
- avoid long-lived plaintext sensitive data
- dispose streams promptly
- avoid unnecessary caching

---

# Dependency Management

## Rules

Avoid:
- abandoned libraries
- unclear licensing
- unknown PDF manipulation packages

Dependencies should:
- be reviewed periodically
- be updated responsibly
- be pinned where appropriate

---

# Plugin Security

Future plugin systems must:
- isolate untrusted code
- limit filesystem access
- prevent arbitrary document transmission

---

# Secure Development Practices

Contributors should:
- validate inputs
- sanitize paths
- avoid insecure deserialization
- prefer parameterized queries
- avoid reflection-heavy unsafe behavior

---

# Export Security

Generated exports:
- should clearly indicate redacted status
- should not include hidden OCR layers unintentionally
- should preserve auditability

---

# Audit Logging

Security-sensitive events may include:
- exports
- AI transmissions
- redaction actions
- deletions

Audit logs should avoid storing sensitive payload contents.

---

# Future Security Enhancements

Potential future improvements:
- encrypted workspace mode
- password-protected projects
- local-only AI mode
- secure evidence vault
- hardware-backed key storage

---

# Security Philosophy

When uncertain:
- prefer transparency
- prefer local processing
- prefer user approval
- prefer safer defaults

Security decisions should prioritize protecting veterans and their data above convenience.