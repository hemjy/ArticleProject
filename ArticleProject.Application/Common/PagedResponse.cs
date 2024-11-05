namespace ArticleProject.Application.Common
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageSize < Total;

        public PagedResponse(T data, int pageNumber, int pageSize, int total, string message = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Total = total;
            Data = data;
            Message = null;
            Succeeded = true;
            Errors = null;
        }
        public PagedResponse(string message, List<string> errors = null)
        {
            Message = message;
            Succeeded = false;
            Errors = errors;
        }

        public static PagedResponse<T> Success(T data, int pageNumber, int pageSize, int total, string message = null)
        {
            message = message ?? $"{total} record(s) found.";
            return new PagedResponse<T>(data, pageNumber, pageSize, total, message);
        }
        public static PagedResponse<T> Failure(string message, List<string> errors = null)
        {
            return new PagedResponse<T>(message, errors);
        }
    }
}
