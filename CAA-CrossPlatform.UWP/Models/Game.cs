﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Game
    {
        public Game()
        {
            //set default values
            this.hidden = false;
            this.Id = 0;
        }

        //game properties
        public int Id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public string imagePath { get; set; }
    }
}
