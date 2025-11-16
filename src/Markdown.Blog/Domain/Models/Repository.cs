namespace Markdown.Blog.Domain.Models
{
    public class Repository
    {
        public string Owner { get; set; }
        public string Repo { get; set; }

        public Repository(string owner, string repo)
        {
            Owner = owner;
            Repo = repo;
        }

        public Repository(string GITHUB_REPOSITORY)
        {
            Owner = GITHUB_REPOSITORY.Split('/')[0]; //"zhiliangpt";
		    Repo = GITHUB_REPOSITORY.Split('/')[1]; //"Zhis.Blog.Draft";
        }

        public string GitUrl => $"https://github.com/{Owner}/{Repo}.git";
    }
}