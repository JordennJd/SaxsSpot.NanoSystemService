--liquibase formatted sql

--changeset saxs_spot:013-create-run-generation-inbox-table
--comment Inbox table for exactly-once-like processing of RunGenerationRequest
CREATE TABLE IF NOT EXISTS run_generation_inbox
(
    id UUID PRIMARY KEY,
    operation_id UUID NOT NULL UNIQUE,
    series_id UUID NOT NULL,
    payload TEXT NOT NULL,
    status INTEGER NOT NULL,
    attempts INTEGER NOT NULL DEFAULT 0,
    last_error TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL,
    processing_started_at TIMESTAMPTZ NULL,
    processed_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_run_generation_inbox_status_created_at
    ON run_generation_inbox (status, created_at);

--rollback DROP TABLE IF EXISTS run_generation_inbox;
