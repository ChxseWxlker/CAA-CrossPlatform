﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class GameQuestion
    {
        public GameQuestion()
        {
            //set default values
            this.Id = 0;
        }

        //question properties
        public int Id { get; set; }
        public int GameID { get; set; }
        public int QuestionID { get; set; }
    }
}
