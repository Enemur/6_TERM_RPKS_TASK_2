using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2RPKS.Model.EncryptionAlgorithm
{
    public abstract class EncryptionAlgorithm
    {
        protected int _currentProgress;
        protected BackgroundWorker _worker;
        protected Action<string> _onPropertyChanged;


        public EncryptionAlgorithm(ref int currentProgress,
            BackgroundWorker worker, Action<string> onPropertyChanged)
        {
            _currentProgress = currentProgress;
            _worker = worker;
            _onPropertyChanged = onPropertyChanged;

        }

        protected byte[] StringToByte(string keyStr)
        {
            byte[] keByte = new byte[keyStr.Length];
            for (int i = 0; i < keyStr.Length; i++)
            {
                keByte[i] = Convert.ToByte(keyStr[i]);
            }

            return keByte;
        }

       



      

    }
}
