--liquibase formatted sql

--changeset Jordenn:create-table-scattering-calculation
CREATE TABLE IF NOT EXISTS scattering_calculation (
    id UUID PRIMARY KEY,
    nanosystem_id UUID NOT NULL,
    object_id UUID NOT NULL,
    calculation_kind INTEGER NOT NULL,
    q_vector_from DOUBLE PRECISION NOT NULL,
    q_vector_to DOUBLE PRECISION NOT NULL,
    q_space_method INTEGER NOT NULL,
    q_scale_method INTEGER NOT NULL,
    q_space_parameter DOUBLE PRECISION NOT NULL,
    excess DOUBLE PRECISION,
    input_date TIMESTAMPTZ NOT NULL,
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_scattering_calculation_nanosystem_id
    ON scattering_calculation (nanosystem_id);
