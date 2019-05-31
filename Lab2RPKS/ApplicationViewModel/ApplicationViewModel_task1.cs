using Lab2RPKS.Model.EncryptionTasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab2RPKS.ApplicationViewModel
{
    public class ApplicationViewModel_task1: BaseAplicationViewModel
    {
        private string _m = "";
        public String M
        {
            get { return _m; }
            set
            {
                _m = value;
                if (_m.Length == 0)
                {
                    PrimeNumbersLessM.Clear();
                    SystemDeduction.Clear();
                    FunctionEiler.Clear();
                    PowerDecomposition.Clear();
                    return;
                }

                
                try
                {
                    PrimeNumbersLessM.Clear();
                    foreach (var num in CryptoAlgorithmsTask1.Task1(BigInteger.Parse(_m)))
                    {
                        PrimeNumbersLessM.Add(num.ToString());
                    }

                    SystemDeduction.Clear();
                    foreach (var num in CryptoAlgorithmsTask1.Task2(BigInteger.Parse(_m)))
                    {
                        SystemDeduction.Add(num.ToString());
                    }

                    FunctionEiler.Clear();
                    FunctionEiler.Add(CryptoAlgorithmsTask1.Task3(BigInteger.Parse(_m)).ToString());

                    PowerDecomposition.Clear();
                    foreach (var num in CryptoAlgorithmsTask1.Task4(BigInteger.Parse(_m)))
                    {
                        PowerDecomposition.Add($"{num.Coefficient} ^ {num.Degree}");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    PrimeNumbersLessM.Clear();
                    SystemDeduction.Clear();
                    FunctionEiler.Clear();
                    PowerDecomposition.Clear();
                }
               
            }
        }

        private ObservableCollection<string> _primeNumbersLessM = new ObservableCollection<string>();
        public ObservableCollection<string> PrimeNumbersLessM
        {
            get { return _primeNumbersLessM; }
            set
            {
                _primeNumbersLessM = value;
                OnPropertyChanged("PrimeNumbersLessM");
            }
        }

        private ObservableCollection<string> _systemDeduction = new ObservableCollection<string>();
        public ObservableCollection<string> SystemDeduction
        {
            get { return _systemDeduction; }
            set
            {
                _systemDeduction = value;
                OnPropertyChanged("SystemDeduction");
            }
        }

        private ObservableCollection<string> _functionEiler = new ObservableCollection<string>();
        public ObservableCollection<string> FunctionEiler
        {
            get { return _functionEiler; }
            set
            {
                _functionEiler = value;
                OnPropertyChanged("FunctionEiler");
            }
        }

        private ObservableCollection<string> _powerDecomposition = new ObservableCollection<string>();
        public ObservableCollection<string> PowerDecomposition
        {
            get { return _powerDecomposition; }
            set
            {
                _powerDecomposition = value;
                OnPropertyChanged("PowerDecomposition");
            }
        }
    }
}

