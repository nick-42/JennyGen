using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace JennyDemo.DAL
{
	public static class JennyDataAccessLayer
	{
		public static void Reset()
		{
			using var ctx = new Context.JennyContext();
			ctx.Reset();
		}

		public static List<DOG.A1Interview> FetchInterviews()
		{
			using var ctx = new Context.JennyContext();
			return ctx.Interview
				.Include( o => o.Candidate )
				.Include( o => o.Answers ).ThenInclude( o => o.Question )
				.ToList();
		}

		public static List<DOG.A1Candidate> FetchCandidates()
		{
			using var ctx = new Context.JennyContext();
			return ctx.Candidate.ToList();
		}
	}
}
