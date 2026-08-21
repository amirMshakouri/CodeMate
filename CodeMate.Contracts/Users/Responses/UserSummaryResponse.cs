namespace CodeMate.Contracts.Users.Responses
{
    public class UserSummaryResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }
}
