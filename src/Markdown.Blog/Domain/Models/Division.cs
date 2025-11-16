namespace Markdown.Blog.Domain.Models
{
    public class Division
    {
        public Repository Repository { get; set; }
        public string Name { get; set; }

        public Division(Repository repository, string name)
        {
            Repository = repository;
            Name = name;
        }
    }
}