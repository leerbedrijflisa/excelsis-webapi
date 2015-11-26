namespace Lisa.Excelsis.WebApi
{
    public class Error
    {
        public Error(int code, string message, object values)
        {
            Code = code;
            Message = message;
            Values = values;
        }

        public int Code { get; set; }
        public string Message { get; set; }
        public object Values { get; set; }
    }
}