using Journal2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Journal2
{
	internal class ApplicationContext : DbContext
	{
		public DbSet<Models.Group> Groups => Set<Models.Group>();
		public DbSet<Student> Students => Set<Student>();
		public string ConnectionString { get; private set; }

		public ApplicationContext(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(ConnectionString);
		}
	}
}
