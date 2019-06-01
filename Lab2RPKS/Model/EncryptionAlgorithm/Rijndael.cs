using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

// https://referencesource.microsoft.com/#mscorlib/system/security/cryptography/rijndaelmanagedtransform.cs

namespace Lab2RPKS.Model.EncryptionAlgorithm
{
    public enum BlockSize
    {
        Size128 = 128, // 128 bits = 16 bytes
        Size192 = 192, // 192 bits = 24 bytes
        Size256 = 256, // 256 bits = 32 bytes
    }

    public enum KeySize
    {
        Size128 = 128, // 128 bits = 16 bytes
        Size192 = 192, // 192 bits = 24 bytes
        Size256 = 256, // 256 bits = 32 bytes
    }

    public static class RijndaelSizesConverter
    {
        public static int BlockSizeToInt(BlockSize blockSize)
        {
            {
                switch (blockSize)
                {
                    case BlockSize.Size128:
                        return 128;
                    case BlockSize.Size192:
                        return 192;
                    case BlockSize.Size256:
                        return 256;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, null);
                }
            }
        }

        public static int KeySizeToInt(KeySize keySize)
        {
            {
                switch (keySize)
                {
                    case KeySize.Size128:
                        return 128;
                    case KeySize.Size192:
                        return 192;
                    case KeySize.Size256:
                        return 256;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(keySize), keySize, null);
                }
            }
        }
    }

    public static class RijndaelTables
    {
        public static readonly byte[] SBox = new byte[] {
            99, 124, 119, 123, 242, 107, 111, 197,  48,   1, 103,  43, 254, 215, 171, 118,
            202, 130, 201, 125, 250,  89,  71, 240, 173, 212, 162, 175, 156, 164, 114, 192,
            183, 253, 147,  38,  54,  63, 247, 204,  52, 165, 229, 241, 113, 216,  49,  21,
            4, 199,  35, 195,  24, 150,   5, 154,   7,  18, 128, 226, 235,  39, 178, 117,
            9, 131,  44,  26,  27, 110,  90, 160,  82,  59, 214, 179,  41, 227,  47, 132,
            83, 209,   0, 237,  32, 252, 177,  91, 106, 203, 190,  57,  74,  76,  88, 207,
            208, 239, 170, 251,  67,  77,  51, 133,  69, 249,   2, 127,  80,  60, 159, 168,
            81, 163,  64, 143, 146, 157,  56, 245, 188, 182, 218,  33,  16, 255, 243, 210,
            205,  12,  19, 236,  95, 151,  68,  23, 196, 167, 126,  61, 100,  93,  25, 115,
            96, 129,  79, 220,  34,  42, 144, 136,  70, 238, 184,  20, 222,  94,  11, 219,
            224,  50,  58,  10,  73,   6,  36,  92, 194, 211, 172,  98, 145, 149, 228, 121,
            231, 200,  55, 109, 141, 213,  78, 169, 108,  86, 244, 234, 101, 122, 174,   8,
            186, 120,  37,  46,  28, 166, 180, 198, 232, 221, 116,  31,  75, 189, 139, 138,
            112,  62, 181, 102,  72,   3, 246,  14,  97,  53,  87, 185, 134, 193,  29, 158,
            225, 248, 152,  17, 105, 217, 142, 148, 155,  30, 135, 233, 206,  85,  40, 223,
            140, 161, 137,  13, 191, 230,  66, 104,  65, 153,  45,  15, 176,  84, 187,  22,
        };

        public static readonly byte[] InverseSBox = new byte[] {
            0x52, 0x09, 0x6a, 0xd5, 0x30, 0x36, 0xa5, 0x38, 0xbf, 0x40, 0xa3, 0x9e, 0x81, 0xf3, 0xd7, 0xfb,
            0x7c, 0xe3, 0x39, 0x82, 0x9b, 0x2f, 0xff, 0x87, 0x34, 0x8e, 0x43, 0x44, 0xc4, 0xde, 0xe9, 0xcb,
            0x54, 0x7b, 0x94, 0x32, 0xa6, 0xc2, 0x23, 0x3d, 0xee, 0x4c, 0x95, 0x0b, 0x42, 0xfa, 0xc3, 0x4e,
            0x08, 0x2e, 0xa1, 0x66, 0x28, 0xd9, 0x24, 0xb2, 0x76, 0x5b, 0xa2, 0x49, 0x6d, 0x8b, 0xd1, 0x25,
            0x72, 0xf8, 0xf6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xd4, 0xa4, 0x5c, 0xcc, 0x5d, 0x65, 0xb6, 0x92,
            0x6c, 0x70, 0x48, 0x50, 0xfd, 0xed, 0xb9, 0xda, 0x5e, 0x15, 0x46, 0x57, 0xa7, 0x8d, 0x9d, 0x84,
            0x90, 0xd8, 0xab, 0x00, 0x8c, 0xbc, 0xd3, 0x0a, 0xf7, 0xe4, 0x58, 0x05, 0xb8, 0xb3, 0x45, 0x06,
            0xd0, 0x2c, 0x1e, 0x8f, 0xca, 0x3f, 0x0f, 0x02, 0xc1, 0xaf, 0xbd, 0x03, 0x01, 0x13, 0x8a, 0x6b,
            0x3a, 0x91, 0x11, 0x41, 0x4f, 0x67, 0xdc, 0xea, 0x97, 0xf2, 0xcf, 0xce, 0xf0, 0xb4, 0xe6, 0x73,
            0x96, 0xac, 0x74, 0x22, 0xe7, 0xad, 0x35, 0x85, 0xe2, 0xf9, 0x37, 0xe8, 0x1c, 0x75, 0xdf, 0x6e,
            0x47, 0xf1, 0x1a, 0x71, 0x1d, 0x29, 0xc5, 0x89, 0x6f, 0xb7, 0x62, 0x0e, 0xaa, 0x18, 0xbe, 0x1b,
            0xfc, 0x56, 0x3e, 0x4b, 0xc6, 0xd2, 0x79, 0x20, 0x9a, 0xdb, 0xc0, 0xfe, 0x78, 0xcd, 0x5a, 0xf4,
            0x1f, 0xdd, 0xa8, 0x33, 0x88, 0x07, 0xc7, 0x31, 0xb1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xec, 0x5f,
            0x60, 0x51, 0x7f, 0xa9, 0x19, 0xb5, 0x4a, 0x0d, 0x2d, 0xe5, 0x7a, 0x9f, 0x93, 0xc9, 0x9c, 0xef,
            0xa0, 0xe0, 0x3b, 0x4d, 0xae, 0x2a, 0xf5, 0xb0, 0xc8, 0xeb, 0xbb, 0x3c, 0x83, 0x53, 0x99, 0x61,
            0x17, 0x2b, 0x04, 0x7e, 0xba, 0x77, 0xd6, 0x26, 0xe1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0c, 0x7d
        };

    public static readonly int[] Rcon = new int[] {
            0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36,
            0x6c, 0xd8, 0xab, 0x4d, 0x9a, 0x2f, 0x5e, 0xbc, 0x63, 0xc6,
            0x97, 0x35, 0x6a, 0xd4, 0xb3, 0x7d, 0xfa, 0xef, 0xc5, 0x91,
        };
    }

    // матрица из 4 строк и n столбцов
    public class ByteMatrix
    {
        protected static readonly int LinesAmount = 4;

        public ByteMatrix(int columns)
        {
            ColumnsAmount = columns;
            Matrix = new byte[LinesAmount][];

            for (var i = 0; i < LinesAmount; i++)
            {
                Matrix[i] = new byte[columns];
            }
        }

        public void SubBytes()
        {
            // В процедуре SubBytes, каждый байт в state заменяется соответствующим элементом в фиксированной 8-битной таблице поиска, S;
            // bij = S(aij).

            foreach (var line in Matrix)
            {
                for (var column = 0; column < ColumnsAmount; column++)
                {
                    line[column] = RijndaelTables.SBox[line[column]];
                }
            }
        }

        public void InverseSubBytes()
        {
            foreach (var line in Matrix)
            {
                for (var column = 0; column < ColumnsAmount; column++)
                {
                    line[column] = RijndaelTables.InverseSBox[line[column]];
                }
            }
        }

        public void ShiftRows()
        {
            // ShiftRows работает со строками State.
            // При этой трансформации строки состояния циклически сдвигаются на r байт по горизонтали в зависимости от номера строки.
            // Для нулевой строки r = 0, для первой строки r = 1 Б и т. д.
            //
            // Для алгоритма Rijndael паттерн смещения строк для 128- и 192-битных строк одинаков.
            // Однако для блока размером 256 (8 столбцов) бит отличается от предыдущих тем,
            // что 2-е, 3-и и 4-е строки смещаются на 1, 3 и 4 байта соответственно.

            LeftRotateLine(0, 0);
            LeftRotateLine(1, 1);

            if (ColumnsAmount != 8)
            {
                LeftRotateLine(2, 2);
                LeftRotateLine(3, 3);
            }
            else
            {
                LeftRotateLine(2, 3);
                LeftRotateLine(3, 4);
            }
        }

        public void InverseShiftRows()
        {
            RightRotateLine(0, 0);
            RightRotateLine(1, 1);

            if (ColumnsAmount != 8)
            {
                RightRotateLine(2, 2);
                RightRotateLine(3, 3);
            }
            else
            {
                RightRotateLine(2, 3);
                RightRotateLine(3, 4);
            }
        }

        // Galois Field (256) Multiplication of two Bytes
        private byte GMul(byte a, byte b)
        { 
            byte p = 0;

            for (var counter = 0; counter < 8; counter++)
            {
                if ((b & 1) != 0)
                {
                    p ^= a;
                }

                var hiBitSet = (a & 0x80) != 0;
                a <<= 1;
                if (hiBitSet)
                {
                    a = (byte)(a ^ 0x11B); /* x^8 + x^4 + x^3 + x + 1 */
                }
                b >>= 1;
            }

            return p;
        }

        public void MixColumns()
        {
            // В процедуре MixColumns четыре байта каждой колонки State смешиваются,
            // используя для этого обратимую линейную трансформацию.
            // MixColumns обрабатывает состояния по колонкам, трактуя каждую из них как полином третьей степени.
            // Над этими полиномами производится умножение в GF(2^{8}) по модулю x^{4}+1
            // на фиксированный многочлен c(x)=3x^{3}+x^{2}+x+2.

            for (var column = 0; column < ColumnsAmount; column++)
            {
                var num0 = (byte)GMul(2, Matrix[0][column]) ^ GMul(3, Matrix[1][column]) ^ GMul(1, Matrix[2][column]) ^ GMul(1, Matrix[3][column]);
                var num1 = (byte)GMul(1, Matrix[0][column]) ^ GMul(2, Matrix[1][column]) ^ GMul(3, Matrix[2][column]) ^ GMul(1, Matrix[3][column]);
                var num2 = (byte)GMul(1, Matrix[0][column]) ^ GMul(1, Matrix[1][column]) ^ GMul(2, Matrix[2][column]) ^ GMul(3, Matrix[3][column]);
                var num3 = (byte)GMul(3, Matrix[0][column]) ^ GMul(1, Matrix[1][column]) ^ GMul(1, Matrix[2][column]) ^ GMul(2, Matrix[3][column]);

                Matrix[0][column] = (byte)num0;
                Matrix[1][column] = (byte)num1;
                Matrix[2][column] = (byte)num2;
                Matrix[3][column] = (byte)num3;
            }
        }

        public void InverseMixColumns()
        {
            for (var column = 0; column < ColumnsAmount; column++)
            {
                var num0 = (byte)GMul(14, Matrix[0][column]) ^ GMul(11, Matrix[1][column]) ^ GMul(13, Matrix[2][column]) ^ GMul(9, Matrix[3][column]);
                var num1 = (byte)GMul(9, Matrix[0][column]) ^ GMul(14, Matrix[1][column]) ^ GMul(11, Matrix[2][column]) ^ GMul(13, Matrix[3][column]);
                var num2 = (byte)GMul(13, Matrix[0][column]) ^ GMul(9, Matrix[1][column]) ^ GMul(14, Matrix[2][column]) ^ GMul(11, Matrix[3][column]);
                var num3 = (byte)GMul(11, Matrix[0][column]) ^ GMul(13, Matrix[1][column]) ^ GMul(9, Matrix[2][column]) ^ GMul(14, Matrix[3][column]);

                Matrix[0][column] = (byte)num0;
                Matrix[1][column] = (byte)num1;
                Matrix[2][column] = (byte)num2;
                Matrix[3][column] = (byte)num3;
            }
        }

        public void AddRoundKey(ByteMatrix roundKey)
        {
            // В процедуре AddRoundKey RoundKey каждого раунда объединяется со State.
            // Для каждого раунда Roundkey получается из CipherKey c помощью процедуры KeyExpansion;
            // каждый RoundKey такого же размера, что и State.
            // Процедура производит побитовый XOR каждого байта State с каждым байтом RoundKey.

            for (var line = 0; line < LinesAmount; line++)
            {
                for (var column = 0; column < ColumnsAmount; column++)
                {
                    Matrix[line][column] ^= roundKey.Matrix[line][column];
                }
            }
        }
        
        // циклический сдвиг влево
        private void LeftRotateLine(int line, int amount)
        {
            if (amount == 0)
            {
                return;
            }

            var savedNumbers = new byte[amount];

            for (var i = 0; i < amount; i++)
            {
                savedNumbers[i] = Matrix[line][i];
            }

            for (var i = 0; i < ColumnsAmount - amount; i++)
            {
                Matrix[line][i] = Matrix[line][i + amount];
            }

            var savedIndex = 0;
            for (var i = ColumnsAmount - amount; i < ColumnsAmount; i++)
            {
                Matrix[line][i] = savedNumbers[savedIndex];
                savedIndex++;
            }
        }

        private void RightRotateLine(int line, int amount)
        {
            LeftRotateLine(line, ColumnsAmount - amount);
        }

        protected readonly int ColumnsAmount;
        protected readonly byte[][] Matrix;
    }

    // матрица из 4 строк и (размер блока / 32) колонок
    public class RijndaelState: ByteMatrix
    {
        public RijndaelState(BlockSize blockSize, byte[] block)
            : base(RijndaelSizesConverter.BlockSizeToInt(blockSize) / 32)
        {
            LoadFromBlock(block);
        }

        private void LoadFromBlock(byte[] block)
        {
            for (var row = 0; row < LinesAmount; row++)
            {
                for (var column = 0; column < ColumnsAmount; column++)
                {
                    Matrix[row][column] = block[row + LinesAmount * column];
                }
            }
        }

        public byte[] ToBytes()
        {
            var output = new byte[LinesAmount * ColumnsAmount];

            for (var row = 0; row < LinesAmount; row++)
            {
                for (var column = 0; column < ColumnsAmount; column++)
                {
                    output[row + LinesAmount * column] = Matrix[row][column];
                }
            }

            return output;
        }
    }

    // матрица из 4 строк и (размер ключа / 32) колонок
    public class RijndaelKey: ByteMatrix
    {
        public RijndaelKey(BlockSize blockSize, int[] keyExpansion, int offset)
            : base(RijndaelSizesConverter.BlockSizeToInt(blockSize) / 32)
        {
            LoadFromKeyExpansion(keyExpansion, offset);
        }

        private void LoadFromKeyExpansion(int[] keyExpansion, int offset)
        {
            for (var i = 0; i < ColumnsAmount; i++)
            {
                var currentKey = keyExpansion[offset + i];

                Matrix[0][i] = (byte)((currentKey >> 0) & 0x000000FF);
                Matrix[1][i] = (byte)((currentKey >> 8) & 0x000000FF);
                Matrix[2][i] = (byte)((currentKey >> 16) & 0x000000FF);
                Matrix[3][i] = (byte)((currentKey >> 24) & 0x000000FF);
            }
        }
    }

    public class Rijndael : EncryptionAlgorithm
    {
        #region Constructors

        public Rijndael(ref int currentProgress, BackgroundWorker worker, Action<string> onPropertyChanged)
            : base(ref currentProgress, worker, onPropertyChanged)
        {
            SetSettings(BlockSize.Size128, KeySize.Size128, new byte[16]);
        }

        #endregion

        #region Public Methods

        public void SetSettings(BlockSize blockSize, KeySize keySize, byte[] key)
        {
            _blockSize = blockSize;
            _keySize = keySize;
            _key = key;

            GenerateKeyExpansion();
        }

        public void EncryptFile(string inFileName, string outFileName)
        {
            using (var inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (var outFIle = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                {
                    ProcessFileANSIX923(inFile, outFIle, Encrypt, true);
                }
            }
        }

        public void DecryptFile(string inFileName, string outFileName)
        {
            using (var inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (var outFIle = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                {
                    ProcessFileANSIX923(inFile, outFIle, Decrypt, false);
                }
            }
        }

        void ProcessFileANSIX923(FileStream inFile, FileStream outFile, Func<byte[], byte[]> handler, bool isEncrypting)
        {
            var amountBytesInBlock = RijndaelSizesConverter.BlockSizeToInt(_blockSize) / 8;

            var fileSize = inFile.Length;
            var blocksAmount = fileSize / amountBytesInBlock;

            // при дешифровке блоков должно быть минимум 2 и размер должен быть кратен блоку
            var incorrectFile = (fileSize % amountBytesInBlock != 0) && !isEncrypting;
            if (incorrectFile)
            {
                throw new Exception("incorrect file format");
            }

            var readedBlocksAmount = 0;

            var inData = new byte[amountBytesInBlock];
            var outData = new byte[0];
            int readBytesAmount = 0;

            while (true)
            {
                readBytesAmount = inFile.Read(inData, 0, inData.Length);
                if (readBytesAmount != amountBytesInBlock)
                {
                    break;
                }

                readedBlocksAmount++;
                const int progressInterval = 100;
                if (readedBlocksAmount % progressInterval == 0)
                {
                    // progressWatcher((unsigned) ((readedBlocksAmount * 100.0) / blocksAmount));
                }

                outData = handler(inData);

                var isLast = blocksAmount == readedBlocksAmount;
                if (isLast && !isEncrypting)
                {
                    // последний блок при расшифровке прочитан
                    var amount = outData[amountBytesInBlock - 1];
                    outFile.Write(outData, 0, amount);
                }
                else
                {
                    outFile.Write(outData, 0, outData.Length);
                }

                inData = new byte[amountBytesInBlock];
            }

            var gCount = (byte)(inFile.Length % amountBytesInBlock); // сколько байт осталось в потоке

            if (isEncrypting)
            {
                // старшие биты пусты - в самый старший добавить (8 - gCount)

                if (gCount != amountBytesInBlock)
                {
                    inData[amountBytesInBlock - 1] = gCount;
                }

                //  зашифруем данные
                outData = handler(inData);
                outFile.Write(outData, 0, amountBytesInBlock);

                if (gCount == amountBytesInBlock)
                {
                    //  добавим gcount в конец файла зашифровав его
                    var gCountIn = new byte[amountBytesInBlock];
                    gCountIn[0] = gCount;
                    var gCountOut = handler(gCountIn);
                    outFile.Write(gCountOut, 0, amountBytesInBlock);
                }
            }
        }

        #endregion

       #region Private Fields

        private int[] _encryptKeyExpansion;
        private int[] _decryptKeyExpansion;

        private BlockSize _blockSize;
        private KeySize _keySize;
        private byte[] _key;

        #endregion

        #region Private Methods

        private byte[] Encrypt(byte[] block)
        {
            var roundsAmount = GetAmountRounds(_blockSize, _keySize);

            var state = new RijndaelState(_blockSize, block);
            var keys = new RijndaelKey[roundsAmount + 1];
            for (var i = 0; i < roundsAmount + 1; i++)
            {
                var offset = i * (_encryptKeyExpansion.Length / (roundsAmount + 1));
                keys[i] = new RijndaelKey(_blockSize, _encryptKeyExpansion, offset);
            }

            state.AddRoundKey(keys[0]);
            for (var i = 1; i < roundsAmount; i++)
            {
                state.SubBytes();
                state.ShiftRows();
                state.MixColumns();
                state.AddRoundKey(keys[i]);
            }

            state.SubBytes();
            state.ShiftRows();
            state.AddRoundKey(keys[roundsAmount]);

            return state.ToBytes();
        }

        private byte[] Decrypt(byte[] block)
        {
            var roundsAmount = GetAmountRounds(_blockSize, _keySize);

            var state = new RijndaelState(_blockSize, block);
            var keys = new RijndaelKey[roundsAmount + 1];
            for (var i = 0; i < roundsAmount + 1; i++)
            {
                var offset = i * (_encryptKeyExpansion.Length / (roundsAmount + 1));
                keys[i] = new RijndaelKey(_blockSize, _encryptKeyExpansion, offset);
            }

            state.AddRoundKey(keys[roundsAmount]);
            state.InverseShiftRows();
            state.InverseSubBytes();

            for (var i = roundsAmount - 1; i >= 1; i--)
            {
                state.AddRoundKey(keys[i]);
                state.InverseMixColumns();
                state.InverseShiftRows();
                state.InverseSubBytes();
            }

            state.AddRoundKey(keys[0]);

            return state.ToBytes();
        }

        private static int GetAmountRounds(BlockSize blockSize, KeySize keySize)
        {
            if (blockSize == BlockSize.Size128)
            {
                switch (keySize)
                {
                    case KeySize.Size128:
                        return 10;
                    case KeySize.Size192:
                        return 12;
                    case KeySize.Size256:
                        return 14;
                }
            }
            else if (blockSize == BlockSize.Size192)
            {
                switch (keySize)
                {
                    case KeySize.Size128:
                    case KeySize.Size192:
                        return 12;
                    case KeySize.Size256:
                        return 14;
                }
            }
            else if (blockSize == BlockSize.Size256)
            {
                return 14;
            }

            throw new Exception("Incorrect sizes");
        }

        private static int SubWord(int a)
        {
            return RijndaelTables.SBox[a & 0xFF] |
                   RijndaelTables.SBox[a >> 8 & 0xFF] << 8 |
                   RijndaelTables.SBox[a >> 16 & 0xFF] << 16 |
                   RijndaelTables.SBox[a >> 24 & 0xFF] << 24;
        }

        private static int Rot1(int val)
        {
            return (val << 8 & unchecked((int)0xFFFFFF00)) | (val >> 24 & unchecked((int)0x000000FF));
        }

        private static int Rot2(int val)
        {
            return (val << 16 & unchecked((int)0xFFFF0000)) | (val >> 16 & unchecked((int)0x0000FFFF));
        }

        private static int Rot3(int val)
        {
            return (val << 24 & unchecked((int)0xFF000000)) | (val >> 8 & unchecked((int)0x00FFFFFF));
        }

        private static int MulX(int x)
        {
            var u = x & unchecked((int)0x80808080);
            return ((x & unchecked((int)0x7f7f7f7f)) << 1) ^ ((u - (u >> 7 & 0x01FFFFFF)) & 0x1b1b1b1b);
        }

        private void GenerateKeyExpansion()
        {
            var blockSize = _blockSize;
            var keySize = _keySize;
            var key = _key;

            var blockSizeBits = RijndaelSizesConverter.BlockSizeToInt(blockSize);
            var nb = blockSizeBits / 32;
            var nr = GetAmountRounds(blockSize, keySize); // amount rounds
            var nk = RijndaelSizesConverter.KeySizeToInt(keySize) / 32; // amount of columns

            switch (blockSizeBits > key.Length * 8 ? blockSizeBits : key.Length * 8)
            {
                case 128:
                    nr = 10;
                    break;

                case 192:
                    nr = 12;
                    break;

                case 256:
                    nr = 14;
                    break;

                default:
                    throw new Exception("InvalidKeySize");
            }

            _encryptKeyExpansion = new int[nb * (nr + 1)];
            _decryptKeyExpansion = new int[nb * (nr + 1)];
            int iTemp;

            var index = 0;
            for (var i = 0; i < nk; ++i)
            {
                int i0 = key[index++];
                int i1 = key[index++];
                int i2 = key[index++];
                int i3 = key[index++];
                _encryptKeyExpansion[i] = i3 << 24 | i2 << 16 | i1 << 8 | i0;
            }

            if (nk <= 6)
            {
                for (var i = nk; i < nb * (nr + 1); ++i)
                {
                    iTemp = _encryptKeyExpansion[i - 1];

                    if (i % nk == 0)
                    {
                        iTemp = SubWord(Rot3(iTemp));
                        iTemp ^= RijndaelTables.Rcon[(i / nk) - 1];
                    }

                    _encryptKeyExpansion[i] = _encryptKeyExpansion[i - nk] ^ iTemp;
                }
            }
            else
            {
                for (var i = nk; i < nb * (nr + 1); ++i)
                {
                    iTemp = _encryptKeyExpansion[i - 1];

                    if (i % nk == 0)
                    {
                        iTemp = SubWord(Rot3(iTemp));
                        iTemp ^= RijndaelTables.Rcon[(i / nk) - 1];
                    }
                    else if (i % nk == 4)
                    {
                        iTemp = SubWord(iTemp);
                    }

                    _encryptKeyExpansion[i] = _encryptKeyExpansion[i - nk] ^ iTemp;
                }
            }

            for (var i = 0; i < nb; ++i)
            {
                _decryptKeyExpansion[i] = _encryptKeyExpansion[i];
                _decryptKeyExpansion[nb * nr + i] = _encryptKeyExpansion[nb * nr + i];
            }

            for (var i = nb; i < nb * nr; ++i)
            {
                var tmpKey = _encryptKeyExpansion[i];
                var mul02 = MulX(tmpKey);
                var mul04 = MulX(mul02);
                var mul08 = MulX(mul04);
                var mul09 = tmpKey ^ mul08;
                _decryptKeyExpansion[i] = mul02 ^ mul04 ^ mul08 ^ Rot3(mul02 ^ mul09) ^ Rot2(mul04 ^ mul09) ^ Rot1(mul09);
            }
            
            for (var i = 0; i < nr + 1; i++)
            {
                for (var k = 0; k < nb; k++)
                {
                    _decryptKeyExpansion[i * nb + k] = _encryptKeyExpansion[(nr - i) * nb + k];
                }
            }
        }

        #endregion
    }
}
