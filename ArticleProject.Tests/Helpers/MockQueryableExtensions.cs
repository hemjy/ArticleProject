using Moq;

namespace ArticleProject.Tests.Helpers
{
    public static class MockQueryableExtensions
    {
        public static Mock<IQueryable<T>> AsAsyncQueryable<T>(this IEnumerable<T> enumerable)
        {
            var mock = new Mock<IQueryable<T>>();
            var queryable = enumerable.AsQueryable();
            mock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            mock.As<IQueryable<T>>()
                .Setup(m => m.Expression)
                .Returns(queryable.Expression);
            mock.As<IQueryable<T>>()
                .Setup(m => m.ElementType)
                .Returns(queryable.ElementType);
            mock.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(queryable.GetEnumerator());

            return mock;
        }
    }

}
