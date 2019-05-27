using System;
using System.IO;
using Lab2RPKS.Model.EncryptionAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab2RPKS.Test
{
    [TestClass]
    public class EncryptionAlgorithmTest
    {
       
        private string inFile = @"6.png";
        private string out1File = @"6out1.png";
        private  string out2File = @"6out2.png";
        [TestInitialize]
        public void SetupContext()
        {  
        }

        [TestMethod]
        public void RSATest()
        {
            int k = 0;
            Assert.IsTrue(Test(new RSA(ref k, null, null)));
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

        private bool Test(EncryptionAlgorithm _encryptionAlgorithm)
        {
            bool result = true;
            _encryptionAlgorithm.Start(inFile, out1File, "keykeykey", ModeEncryption.Encrypt);
            _encryptionAlgorithm.Start(out1File, out2File, "keykeykey", ModeEncryption.Decipher);
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
