using System;
using System.Collections;

namespace Lab2BinaryOperation
{
    class Program
    {
        static BitArray Add(BitArray l, BitArray r)
        {
            BitArray result = new BitArray(l.Length);

            bool remembered = false;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = !l[i] && !r[i] && remembered ||
                    l[i] && !r[i] && !remembered ||
                    !l[i] && r[i] && !remembered ||
                    l[i] && r[i] && remembered;//xor

                remembered = r[i] && l[i] || //if we have 1 1
                    (r[i] || l[i]) && remembered; //or we have one 1 and remember from last iteration
            }

            return result;
        }
        static BitArray Multiplication(BitArray multiplier, BitArray Multiplicand)
        {
            Console.WriteLine("Multiplier");
            Print(multiplier);
            Console.WriteLine("Multiplicand");
            Print(Multiplicand);

            BitArray result = new BitArray(multiplier.Count),
               temp = (BitArray)multiplier.Clone(); ;
            int shift = 0;
            foreach (bool item in Multiplicand)
            {
                if (item)
                {
                    Console.WriteLine("Step " + shift + 1);
                    result = Add(result, temp.LeftShift(shift));
                    temp.RightShift(shift);
                    Console.WriteLine("Partial product for relative shift " + shift);
                    Print(result);
                }
                shift++;
            }
            //bool substraction = l[l.Length - 1] || r[r.Length - 1];
            //if (substraction)
            //{
            //    result = Add(result, new BitArray(BitConverter.GetBytes(-1)));
            //    for (int i = 0; i < result.Length; i++)
            //        result[i] = !result[i];
            //}

            return result;
        }
        static BitArray InverseCode(BitArray bitArray)
        {
            BitArray result = new BitArray(bitArray.Count);
            for (int i = 0; i < result.Length; i++)
                result[i] = !bitArray[i];
            result = Add(result, new BitArray(BitConverter.GetBytes(1)));
            return result;
        }
        //static bool IsRemainderEmpty(BitArray remainder)
        //{
        //    foreach (bool e in remainder)
        //        if (e)
        //            return false;
        //    return true;
        //}
        static void Print(BitArray q)
        {
            foreach (bool item in q)
            {
                Console.Write(item ? 1 : 0);
            }
            Console.WriteLine();
            Console.WriteLine("=======");
        }
        static BitArray Division(BitArray dividend, BitArray divisor)
        {
            BitArray remainder = dividend.LeftShift(1),
                result = new BitArray(dividend.Count);
            int shiftDivisorOn = 0;
            for (int j = divisor.Count - 1; j >= 0; j--)
            {
                if (dividend[j])
                {
                    shiftDivisorOn = j;
                    break;
                }
            }
            BitArray shiftedDiv = divisor.LeftShift(shiftDivisorOn + 1);
            Console.WriteLine("Initial remainder");
            Print(remainder);
            result = result.LeftShift(1);
            bool minus = true;
            for (int i = 0; i < shiftDivisorOn + 1; i++)
            {
                if (minus)
                {
                    Console.WriteLine("remainder-=divisor");
                    remainder = Add(remainder, InverseCode(shiftedDiv));
                }
                else
                {
                    Console.WriteLine("remainder+=divisor");
                    remainder = Add(remainder, shiftedDiv);
                }

                Print(remainder);
                result.LeftShift(1);
                if (!remainder[remainder.Length - 1])
                {
                    result[0] = true;
                    Console.WriteLine("remainder>0 ");
                    remainder[0] = true;
                    minus = true;
                }
                else
                {
                    result[0] = false;
                    Console.WriteLine("remainder<0 ");
                    minus = false;
                }
                remainder.LeftShift(1);

                Print(remainder);
                Print(result);
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine();

            }
            return result;
        }
        static int FromBitToInt(BitArray array)
        {
            byte[] shiftA = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                shiftA[i] = (byte)(array[i] ? 1 : 0);
            }
            return BitConverter.ToInt32(shiftA, 0);
        }
        static float FromBitToFloat(BitArray array)
        {
            byte[] shiftA = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                shiftA[array.Length - 1 - i] = (byte)(array[i] ? 1 : 0);
            }
            return (float)BitConverter.ToDouble(shiftA, 0);
        }
        static BitArray AddFloat(BitArray l, BitArray r)
        {
            Console.WriteLine("Left operand");
            Print(l);
            Console.WriteLine("Right operand");
            Print(r);
            //getting exponent
            BitArray lExp = new BitArray(8), rExp = new BitArray(8);
            for (int i = 0; i < lExp.Length; i++)
            {
                lExp[i] = l[i + 23];
                rExp[i] = r[i + 23];
            }
            Console.WriteLine("Exponent");
            Print(lExp);
            Print(rExp);
            //findind what number we need to shift 
            int shift = FromBitToInt(Add(lExp, InverseCode(rExp)));
            //Console.WriteLine(shift);
            //getting mantisa
            BitArray lMantisa = new BitArray(24), rMantisa = new BitArray(24);
            for (int i = 0; i < lMantisa.Length - 1; i++)
            {
                lMantisa[i] = l[i];
                rMantisa[i] = r[i];
            }
            lMantisa[lMantisa.Length - 1] = true;
            rMantisa[rMantisa.Length - 1] = true;
            Console.WriteLine("Mantisa");
            Print(lMantisa);
            Print(rMantisa);

            if (shift > 0)
                rMantisa.RightShift(shift);
            else
                lMantisa.RightShift(shift);
            Console.WriteLine("Alignment Step");
            Print(lMantisa);
            Print(rMantisa);

            BitArray nMantisa = Add(lMantisa, rMantisa);
            Console.WriteLine("Result mantisa");
            Print(nMantisa);
            BitArray result = new BitArray(32);
            for (int i = 0; i < 23; i++)
            {
                result[i] = nMantisa[i];
            }
            BitArray biggerExp;
            if (shift >= 0)
                biggerExp = lExp;
            else
                biggerExp = rExp;
            Console.WriteLine("Result exponent");
            Print(biggerExp);
            for (int i = 23; i < 31; i++)
            {
                result[i] = biggerExp[i - 23];
            }
            result[31] = false;
            Console.WriteLine("Result");
            return result;


        }
        static void Main(string[] args)
        {
            float first, second;
            Console.WriteLine("Input first number");
            first = float.Parse(Console.ReadLine());
            Console.WriteLine("Input second number");
            second = float.Parse(Console.ReadLine());

            //int first, second;
            //Console.WriteLine("Input first number");
            //first = int.Parse(Console.ReadLine());
            //Console.WriteLine("Input second number");
            //second = int.Parse(Console.ReadLine());
            BitArray temp,
                f = new BitArray(BitConverter.GetBytes(first)),
             s = new BitArray(BitConverter.GetBytes(second)),
             result = new BitArray(f.Count)
             ;
            temp = (BitArray)f.Clone();
            //Print(f);
            //Print(s);
            // Print(f);
            //Print(s);

            //Print(Multiplication(f, s));

            // Print(Division(f, s));
            Print(AddFloat(f, s));
            Console.WriteLine("Result");
            Console.WriteLine();
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
