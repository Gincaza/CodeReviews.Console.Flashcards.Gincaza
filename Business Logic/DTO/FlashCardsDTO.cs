using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.DTO;
public class FlashCardsDTO
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public required int Stack { get; set; }
}
