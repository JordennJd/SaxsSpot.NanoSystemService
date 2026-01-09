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
    inter_center_dist_more_diag_check_pos INTEGER NOT NULL DEFAULT 0,
    inter_center_dist_more_diag_check_total INTEGER NOT NULL DEFAULT 0,
    inter_center_dist_less_sides_check_pos INTEGER NOT NULL DEFAULT 0,
    inter_center_dist_less_sides_check_total INTEGER NOT NULL DEFAULT 0,
    elem_intersect_borders_new_transf_times_pos INTEGER NOT NULL DEFAULT 0,
    elem_intersect_borders_new_transf_times_total INTEGER NOT NULL DEFAULT 0,
    elem_intersect_borders_old_transf_times_pos INTEGER NOT NULL DEFAULT 0,
    elem_intersect_borders_old_transf_times_total INTEGER NOT NULL DEFAULT 0,
    back_rotate_matrix_reused INTEGER NOT NULL DEFAULT 0,
    sat_check_times_positive INTEGER NOT NULL DEFAULT 0,
    sat_check_times_total INTEGER NOT NULL DEFAULT 0,
    input_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_generation_metrics_nanosystem FOREIGN KEY (nanosystem_id) REFERENCES saxs.nanosystem(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_generation_metrics_nanosystem_id ON saxs.generation_metrics(nanosystem_id);

--rollback DROP TABLE IF EXISTS saxs.generation_metrics;
