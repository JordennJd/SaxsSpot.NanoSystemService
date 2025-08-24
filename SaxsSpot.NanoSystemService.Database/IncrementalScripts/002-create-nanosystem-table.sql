--liquibase formatted sql

--changeset Jordenn:create-table-nanosystem
CREATE TABLE IF NOT EXISTS nanosystem (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    particle_kind INTEGER NOT NULL,
    series_id UUID NOT NULL,
    object_id UUID NOT NULL,
    user_id BIGINT NOT NULL,
    particle_count INTEGER NOT NULL,
    global_size DOUBLE PRECISION NOT NULL,
    generation_zone_form INTEGER NOT NULL,
    generation_zone_volume DOUBLE PRECISION NOT NULL,
    numerical_concentration DOUBLE PRECISION NOT NULL,
    max_particle_size REAL NOT NULL,
    min_particle_size REAL NOT NULL,
    excess DOUBLE PRECISION NOT NULL,
    k REAL NOT NULL,
    theta REAL NOT NULL,
    generation_start timestamp NOT NULL,
    generation_end timestamp NOT NULL,
    input_date timestamp NOT NULL
);
