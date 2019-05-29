using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2RPKS.Model.EncryptionAlgorithm
{
   
    public class Rijndael : EncryptionAlgorithm
    {
        public Rijndael(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged) : base(
            ref currentProgress, worker, onPropertyChanged)
        {
        }

      
    }
}
