using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto
{
    public class AnswerDto
    {
        public int FormId { get; set; }
        public int UserId { get; set; }
        public List<int> ChosenOptionIds { get; set; } = new List<int>();
        public AnswerStatus Status { get; set; } = AnswerStatus.Saved;
    }
}