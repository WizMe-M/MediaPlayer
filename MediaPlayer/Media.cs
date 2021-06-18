using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MediaPlayer
{
    class Media
    {
        public string Path { get; set; }
        Media()
        {

        }
        public Media(string Path)
        {
            this.Path = Path;
        }
    }
}
