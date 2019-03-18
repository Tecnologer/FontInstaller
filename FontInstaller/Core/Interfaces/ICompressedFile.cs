using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontInstaller.Core.Interfaces
{
    public interface ICompressedFile
    {
        bool HasPassword(string filePath);

        void RequestPassword(BackgroundWorker worker);
    }
}
