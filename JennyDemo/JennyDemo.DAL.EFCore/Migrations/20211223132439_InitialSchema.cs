using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JennyDemo.DAL.EFCore.Migrations
{
	public partial class InitialSchema : Migration
	{
		protected override void Up( MigrationBuilder migrationBuilder )
		{
			migrationBuilder.CreateTable(
				name: "A1Candidate",
				columns: table => new
				{
					Id = table.Column<int>( type: "int", nullable: false )
						.Annotation( "SqlServer:Identity", "1, 1" ),
					GivenName = table.Column<string>( type: "nvarchar(128)", maxLength: 128, nullable: true ),
					FamilyName = table.Column<string>( type: "nvarchar(128)", maxLength: 128, nullable: true )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_A1Candidate", x => x.Id );
				} );

			migrationBuilder.CreateTable(
				name: "A1Question",
				columns: table => new
				{
					Id = table.Column<int>( type: "int", nullable: false )
						.Annotation( "SqlServer:Identity", "1, 1" ),
					Question = table.Column<string>( type: "nvarchar(1024)", maxLength: 1024, nullable: true )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_A1Question", x => x.Id );
				} );

			migrationBuilder.CreateTable(
				name: "A1Interview",
				columns: table => new
				{
					Id = table.Column<int>( type: "int", nullable: false )
						.Annotation( "SqlServer:Identity", "1, 1" ),
					DateTime = table.Column<DateTime>( type: "datetime2", nullable: false ),
					CandidateId = table.Column<int>( type: "int", nullable: false )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_A1Interview", x => x.Id );
					table.ForeignKey(
						name: "FK_A1Interview_A1Candidate_CandidateId",
						column: x => x.CandidateId,
						principalTable: "A1Candidate",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			migrationBuilder.CreateTable(
				name: "A1Answer",
				columns: table => new
				{
					Id = table.Column<int>( type: "int", nullable: false )
						.Annotation( "SqlServer:Identity", "1, 1" ),
					InterviewId = table.Column<int>( type: "int", nullable: false ),
					QuestionId = table.Column<int>( type: "int", nullable: false ),
					Stars = table.Column<int>( type: "int", nullable: false )
				},
				constraints: table =>
				{
					table.PrimaryKey( "PK_A1Answer", x => x.Id );
					table.ForeignKey(
						name: "FK_A1Answer_A1Interview_InterviewId",
						column: x => x.InterviewId,
						principalTable: "A1Interview",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					table.ForeignKey(
						name: "FK_A1Answer_A1Question_QuestionId",
						column: x => x.QuestionId,
						principalTable: "A1Question",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			migrationBuilder.CreateIndex(
				name: "IX_A1Answer_InterviewId",
				table: "A1Answer",
				column: "InterviewId" );

			migrationBuilder.CreateIndex(
				name: "IX_A1Answer_QuestionId",
				table: "A1Answer",
				column: "QuestionId" );

			migrationBuilder.CreateIndex(
				name: "IX_A1Interview_CandidateId",
				table: "A1Interview",
				column: "CandidateId" );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			migrationBuilder.DropTable(
				name: "A1Answer" );

			migrationBuilder.DropTable(
				name: "A1Interview" );

			migrationBuilder.DropTable(
				name: "A1Question" );

			migrationBuilder.DropTable(
				name: "A1Candidate" );
		}
	}
}
