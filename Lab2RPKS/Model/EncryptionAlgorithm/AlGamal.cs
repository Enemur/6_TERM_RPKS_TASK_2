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
    public class AlGamal : EncryptionAlgorithm
    {

        private static Random _random = new Random(DateTime.Now.Millisecond);
        public AlGamal(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged) : base(
            ref currentProgress, worker, onPropertyChanged)
        {
        }

        #region PrivateMethods
        private System.Numerics.BigInteger getRandomValue(BigInteger minValue, BigInteger maxValue)
        {
            var bytes = maxValue.ToByteArray();
            var length = bytes.Length;

            BigInteger randomValue;

            do
            {
                _random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F;
                randomValue = new BigInteger(bytes);
            } while (randomValue < minValue || randomValue >= maxValue);

            return randomValue;
        }

        private void CheckInput(BigInteger p, BigInteger g, BigInteger q)
        {
            if (!CryptoAlgorithmsTask1.IsSimple(p))
                throw new Exception("P isn't simple");

            if (!CryptoAlgorithmsTask1.IsSimple(q))
                throw new Exception("Q isn't simple");

            if (BigInteger.ModPow(g, (p - 1) / q, p) == 1)
                throw new Exception("G^((p - 1) / Q) (mod p) == 1");
        }
        #endregion

        public Pair<BigInteger, BigInteger> EncryptByte(byte data, BigInteger g, BigInteger h, BigInteger p)
        {
            var k = getRandomValue(1, p - 1);
            var result = new Pair<BigInteger, BigInteger>();

            result.First = BigInteger.ModPow(g, k, p);
            result.Second = Operations.Mod(data * BigInteger.ModPow(h, k, p), p);

            return result;
        }

        public void Encrypt(string inputFilePath, string outpuFilePath, BigInteger p, BigInteger q, BigInteger g, BigInteger x)
        {
            _currentProgress = 0;
            CheckInput(p, g, q);

            var h = BigInteger.ModPow(g, x, p);

            using (var fileReader = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var fileWriter = new StreamWriter(outpuFilePath))
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

                        var readedByte = (byte)fileReader.ReadByte();

                        var pair = EncryptByte(readedByte, g, h, p);
                        fileWriter.WriteLine($"{pair.First},{pair.Second}");
                    }
                }
            }
        }

        public BigInteger DecryptData(Pair<BigInteger, BigInteger> pair, BigInteger x, BigInteger p)
        {
            var denominator = BigInteger.ModPow(pair.First, x, p);
            var divide = Operations.Divide(pair.Second, denominator, p);
            return Operations.Mod(divide, p);
        }

        public void Decrypt(string inputFilePath, string outpuFilePath, BigInteger p, BigInteger q, BigInteger g, BigInteger x)
        {
            CheckInput(p, g, q);

            using (var fileReader = new StreamReader(inputFilePath, Encoding.UTF8))
            {
                using (var fileWriter = new FileStream(outpuFilePath, FileMode.Create, FileAccess.Write))
                {
                    var oneTick = fileReader.BaseStream.Length;
                    if (oneTick > 100)
                        oneTick /= 100;

                    var resultBytes = new List<byte>();

                    string line;
                    for (var i = 0; (line = fileReader.ReadLine()) != null; i++)
                    {
                        if (_worker != null)
                            if (i % oneTick == 0 && i != 0)
                            {
                                _worker.ReportProgress(_currentProgress);
                                Thread.Sleep(1);
                                _currentProgress = _currentProgress + 1;
                                _onPropertyChanged("CurrentProgress");
                            }

                        if (line == "")
                            continue;

                        var values = line.Split(',');

                        if (values.Length != 2)
                            throw new Exception("Incorrect format of file");

                        var firstValue = BigInteger.Parse(values[0]);
                        var secondValue = BigInteger.Parse(values[1]);

                        var pair = new Pair<BigInteger, BigInteger>(firstValue, secondValue);
                        var data = DecryptData(pair, x, p);
                        var bytes = data.ToByteArray();
                        resultBytes.AddRange(bytes);
                    }

                    fileWriter.Write(resultBytes.ToArray(), 0, resultBytes.Count);
                }
            }
        }
    }
}
