namespace CodeMate.Contracts.Teams.Requests;

public class CreateTeamRequest
{
    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}