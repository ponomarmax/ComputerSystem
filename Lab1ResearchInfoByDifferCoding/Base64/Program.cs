using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab1ResearchInfoByDifferCoding;

namespace Base64
{
    class Program
    {
        public static void ComparingFileAndArchive(string pathFile, string pathArchive)
        {
            var file = new StreamReader(pathFile);
            var archive = new BinaryReader(new FileStream(pathArchive, FileMode.Open));
            List<byte> a = new List<byte>();

            while (archive.BaseStream.Position != archive.BaseStream.Length)
                a.Add(archive.ReadByte());
            byte[] binaryFile = Encoding.GetEncoding(1200).GetBytes(file.ReadToEnd()),
                archiveFile = a.ToArray();

            char[] base64File = new Base64Encoder(binaryFile).GetEncoded(),
                base64Archive = new Base64Encoder(archiveFile).GetEncoded();
            var infoFile = Lab1ResearchInfoByDifferCoding.Program.FindAmountLetters(String.Join("",base64File));
            var infoArchive = Lab1ResearchInfoByDifferCoding.Program.FindAmountLetters(String.Join("", base64Archive));
            Console.WriteLine("Середня ентропія алфавіту для файлу = {0} bits",(int) infoFile.Item3);
            Console.WriteLine("Середня ентропія алфавіту для архіву = {0} bits", (int)infoArchive.Item3);
            Console.WriteLine("Кількість інформації в тексті файлу {0} bytes", (int)infoFile.Item4/8);
            Console.WriteLine("Кількість інформації в тексті архіву {0} bytes", (int)infoArchive.Item4/8);
        }
        static void Main(string[] args)
        {
            string file1 = "Мені тринадцятий минало.txt",
                file2 = "Казка про ріпку.txt",
                file3 = "PCI.txt",
                archive1 = file1 + ".bz2",
                archive2 = file2 + ".bz2",
                archive3 = file3 + ".bz2",
                path = @"E:\c3_2\ComputerSystem\Lab1ResearchInfoByDifferCoding\lab1files\";
            Console.OutputEncoding = Encoding.GetEncoding(1200);
            ComparingFileAndArchive(path + file1, path + archive1);
            Console.WriteLine();
            ComparingFileAndArchive(path + file2, path + archive2);
            Console.WriteLine();
            ComparingFileAndArchive(path + file3, path + archive3);
            //var res1 = Encoding.GetEncoding(1200).GetChars(new Base64Decoder(base64File).GetDecoded());
            //var enco2 = Convert.ToBase64String(binaryFile);
            //var res2 = Encoding.GetEncoding(1200).GetChars(Convert.FromBase64String(enco2));
            //Console.WriteLine(res1);
            //Console.WriteLine("==================================");
            //Console.WriteLine(res2);


        }
    }

    public class Base64Decoder
    {
        char[] source;
        int length, length2, length3;
        int blockCount;
        int paddingCount;
        public Base64Decoder(char[] input)
        {
            int temp = 0;
            source = input;
            length = input.Length;

            //find how many padding are there
            for (int x = 0; x < 2; x++)
                if (input[length - x - 1] == '=')
                    temp++;

            paddingCount = temp;
            //calculate the blockCount;
            //assuming all whitespace and carriage returns/newline were removed.
            blockCount = length / 4;
            length2 = blockCount * 3;
        }

        public byte[] GetDecoded()
        {
            byte[] buffer = new byte[length];//first conversion result
            byte[] buffer2 = new byte[length2];//decoded array with padding

            for (int x = 0; x < length; x++)
                buffer[x] = char2sixbit(source[x]);

            byte b, b1, b2, b3;
            byte temp1, temp2, temp3, temp4;

            for (int x = 0; x < blockCount; x++)
            {
                temp1 = buffer[x * 4];
                temp2 = buffer[x * 4 + 1];
                temp3 = buffer[x * 4 + 2];
                temp4 = buffer[x * 4 + 3];

                b = (byte)(temp1 << 2);
                b1 = (byte)((temp2 & 48) >> 4);
                b1 += b;

                b = (byte)((temp2 & 15) << 4);
                b2 = (byte)((temp3 & 60) >> 2);
                b2 += b;

                b = (byte)((temp3 & 3) << 6);
                b3 = temp4;
                b3 += b;

                buffer2[x * 3] = b1;
                buffer2[x * 3 + 1] = b2;
                buffer2[x * 3 + 2] = b3;
            }
            //remove paddings
            length3 = length2 - paddingCount;
            byte[] result = new byte[length3];

            for (int x = 0; x < length3; x++)
                result[x] = buffer2[x];

            return result;
        }

        private byte char2sixbit(char c)
        {
            char[] lookupTable = new char[64]
                {
    'A','B','C','D','E','F','G','H','I','J','K','L','M','N',
    'O','P','Q','R','S','T','U','V','W','X','Y', 'Z',
    'a','b','c','d','e','f','g','h','i','j','k','l','m','n',
    'o','p','q','r','s','t','u','v','w','x','y','z',
    '0','1','2','3','4','5','6','7','8','9','+','/'};
            if (c == '=')
                return 0;

            for (int x = 0; x < 64; x++)
                if (lookupTable[x] == c)
                    return (byte)x;

            //should not reach here
            return 0;
        }

    }

    /// <summary>
    /// Summary description for Base64Encoder.
    /// </summary>
    public class Base64Encoder
    {
        byte[] source;
        int length, length2;
        int blockCount;
        int paddingCount;
        public Base64Encoder(byte[] input)
        {
            source = input;
            length = input.Length;
            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);//need to add padding
                blockCount = (length + paddingCount) / 3;
            }
            length2 = length + paddingCount;//or blockCount *3
        }

        public char[] GetEncoded()
        {
            byte[] source2;
            source2 = new byte[length2];
            //copy data over insert padding
            for (int x = 0; x < length2; x++)
                if (x < length)
                    source2[x] = source[x];
                else
                    source2[x] = 0;

            byte b1, b2, b3,
             temp, temp1, temp2, temp3, temp4;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];
            for (int x = 0; x < blockCount; x++)
            {
                b1 = source2[x * 3];
                b2 = source2[x * 3 + 1];
                b3 = source2[x * 3 + 2];

                temp1 = (byte)((b1 & 252) >> 2);//first

                temp = (byte)((b1 & 3) << 4);
                temp2 = (byte)((b2 & 240) >> 4);
                temp2 += temp; //second

                temp = (byte)((b2 & 15) << 2);
                temp3 = (byte)((b3 & 192) >> 6);
                temp3 += temp; //third

                temp4 = (byte)(b3 & 63); //fourth

                buffer[x * 4] = temp1;
                buffer[x * 4 + 1] = temp2;
                buffer[x * 4 + 2] = temp3;
                buffer[x * 4 + 3] = temp4;

            }

            for (int x = 0; x < blockCount * 4; x++)
                result[x] = sixbit2char(buffer[x]);

            //covert last "A"s to "=", based on paddingCount
            switch (paddingCount)
            {
                case 0: break;
                case 1: result[blockCount * 4 - 1] = '='; break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default: break;
            }
            return result;
        }

        private char sixbit2char(byte b)
        {
            char[] lookupTable = new char[64]
                {  'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9','+','/'};

            if ((b >= 0) && (b <= 63))
                return lookupTable[b];
            //should not happen;
            return ' ';
        }
    }
}

