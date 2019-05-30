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
        private string out1File = @"6chipher.png";
        private string out2File = @"6dechipher.png";
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

            string answer = _encryptionAlgorithm.Encode(inFile, out1File,  47, 97);
            var ans = answer.Split(' ');

            _encryptionAlgorithm.Decipher(out1File, out2File,  (long)Convert.ToInt64(ans[0]), (long)Convert.ToInt64(ans[1]));

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
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void RijndaelTest()
        {
            int k = 0;
            bool result = true;
            Rijndael _encryptionAlgorithm = new Rijndael(ref k, null, null);

            Assert.IsTrue(false);
            /*   string answer = _encryptionAlgorithm.Encode(inFile, out1File, 47, 97);
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

               File.Delete(out1File);
               File.Delete(out2File);
               Assert.IsTrue(result);*/

        }
        [TestMethod]
        public void RabinTest()
        {
          
           Assert.IsTrue(false);
        }
        [TestMethod]
        public void AlGamalTest()
        {

            Assert.IsTrue(false);
        }

       
    }
}
