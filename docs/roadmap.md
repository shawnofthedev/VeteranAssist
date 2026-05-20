# VeteranEvidenceAssist Roadmap

This roadmap captures the durable product direction for VeteranEvidenceAssist. It should be updated as milestones change, but implementation must continue to follow the privacy, security, and ethical constraints in `AGENTS.md`, `docs/architecture.md`, `docs/security.md`, and `docs/redaction-spec.md`.

## Product Vision

VeteranEvidenceAssist is a veteran-controlled, privacy-first assistant for organizing sensitive military, VA, and medical records. It helps users import records, detect and redact PII, extract structured evidence, build timelines, prepare reviewed prompt packets, and optionally use a chosen AI provider.

The application is not a VA representative, legal service, medical service, claim submission system, claim outcome guarantee, or disability rating estimator.

## Product Principles

- Privacy first: prefer local processing and require explicit consent before external transmission.
- User control: users review imports, redactions, prompt payloads, exports, and AI-bound text.
- Provider agnostic: future AI integrations should support local LLMs, OpenAI-compatible providers, and user-configured external APIs through swappable abstractions.
- Fact separation: extracted factual evidence must remain distinct from AI-generated suggestions or summaries.
- Provenance: extracted evidence should retain source document, page, confidence, and extraction timestamp where available.
- Auditability: redaction, export, and AI transmission workflows should leave useful audit trails without storing raw sensitive values.
- Ethical guardrails: the app may organize, summarize, and suggest topics to review, but must not diagnose, provide legal representation, fabricate evidence, guarantee outcomes, or estimate ratings.

## Current State

Current implemented milestone: Phase 1 Local Document Intake complete.

The repository currently targets .NET 9 for the Windows-first MAUI MVP. .NET 10 alignment is a future platform upgrade task once the toolchain and project dependencies are ready.

Implemented capabilities include MAUI Shell navigation, theme selection, local PDF import, workspace copying, SHA-256 hashing, duplicate import reuse, JSON metadata persistence, embedded PDF text extraction through PdfPig, OCR-needed status detection, local document review, and unit tests for the intake pipeline.

Local OCR architecture is documented in ADR-0009. ADR-0010 selects Tesseract as the first planned local OCR engine, but no OCR package has been installed yet. A local OCR service skeleton exists behind `ILocalOcrEngine`.

## Phase 0 - Foundation and Architecture

Status: mostly complete.

Goals:

- MAUI solution scaffolding.
- Clean Architecture project boundaries.
- MVVM-oriented app structure.
- Dependency injection.
- Configuration and logging foundations.
- ADR documentation.
- AI continuity files and repository map.
- README and setup documentation.

## Phase 0.5 - Core UI Shell

Status: in progress.

Goals:

- Navigation shell.
- Responsive desktop-first layouts.
- Light, dark, and system theme support.
- Settings infrastructure.
- Accessibility-focused visual foundation.
- Calm, readable, professional workflow screens.

Near-term work:

- Continue polishing settings and theme behavior.
- Display local workspace path and privacy status clearly.

## Phase 1 - Document Intake System

Status: local PDF intake complete; OCR and broader document formats remain future work.

Goals:

- Secure local ingestion of veteran records.
- File validation and workspace copying.
- Metadata extraction and persistence.
- File hashing and duplicate detection.
- Embedded PDF text extraction.
- Future OCR and image preprocessing.
- Future secure storage improvements.

Supported now:

- Local PDF import.
- Hash-based duplicate reuse.
- Embedded text extraction.
- OCR-needed marking for scanned-like PDFs.

Not implemented yet:

- Real OCR engine integration.
- Image import.
- Bulk folder monitoring.
- Encrypted database-backed storage.

## Phase 2 - PII Detection and Redaction

Status: planned.

Goals:

- Detect likely PII such as SSNs, dates of birth, addresses, phone numbers, emails, VA file numbers, service numbers, banking information, and dependent information.
- Provide confidence scoring and user review.
- Support manual approval and rejection.
- Export sanitized copies only after user approval.

Requirements:

- Never rely on fake overlay-only redaction.
- Permanently remove redacted data in exported PDFs.
- Keep originals unchanged unless explicitly replaced by the user.
- Preserve auditable redaction metadata without raw sensitive values.

## Phase 3 - Structured Data Extraction

Status: planned.

Goals:

- Transform records into structured, source-linked veteran data.
- Extract dates of service, branch, MOS/rate, deployments, awards, unit assignments, medical conditions, diagnoses, exposure indicators, and other user-reviewed evidence fields.
- Generate timelines and evidence tags.
- Preserve confidence and provenance.

Important boundary:

- AI-assisted parsing may help, but generated interpretation must remain separate from extracted factual evidence.

## Phase 4 - Claim Discovery Assistant

Status: planned.

Goals:

- Help users identify possible topics worth reviewing, such as missing evidence, relevant record types, or source-linked patterns.
- Provide evidence gap analysis and suggested discussion topics.

Ethical constraints:

- Suggestions must be labeled as possibilities only.
- The app must not diagnose, guarantee eligibility, imply representation, recommend fraud, or estimate ratings.

## Phase 5 - AI Prompt Builder

Status: planned, with early UI wireframe groundwork.

Goals:

- Build prompt packets from selected, user-reviewed evidence.
- Support templates for summaries, timelines, evidence review, and provider-specific formatting.
- Allow users to preview and edit all prompt text.
- Estimate tokens and costs where feasible.

Requirement:

- Prompt payloads must be visible before copying, exporting, or sending externally.

## Phase 6 - AI Integration Layer

Status: planned.

Goals:

- Provide swappable AI provider abstractions.
- Support future providers such as OpenAI-compatible APIs, Gemini, Claude, Ollama, and local models.
- Support streaming, retry handling, rate-limit handling, timeout handling, token accounting, and cost tracking.

Security requirements:

- Store API keys through secure OS credential storage where possible.
- Require explicit user approval before transmitting any document-derived content.

## Phase 7 - Evidence Workspace

Status: planned.

Goals:

- Centralize timelines, evidence tagging, notes, claim grouping, search, filtering, and evidence completeness tracking.
- Preserve source references for all evidence items.

Future possibilities:

- Smart evidence linking.
- Citation generation.
- Timeline visualization.

## Phase 8 - Report Generation

Status: planned.

Goals:

- Export organized summaries, evidence packets, condition summaries, medical timelines, and AI conversation exports.

Requirement:

- Reports that include AI assistance must clearly state that the content is AI-generated assistance only and should be independently verified.

## Phase 9 - Offline and Local AI Support

Status: planned.

Goals:

- Reduce cloud dependency through Ollama/local model integration, local embeddings, offline OCR, local vector search, CPU fallback, and optional GPU acceleration.

Requirement:

- Offline/local modes must preserve the same user-control and payload-review model as external AI modes.

## Phase 10 - Advanced Intelligence Features

Status: future possibility.

Potential features:

- Timeline anomaly detection.
- Record inconsistency detection.
- Smart evidence recommendations.
- Exposure pattern engine.
- Medical and VA terminology explanation.
- Local retrieval-augmented generation pipelines.

Boundary:

- These features must stay optional, reviewable, source-linked, and non-authoritative.

## Phase 11 - Mobile Polish and Release

Status: future possibility.

Goals:

- Performance optimization.
- Store assets.
- Accessibility validation.
- Offline resiliency.
- Backup and export workflows.
- Optional, privacy-preserving telemetry only after explicit opt-in.

## Development Priorities

Priority order:

1. Privacy.
2. Reliability.
3. Transparency.
4. User control.
5. AI flexibility.
6. UX polish.
7. Performance.

## Next Recommended Focus

1. Add PDF page image rendering/conversion for OCR with temporary file cleanup tests.
2. Spike Tesseract wrapper vs CLI integration before installing the final OCR dependency.
3. Document `tessdata` storage/configuration before enabling OCR actions.
4. Start PII detection/redaction only with tests and permanent-redaction validation strategy.
5. Keep AI provider work behind explicit preview and approval workflows.
