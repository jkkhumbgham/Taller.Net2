CREATE TABLE IF NOT EXISTS users (
    id         BIGSERIAL PRIMARY KEY,
    name       VARCHAR(255) NOT NULL,
    email      VARCHAR(255) NOT NULL UNIQUE,
    password   VARCHAR(255) NOT NULL,
    created_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS enrollments (
    id          BIGSERIAL PRIMARY KEY,
    user_id     BIGINT NOT NULL REFERENCES users(id),
    course_id   BIGINT NOT NULL,
    enrolled_at TIMESTAMP,
    progress    DOUBLE PRECISION,
    UNIQUE (user_id, course_id)
);

CREATE TABLE IF NOT EXISTS lesson_progress (
    id               BIGSERIAL PRIMARY KEY,
    user_id          BIGINT NOT NULL REFERENCES users(id),
    lesson_id        BIGINT NOT NULL,
    status           VARCHAR(20) NOT NULL DEFAULT 'NOT_STARTED',
    progress_percent DOUBLE PRECISION,
    last_position    INTEGER,
    time_spent       INTEGER,
    completed_at     TIMESTAMP,
    updated_at       TIMESTAMP,
    UNIQUE (user_id, lesson_id)
);

CREATE TABLE IF NOT EXISTS quiz_attempts (
    id             BIGSERIAL PRIMARY KEY,
    user_id        BIGINT NOT NULL REFERENCES users(id),
    quiz_id        BIGINT NOT NULL,
    score          DOUBLE PRECISION,
    max_score      DOUBLE PRECISION,
    attempt_number INTEGER NOT NULL,
    time_spent     INTEGER,
    attempted_at   TIMESTAMP
);

CREATE TABLE IF NOT EXISTS question_attempts (
    id                 BIGSERIAL PRIMARY KEY,
    quiz_attempt_id    BIGINT NOT NULL REFERENCES quiz_attempts(id),
    question_id        BIGINT NOT NULL,
    selected_answer_id BIGINT,
    is_correct         BOOLEAN,
    time_spent         INTEGER
);
