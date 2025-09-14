--liquibase formatted sql

--changeset Jordenn:create-table-nanosystem-series
CREATE TABLE IF NOT EXISTS nanosystem_series (
     id UUID PRIMARY KEY,
     particle_kind INTEGER NOT NULL,
     particle_count_from INTEGER NOT NULL,
     particle_count_to INTEGER NOT NULL,
     global_size_from DOUBLE PRECISION NOT NULL,
     global_size_to DOUBLE PRECISION NOT NULL,
     numerical_concentration_from DOUBLE PRECISION NOT NULL,
     numerical_concentration_to DOUBLE PRECISION NOT NULL,
     excess_from DOUBLE PRECISION,
     excess_to DOUBLE PRECISION,
     max_particle_size_from REAL NOT NULL,
     max_particle_size_to REAL NOT NULL,
     min_particle_size_from REAL NOT NULL,
     min_particle_size_to REAL NOT NULL,
     k_from REAL NOT NULL,
     k_to REAL NOT NULL,
     theta_from REAL NOT NULL,
     theta_to REAL NOT NULL
);
