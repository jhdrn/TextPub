﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub.DropBox.Models
{
    [Serializable]
    public class UserCredentials
    {
        public string Token { get; set; }
        public string Secret { get; set; }
    }
}
