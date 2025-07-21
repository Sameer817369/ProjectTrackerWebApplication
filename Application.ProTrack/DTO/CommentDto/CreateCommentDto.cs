using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProTrack.DTO.CommentDto
{
    public class CreateCommentDto
    {
        [Required]
        public string Description { get; set; }
    }
}
