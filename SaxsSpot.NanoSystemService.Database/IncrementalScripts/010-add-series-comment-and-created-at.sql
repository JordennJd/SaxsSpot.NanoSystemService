--liquibase formatted sql

--changeset saxs_spot:010-add-series-comment-and-created-at
--comment Add missing nanosystem_series columns used by API filters and UI
ALTER TABLE nanosystem_series
    ADD COLUMN IF NOT EXISTS series_comment TEXT;

ALTER TABLE nanosystem_series
    ADD COLUMN IF NOT EXISTS created_at TIMESTAMP WITHOUT TIME ZONE;

UPDATE nanosystem_series
SET created_at = CURRENT_TIMESTAMP
WHERE created_at IS NULL;

ALTER TABLE nanosystem_series
    ALTER COLUMN created_at SET DEFAULT CURRENT_TIMESTAMP;

ALTER TABLE nanosystem_series
    ALTER COLUMN created_at SET NOT NULL;

--rollback ALTER TABLE nanosystem_series DROP COLUMN IF EXISTS series_comment;
--rollback ALTER TABLE nanosystem_series DROP COLUMN IF EXISTS created_at;
