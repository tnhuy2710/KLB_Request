using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Models
{
    public class Comment : Entity<int>
    {
        public string FormID { get; set; }
        public virtual Form Form { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DateComment { get; set; }

        public int? ReplyID { get; set; }
        public virtual Comment Reply { get; set; }
        public string UserID { get; set; }
        public virtual User User{ get; set; }

        public Comment()
        {

        }
        public Comment(string _formID, string _cONTENT, DateTimeOffset _dateComment, int? _replyID, string _userID)
        {
            FormID = _formID;
            Content = _cONTENT;
            DateComment = _dateComment;
            ReplyID = _replyID;
            UserID = _userID;            
        }
    }
}
