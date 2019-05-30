using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Numerics;
namespace Lab2RPKS.Model.EncryptionAlgorithm
{
    public class RSA : EncryptionAlgorithm
    {
        public RSA(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged) : base(
            ref currentProgress, worker, onPropertyChanged)
        {
        }


        public string Encode(string inputFileName, string outputFileName, long p, long q)//возвращает секретные ключ в виде 2 чисел
        {
            if (!IsTheNumberSimple(p) || !IsTheNumberSimple(q))
            {
                throw new Exception("p или q - не простые числа!");
            }

            long n = p * q;
            long m = (p - 1) * (q - 1);
            long d = Calculate_d(m);
            long e_ = Calculate_e(d, m);
            using (FileStream fsread = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamWriter fswrite = new StreamWriter(outputFileName))
                {


                    var OneTick = fsread.Length / 100;
                    if (fsread.Length == 0) throw new Exception("Пустой файл");

                    fswrite.WriteLine(fsread.Length);


                    for (long i = 0; i < fsread.Length; i++)
                    {
                        if (_worker != null)
                            if (i % OneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");

                            }

                        int readByte = fsread.ReadByte();

                        uint result = RSAByteEncryption((uint)readByte, d, n);

                        fswrite.WriteLine(result);
                    }
                }
            }
            return $"{d} {n}";


        }

        public void Decipher(string inputFileName, string outputFileName, long d, long n)
        {


            using (StreamReader fsread = new StreamReader(inputFileName))
            {
                using (FileStream fswrite = new FileStream(outputFileName, FileMode.Create, FileAccess.Write))
                {



                    long length = Convert.ToInt64(fsread.ReadLine());
                    var OneTick = length / 100;

                    if (length == 0)
                        throw new Exception("Пустой файл");
                    for (long i = 0; i < length; i++)
                    {
                        uint readByte = Convert.ToUInt32(fsread.ReadLine());


                        uint result = RSAByteEncryption((uint)readByte, d, n);

                        fswrite.WriteByte(BitConverter.GetBytes(result)[0]);

                        if (_worker != null)
                            if (i % OneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");

                            }
                    }



                }
            }




        }

        private uint RSAByteEncryption(uint data, long d, long n)
        {
            string result = "";

            BigInteger bi;


            bi = new BigInteger(data);
            bi = BigInteger.Pow(bi, (int)d);

            BigInteger n_ = new BigInteger((int)n);

            bi = bi % n_;
            if (bi > n_)
            {
                throw new Exception("Введите числа больше");
            }
            return (uint)bi;

        }


        private bool IsTheNumberSimple(long n)
        {
            if (n < 2)
                return false;

            if (n == 2)
                return true;

            for (long i = 2; i < n; i++)
                if (n % i == 0)
                    return false;

            return true;
        }

        private long Calculate_d(long m)
        {
            long d = m - 1;

            for (long i = 2; i <= m; i++)
                if ((m % i == 0) && (d % i == 0))
                {
                    d--;
                    i = 1;
                }

            return d;
        }


        private long Calculate_e(long d, long m)
        {
            long e = 10;

            while (true)
            {
                if ((e * d) % m == 1)
                    break;
                else
                    e++;
            }

            return e;
        }






    }
}