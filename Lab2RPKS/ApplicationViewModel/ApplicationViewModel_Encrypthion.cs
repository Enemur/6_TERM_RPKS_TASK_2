using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Essy.Tools.InputBox;
using Lab2RPKS.Model.EncryptionAlgorithm;
using Microsoft.Win32;

namespace Lab2RPKS.ApplicationViewModel
{
    public class ApplicationViewModel_Encrypthion : BaseAplicationViewModel
    {
        private static bool _isRunning;

        enum SelectedAction//сюда добавлять новые шифровки 
        {
            IsNotChosen = -1,
            IsRSA,
            IsRijndael,
            IsAlGamal,
            isRabin


        }
        private RSA _rsaEncryption;
        private Rijndael _rijndaelEncryption;
        private AlGamal _alGamalEncryption;
        private Rabin _rabinEncryption;
        private BackgroundWorker worker = new BackgroundWorker();

        private SelectedAction _selectedAction;



        public ApplicationViewModel_Encrypthion()
        {
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            CurrentProgress = -1;
            IsRunning = true;
            _selectedAction = SelectedAction.IsNotChosen;
            _rsaEncryption = new RSA(ref _currentProgress, worker, OnPropertyChanged);
            _rijndaelEncryption = new Rijndael(ref _currentProgress, worker, OnPropertyChanged);
            _alGamalEncryption = new AlGamal(ref _currentProgress, worker, OnPropertyChanged);
            _rabinEncryption = new Rabin(ref _currentProgress, worker, OnPropertyChanged);
        }

        private ICommand _radioCommand;
        public ICommand RadioCommand
        {
            get
            {
                if (_radioCommand == null)
                    _radioCommand = new RelayCommand((param) => { RadioMethod(param); });

                return _radioCommand;
            }
        }


        private void RadioMethod(object parametr)
        {

            switch (parametr.ToString())
            {
                case "RSA":
                    _selectedAction = SelectedAction.IsRSA;
                    break;
                case "Rijndael":
                    _selectedAction = SelectedAction.IsRijndael;
                    break;
                case "AlGamal":
                    _selectedAction = SelectedAction.IsAlGamal;
                    break;
                case "Rabin":
                    _selectedAction = SelectedAction.isRabin;
                    break;

            }


        }

        private String _inputFileName = "";
        private String _outputFileName = "";

        public String InputFileName
        {
            get { return _inputFileName; }
            set { _inputFileName = value; }
        }

        public String OutputFileName
        {
            get { return _outputFileName; }
            set { _outputFileName = value; }
        }


        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            if (_selectedAction == SelectedAction.IsNotChosen)
            {
                IsRunning = true;
                OnPropertyChanged($"IsRunning");
                MessageBox.Show("Не выбран метод шифрования");
                Thread.Sleep(1);
                return;
            }

            if (_inputFileName.Length == 0 || _outputFileName.Length == 0)
            {
                IsRunning = true;
                OnPropertyChanged($"IsRunning");
                MessageBox.Show("Пути до файлов не заданы");
                Thread.Sleep(1);
                return;
            }


            CurrentProgress = 0;
            OnPropertyChanged($"CurrentProgress");
            Thread.Sleep(1);



            try
            {
                switch (_selectedAction)
                {
                    case SelectedAction.IsRSA:
                        {
                            string key = InputBox.ShowInputBox("Введите 2 простых числа для шифровки через пробел");
                            if (string.IsNullOrEmpty(key))
                            {
                                MessageBox.Show("числа не заданы");
                                return;
                            }

                            var numbers = key.Split(' ');
                            if (_modeEncryption == ModeEncryption.Encrypt)
                            {
                                string answer = _rsaEncryption.Encode(_inputFileName, _outputFileName, Convert.ToInt64(numbers[0]), Convert.ToInt64(numbers[1]));
                                if (!string.IsNullOrEmpty(answer))
                                {
                                    MessageBox.Show($"Числа для расшифровки: {answer}");

                                }
                            }
                            else
                            {
                                _rsaEncryption.Decipher(_inputFileName, _outputFileName, Convert.ToInt64(numbers[0]), Convert.ToInt64(numbers[1]));
                            }
                        }
                        break;
                    case SelectedAction.IsAlGamal:
                        {
                            string key = InputBox.ShowInputBox("Введите p, q, g, x через пробел");
                            if (string.IsNullOrEmpty(key))
                            {
                                MessageBox.Show("Числа не заданы");
                                return;
                            }

                            var numbers = key.Split(' ');

                            if (numbers.Length != 4)
                                throw new Exception("Incorrect input");

                            var p = BigInteger.Parse(numbers[0]);
                            var q = BigInteger.Parse(numbers[1]);
                            var g = BigInteger.Parse(numbers[2]);
                            var x = BigInteger.Parse(numbers[3]);

                            if (_modeEncryption == ModeEncryption.Encrypt)
                                _alGamalEncryption.Encrypt(_inputFileName, _outputFileName, p, q, g, x);
                            else
                                _alGamalEncryption.Decrypt(_inputFileName, _outputFileName, p, q, g, x);
                        }
                        break;
                    case SelectedAction.isRabin:
                        {
                            string key = InputBox.ShowInputBox("Введите p, q, b через пробел");
                            if (string.IsNullOrEmpty(key))
                            {
                                MessageBox.Show("Числа не заданы");
                                return;
                            }


                            var numbers = key.Split(' ');

                            if (numbers.Length != 3)
                                throw new Exception("Incorrect input");

                            var p = BigInteger.Parse(numbers[0]);
                            var q = BigInteger.Parse(numbers[1]);
                            var b = BigInteger.Parse(numbers[2]);

                            if (_modeEncryption == ModeEncryption.Encrypt)
                                _rabinEncryption.Encrypt(_inputFileName, _outputFileName, p, q, b);
                            else
                                _rabinEncryption.Decrypt(_inputFileName, _outputFileName, p, q, b);
                        }
                        break;
                        //case SelectedAction.IsRijndael:
                        //    _encryptionAlgorithm[(int) SelectedAction.IsRijndael]
                        //        .Start(_inputFileName, _outputFileName, _modeEncryption);
                        //    break;
                }


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                CurrentProgress = -1;
                Thread.Sleep(1);
                IsRunning = true;
                OnPropertyChanged($"IsRunning");
                return;
            }

            CurrentProgress = 100;
            Thread.Sleep(1);
            OnPropertyChanged($"CurrentProgress");
            IsRunning = true;
            OnPropertyChanged($"IsRunning");





        }


        private ICommand _inputFileCommand;

        public ICommand InputFileCommand
        {
            get
            {
                return _inputFileCommand ?? (_inputFileCommand = new RelayCommand(x =>
                {

                    var dialog = new OpenFileDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        _inputFileName = dialog.FileName;
                        if (_outputFileName.Length != 0 &&
                            Path.GetExtension(_inputFileName) != Path.GetExtension(_outputFileName))
                        {
                            _outputFileName = _outputFileName.Replace(Path.GetExtension(_outputFileName), "");
                            _outputFileName += Path.GetExtension(_inputFileName);
                            OnPropertyChanged($"OutputFileName");
                        }

                        OnPropertyChanged($"InputFileName");

                    }
                }));
            }
        }

        private ICommand _outputFileCommand;

        public ICommand OutputFileCommand
        {
            get
            {
                return _outputFileCommand ?? (_outputFileCommand = new RelayCommand(x =>
                {

                    var dialog = new SaveFileDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        _outputFileName = dialog.FileName;
                        if (_inputFileName.Length != 0 && Path.GetExtension(_inputFileName) != Path.GetExtension(_outputFileName))
                            _outputFileName += Path.GetExtension(_inputFileName);
                        OnPropertyChanged($"OutputFileName");

                    }
                }));
            }
        }
        private ICommand _cryptCommand;
        private ModeEncryption _modeEncryption;

        public ICommand CryptCommand
        {
            get
            {
                return _cryptCommand ?? (_cryptCommand = new RelayCommand(x =>
                {


                    if (_isRunning)
                    {
                        _modeEncryption = ModeEncryption.Encrypt;
                        worker.RunWorkerAsync();
                        IsRunning = !IsRunning;
                        OnPropertyChanged($"IsRunning");
                    }


                }));
            }
        }
        private ICommand _decryptCommand;
        public ICommand DecryptCommand
        {
            get
            {
                return _decryptCommand ?? (_decryptCommand = new RelayCommand(x =>
                {


                    if (_isRunning)
                    {
                        _modeEncryption = ModeEncryption.Decipher;
                        worker.RunWorkerAsync();
                        IsRunning = !IsRunning;

                        OnPropertyChanged($"IsRunning");
                    }

                }));
            }
        }

        private int _currentProgress = -1;
        public int CurrentProgress
        {
            get
            {
                OnPropertyChanged($"StatusStr");
                return _currentProgress == -1 ? 0 : _currentProgress;

            }
            private set
            {

                _currentProgress = value;
                OnPropertyChanged($"StatusStr");

            }
        }

        public String StatusStr
        {
            get
            {
                if (_currentProgress == 100)
                    return "Процесс завершен";
                else if (_currentProgress != -1)
                    return "Процесс запущен ( " + _currentProgress.ToString() + " %), Ожидайте...";
                else
                {
                    return "";
                }

            }

        }

        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {

                _isRunning = value;

            }
        }




    }
}
