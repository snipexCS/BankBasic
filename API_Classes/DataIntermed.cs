namespace API_Classes
{
    public class DataIntermed
    {
        public int bal { get; set; }
        public uint acct { get; set; }
        public uint pin { get; set; }
        public string fname { get; set; } = "";
        public string lname { get; set; } = "";
        public byte[] image { get; set; }
    }

    public class DataIntermedDTO
    {
        public int bal { get; set; }
        public uint acct { get; set; }
        public uint pin { get; set; }
        public string fname { get; set; } = "";
        public string lname { get; set; } = "";
        public string imageBase64 { get; set; }
    }

  

    public class ApiError
    {
        public string Message { get; set; } = "";
        public string StackTrace { get; set; }
    }
}
