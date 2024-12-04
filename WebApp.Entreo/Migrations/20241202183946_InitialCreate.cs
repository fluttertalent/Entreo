using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Entreo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountabilityPartners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PartnerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ConnectedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanViewAll = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountabilityPartners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    PlainTextContent = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PreviewImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostalCode",
                columns: table => new
                {
                    PoststalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Township = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "PushNotificationTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastSuccessfulDelivery = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    DeviceTimeZone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotificationTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "int", maxLength: 16, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TranslationSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsTemplate = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    CueDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CravingDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ResponseDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RewardDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FrequencyType = table.Column<int>(type: "int", nullable: false),
                    FrequencyCount = table.Column<int>(type: "int", nullable: true),
                    WhyStatement = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    WhenTrigger = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WhereTrigger = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EnvironmentSetup = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ObstacleRemoval = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MinimalVersion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullVersion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    LastCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    TargetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionCount = table.Column<int>(type: "int", nullable: false),
                    AccountabilityPartnerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Habits_AccountabilityPartners_AccountabilityPartnerId",
                        column: x => x.AccountabilityPartnerId,
                        principalTable: "AccountabilityPartners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Habits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EnablePushNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableSmsNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableInAppNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableDailyDigest = table.Column<bool>(type: "bit", nullable: false),
                    EnableWeeklyReport = table.Column<bool>(type: "bit", nullable: false),
                    EnableMonthlyNewsletter = table.Column<bool>(type: "bit", nullable: false),
                    QuietHoursStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    QuietHoursEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    TimeZoneId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxNotificationsPerDay = table.Column<int>(type: "int", nullable: false),
                    MinTimeBetweenNotifications = table.Column<int>(type: "int", nullable: false),
                    PreferredNotificationTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    GroupSimilarNotifications = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotificationPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HabitCompletion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    HabitTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsSkipped = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitCompletion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitCompletion_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HabitLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Measurement = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MeasurementUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    HabitId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitLogs_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HabitLogs_Habits_HabitId1",
                        column: x => x.HabitId1,
                        principalTable: "Habits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HabitLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HabitReminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    NotificationTemplateId = table.Column<int>(type: "int", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    DaysOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitReminders_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HabitReminders_NotificationTemplates_NotificationTemplateId",
                        column: x => x.NotificationTemplateId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HabitReminders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HabitResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitResources_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabitStreaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    StreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreakStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LongestStreakEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllowSkipWeekends = table.Column<bool>(type: "bit", nullable: false),
                    AllowSkipHolidays = table.Column<bool>(type: "bit", nullable: false),
                    MaxSkipsAllowed = table.Column<int>(type: "int", nullable: false),
                    SkipsUsed = table.Column<int>(type: "int", nullable: false),
                    RecoveryDayCount = table.Column<int>(type: "int", nullable: false),
                    LastMissedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalCompletions = table.Column<int>(type: "int", nullable: false),
                    TotalDaysMissed = table.Column<int>(type: "int", nullable: false),
                    CompletionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeeklyTarget = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitStreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitStreaks_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabitTags",
                columns: table => new
                {
                    HabitsId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitTags", x => new { x.HabitsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_HabitTags_Habits_HabitsId",
                        column: x => x.HabitsId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HabitTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabitTriggers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    TriggerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TimeOfDay = table.Column<TimeSpan>(type: "time", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrecedingAction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmotionalState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SuccessRate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HabitTriggers_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationTemplateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    HabitId = table.Column<int>(type: "int", nullable: true),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SendPush = table.Column<bool>(type: "bit", nullable: false),
                    SendEmail = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSchedules_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationSchedules_NotificationTemplates_NotificationTemplateId",
                        column: x => x.NotificationTemplateId,
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationCategoryPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNotificationPreferenceId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationCategoryPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationCategoryPreferences_UserNotificationPreferences_UserNotificationPreferenceId",
                        column: x => x.UserNotificationPreferenceId,
                        principalTable: "UserNotificationPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationScheduleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_NotificationSchedules_NotificationScheduleId",
                        column: x => x.NotificationScheduleId,
                        principalTable: "NotificationSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HabitCompletion_HabitId",
                table: "HabitCompletion",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitLogs_HabitId",
                table: "HabitLogs",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitLogs_HabitId1",
                table: "HabitLogs",
                column: "HabitId1");

            migrationBuilder.CreateIndex(
                name: "IX_HabitLogs_UserId",
                table: "HabitLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitReminders_HabitId_UserId",
                table: "HabitReminders",
                columns: new[] { "HabitId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HabitReminders_NotificationTemplateId",
                table: "HabitReminders",
                column: "NotificationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitReminders_UserId",
                table: "HabitReminders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitResources_HabitId",
                table: "HabitResources",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_AccountabilityPartnerId",
                table: "Habits",
                column: "AccountabilityPartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId",
                table: "Habits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitStreaks_HabitId",
                table: "HabitStreaks",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitTags_TagsId",
                table: "HabitTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitTriggers_HabitId",
                table: "HabitTriggers",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationCategoryPreferences_UserNotificationPreferenceId",
                table: "NotificationCategoryPreferences",
                column: "UserNotificationPreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationScheduleId",
                table: "NotificationDeliveries",
                column: "NotificationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSchedules_HabitId",
                table: "NotificationSchedules",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSchedules_NotificationTemplateId",
                table: "NotificationSchedules",
                column: "NotificationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageCode_FieldName",
                table: "Translations",
                columns: new[] { "LanguageCode", "FieldName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationPreferences_UserId",
                table: "UserNotificationPreferences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HabitCompletion");

            migrationBuilder.DropTable(
                name: "HabitLogs");

            migrationBuilder.DropTable(
                name: "HabitReminders");

            migrationBuilder.DropTable(
                name: "HabitResources");

            migrationBuilder.DropTable(
                name: "HabitStreaks");

            migrationBuilder.DropTable(
                name: "HabitTags");

            migrationBuilder.DropTable(
                name: "HabitTriggers");

            migrationBuilder.DropTable(
                name: "NotificationCategoryPreferences");

            migrationBuilder.DropTable(
                name: "NotificationDeliveries");

            migrationBuilder.DropTable(
                name: "PostalCode");

            migrationBuilder.DropTable(
                name: "PushNotificationTokens");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "UserNotificationPreferences");

            migrationBuilder.DropTable(
                name: "NotificationSchedules");

            migrationBuilder.DropTable(
                name: "Habits");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "AccountabilityPartners");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
