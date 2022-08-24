namespace ArticleService.App.Interface
{
    public interface IArticleTagApp
    {
        public void UpdateArticleTag(int articleId, List<int> tagIds);
        public void AddArticleTag(int articleId, List<int> tagId);
    }
}
