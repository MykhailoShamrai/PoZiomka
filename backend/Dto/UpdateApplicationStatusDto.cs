namespace backend.Dto;

public class UpdateApplicationStatusDto
{
    public int ApplicationId { get; set; }
    public int Status { get; set; }
    public string Description { get; set; } = string.Empty;
}