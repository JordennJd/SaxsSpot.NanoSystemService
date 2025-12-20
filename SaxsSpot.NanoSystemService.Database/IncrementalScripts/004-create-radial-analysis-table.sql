--liquibase formatted sql

--changeset Jordenn:create-table-radial-analysis
CREATE TABLE IF NOT EXISTS radial_analysis (
     id UUID PRIMARY KEY,
     nanosystem_id UUID NOT NULL,
     object_id UUID NOT NULL,
     layer_count INTEGER NOT NULL,
     point_count DOUBLE PRECISION NOT NULL
);
