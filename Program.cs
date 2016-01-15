using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_GOST_34._10_2012
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger p = new BigInteger("6277101735386680763835789423207666416083908700390324961279", 10);
            BigInteger a = new BigInteger("-3", 10);
            BigInteger b = new BigInteger("64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1", 16);
            byte[] xG = fromHexStringToByte("03188da80eb03090f67cbf20eb43a18800f4ff0afd82ff1012");
            BigInteger n = new BigInteger("ffffffffffffffffffffffff99def836146bc9b1b4d22831", 16);
            
            CDS DS = new CDS(p, a, b, n, xG);
            BigInteger d = DS.genPrivateKey(192);
            CECPoint Q = DS.genPublicKey(d);
            CStribog hash = new CStribog(256);
            /*
            byte[] H = hash.GetHash(Encoding.Default.GetBytes("Message"));
            string sign = DS.genDS(H, d);

            byte[] H2 = hash.GetHash(Encoding.Default.GetBytes("message"));
            string sign2 = DS.genDS(H2, d);
            */

            Console.WriteLine("Выберите файл содержащий сообщение: ");
            String path=Console.ReadLine();
            String message = readFile(path);
            byte[] H = hash.GetHash(Encoding.Default.GetBytes(message));
            string sign = DS.genDS(H, d);
            Console.WriteLine("Сообщение \"{0}\" имеет следующую ЭЦП: {1}", message, sign);
            Console.WriteLine("Выберите файл для сохранение ЭЦП: ");
            path = Console.ReadLine();
            writeFile(path, sign);

            Console.WriteLine("Выберите файл для верификации сообщения: ");
            path = Console.ReadLine();
            String message2 = readFile(path);
            byte[] H2 = hash.GetHash(Encoding.Default.GetBytes(message2));
            Console.WriteLine("Выберите файл содержащий цифровую подпись: ");
            path = Console.ReadLine();
            string signVer= readFile(path);
            //string sign2 = DS.genDS(H2, d);

            bool result = DS.verifDS(H2, signVer, Q);

            if (result)
                System.Console.WriteLine("Верификация прошла успешно. Цифровая подпись верна.");
            else
                System.Console.WriteLine("Верификация не прошла! Цифровая подпись не верна.");

            System.Console.ReadLine();
        }

        private static byte[] fromHexStringToByte(string input)
        {
            byte[] data = new byte[input.Length / 2];
            string HexByte = "";
            for (int i = 0; i < data.Length; i++)
            {
                HexByte = input.Substring(i * 2, 2);
                data[i] = Convert.ToByte(HexByte, 16);
            }
            return data;
        }

        private static string readFile(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            return (text);
        }

        private static void writeFile(string path, string text)
        {
            System.IO.File.WriteAllText(path, text);
        }
    }
}
