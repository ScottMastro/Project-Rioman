using System;
using System.Drawing;
using System.IO;

namespace WindowsFormsApplication1
{
    static class Encrypter
    {
        public static void Encrypt(int[,] level, int width, int height, string location, Color bgcolour)
        {
            string[] letters = new string[501];
            SetLetters(letters);

            StreamWriter write = new StreamWriter(location);
            write.WriteLine(width.ToString() + "&" + height.ToString() + "&" + bgcolour.R.ToString() + "&" + bgcolour.G.ToString() + "&" + bgcolour.B.ToString());

            string line = "";

            for (int y = 0; y <= height - 1; y++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    if (level[x, y] == 0)
                        line += "%%";
                    else
                        line += letters[level[x, y]];
                }

                write.WriteLine(line);
                line = "";
            }

            write.Close();
        }

        public static int[,] Decrypt(int[,] level, ref int width, ref int height, string location, ref Color bgcolour)
        {
            string[] letters = new string[501];
            SetLetters(letters);

            StreamReader read = new StreamReader(location);
            string rc = read.ReadLine();
            string[] parts = rc.Split('&');
            width = Convert.ToInt32(parts[0]);
            height = Convert.ToInt32(parts[1]);
            bgcolour = Color.FromArgb(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]), Convert.ToInt32(parts[4]));


            level = new int[width + 1, height + 1];

            string line = "";

            for (int y = 0; y <= height - 1; y++)
            {
                line = read.ReadLine();

                for (int x = 0; x <= width * 2 - 2; x += 2)
                {
                    if (line.Substring(x, 2) == "%%")
                        level[x/2, y] = 0;
                    else
                    {
                        for (int i = 1; i <= 500; i++)
                        {
                            if (letters[i] == line.Substring(x, 2))
                                level[x/2, y] = i;
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
