namespace CodeMate.Contracts.Teams.Requests;

public class SendJoinRequest
{
    public Guid TeamId { get; set; }

    public string? Message { get; set; }
}