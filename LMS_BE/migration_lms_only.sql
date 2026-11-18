START TRANSACTION;
CREATE TABLE "Courses" (
    "Id" uuid NOT NULL,
    "CourseCode" text NOT NULL,
    "CourseName" text NOT NULL,
    "Description" text NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Courses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Courses_Users_CreatedById" FOREIGN KEY ("CreatedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Notifications" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Title" text NOT NULL,
    "Body" text NOT NULL,
    "IsRead" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Notifications_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Assessments" (
    "Id" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "Title" text NOT NULL,
    "AssessmentType" text NOT NULL,
    "MaxScore" numeric(6,2) NOT NULL,
    "DueAt" timestamp with time zone,
    CONSTRAINT "PK_Assessments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Assessments_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CourseClasses" (
    "Id" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "InstructorId" uuid NOT NULL,
    "Semester" text NOT NULL,
    "ClassName" text NOT NULL,
    "StartDate" date NOT NULL,
    "EndDate" date NOT NULL,
    CONSTRAINT "PK_CourseClasses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CourseClasses_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CourseClasses_Users_InstructorId" FOREIGN KEY ("InstructorId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "LearningPaths" (
    "Id" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "GeneratedAt" timestamp with time zone NOT NULL,
    "Status" text NOT NULL,
    "Explanation" text NOT NULL,
    CONSTRAINT "PK_LearningPaths" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_LearningPaths_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_LearningPaths_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Modules" (
    "Id" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "Title" text NOT NULL,
    "SequenceNo" integer NOT NULL,
    CONSTRAINT "PK_Modules" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Modules_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES "Courses" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Questions" (
    "Id" uuid NOT NULL,
    "AssessmentId" uuid NOT NULL,
    "QuestionText" text NOT NULL,
    "QuestionType" text NOT NULL,
    "Points" numeric(6,2) NOT NULL,
    CONSTRAINT "PK_Questions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Questions_Assessments_AssessmentId" FOREIGN KEY ("AssessmentId") REFERENCES "Assessments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Submissions" (
    "Id" uuid NOT NULL,
    "AssessmentId" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "Score" numeric(6,2),
    "SubmittedAt" timestamp with time zone,
    "Status" text NOT NULL,
    CONSTRAINT "PK_Submissions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Submissions_Assessments_AssessmentId" FOREIGN KEY ("AssessmentId") REFERENCES "Assessments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Submissions_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ClassSessions" (
    "Id" uuid NOT NULL,
    "ClassId" uuid NOT NULL,
    "Title" text NOT NULL,
    "StartsAt" timestamp with time zone NOT NULL,
    "EndsAt" timestamp with time zone NOT NULL,
    "Room" text NOT NULL,
    "SessionType" text NOT NULL,
    CONSTRAINT "PK_ClassSessions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ClassSessions_CourseClasses_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseClasses" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Enrollments" (
    "Id" uuid NOT NULL,
    "ClassId" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "EnrolledAt" timestamp with time zone NOT NULL,
    "Status" text NOT NULL,
    CONSTRAINT "PK_Enrollments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Enrollments_CourseClasses_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseClasses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Enrollments_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "RiskPredictions" (
    "Id" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "ClassId" uuid NOT NULL,
    "RiskScore" numeric(5,2) NOT NULL,
    "RiskLevel" text NOT NULL,
    "Explanation" text NOT NULL,
    "PredictedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_RiskPredictions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RiskPredictions_CourseClasses_ClassId" FOREIGN KEY ("ClassId") REFERENCES "CourseClasses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RiskPredictions_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Lessons" (
    "Id" uuid NOT NULL,
    "ModuleId" uuid NOT NULL,
    "Title" text NOT NULL,
    "ContentType" text NOT NULL,
    "ContentUrl" text NOT NULL,
    "Content" text NOT NULL,
    "SequenceNo" integer NOT NULL,
    "EstimatedMinutes" integer NOT NULL,
    CONSTRAINT "PK_Lessons" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Lessons_Modules_ModuleId" FOREIGN KEY ("ModuleId") REFERENCES "Modules" ("Id") ON DELETE CASCADE
);

CREATE TABLE "LearningActivities" (
    "Id" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "LessonId" uuid,
    "ActivityType" text NOT NULL,
    "DurationSeconds" integer NOT NULL,
    "OccurredAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_LearningActivities" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_LearningActivities_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES "Lessons" ("Id"),
    CONSTRAINT "FK_LearningActivities_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "LearningPathItems" (
    "Id" uuid NOT NULL,
    "PathId" uuid NOT NULL,
    "LessonId" uuid NOT NULL,
    "SequenceNo" integer NOT NULL,
    "PriorityScore" numeric(7,4) NOT NULL,
    "Status" text NOT NULL,
    CONSTRAINT "PK_LearningPathItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_LearningPathItems_LearningPaths_PathId" FOREIGN KEY ("PathId") REFERENCES "LearningPaths" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_LearningPathItems_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES "Lessons" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Assessments_CourseId" ON "Assessments" ("CourseId");

CREATE INDEX "IX_ClassSessions_ClassId" ON "ClassSessions" ("ClassId");

CREATE INDEX "IX_CourseClasses_CourseId" ON "CourseClasses" ("CourseId");

CREATE INDEX "IX_CourseClasses_InstructorId" ON "CourseClasses" ("InstructorId");

CREATE UNIQUE INDEX "IX_Courses_CourseCode" ON "Courses" ("CourseCode");

CREATE INDEX "IX_Courses_CreatedById" ON "Courses" ("CreatedById");

CREATE UNIQUE INDEX "IX_Enrollments_ClassId_StudentId" ON "Enrollments" ("ClassId", "StudentId");

CREATE INDEX "IX_Enrollments_StudentId" ON "Enrollments" ("StudentId");

CREATE INDEX "IX_LearningActivities_LessonId" ON "LearningActivities" ("LessonId");

CREATE INDEX "IX_LearningActivities_StudentId_OccurredAt" ON "LearningActivities" ("StudentId", "OccurredAt");

CREATE INDEX "IX_LearningPathItems_LessonId" ON "LearningPathItems" ("LessonId");

CREATE INDEX "IX_LearningPathItems_PathId" ON "LearningPathItems" ("PathId");

CREATE INDEX "IX_LearningPaths_CourseId" ON "LearningPaths" ("CourseId");

CREATE INDEX "IX_LearningPaths_StudentId" ON "LearningPaths" ("StudentId");

CREATE INDEX "IX_Lessons_ModuleId" ON "Lessons" ("ModuleId");

CREATE INDEX "IX_Modules_CourseId" ON "Modules" ("CourseId");

CREATE INDEX "IX_Notifications_UserId_CreatedAt" ON "Notifications" ("UserId", "CreatedAt");

CREATE INDEX "IX_Questions_AssessmentId" ON "Questions" ("AssessmentId");

CREATE INDEX "IX_RiskPredictions_ClassId" ON "RiskPredictions" ("ClassId");

CREATE INDEX "IX_RiskPredictions_StudentId" ON "RiskPredictions" ("StudentId");

CREATE UNIQUE INDEX "IX_Submissions_AssessmentId_StudentId" ON "Submissions" ("AssessmentId", "StudentId");

CREATE INDEX "IX_Submissions_StudentId" ON "Submissions" ("StudentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261118133125_AddLmsBusinessTables', '10.0.12');

COMMIT;

