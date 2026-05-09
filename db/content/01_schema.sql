CREATE TABLE IF NOT EXISTS courses (
    id          BIGSERIAL PRIMARY KEY,
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    level       VARCHAR(100),
    language    VARCHAR(50),
    is_published BOOLEAN NOT NULL DEFAULT FALSE,
    created_at  TIMESTAMP,
    updated_at  TIMESTAMP
);

CREATE TABLE IF NOT EXISTS modules (
    id         BIGSERIAL PRIMARY KEY,
    course_id  BIGINT NOT NULL REFERENCES courses(id),
    title      VARCHAR(255) NOT NULL,
    position   INTEGER NOT NULL,
    created_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS lessons (
    id         BIGSERIAL PRIMARY KEY,
    module_id  BIGINT NOT NULL REFERENCES modules(id),
    title      VARCHAR(255) NOT NULL,
    description TEXT,
    position   INTEGER NOT NULL,
    duration   INTEGER,
    is_preview BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS lesson_dependencies (
    id                   BIGSERIAL PRIMARY KEY,
    lesson_id            BIGINT NOT NULL REFERENCES lessons(id),
    depends_on_lesson_id BIGINT NOT NULL REFERENCES lessons(id)
);

CREATE TABLE IF NOT EXISTS contents (
    id         BIGSERIAL PRIMARY KEY,
    lesson_id  BIGINT NOT NULL REFERENCES lessons(id),
    type       VARCHAR(50) NOT NULL,
    position   INTEGER NOT NULL,
    purpose    VARCHAR(20) NOT NULL DEFAULT 'LESSON',
    created_at TIMESTAMP
);

CREATE TABLE IF NOT EXISTS video_contents (
    id         BIGSERIAL PRIMARY KEY,
    content_id BIGINT NOT NULL UNIQUE REFERENCES contents(id),
    url        VARCHAR(500) NOT NULL,
    duration   INTEGER,
    provider   VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS article_contents (
    id         BIGSERIAL PRIMARY KEY,
    content_id BIGINT NOT NULL UNIQUE REFERENCES contents(id),
    body       TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS file_contents (
    id         BIGSERIAL PRIMARY KEY,
    content_id BIGINT NOT NULL UNIQUE REFERENCES contents(id),
    file_url   VARCHAR(500) NOT NULL,
    file_type  VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS quizzes (
    id         BIGSERIAL PRIMARY KEY,
    content_id BIGINT NOT NULL UNIQUE REFERENCES contents(id),
    title      VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS questions (
    id            BIGSERIAL PRIMARY KEY,
    quiz_id       BIGINT NOT NULL REFERENCES quizzes(id),
    question_text TEXT NOT NULL,
    type          VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS answers (
    id          BIGSERIAL PRIMARY KEY,
    question_id BIGINT NOT NULL REFERENCES questions(id),
    answer_text TEXT NOT NULL,
    is_correct  BOOLEAN NOT NULL DEFAULT FALSE
);
