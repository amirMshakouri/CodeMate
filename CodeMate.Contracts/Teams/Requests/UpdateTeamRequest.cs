namespace CodeMate.Contracts.Teams.Requests;

public class UpdateTeamRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}