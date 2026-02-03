--liquibase formatted sql

--changeset saxs_spot:008-create-radial-analysis-layer-table
--comment Table for storing radial analysis layer data (moved from MinIO object storage)
CREATE TABLE IF NOT EXISTS saxs.radial_analysis_layer (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    radial_analysis_id UUID NOT NULL,
    nanosystem_id UUID NOT NULL,
    layer_index INTEGER NOT NULL,
    layer_from DOUBLE PRECISION NOT NULL,
    layer_to DOUBLE PRECISION NOT NULL,
    numerical_concentration DOUBLE PRECISION NOT NULL,
    point_count INTEGER NOT NULL,
    CONSTRAINT fk_radial_analysis_layer_radial_analysis FOREIGN KEY (radial_analysis_id) REFERENCES saxs.radial_analysis(id) ON DELETE CASCADE,
    -- nanosystem_id not FK: nanosystem may live in public schema; integrity via radial_analysis.nanosystem_id
    CONSTRAINT chk_radial_analysis_layer_point_count CHECK (point_count >= 0),
    CONSTRAINT chk_radial_analysis_layer_layer_index CHECK (layer_index >= 0),
    CONSTRAINT chk_radial_analysis_layer_concentration CHECK (numerical_concentration >= 0),
    CONSTRAINT uk_radial_analysis_layer_analysis_index UNIQUE (radial_analysis_id, layer_index)
);

CREATE INDEX IF NOT EXISTS idx_radial_analysis_layer_radial_analysis_id ON saxs.radial_analysis_layer(radial_analysis_id);
CREATE INDEX IF NOT EXISTS idx_radial_analysis_layer_nanosystem_id ON saxs.radial_analysis_layer(nanosystem_id);
CREATE INDEX IF NOT EXISTS idx_radial_analysis_layer_layer_index ON saxs.radial_analysis_layer(layer_index);

--rollback DROP INDEX IF EXISTS saxs.idx_radial_analysis_layer_layer_index;
--rollback DROP INDEX IF EXISTS saxs.idx_radial_analysis_layer_nanosystem_id;
--rollback DROP INDEX IF EXISTS saxs.idx_radial_analysis_layer_radial_analysis_id;
--rollback DROP TABLE IF EXISTS saxs.radial_analysis_layer;
