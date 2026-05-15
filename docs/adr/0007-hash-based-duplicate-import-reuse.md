# ADR-0007: Hash-based Duplicate Import Reuse

## Status

Accepted

## Context

Phase 1 imports local PDFs into the app workspace, hashes them, extracts embedded text, and persists metadata locally. Users may accidentally import the same PDF more than once.

Duplicate records can create confusion, waste local storage, and duplicate sensitive extracted text in metadata. At the same time, original source paths should not be stored or used as the identity for a document because file paths may reveal sensitive local information and can change.

## Decision

Phase 1 duplicate detection uses the SHA-256 hash of the selected PDF.

Import behavior:

- Compute the selected PDF hash locally before copying.
- Search existing local metadata for a document with the same hash.
- If a matching document exists and its workspace copy still exists, return the existing document record.
- Do not create a second metadata record.
- Do not create a second workspace copy.
- If matching metadata exists but the workspace copy is missing, perform a new import and persist a new record.
- The repository exposes hash lookup through `IDocumentRepository.FindBySha256HashAsync` so duplicate detection does not depend on UI or storage implementation details.

## Consequences

Positive:

- Avoids duplicate metadata rows for the same file content.
- Avoids duplicate workspace copies for the same file content.
- Keeps duplicate detection independent of original file path.
- Keeps original files untouched.

Tradeoffs:

- Two different source files with identical bytes are treated as the same imported document.
- The UI currently reports duplicate handling in a general processed-files message rather than a per-file duplicate notice.
- Stale metadata with missing workspace files can still accumulate until a future cleanup workflow exists.

## Notes

Future UX may add a visible duplicate warning with a link to the existing document.
