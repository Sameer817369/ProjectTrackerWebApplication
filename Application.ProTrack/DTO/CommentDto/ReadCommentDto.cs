using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProTrack.DTO.CommentDto
{
    public class ReadCommentDto
    {
        public string Description {get; set;}
        public string TaskTitle { get; set;}
        public DateTime CommentTime { get; set;}
        public string CommentUser { get; set;}
    }
}
