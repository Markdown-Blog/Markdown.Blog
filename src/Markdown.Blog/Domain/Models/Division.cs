namespace Markdown.Blog.Domain.Models
{
	public class Division
	{
		public Repository Repository { get; set; } = default!;
		public string Name { get; set; } = default!;

		public Division() { }

		public Division(Repository repository, string name)
		{
			Repository = repository;
			Name = name;
		}
	}
}