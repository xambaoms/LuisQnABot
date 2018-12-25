using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Dialogs
{
    [Serializable]
    public class QnAAnswer
    {
        public IList<Answer> answers { get; set; }
    }
}