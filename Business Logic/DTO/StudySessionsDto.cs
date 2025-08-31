using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Dto;
public class StudySessionDto
{
    public int Id { get; set; }
    public required int StackId { get; set; }
    public required DateTime SessionDate { get; set; }
    public required int TotalCards { get; set; }
    public required int TotalAttempts { get; set; }
    public required int CorrectAnswers { get; set; }
    public required TimeSpan StudyDuration { get; set; }

    // Propriedades calculadas
    public int ExtraAttempts => TotalAttempts - TotalCards;
    public double AccuracyPercentage => TotalAttempts > 0 ? (double)TotalCards / TotalAttempts * 100 : 0;
    public bool PerfectSession => TotalAttempts == TotalCards;
}