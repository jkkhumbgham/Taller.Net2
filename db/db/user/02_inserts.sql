-- =========================
-- USERS
-- =========================
INSERT INTO users (id, name, email, password, created_at) VALUES
(1,'Juan','juan@test.com','hashed_pass',NOW()),
(2,'Maria','maria@test.com','hashed_pass',NOW());

-- =========================
-- ENROLLMENTS
-- =========================
INSERT INTO enrollments (id, user_id, course_id, enrolled_at, progress) VALUES
(1,1,1,NOW(),20),
(2,2,1,NOW(),50);

-- =========================
-- LESSON PROGRESS
-- =========================
INSERT INTO lesson_progress (id, user_id, lesson_id, status, progress_percent, last_position, time_spent, completed_at, updated_at) VALUES
(1,1,1,'COMPLETED',100,600,600,NOW(),NOW()),
(2,1,2,'IN_PROGRESS',40,200,300,NULL,NOW());

-- =========================
-- QUIZ ATTEMPTS
-- =========================
INSERT INTO quiz_attempts (id, user_id, quiz_id, score, max_score, attempt_number, time_spent, attempted_at) VALUES
(1,1,1,80,100,1,120,NOW());

-- =========================
-- QUESTION ATTEMPTS
-- =========================
INSERT INTO question_attempts (id, quiz_attempt_id, question_id, selected_answer_id, is_correct, time_spent) VALUES
(1,1,1,1,true,20);

-- =========================
-- RESET SEQUENCES
-- =========================
SELECT setval(pg_get_serial_sequence('users',             'id'), (SELECT MAX(id) FROM users));
SELECT setval(pg_get_serial_sequence('enrollments',       'id'), (SELECT MAX(id) FROM enrollments));
SELECT setval(pg_get_serial_sequence('lesson_progress',   'id'), (SELECT MAX(id) FROM lesson_progress));
SELECT setval(pg_get_serial_sequence('quiz_attempts',     'id'), (SELECT MAX(id) FROM quiz_attempts));
SELECT setval(pg_get_serial_sequence('question_attempts', 'id'), (SELECT MAX(id) FROM question_attempts));
