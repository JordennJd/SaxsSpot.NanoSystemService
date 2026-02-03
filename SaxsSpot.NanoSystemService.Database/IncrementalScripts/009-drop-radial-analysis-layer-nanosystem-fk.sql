--liquibase formatted sql

--changeset saxs_spot:009-drop-radial-analysis-layer-nanosystem-fk
--comment Drop FK to nanosystem to avoid schema mismatch (nanosystem may be in public, layer table in saxs). Integrity preserved via radial_analysis_id -> radial_analysis -> nanosystem_id.
ALTER TABLE saxs.radial_analysis_layer
    DROP CONSTRAINT IF EXISTS fk_radial_analysis_layer_nanosystem;

--rollback ALTER TABLE saxs.radial_analysis_layer
--rollback     ADD CONSTRAINT fk_radial_analysis_layer_nanosystem FOREIGN KEY (nanosystem_id) REFERENCES saxs.nanosystem(id) ON DELETE CASCADE;
