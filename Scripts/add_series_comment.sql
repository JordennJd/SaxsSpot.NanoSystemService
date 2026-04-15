-- Run once against the nanosystem service database.
ALTER TABLE nanosystem_series
    ADD COLUMN IF NOT EXISTS series_comment text;
