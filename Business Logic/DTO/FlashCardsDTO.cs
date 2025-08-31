namespace Business_Logic.Dto;
public class FlashCardsDto
{
    public int Id { get; set; }
    public required string Word { get; set; }
    public required int Stack { get; set; }
    public required string Translation { get; set; }
}
