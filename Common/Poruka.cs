﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class Poruka
    {
        public string MessageText { get; set; }
        public bool IsSuccessful { get; set; }
        public Operations Operations { get; set; }
        public Object PorukaObject { get; set; }
    }
}
