using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STools.CommonLibrary
{
    public enum AlarmCategory
    {
        NotUsed  = 1,
        Warnning = 2,
        Error    = 3,
    }

    public class Alarm
    {
        private int _id = 0;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        AlarmCategory _category = AlarmCategory.NotUsed;
        public AlarmCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private int _reportId = -1;
        public int ReportId
        {
            get { return _reportId; }
            set { _reportId = value; }
        }

        private List<string> _replyList = new List<string>();
        public List<string> ReplyList
        {
            get { return _replyList; }
            set { _replyList = value; }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _comment = string.Empty;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
