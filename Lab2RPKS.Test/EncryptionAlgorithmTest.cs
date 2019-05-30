using System;
using System.IO;
using Lab2RPKS.Model.EncryptionAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab2RPKS.Test
{
    [TestClass]
    public class EncryptionAlgorithmTest
    {

        private string inFile = @"C:\Users\aleks\Desktop\test rpks\6.png";
        private string out1File = @"C:\Users\aleks\Desktop\test rpks\62.png";
        private string out2File = @"C:\Users\aleks\Desktop\test rpks\63.png";
        [TestInitialize]
        public void SetupContext()
        {
        }

        [TestMethod]
        public void RSATest()
        {
            int k = 0;

            bool result = true;
            RSA _encryptionAlgorithm = new RSA(ref k, null, null);
            string answer = _encryptionAlgorithm.Encode(inFile, out1File, 47, 97);
            var ans = answer.Split(' ');

            _encryptionAlgorithm.Decipher(out1File, out2File, (long)Convert.ToInt64(ans[0]), (long)Convert.ToInt64(ans[1]));
            try
            {
                using (FileStream fsread1 = new FileStream(inFile, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream fsread2 = new FileStream(out2File, FileMode.Open, FileAccess.Read))
                    {

                        if (fsread1.Length != fsread2.Length)
                        {
                            result = false;
                        }
                        else
                            for (int i = 0; i < fsread1.Length; i++)
                            {
                                if (fsread1.ReadByte() != fsread2.ReadByte())
                                {
                                    result = false;
                                    break;

                                }
                            }

                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }

            //  File.Delete(out1File);
            // File.Delete(out2File);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void RijndaelTest()
        {
            int k = 0;
            Assert.IsTrue(Test(new Rijndael(ref k, null, null)));
        }
        [TestMethod]
        public void RabinTest()
        {
            int k = 0;
            Assert.IsTrue(Test(new Rabin(ref k, null, null)));
        }
        [TestMethod]
        public void AlGamalTest()
        {
            int k = 0;
            Assert.IsTrue(Test(new AlGamal(ref k, null, null)));
        }

        private bool Test(EncryptionAlgorithm _encryptionAlgorithm, params object[] list)
        {
            bool result = true;
            // _encryptionAlgorithm.Start(inFile, out1File, ModeEncryption.Encrypt, list);
            // _encryptionAlgorithm.Start(out1File, out2File, ModeEncryption.Decipher, list);
            try
            {
                using (FileStream fsread1 = new FileStream(inFile, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream fsread2 = new FileStream(out2File, FileMode.Open, FileAccess.Read))
                    {

                        if (fsread1.Length != fsread2.Length)
                        {
                            result = false;
                        }
                        else
                            for (int i = 0; i < fsread1.Length; i++)
                            {
                                if (fsread1.ReadByte() != fsread2.ReadByte())
                                {
                                    result = false;
                                    break;

                                }
                            }

                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }

            File.Delete(out1File);
            File.Delete(out2File);
            return result;
        }
    }


}
