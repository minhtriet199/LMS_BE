START TRANSACTION;
CREATE TABLE IF NOT EXISTS "Courses" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "CourseCode" text NOT NULL UNIQUE, "CourseName" text NOT NULL, "Description" text NOT NULL DEFAULT '', "CreatedById" uuid NOT NULL, "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(), FOREIGN KEY ("CreatedById") REFERENCES "Users"("Id") ON DELETE RESTRICT);

CREATE TABLE IF NOT EXISTS "CourseClasses" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "CourseId" uuid NOT NULL, "InstructorId" uuid NOT NULL, "Semester" text NOT NULL, "ClassName" text NOT NULL, "StartDate" date NOT NULL, "EndDate" date NOT NULL, FOREIGN KEY ("CourseId") REFERENCES "Courses"("Id") ON DELETE CASCADE, FOREIGN KEY ("InstructorId") REFERENCES "Users"("Id") ON DELETE RESTRICT);

CREATE TABLE IF NOT EXISTS "Enrollments" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "ClassId" uuid NOT NULL, "StudentId" uuid NOT NULL, "EnrolledAt" timestamp with time zone NOT NULL DEFAULT now(), "Status" text NOT NULL DEFAULT 'Active', CONSTRAINT "UX_Enrollments_ClassId_StudentId" UNIQUE ("ClassId", "StudentId"), FOREIGN KEY ("ClassId") REFERENCES "CourseClasses"("Id") ON DELETE CASCADE, FOREIGN KEY ("StudentId") REFERENCES "Users"("Id") ON DELETE RESTRICT);

CREATE TABLE IF NOT EXISTS "Modules" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "CourseId" uuid NOT NULL, "Title" text NOT NULL, "SequenceNo" integer NOT NULL, FOREIGN KEY ("CourseId") REFERENCES "Courses"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "Lessons" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "ModuleId" uuid NOT NULL, "Title" text NOT NULL, "ContentType" text NOT NULL DEFAULT 'text', "ContentUrl" text NOT NULL DEFAULT '', "Content" text NOT NULL DEFAULT '', "SequenceNo" integer NOT NULL, "EstimatedMinutes" integer NOT NULL DEFAULT 30, FOREIGN KEY ("ModuleId") REFERENCES "Modules"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "Assessments" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "CourseId" uuid NOT NULL, "Title" text NOT NULL, "AssessmentType" text NOT NULL, "MaxScore" numeric(6,2) NOT NULL, "DueAt" timestamp with time zone, FOREIGN KEY ("CourseId") REFERENCES "Courses"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "Questions" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "AssessmentId" uuid NOT NULL, "QuestionText" text NOT NULL, "QuestionType" text NOT NULL, "Points" numeric(6,2) NOT NULL, FOREIGN KEY ("AssessmentId") REFERENCES "Assessments"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "Submissions" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "AssessmentId" uuid NOT NULL, "StudentId" uuid NOT NULL, "Score" numeric(6,2), "SubmittedAt" timestamp with time zone, "Status" text NOT NULL, CONSTRAINT "UX_Submissions_AssessmentId_StudentId" UNIQUE ("AssessmentId", "StudentId"), FOREIGN KEY ("AssessmentId") REFERENCES "Assessments"("Id") ON DELETE CASCADE, FOREIGN KEY ("StudentId") REFERENCES "Users"("Id") ON DELETE RESTRICT);

CREATE TABLE IF NOT EXISTS "LearningActivities" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "StudentId" uuid NOT NULL, "LessonId" uuid, "ActivityType" text NOT NULL, "DurationSeconds" integer NOT NULL, "OccurredAt" timestamp with time zone NOT NULL DEFAULT now(), FOREIGN KEY ("StudentId") REFERENCES "Users"("Id") ON DELETE CASCADE, FOREIGN KEY ("LessonId") REFERENCES "Lessons"("Id") ON DELETE SET NULL);

CREATE TABLE IF NOT EXISTS "LearningPaths" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "StudentId" uuid NOT NULL, "CourseId" uuid NOT NULL, "GeneratedAt" timestamp with time zone NOT NULL DEFAULT now(), "Status" text NOT NULL, "Explanation" text NOT NULL, FOREIGN KEY ("StudentId") REFERENCES "Users"("Id") ON DELETE CASCADE, FOREIGN KEY ("CourseId") REFERENCES "Courses"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "LearningPathItems" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "PathId" uuid NOT NULL, "LessonId" uuid NOT NULL, "SequenceNo" integer NOT NULL, "PriorityScore" numeric(7,4) NOT NULL, "Status" text NOT NULL, FOREIGN KEY ("PathId") REFERENCES "LearningPaths"("Id") ON DELETE CASCADE, FOREIGN KEY ("LessonId") REFERENCES "Lessons"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "RiskPredictions" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "StudentId" uuid NOT NULL, "ClassId" uuid NOT NULL, "RiskScore" numeric(5,2) NOT NULL, "RiskLevel" text NOT NULL, "Explanation" text NOT NULL, "PredictedAt" timestamp with time zone NOT NULL DEFAULT now(), FOREIGN KEY ("StudentId") REFERENCES "Users"("Id") ON DELETE CASCADE, FOREIGN KEY ("ClassId") REFERENCES "CourseClasses"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "ClassSessions" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "ClassId" uuid NOT NULL, "Title" text NOT NULL, "StartsAt" timestamp with time zone NOT NULL, "EndsAt" timestamp with time zone NOT NULL, "Room" text NOT NULL, "SessionType" text NOT NULL, FOREIGN KEY ("ClassId") REFERENCES "CourseClasses"("Id") ON DELETE CASCADE);

CREATE TABLE IF NOT EXISTS "Notifications" ("Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(), "UserId" uuid NOT NULL, "Title" text NOT NULL, "Body" text NOT NULL, "IsRead" boolean NOT NULL DEFAULT false, "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(), FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE);

CREATE INDEX IF NOT EXISTS "IX_CourseClasses_CourseId" ON "CourseClasses"("CourseId");

CREATE INDEX IF NOT EXISTS "IX_CourseClasses_InstructorId" ON "CourseClasses"("InstructorId");

CREATE INDEX IF NOT EXISTS "IX_Enrollments_StudentId" ON "Enrollments"("StudentId");

CREATE INDEX IF NOT EXISTS "IX_Modules_CourseId" ON "Modules"("CourseId");

CREATE INDEX IF NOT EXISTS "IX_Lessons_ModuleId" ON "Lessons"("ModuleId");

CREATE INDEX IF NOT EXISTS "IX_Assessments_CourseId" ON "Assessments"("CourseId");

CREATE INDEX IF NOT EXISTS "IX_Questions_AssessmentId" ON "Questions"("AssessmentId");

CREATE INDEX IF NOT EXISTS "IX_Submissions_StudentId" ON "Submissions"("StudentId");

CREATE INDEX IF NOT EXISTS "IX_LearningActivities_StudentId" ON "LearningActivities"("StudentId");

CREATE INDEX IF NOT EXISTS "IX_LearningActivities_LessonId" ON "LearningActivities"("LessonId");

CREATE INDEX IF NOT EXISTS "IX_LearningPaths_StudentId" ON "LearningPaths"("StudentId");

CREATE INDEX IF NOT EXISTS "IX_LearningPaths_CourseId" ON "LearningPaths"("CourseId");

CREATE INDEX IF NOT EXISTS "IX_LearningPathItems_PathId" ON "LearningPathItems"("PathId");

CREATE INDEX IF NOT EXISTS "IX_LearningPathItems_LessonId" ON "LearningPathItems"("LessonId");

CREATE INDEX IF NOT EXISTS "IX_RiskPredictions_StudentId" ON "RiskPredictions"("StudentId");

CREATE INDEX IF NOT EXISTS "IX_RiskPredictions_ClassId" ON "RiskPredictions"("ClassId");

CREATE INDEX IF NOT EXISTS "IX_ClassSessions_ClassId" ON "ClassSessions"("ClassId");

CREATE INDEX IF NOT EXISTS "IX_Notifications_UserId" ON "Notifications"("UserId");

CREATE INDEX IF NOT EXISTS "IX_LearningActivities_StudentId_OccurredAt" ON "LearningActivities"("StudentId","OccurredAt");

CREATE INDEX IF NOT EXISTS "IX_Notifications_UserId_CreatedAt" ON "Notifications"("UserId","CreatedAt");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261118000100_AddLmsBusinessTables', '10.0.12');

COMMIT;

