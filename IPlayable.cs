using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace praktik_07._04._2023
{
    internal interface IPlayable
    {
        void move(int i, int j);
        bool winCheck();
        bool loseCheck();
    }
}
