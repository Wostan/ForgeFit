using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ForgeFit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfile_Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserProfile_AvatarUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserProfile_DateOfBirth_Value = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserProfile_Gender = table.Column<int>(type: "int", nullable: false),
                    WeightValue = table.Column<double>(type: "float", nullable: false),
                    WeightUnit = table.Column<int>(type: "int", nullable: false),
                    HeightValue = table.Column<double>(type: "float", nullable: false),
                    HeightUnit = table.Column<int>(type: "int", nullable: false),
                    Email_Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_Users_HeightCheck", "HeightValue > 0");
                    table.CheckConstraint("CK_Users_HeightUnitCheck", "HeightUnit IN (1, 2)");
                    table.CheckConstraint("CK_Users_UserProfile_GenderCheck", "UserProfile_Gender IN (1, 2)");
                    table.CheckConstraint("CK_Users_WeightCheck", "WeightValue > 0");
                    table.CheckConstraint("CK_Users_WeightUnitCheck", "WeightUnit IN (1, 2)");
                });

            migrationBuilder.CreateTable(
                name: "BodyGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WeightGoal_Value = table.Column<double>(type: "float", nullable: false),
                    WeightGoal_Unit = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GoalType = table.Column<int>(type: "int", nullable: false),
                    GoalStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyGoals", x => x.Id);
                    table.CheckConstraint("CK_BodyGoals_GoalStatusCheck", "GoalStatus IN (1, 2, 3)");
                    table.CheckConstraint("CK_BodyGoals_GoalTypeCheck", "GoalType IN (1, 2, 3, 4)");
                    table.CheckConstraint("CK_BodyGoals_WeightGoal_UnitCheck", "WeightGoal_Unit IN (1, 2)");
                    table.CheckConstraint("CK_BodyGoals_WeightGoal_ValueCheck", "WeightGoal_Value > 0");
                    table.ForeignKey(
                        name: "FK_BodyGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrinkEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VolumeMl = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkEntries", x => x.Id);
                    table.CheckConstraint("CK_DrinkEntries_VolumeMlCheck", "VolumeMl > 0");
                    table.ForeignKey(
                        name: "FK_DrinkEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Calories = table.Column<double>(type: "float", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    DayTime = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodEntries", x => x.Id);
                    table.CheckConstraint("CK_FoodEntries_CaloriesCheck", "Calories > 0");
                    table.CheckConstraint("CK_FoodEntries_CarbsCheck", "Carbs > 0");
                    table.CheckConstraint("CK_FoodEntries_DayTimeCheck", "DayTime IN (1, 2, 3, 4)");
                    table.CheckConstraint("CK_FoodEntries_FatCheck", "Fat > 0");
                    table.CheckConstraint("CK_FoodEntries_ProteinCheck", "Protein > 0");
                    table.ForeignKey(
                        name: "FK_FoodEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Frequency_Interval = table.Column<int>(type: "int", nullable: false),
                    Frequency_FrequencyUnit = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.CheckConstraint("CK_Notifications_Frequency_FrequencyUnitCheck", "Frequency_FrequencyUnit IN (1, 2, 3)");
                    table.CheckConstraint("CK_Notifications_Frequency_IntervalCheck", "Frequency_Interval > 0");
                    table.CheckConstraint("CK_Notifications_NotificationTypeCheck", "NotificationType IN (1, 2, 3, 4, 5)");
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NutritionGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyNutritionPlan_TargetCalories = table.Column<int>(type: "int", nullable: false),
                    DailyNutritionPlan_Carbs = table.Column<int>(type: "int", nullable: false),
                    DailyNutritionPlan_Protein = table.Column<int>(type: "int", nullable: false),
                    DailyNutritionPlan_Fat = table.Column<int>(type: "int", nullable: false),
                    DailyNutritionPlan_WaterMl = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionGoals", x => x.Id);
                    table.CheckConstraint("CK_NutritionGoals_DailyNutritionPlan_CaloriesCheck", "DailyNutritionPlan_TargetCalories > 1000");
                    table.CheckConstraint("CK_NutritionGoals_DailyNutritionPlan_CarbsCheck", "DailyNutritionPlan_Carbs > 0");
                    table.CheckConstraint("CK_NutritionGoals_DailyNutritionPlan_FatCheck", "DailyNutritionPlan_Fat > 0");
                    table.CheckConstraint("CK_NutritionGoals_DailyNutritionPlan_ProteinCheck", "DailyNutritionPlan_Protein > 0");
                    table.CheckConstraint("CK_NutritionGoals_DailyNutritionPlan_WaterGoalMlCheck", "DailyNutritionPlan_WaterMl > 1000");
                    table.ForeignKey(
                        name: "FK_NutritionGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutPlan_WorkoutsPerWeek = table.Column<int>(type: "int", nullable: false),
                    WorkoutPlan_Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    WorkoutPlan_WorkoutType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutGoals", x => x.Id);
                    table.CheckConstraint("CK_WorkoutGoals_WorkoutPlan_DurationCheck", "WorkoutPlan_Duration BETWEEN '00:10:00' AND '05:00:00'");
                    table.CheckConstraint("CK_WorkoutGoals_WorkoutPlan_WorkoutsPerWeekCheck", "WorkoutPlan_WorkoutsPerWeek > 0 AND WorkoutPlan_WorkoutsPerWeek < 8");
                    table.CheckConstraint("CK_WorkoutGoals_WorkoutPlan_WorkoutTypeCheck", "WorkoutPlan_WorkoutType IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_WorkoutGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutPrograms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Calories = table.Column<double>(type: "float", nullable: false),
                    Carbs = table.Column<double>(type: "float", nullable: false),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    ServingUnit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "g"),
                    Amount = table.Column<double>(type: "float", nullable: false, defaultValue: 100.0),
                    FoodEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.Id);
                    table.CheckConstraint("CK_FoodItems_AmountCheck", "Amount > 0");
                    table.CheckConstraint("CK_FoodItems_CaloriesCheck", "Calories > 0");
                    table.CheckConstraint("CK_FoodItems_CarbsCheck", "Carbs > 0");
                    table.CheckConstraint("CK_FoodItems_FatCheck", "Fat > 0");
                    table.CheckConstraint("CK_FoodItems_ProteinCheck", "Protein > 0");
                    table.ForeignKey(
                        name: "FK_FoodItems_FoodEntries_FoodEntryId",
                        column: x => x.FoodEntryId,
                        principalTable: "FoodEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutSchedule_Start = table.Column<TimeOnly>(type: "time", nullable: false),
                    WorkoutSchedule_End = table.Column<TimeOnly>(type: "time", nullable: false),
                    WorkoutSchedule_Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutEntries", x => x.Id);
                    table.CheckConstraint("CK_WorkoutEntries_WorkoutSchedule_DurationCheck", "WorkoutSchedule_Duration BETWEEN '00:10:00' AND '05:00:00'");
                    table.ForeignKey(
                        name: "FK_WorkoutEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutEntries_WorkoutPrograms_WorkoutProgramId",
                        column: x => x.WorkoutProgramId,
                        principalTable: "WorkoutPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercisePlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutExercise_ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkoutExercise_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WorkoutExercise_GifUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkoutExercise_TargetMuscles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkoutExercise_BodyParts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkoutExercise_Equipment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkoutExercise_SecondaryMuscles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkoutExercise_Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercisePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExercisePlans_WorkoutPrograms_WorkoutProgramId",
                        column: x => x.WorkoutProgramId,
                        principalTable: "WorkoutPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformedExercises",
                columns: table => new
                {
                    WorkoutEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Snapshot_ExternalId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Snapshot_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Snapshot_GifUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Snapshot_TargetMuscles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Snapshot_BodyParts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Snapshot_Equipment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Snapshot_SecondaryMuscles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Snapshot_Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformedExercises", x => new { x.WorkoutEntryId, x.Id });
                    table.ForeignKey(
                        name: "FK_PerformedExercises_WorkoutEntries_WorkoutEntryId",
                        column: x => x.WorkoutEntryId,
                        principalTable: "WorkoutEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    RestTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    WeightValue = table.Column<double>(type: "float", nullable: false),
                    WeightUnit = table.Column<int>(type: "int", nullable: false),
                    WorkoutExercisePlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSets", x => x.Id);
                    table.CheckConstraint("CK_WorkoutSet_OrderCheck", "[Order] > 0");
                    table.CheckConstraint("CK_WorkoutSet_RepsCheck", "Reps > 0");
                    table.CheckConstraint("CK_WorkoutSets_WeightCheck", "WeightValue > 0");
                    table.CheckConstraint("CK_WorkoutSets_WeightUnitCheck", "WeightUnit IN (1, 2)");
                    table.ForeignKey(
                        name: "FK_WorkoutSets_WorkoutExercisePlans_WorkoutExercisePlanId",
                        column: x => x.WorkoutExercisePlanId,
                        principalTable: "WorkoutExercisePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformedSets",
                columns: table => new
                {
                    WorkoutEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedExerciseId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    WeightValue = table.Column<double>(type: "float", nullable: false),
                    WeightUnit = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformedSets", x => new { x.WorkoutEntryId, x.PerformedExerciseId, x.Id });
                    table.ForeignKey(
                        name: "FK_PerformedSets_PerformedExercises_WorkoutEntryId_PerformedExerciseId",
                        columns: x => new { x.WorkoutEntryId, x.PerformedExerciseId },
                        principalTable: "PerformedExercises",
                        principalColumns: new[] { "WorkoutEntryId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyGoals_UserId",
                table: "BodyGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DrinkEntries_UserId",
                table: "DrinkEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodEntries_UserId",
                table: "FoodEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodItems_FoodEntryId",
                table: "FoodItems",
                column: "FoodEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionGoals_UserId",
                table: "NutritionGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Value",
                table: "Users",
                column: "Email_Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutEntries_UserId",
                table: "WorkoutEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutEntries_WorkoutProgramId",
                table: "WorkoutEntries",
                column: "WorkoutProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercisePlans_WorkoutProgramId",
                table: "WorkoutExercisePlans",
                column: "WorkoutProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutGoals_UserId",
                table: "WorkoutGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPrograms_UserId",
                table: "WorkoutPrograms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_UserId",
                table: "WorkoutSets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_WorkoutExercisePlanId",
                table: "WorkoutSets",
                column: "WorkoutExercisePlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyGoals");

            migrationBuilder.DropTable(
                name: "DrinkEntries");

            migrationBuilder.DropTable(
                name: "FoodItems");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NutritionGoals");

            migrationBuilder.DropTable(
                name: "PerformedSets");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "WorkoutGoals");

            migrationBuilder.DropTable(
                name: "WorkoutSets");

            migrationBuilder.DropTable(
                name: "FoodEntries");

            migrationBuilder.DropTable(
                name: "PerformedExercises");

            migrationBuilder.DropTable(
                name: "WorkoutExercisePlans");

            migrationBuilder.DropTable(
                name: "WorkoutEntries");

            migrationBuilder.DropTable(
                name: "WorkoutPrograms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
