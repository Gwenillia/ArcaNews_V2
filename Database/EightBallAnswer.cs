using System.ComponentModel.DataAnnotations;

namespace ArcaNews_V2.Database;

public partial class EightBallAnswer
{
    [Key]
    public long AnswerId { get; set; }
    public string AnswerText { get; set; }
    public string AnswerColor { get; set; }
}