--liquibase formatted sql

--changeset saxs_spot:012-add-nanosystem-disable-intersection-optimizations
--comment Per-nanosystem flag: generation used SAT-only placement (no intersection shortcuts)
ALTER TABLE nanosystem
    ADD COLUMN IF NOT EXISTS disable_intersection_optimizations BOOLEAN NOT NULL DEFAULT FALSE;

--rollback ALTER TABLE nanosystem DROP COLUMN IF EXISTS disable_intersection_optimizations;
