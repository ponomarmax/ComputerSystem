using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab1ResearchInfoByDifferCoding
{
    public class Program
    {
        public static Tuple<SortedDictionary<char, int>, SortedDictionary<char, double>, double, double> FindAmountLetters(string text)
        {
            //Regex escapeMark = new Regex(@"\s");
            //text = escapeMark.Replace(text.ToString(), "");

            SortedDictionary<char, int> alphabet = new SortedDictionary<char, int>();
            SortedDictionary<char, double> frequency = new SortedDictionary<char, double>();

            foreach (char c in text)
                if (alphabet.ContainsKey(Char.ToLower(c)))
                    alphabet[Char.ToLower(c)]++;
                else
                    alphabet.Add(Char.ToLower(c), 1);

            int howManyLetters = alphabet.Values.Sum();

            foreach (char c in alphabet.Keys)
                frequency.Add(c, alphabet[c] * 1.0 / howManyLetters);

            double middleEntropy = 0;
            double log;
            foreach (char c in frequency.Keys)
            {
                log = Math.Log(frequency[c], 2);
                if (log != double.NaN && !double.IsInfinity(log))
                    middleEntropy -= frequency[c] * log;
            }

            double amountInfoInText = middleEntropy * howManyLetters;
            return Tuple.Create(alphabet, frequency, middleEntropy, amountInfoInText);
        }
        public static void Output(string path, bool printFrequency = true)
        {
            var i = FindAmountLetters(File.ReadAllText(path));
            Console.WriteLine("Середня ентропія алфавіту для цього тексту = {0} bits", (int)i.Item3);
            Console.WriteLine("Кількість інформації в тексті {0} bytes", (int)i.Item4 / 8);
            Console.WriteLine("Частоту кожного символу: ");
            if (printFrequency)
                foreach (char c in i.Item2.Keys)
                    Console.WriteLine("{0} = {1:0.00000}",c, i.Item2[c]);
        }
        static void Main(string[] args)
        {
            string file1 = "Мені тринадцятий минало.txt",
                file2 = "Казка про ріпку.txt",
                file3 = "PCI.txt",
                path = @"E:\c3_2\ComputerSystem\Lab1ResearchInfoByDifferCoding\lab1files\";
            Console.OutputEncoding = Encoding.GetEncoding(1200);

            Output(path + file1);
            Output(path + file2);
            Output(path + file3);
        }
    }
}
