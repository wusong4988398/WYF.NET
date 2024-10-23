using System;
using System.Text;

class Program
{
    static void Main()
    {
        string k = "000000000000000100001101000001111011111000111000101000100000";
        StringBuilder result = new StringBuilder();
        int results = 0;

        for (int i = 0; i < k.Length; i++)
        {
            results = i + 1;
            if (k[i] == '1')
            {
                result.Append("<font color='red'>").Append(results).Append("</font>");
            }
            else
            {
                result.Append(results).Append(",");
            }

            // 重置results
            results = 0;
        }

        Console.WriteLine(result.ToString());
        Console.ReadLine();
    }
}