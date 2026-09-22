# Phase Eight: Commercial operations modules

## Delivered

- Small Commercial Portal module with submission metrics, quote queue, stage filtering, and new-submission entry point.
- Underwriting Guidelines module with searchable rules, line-of-business filtering, versions, review dates, and current/review-due status.
- Shared navigation, responsive card/table patterns, and browser-local interaction state.

## Production integration boundary

The Small Commercial module should consume submission, quote, and bind APIs. Underwriting Guidelines should consume versioned guideline documents from the API and enforce broker visibility through the same Auth0 permission and broker-scope policies.

Recommended next endpoints:

```text
GET  /api/commercial/submissions?stage=&brokerId=
POST /api/commercial/submissions
GET  /api/underwriting/guidelines?line=&search=
GET  /api/underwriting/guidelines/{guidelineId}
```

The current module data is local seed data for workflow validation and will be replaced by authenticated API responses.
