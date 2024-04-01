using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class User
    {
        public int Id { get; set; }
        public Socket? Socket { get; set; }
        public bool RunGame { get; set; }
        public bool? IsBlack { get; set; }
    }
}
