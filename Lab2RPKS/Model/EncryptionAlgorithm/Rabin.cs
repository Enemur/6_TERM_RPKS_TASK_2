using Lab2RPKS.Model.EncryptionTasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab2RPKS.Model.EncryptionAlgorithm
{

    public class Rabin : EncryptionAlgorithm
    {
        private static Random _random = new Random(DateTime.Now.Millisecond);

        public Rabin(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged) : base(
            ref currentProgress, worker, onPropertyChanged)
        {
        }

        #region PrivateMethods        
        private void CheckInput(BigInteger p, BigInteger q, BigInteger b)
        {
            var isSimpleP = CryptoAlgorithmsTask1.IsSimple(p);

            if (!isSimpleP)
                throw new Exception("P isn't simple");

            var isSimpleQ = CryptoAlgorithmsTask1.IsSimple(q);

            if (!isSimpleQ)
                throw new Exception("Q isn't simple");

            if (!((p % 4 == 3) && (q % 4 == 3)))
                throw new Exception("Condition q = p = 3 mod 4 failed");

            var n = p * q;

            if ((b < 0) || (b > n - 1))
                throw new Exception("Incorrect value of b");
        }

        private BigInteger Sqrt(BigInteger value, BigInteger module)
        {
            BigInteger result = 0;

            var power = (int)((module + 1) / 4);
            result = Operations.Mod(BigInteger.Pow(value, power), module);

            return result;
        }

        private void ExtendedEuclid(out BigInteger yp, out BigInteger yq, BigInteger a, BigInteger b)
        {
            //Расширенный алгоритм Евклида

            BigInteger x0 = 1, x1 = 0, y0 = 0, y1 = 1;

            while (b != 0)
            {
                var q = BigInteger.Divide(a, b);

                var tmpA = a;
                a = b;
                b = tmpA % b;

                var tmpX0 = x0;
                x0 = x1;
                x1 = tmpX0 - x1 * q;

                var tmpY0 = y0;
                y0 = y1;
                y1 = tmpY0 - y1 * q;
            }

            yp = x0;
            yq = y0;
        }
        #endregion

        public BigInteger EncryptByte(BigInteger data, BigInteger n, BigInteger b)
        {
            BigInteger result;

            result = Operations.Mod(data * (data + b), n);

            return result;
        }

        public void Encrypt(string inputFilePath, string outputFilePath, BigInteger p, BigInteger q, BigInteger b)
        {
            CheckInput(p, q, b);

            var n = p * q;

            using (var fileReader = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var fileWriter = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    var oneTick = fileReader.Length;
                    if (oneTick > 100)
                        oneTick /= 100;

                    var resultBytes = new List<byte>();

                    for (var i = 0; i < fileReader.Length; i++)
                    {
                        if (_worker != null)
                            if (i % oneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");
                            }

                        var readedByte = fileReader.ReadByte();

                        var c = EncryptByte(readedByte, n, b);
                        resultBytes.AddRange(c.ToByteArray());
                    }

                    fileWriter.Write(resultBytes.ToArray(), 0, resultBytes.Count);
                }
            }
        }

        public IEnumerable<BigInteger> DecryptByte(BigInteger data, BigInteger p, BigInteger q, BigInteger n, BigInteger b)
        {
            var D = Operations.Mod((BigInteger.Pow(b, 2) + 4 * data), n);

            // 4 корня +/-
            var mp = Sqrt(D, p);
            var mq = Sqrt(D, q);

            ExtendedEuclid(out var yp, out var yq, p, q);

            var d = new List<BigInteger>();

            d.Add(Operations.Mod((yp * p * mq + yq * q * mp), n));
            d.Add(n - d[0]);
            d.Add(Operations.Mod((yp * p * mq - yq * q * mp), n));
            d.Add(n - d[2]);


            var m = new List<BigInteger>();
            foreach (var di in d)
            {
                if (Operations.Mod((di - b), 2) == 0)
                    m.Add(Operations.Mod(((-b + di) / 2), n));
                else
                    m.Add((Operations.Mod(((-b + di + n) / 2), n)));
            }

            return m;
        }

        public void Decrypt(string inputFilePath, string outputFilePath, BigInteger p, BigInteger q, BigInteger b)
        {
            CheckInput(p, q, b);

            var n = p * q;

            using (var fileReader = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var fileWriter = new StreamWriter(outputFilePath))
                {
                    var oneTick = fileReader.Length;
                    if (oneTick > 100)
                        oneTick /= 100;

                    for (var i = 0; i < fileReader.Length; i++)
                    {
                        if (_worker != null)
                            if (i % oneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");
                            }

                        var readedByte = fileReader.ReadByte();

                        var m = DecryptByte(readedByte, p, q, n, b);

                        foreach (var mi in m)
                        {
                            fileWriter.Write(mi + " ");
                        }
                        fileWriter.WriteLine();

                    }
                }
            }
        }
    }
}
