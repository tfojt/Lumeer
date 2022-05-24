using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models.Rest
{
    //"{\"stems\":[],\"fulltexts\":[],\"page\":null,\"pageSize\":null}"
    public class TasksFilter
    {
        public List<Stem> Stems { get; set; } = new List<Stem>();
        public List<string> Fulltexts { get; set; } = new List<string>();
        public object Page { get; set; }
        public object PageSize { get; set; }
    }
}
