--liquibase formatted sql

--changeset saxs_spot:011-add-series-disable-intersection-optimizations
--comment Store whether series generation used SAT-only intersection placement
ALTER TABLE nanosystem_series
    ADD COLUMN IF NOT EXISTS disable_intersection_optimizations BOOLEAN NOT NULL DEFAULT FALSE;

--rollback ALTER TABLE nanosystem_series DROP COLUMN IF EXISTS disable_intersection_optimizations;
