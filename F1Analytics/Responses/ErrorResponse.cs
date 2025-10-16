namespace F1Analytics.Responses;

public class ErrorResponse
{
   
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    

    public class ValidationErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}