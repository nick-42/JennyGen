namespace JennyDemo.DAL.EF6.Framework.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class InitialSchema : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.A1Answer",
				c => new
				{
					Id = c.Int( nullable: false, identity: true ),
					InterviewId = c.Int( nullable: false ),
					QuestionId = c.Int( nullable: false ),
					Stars = c.Int( nullable: false ),
				} )
				.PrimaryKey( t => t.Id )
				.ForeignKey( "dbo.A1Interview", t => t.InterviewId )
				.ForeignKey( "dbo.A1Question", t => t.QuestionId )
				.Index( t => t.InterviewId )
				.Index( t => t.QuestionId );

			CreateTable(
				"dbo.A1Interview",
				c => new
				{
					Id = c.Int( nullable: false, identity: true ),
					DateTime = c.DateTime( nullable: false ),
					CandidateId = c.Int( nullable: false ),
				} )
				.PrimaryKey( t => t.Id )
				.ForeignKey( "dbo.A1Candidate", t => t.CandidateId )
				.Index( t => t.CandidateId );

			CreateTable(
				"dbo.A1Candidate",
				c => new
				{
					Id = c.Int( nullable: false, identity: true ),
					GivenName = c.String( nullable: false, maxLength: 128 ),
					FamilyName = c.String( nullable: false, maxLength: 128 ),
				} )
				.PrimaryKey( t => t.Id );

			CreateTable(
				"dbo.A1Question",
				c => new
				{
					Id = c.Int( nullable: false, identity: true ),
					Question = c.String( nullable: false, maxLength: 1024 ),
				} )
				.PrimaryKey( t => t.Id );

		}

		public override void Down()
		{
			DropForeignKey( "dbo.A1Answer", "QuestionId", "dbo.A1Question" );
			DropForeignKey( "dbo.A1Interview", "CandidateId", "dbo.A1Candidate" );
			DropForeignKey( "dbo.A1Answer", "InterviewId", "dbo.A1Interview" );
			DropIndex( "dbo.A1Interview", new[] { "CandidateId" } );
			DropIndex( "dbo.A1Answer", new[] { "QuestionId" } );
			DropIndex( "dbo.A1Answer", new[] { "InterviewId" } );
			DropTable( "dbo.A1Question" );
			DropTable( "dbo.A1Candidate" );
			DropTable( "dbo.A1Interview" );
			DropTable( "dbo.A1Answer" );
		}
	}
}
