using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.net4._7
{
    public class IncomMessEvent : EventArgs
    {
        public IncomMessEvent(string messsage)
        {
            Message = messsage;
        }

        public string  Message{get;private set;}
    }
}
