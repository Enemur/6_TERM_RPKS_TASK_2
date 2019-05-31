using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Lab2RPKS.Model.EncryptionTasks;

namespace Lab2RPKS.ApplicationViewModel
{ 
    public class ApplicationViewModel_task3 : ApplicationViewModel_task1
    {
        private string _x = "";
        public String X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged("Answer");
            }
        }

        private string _n = "";
        public String N
        {
            get { return _n; }
            set
            {
                _n = value;
                OnPropertyChanged("Answer");
            }
        }

        private string _module = "";
        public String Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged("Answer");
            }
        }
        
        public String Answer
        {
            get
            {
                if(!string.IsNullOrEmpty(_x) && !string.IsNullOrEmpty(_n)&& !string.IsNullOrEmpty(_module))
                    return Task3.PowInResidueRing(BigInteger.Parse(_x), BigInteger.Parse(_n), BigInteger.Parse(_module)).ToString();
                else
                {
                    return "";
                }
            }

        }

    }
}
