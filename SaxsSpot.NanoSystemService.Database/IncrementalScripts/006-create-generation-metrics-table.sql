--liquibase formatted sql

--changeset saxs_spot:006-create-generation-metrics-table
CREATE TABLE IF NOT EXISTS saxs.generation_metrics (
    id UUID PRIMARY KEY,
    nanosystem_id UUID NOT NULL,
    total_attempts INTEGER NOT NULL DEFAULT 0,
    positive_attempts INTEGER NOT NULL DEFAULT 0,
    total_change_position_attempts INTEGER NOT NULL DEFAULT 0,
    tree_build_time_ms BIGINT NOT NULL DEFAULT 0,
    generation_time_ms BIGINT NOT NULL DEFAULT 0,
    first_node_intersection_find_times INTEGER NOT NULL DEFAULT 0,
    total_neighbors_nodes_checked_count INTEGER NOT NULL DEFAULT 0,
    is_inter_center_distance_more_then_diagonal_check_times_positive INTEGER NOT NULL DEFAULT 0,
    is_inter_center_distance_more_then_diagonal_check_times_total INTEGER NOT NULL DEFAULT 0,
    is_inter_center_distance_less_then_sides_check_times_positive INTEGER NOT NULL DEFAULT 0,
    is_inter_center_distance_less_then_sides_check_times_total INTEGER NOT NULL DEFAULT 0,
    elementary_intersect_check_only_borders_new_transformation_times_positive INTEGER NOT NULL DEFAULT 0,
    elementary_intersect_check_only_borders_new_transformation_times_total INTEGER NOT NULL DEFAULT 0,
    elementary_intersect_check_only_borders_old_transformation_times_positive INTEGER NOT NULL DEFAULT 0,
    elementary_intersect_check_only_borders_old_transformation_times_total INTEGER NOT NULL DEFAULT 0,
    back_rotate_matrix_reused INTEGER NOT NULL DEFAULT 0,
    sat_check_times_positive INTEGER NOT NULL DEFAULT 0,
    sat_check_times_total INTEGER NOT NULL DEFAULT 0,
    input_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_generation_metrics_nanosystem FOREIGN KEY (nanosystem_id) REFERENCES saxs.nanosystem(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_generation_metrics_nanosystem_id ON saxs.generation_metrics(nanosystem_id);

--rollback DROP TABLE IF EXISTS saxs.generation_metrics;
