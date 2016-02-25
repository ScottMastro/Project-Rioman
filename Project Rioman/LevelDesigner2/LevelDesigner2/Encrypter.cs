using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    static class Encrypter
    {
        public static void Encrypt(int[,] level, int row, int column, string location, Color bgcolour)
        {
            string[] letters = new string[501];
            SetLetters(letters);

            StreamWriter write = new StreamWriter(location);
            write.WriteLine(row.ToString() + "&" + column.ToString() + "&" + bgcolour.R.ToString() + "&" + bgcolour.G.ToString() + "&" + bgcolour.B.ToString());

            string line = "";

            for (int r = 0; r <= row - 1; r++)
            {
                for (int c = 0; c <= column - 1; c++)
                {
                    if (level[r, c] == 0)
                        line += "%%";
                    else
                        line += letters[level[r, c]];
                }

                write.WriteLine(line);
                line = "";
            }

            write.Close();
        }

        public static int[,] Decrypt(int[,] level, ref int row, ref int column, string location, ref Color bgcolour)
        {
            string[] letters = new string[501];
            SetLetters(letters);

            StreamReader read = new StreamReader(location);
            string rc = read.ReadLine();
            string[] parts = rc.Split('&');
            row = Convert.ToInt32(parts[0]);
            column = Convert.ToInt32(parts[1]);
            bgcolour = Color.FromArgb(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]));


            level = new int[row + 1, column + 1];

            string line = "";

            for (int r = 0; r <= row - 1; r++)
            {
                line = read.ReadLine();

                for (int c = 0; c <= column * 2 - 2; c += 2)
                {
                    if (line.Substring(c, 2) == "%%")
                        level[r, c / 2] = 0;
                    else
                    {
                        for (int i = 1; i <= 500; i++)
                        {
                            if (letters[i] == line.Substring(c, 2))
                                level[r, c / 2] = i;
                        }
                    }
                }

                line = "";
            }

            read.Close();

            return level;
        }

        private static void SetLetters(string[] letters)
        {
            string crypt = "ͺͻͼͽ;΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϰϪЖ";

            int gap = 1;
            int counter = 0;

            for (int i = 1; i <= 500; i++)
            {
                letters[i] = crypt.Substring(counter, 1) + crypt.Substring(counter + gap, 1);

                counter++;

                if (counter + gap > 99)
                {
                    counter = 0;
                    gap++;
                }
            }
        }
    }
}
