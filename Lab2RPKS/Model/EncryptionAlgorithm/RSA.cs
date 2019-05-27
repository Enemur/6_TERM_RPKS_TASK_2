using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Lab2RPKS.Model.EncryptionAlgorithm
{
    public class RSA : EncryptionAlgorithm
    {
        public RSA(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged) : base(
            ref currentProgress, worker, onPropertyChanged)
        {
        }

        public override void Start(string inputFileName, string outputFileName, ModeEncryption mode, params object[] list)
        {
            MessageBox.Show($"{(int)list[0]}");
        }
    }
}