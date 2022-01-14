using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Models
{
  public class PRFQuestionnaire
  {
    [Key]
    [Required]
    public Guid Id { get; set; }
    public User User { get; set; }  // User 1...* Questionnaire
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement1 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement2 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement3 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement4 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement5 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement6 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement7 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement8 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement9 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement10 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement11 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement12 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement13 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement14 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement15 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement16 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement17 { get; set; } = 0;
    [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Statement18 { get; set; } = 0;

    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public IDictionary<int, string> Statements = new Dictionary<int, string>()
  {
    { 1, "The only time I’m certain my child loves me is when he or she is smiling at me" },
    { 2, "I always know what my child wants" },
    { 3, "I like to think about the reasons behind the way my child behaves and feels" },
    { 4, "My child cries around strangers to embarrass me" },
    { 5, "I can completely read my child’s mind" },
    { 6, "I wonder a lot about what my child is thinking and feeling" },
    { 7, "I find it hard to actively participate in make believe play with my child" },
    { 8, "I can always predict what my child will do" },
    { 9, "I am often curious to find out how my child feels" },
    { 10, "My child sometimes gets sick to keep me from doing what I want to do" },
    { 11, "I can sometimes misunderstand the reactions of my child" },
    { 12, "I try to see situations through the eyes of my child" },
    { 13, "When my child is fussy he or she does that just to annoy me" },
    { 14, "I always know why I do what I do to my child" },
    { 15, "I try to understand the reasons why my child misbehaves" },
    { 16, "Often, my child’s behavior is too confusing to bother figuring out" },
    { 17, "I always know why my child acts the way he or she does" },
    { 18, "I believe there is no point in trying to guess what my child feels" },
  };
  }
}
