using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Color bgcolour = Color.White;

        List<int[,]> undoTilesets = new List<int[,]>();
        List<int[,]> redoTilesets = new List<int[,]>();

        bool hasLevel = false;
        Point[,] point;
        int width;
        int height;
        int[,] tile;

        int[,] previousTiles;
        int[,] previousMouse;

        Image[] tiles;
        int activetile;
        Graphics g;

        const int WIDTH = 16;
        const int HEIGHT = 16;

        bool mouseDown;
        bool shiftDown;

        Point mouseDownPos;

        Image[] BunnyMan;
        Image[] AuroraMan;
        Image[] CloverMan;
        Image[] InfernoMan;
        Image[] GeoGirl;
        Image[] LurkerMan;
        Image[] PosterMan;
        Image[] ToxicMan;
        Image[] CCM;
        Image[] Oli;
        Image[] DeezKirb;
        Image[] Mushie;

        Image[] Characters;
        Image[] Scroller;
        Image[] Enemies;
        Image[] Other;

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAllImages(500);
            UpdateTileset("ENM");
        }

        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogbox dialog = new dialogbox();
            dialog.ShowDialog();

            if (dialog.DialogResult == DialogResult.OK)
            {
                height = dialog.GetHeight;
                width = dialog.GetWidth;

                tile = new int[width + 1, height + 1];

                Start();
            }
        }

        private int[,] CopyArray(int[,] array, int w, int h)
        {
            int[,] newArray = new int[w, h];

            for (int x = 0; x <= w - 1; x++)
                for (int y = 0; y <= h - 1; y++)
                    newArray[x, y] = array[x, y];

            return newArray;
        }

        private void Start()
        {

            panel.Width = width * 16;
            panel.Height = height * 16;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = bgcolour;


            g = panel.CreateGraphics();

            point = new Point[width + 1, height + 1];

            previousTiles = new int[width + 1, height + 1];
            previousMouse = new int[width + 1, height + 1];

            hasLevel = true;

            panel.BackColor = bgcolour;


            for (int x = 0; x <= width - 1; x++)
            {
                for (int y = 0; y <= height - 1; y++)
                {
                    point[x, y] = new Point(x * WIDTH, y * HEIGHT);
                }
            }
        }


        private void FullRefreshLevel()
        {
            if (hasLevel)
            {

                for (int x = 0; x <= width - 1; x++)
                    for (int y = 0; y <= height - 1; y++)
                        DrawSquare(x, y);
                

                DrawMouseLocation();


                if (shiftDown)
                    DrawRectangles();

            }
        }

        private void PartialRefreshLevel()
        {
            if (hasLevel)
            {

                int[,] mouse = GetMouseTiles();

                
                for (int x = 0; x <= width - 1; x++)
                    for (int y = 0; y <= height - 1; y++)
                    {
                        try
                        {
                            if (previousTiles[x, y] != tile[x, y] || previousMouse[x, y] == 1 && mouse[x, y] == 0)
                                DrawSquare(x, y);

                            if (previousMouse[x, y] == 0 && mouse[x, y] == 1)
                                g.FillRectangle(new SolidBrush(Color.FromArgb(128, 255, 0, 0)), new Rectangle(x * WIDTH, y * HEIGHT, WIDTH, HEIGHT));

                        }
                        catch (IndexOutOfRangeException e)
                        {
                            previousMouse = mouse;
                            previousTiles = tile;
                            return;
                        }


                    }

                previousMouse = mouse;
                previousTiles = tile;

                if (shiftDown)
                    DrawRectangles();
            }
        }

        private void DrawRectangles()
        {
            for (int x = 0; x <= width - 1; x++)
                for (int y = 0; y <= height - 1; y++)
                {
                    if (tile[x, y] == 322)
                    {
                        int x2 = 0, y2 = 0;

                        int counter = x;
                        while(counter <= width)
                        {
                            if (tile[counter, y] == 323)
                                x2 = counter;
                            counter++;
                        }

                        counter = y;
                        while (counter <= height)
                        {
                            if (tile[x, counter] == 325)
                                y2 = counter;
                            counter++;
                        }

                        g.DrawRectangle(new Pen(Brushes.Red, 3), new Rectangle((x+1) * WIDTH, (y+1) * HEIGHT,
                            (x2 - x - 1) * WIDTH, (y2 - y - 1) * HEIGHT));

                    }

                    if (tile[x, y] == 326)
                    {
                        int x2 = 0, y2 = 0;

                        int counter = x;
                        while (counter < width)
                        {
                            if (tile[counter, y] == 327)
                                x2 = counter;
                            counter++;
                        }

                        counter = y;
                        while (counter < height)
                        {
                            if (tile[x, counter] == 329)
                                y2 = counter;
                            counter++;
                        }

                        g.DrawRectangle(new Pen(Brushes.Green, 3), new Rectangle((x + 1) * WIDTH, (y + 1) * HEIGHT,
                            (x2 - x - 1) * WIDTH, (y2 - y - 1) * HEIGHT));

                    }

                }
        }


        private void DrawSquare(int x, int y)
        {
            if (tile[x, y] > 0)
                g.DrawImage(tiles[tile[x, y]], new Point(point[x, y].X, point[x, y].Y));
            else
                g.FillRectangle(new SolidBrush(bgcolour), new Rectangle(point[x, y].X, point[x, y].Y, WIDTH, HEIGHT));

        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (hasLevel)
            {
                mouseDownPos = TopLeftPoint(new Point(e.X, e.Y));
                mouseDown = true;
            }

        }

        private void panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (hasLevel)
            {

                mouseDown = false;

                int[,] oldTiles = CopyArray(tile, width, height);

                Point pos = GetMouseLocation();

                int x1 = pos.X / WIDTH;
                int x2 = mouseDownPos.X / WIDTH;
                int y1 = pos.Y / HEIGHT;
                int y2 = mouseDownPos.Y / HEIGHT;

                for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
                    for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
                    {
                        if(e.Button == MouseButtons.Left)
                            tile[i, j] = activetile;
                        else if (e.Button == MouseButtons.Right)
                            tile[i, j] = 0;
                    }


                if (!oldTiles.Equals(tile)) {
                    undoTilesets.Add(oldTiles);
                    redoTilesets = new List<int[,]>();
                }


                PartialRefreshLevel();
            }
        }

        private Point GetMouseLocation()
        {
            Point pos = panel.PointToClient(Cursor.Position);
            pos = TopLeftPoint(pos);

            if (pos.X < 0 || pos.X > width * WIDTH ||
                pos.Y < 0 || pos.Y > height * HEIGHT)
                return new Point(0,0);

            return pos;
        }


        private void DrawMouseLocation()
        {
            if (hasLevel)
            {
                Point pos;
                if (!mouseDown)
                {
                    pos = GetMouseLocation();

                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 255, 0, 0)), new Rectangle(pos.X, pos.Y, WIDTH, HEIGHT));
                }
                else
                {
                    pos = GetMouseLocation();


                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 255, 0)),
                        new Rectangle(Math.Min(mouseDownPos.X, pos.X),
                        Math.Min(mouseDownPos.Y, pos.Y),
                        Math.Abs(pos.X - mouseDownPos.X),
                        Math.Abs(pos.Y - mouseDownPos.Y)));


                }
            }
        }

        private int[,] GetMouseTiles()
        {
            int[,] mouse = new int[width + 1, height + 1];
            if (!mouseDown)
            {
                Point pos = GetMouseLocation();
                mouse[pos.X / WIDTH, pos.Y / HEIGHT] = 1;
            }
            else
            {
                Point pos = GetMouseLocation();


                int x1 = pos.X / WIDTH;
                int x2 = mouseDownPos.X / WIDTH;
                int y1 = pos.Y / HEIGHT;
                int y2 = mouseDownPos.Y / HEIGHT;

                for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
                    for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
                        mouse[i, j] = 1;

            }
            return mouse;
        }

        private Point TopLeftPoint(Point p)
        {
            return new Point((int)(p.X / WIDTH) * WIDTH, (int)(p.Y / HEIGHT) * HEIGHT);

        }

        private void refresher_Tick(object sender, EventArgs e)
        {
            PartialRefreshLevel();

            if (undoTilesets.Count > 10)
                undoTilesets.RemoveAt(0);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            FullRefreshLevel();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            FullRefreshLevel();
        }

        private void panelpanel_Scroll(object sender, ScrollEventArgs e)
        {
            FullRefreshLevel();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            FullRefreshLevel();
        }

        private void ChangeActiveTile(Image tle)
        {
            if (tle != null)
            {
                for (int i = 1; i <= 500; i++)
                {
                    if (tle == tiles[i])
                    {
                        activetile = i;
                    }
                }
            }
            picactivetile.Image = tle;
            numLabel.Text = activetile.ToString();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (hasLevel)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z && undoTilesets.Count > 0)
                {
                    redoTilesets.Add(tile);
                    tile = undoTilesets[undoTilesets.Count - 1];
                    undoTilesets.RemoveAt(undoTilesets.Count - 1);

                    PartialRefreshLevel();
                }

                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y && redoTilesets.Count > 0)
                {
                    undoTilesets.Add(tile);
                    tile = redoTilesets[redoTilesets.Count - 1];
                    redoTilesets.RemoveAt(redoTilesets.Count - 1);

                    PartialRefreshLevel();
                }

                if (e.KeyCode == Keys.S)
                {
                    MessageBox.Show(undoTilesets.Count.ToString() + " " + redoTilesets.Count.ToString());
                }

                if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Shift)
                {
                    if(!shiftDown)
                        FullRefreshLevel();

                    shiftDown = true;
                }

            }

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Shift)
            {
                shiftDown = false;
                FullRefreshLevel();
            }
        }

        private void changeColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            bgcolour = colorDialog1.Color;

            panel.BackColor = bgcolour;
            FullRefreshLevel();
        }


        private void exportToTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Save Level";
            saveFileDialog1.FileName = "level";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";

            saveFileDialog1.ShowDialog();
        }

        private void importFromTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open Level";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            openFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Encrypter.Encrypt(tile, width, height, saveFileDialog1.FileName, bgcolour);

            MessageBox.Show("Saved to " + saveFileDialog1.FileName);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            tile = Encrypter.Decrypt(tile, ref width, ref height, openFileDialog1.FileName, ref bgcolour);
            MessageBox.Show("Loaded from " + openFileDialog1.FileName);
            Start();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            help help = new help();

            help.ShowDialog();
        }

        private void UpdateTileset(string type)
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            pictureBox6.Image = null;
            pictureBox7.Image = null;
            pictureBox8.Image = null;
            pictureBox9.Image = null;
            pictureBox10.Image = null;
            pictureBox11.Image = null;
            pictureBox12.Image = null;
            pictureBox13.Image = null;
            pictureBox14.Image = null;
            pictureBox15.Image = null;
            pictureBox16.Image = null;
            pictureBox17.Image = null;
            pictureBox18.Image = null;
            pictureBox19.Image = null;
            pictureBox20.Image = null;
            pictureBox21.Image = null;
            pictureBox22.Image = null;
            pictureBox23.Image = null;
            pictureBox24.Image = null;
            pictureBox25.Image = null;
            pictureBox26.Image = null;
            pictureBox27.Image = null;
            pictureBox28.Image = null;
            pictureBox29.Image = null;
            pictureBox30.Image = null;
            pictureBox31.Image = null;
            pictureBox32.Image = null;
            pictureBox33.Image = null;
            pictureBox34.Image = null;
            pictureBox35.Image = null;
            pictureBox36.Image = null;
            pictureBox37.Image = null;
            pictureBox38.Image = null;
            pictureBox39.Image = null;
            pictureBox40.Image = null;
            pictureBox41.Image = null;
            pictureBox42.Image = null;
            pictureBox43.Image = null;
            pictureBox44.Image = null;
            pictureBox45.Image = null;
            pictureBox46.Image = null;
            pictureBox47.Image = null;
            pictureBox48.Image = null;



            if (type == "BM")
            {
                picactivetile.Image = BunnyMan[1];
                ChangeActiveTile(BunnyMan[1]);

                pictureBox1.Image = BunnyMan[1];
                pictureBox2.Image = BunnyMan[2];
                pictureBox3.Image = BunnyMan[3];
                pictureBox4.Image = BunnyMan[4];
                pictureBox5.Image = BunnyMan[5];
                pictureBox6.Image = BunnyMan[6];
                pictureBox7.Image = BunnyMan[7];
                pictureBox8.Image = BunnyMan[8];
                pictureBox9.Image = BunnyMan[9];
                pictureBox10.Image = BunnyMan[10];
                pictureBox11.Image = BunnyMan[11];
                pictureBox12.Image = BunnyMan[12];
                pictureBox13.Image = BunnyMan[13];
                pictureBox14.Image = BunnyMan[14];
                pictureBox15.Image = BunnyMan[15];
                pictureBox16.Image = BunnyMan[16];
                pictureBox17.Image = BunnyMan[17];
                pictureBox18.Image = BunnyMan[18];
                pictureBox19.Image = BunnyMan[19];
                pictureBox20.Image = BunnyMan[20];
                pictureBox21.Image = BunnyMan[21];
                pictureBox22.Image = BunnyMan[22];
                pictureBox23.Image = BunnyMan[23];
                pictureBox24.Image = BunnyMan[24];
                pictureBox25.Image = BunnyMan[25];
                pictureBox26.Image = BunnyMan[26];
                pictureBox27.Image = BunnyMan[27];
                pictureBox28.Image = BunnyMan[28];
                pictureBox29.Image = BunnyMan[29];
                pictureBox30.Image = BunnyMan[30];
                pictureBox31.Image = BunnyMan[31];
                pictureBox32.Image = BunnyMan[32];
                pictureBox33.Image = BunnyMan[33];
                pictureBox34.Image = BunnyMan[34];
                pictureBox35.Image = BunnyMan[35];
                pictureBox36.Image = BunnyMan[36];
                pictureBox37.Image = BunnyMan[37];
                pictureBox38.Image = BunnyMan[38];
                pictureBox39.Image = Other[2];
                pictureBox40.Image = Other[3];
                pictureBox41.Image = Other[4];
                pictureBox42.Image = Other[5];

            }

            if (type == "AM")
            {
                picactivetile.Image = AuroraMan[1];
                ChangeActiveTile(AuroraMan[1]);

                pictureBox1.Image = AuroraMan[1];
                pictureBox2.Image = AuroraMan[2];
                pictureBox3.Image = AuroraMan[3];
                pictureBox4.Image = AuroraMan[4];
                pictureBox5.Image = AuroraMan[5];
                pictureBox6.Image = AuroraMan[6];
                pictureBox7.Image = AuroraMan[7];
                pictureBox8.Image = AuroraMan[8];
                pictureBox9.Image = AuroraMan[9];
                pictureBox10.Image = AuroraMan[10];
                pictureBox11.Image = AuroraMan[11];
                pictureBox12.Image = AuroraMan[12];
                pictureBox13.Image = AuroraMan[13];
                pictureBox14.Image = AuroraMan[14];
                pictureBox15.Image = AuroraMan[15];
                pictureBox16.Image = AuroraMan[16];
                pictureBox17.Image = AuroraMan[17];
                pictureBox18.Image = AuroraMan[18];
                pictureBox19.Image = AuroraMan[19];
                pictureBox20.Image = AuroraMan[20];
            }

            if (type == "CM")
            {
                picactivetile.Image = CloverMan[1];
                ChangeActiveTile(CloverMan[1]);

                pictureBox1.Image = CloverMan[1];
                pictureBox2.Image = CloverMan[2];
                pictureBox3.Image = CloverMan[3];
                pictureBox4.Image = CloverMan[4];
                pictureBox5.Image = CloverMan[5];
                pictureBox6.Image = CloverMan[6];
                pictureBox7.Image = CloverMan[7];
                pictureBox8.Image = CloverMan[8];
                pictureBox9.Image = CloverMan[9];
                pictureBox10.Image = CloverMan[10];
                pictureBox11.Image = CloverMan[11];
                pictureBox12.Image = CloverMan[12];
                pictureBox13.Image = CloverMan[13];
                pictureBox14.Image = CloverMan[14];
                pictureBox15.Image = CloverMan[15];
                pictureBox16.Image = CloverMan[16];
                pictureBox17.Image = CloverMan[17];
                pictureBox18.Image = CloverMan[18];
                pictureBox19.Image = CloverMan[19];
                pictureBox20.Image = CloverMan[20];
                pictureBox21.Image = CloverMan[21];
                pictureBox22.Image = CloverMan[22];
                pictureBox23.Image = CloverMan[23];
                pictureBox24.Image = CloverMan[24];
                pictureBox25.Image = CloverMan[25];
                pictureBox26.Image = CloverMan[26];
                pictureBox27.Image = CloverMan[27];
                pictureBox28.Image = CloverMan[28];
                pictureBox29.Image = CloverMan[29];
                pictureBox30.Image = CloverMan[30];
                pictureBox31.Image = CloverMan[31];
                pictureBox32.Image = CloverMan[32];
                pictureBox33.Image = CloverMan[33];
                pictureBox34.Image = Other[13];
                pictureBox35.Image = Other[14];
                pictureBox36.Image = Other[15];
            }

            if (type == "GG")
            {
                picactivetile.Image = GeoGirl[1];
                ChangeActiveTile(GeoGirl[1]);

                pictureBox1.Image = GeoGirl[1];
                pictureBox2.Image = GeoGirl[2];
                pictureBox3.Image = GeoGirl[3];
                pictureBox4.Image = GeoGirl[4];
                pictureBox5.Image = GeoGirl[5];
                pictureBox6.Image = GeoGirl[6];
                pictureBox7.Image = GeoGirl[7];
                pictureBox8.Image = GeoGirl[8];
                pictureBox9.Image = GeoGirl[9];
                pictureBox10.Image = GeoGirl[10];
                pictureBox11.Image = Other[19];
            }

            if (type == "IM")
            {
                picactivetile.Image = InfernoMan[1];
                ChangeActiveTile(InfernoMan[1]);

                pictureBox1.Image = InfernoMan[1];
                pictureBox2.Image = InfernoMan[2];
                pictureBox3.Image = InfernoMan[3];
                pictureBox4.Image = InfernoMan[4];
                pictureBox5.Image = InfernoMan[5];
                pictureBox6.Image = InfernoMan[6];
                pictureBox7.Image = InfernoMan[7];
                pictureBox8.Image = InfernoMan[8];
                pictureBox9.Image = InfernoMan[9];
                pictureBox10.Image = InfernoMan[10];
                pictureBox11.Image = InfernoMan[11];
                pictureBox12.Image = InfernoMan[12];
                pictureBox13.Image = Other[16];
                pictureBox14.Image = Other[17];
                pictureBox15.Image = Other[18];
            }

            if (type == "LM")
            {
                picactivetile.Image = LurkerMan[1];
                ChangeActiveTile(LurkerMan[1]);

                pictureBox1.Image = LurkerMan[1];
                pictureBox2.Image = LurkerMan[2];
                pictureBox3.Image = LurkerMan[3];
                pictureBox4.Image = LurkerMan[4];
                pictureBox5.Image = LurkerMan[5];
                pictureBox6.Image = LurkerMan[6];
                pictureBox7.Image = LurkerMan[7];
                pictureBox8.Image = LurkerMan[8];
                pictureBox9.Image = LurkerMan[9];
                pictureBox10.Image = LurkerMan[10];
                pictureBox11.Image = LurkerMan[11];
                pictureBox12.Image = LurkerMan[12];
                pictureBox13.Image = LurkerMan[13];
                pictureBox14.Image = LurkerMan[14];
                pictureBox15.Image = LurkerMan[15];
                pictureBox16.Image = LurkerMan[16];
                pictureBox17.Image = LurkerMan[17];
                pictureBox18.Image = LurkerMan[18];
                pictureBox19.Image = LurkerMan[19];
                pictureBox20.Image = LurkerMan[20];
                pictureBox21.Image = LurkerMan[21];
                pictureBox22.Image = LurkerMan[22];
                pictureBox23.Image = LurkerMan[23];
                pictureBox24.Image = LurkerMan[24];
                pictureBox25.Image = Other[20];
                pictureBox26.Image = Other[21];
            }

            if (type == "PM")
            {
                picactivetile.Image = PosterMan[1];
                ChangeActiveTile(PosterMan[1]);

                pictureBox1.Image = PosterMan[1];
                pictureBox2.Image = PosterMan[2];
                pictureBox3.Image = PosterMan[3];
                pictureBox4.Image = PosterMan[4];
                pictureBox5.Image = PosterMan[5];
                pictureBox6.Image = PosterMan[6];
                pictureBox7.Image = PosterMan[7];
                pictureBox8.Image = PosterMan[8];
                pictureBox9.Image = PosterMan[9];
                pictureBox10.Image = PosterMan[10];
                pictureBox11.Image = PosterMan[11];
                pictureBox12.Image = PosterMan[12];
                pictureBox13.Image = PosterMan[13];
                pictureBox14.Image = PosterMan[14];
                pictureBox15.Image = PosterMan[15];
                pictureBox16.Image = PosterMan[16];
                pictureBox17.Image = Other[6];
                pictureBox18.Image = Other[7];
                pictureBox19.Image = Other[8];
                pictureBox20.Image = Other[9];
                pictureBox20.Image = Other[10];
                pictureBox20.Image = Other[11];
            }

            if (type == "TM")
            {
                picactivetile.Image = ToxicMan[1];
                ChangeActiveTile(ToxicMan[1]);

                pictureBox1.Image = ToxicMan[1];
                pictureBox2.Image = ToxicMan[2];
                pictureBox3.Image = ToxicMan[3];
                pictureBox4.Image = ToxicMan[4];
                pictureBox5.Image = ToxicMan[5];
                pictureBox6.Image = ToxicMan[6];
                pictureBox7.Image = ToxicMan[7];
                pictureBox8.Image = ToxicMan[8];
                pictureBox9.Image = ToxicMan[9];
                pictureBox10.Image = ToxicMan[10];
                pictureBox11.Image = ToxicMan[11];
                pictureBox12.Image = Other[12];
            }

            if (type == "CCM")
            {
                picactivetile.Image = CCM[1];
                ChangeActiveTile(CCM[1]);

                pictureBox1.Image = CCM[1];
                pictureBox2.Image = CCM[2];
                pictureBox3.Image = CCM[3];
                pictureBox4.Image = CCM[4];
                pictureBox5.Image = CCM[5];
                pictureBox6.Image = CCM[6];
                pictureBox7.Image = CCM[7];
                pictureBox8.Image = CCM[8];
                pictureBox9.Image = CCM[9];
                pictureBox10.Image = CCM[10];
                pictureBox11.Image = CCM[11];
                pictureBox12.Image = CCM[12];
                pictureBox13.Image = CCM[13];
                pictureBox14.Image = CCM[14];
                pictureBox15.Image = CCM[15];
                pictureBox16.Image = CCM[16];
                pictureBox17.Image = CCM[17];
                pictureBox18.Image = CCM[18];
                pictureBox19.Image = CCM[19];
                pictureBox20.Image = Other[22];
                pictureBox21.Image = Other[23];
                pictureBox22.Image = Other[24];
                pictureBox23.Image = Other[25];
                pictureBox24.Image = Other[26];
                pictureBox25.Image = Other[27];
                pictureBox26.Image = Other[28];
                pictureBox27.Image = Other[29];
            }

            if (type == "OLI")
            {
                picactivetile.Image = Oli[1];
                ChangeActiveTile(Oli[1]);

                pictureBox1.Image = Oli[1];
                pictureBox2.Image = Oli[2];
                pictureBox3.Image = Oli[3];
                pictureBox4.Image = Oli[4];
                pictureBox5.Image = Oli[5];
                pictureBox6.Image = Oli[6];
                pictureBox7.Image = Oli[7];
                pictureBox8.Image = Oli[8];
                pictureBox9.Image = Oli[9];
                pictureBox10.Image = Oli[10];
                pictureBox11.Image = Oli[11];
                pictureBox12.Image = Oli[12];
                pictureBox13.Image = Oli[13];
                pictureBox14.Image = Oli[14];
                pictureBox15.Image = Oli[15];
                pictureBox16.Image = Oli[16];
                pictureBox17.Image = Oli[17];
                pictureBox18.Image = Oli[18];
                pictureBox19.Image = Oli[19];
                pictureBox20.Image = Oli[20];
                pictureBox21.Image = Oli[21];
                pictureBox22.Image = Oli[22];
                pictureBox23.Image = Oli[23];
                pictureBox24.Image = Oli[24];
                pictureBox25.Image = Oli[25];
            }

            if (type == "DK")
            {
                picactivetile.Image = DeezKirb[1];
                ChangeActiveTile(DeezKirb[1]);

                pictureBox1.Image = DeezKirb[1];
                pictureBox2.Image = DeezKirb[2];
                pictureBox3.Image = DeezKirb[3];
                pictureBox4.Image = DeezKirb[4];
                pictureBox5.Image = DeezKirb[5];
                pictureBox6.Image = DeezKirb[6];
                pictureBox7.Image = DeezKirb[7];
                pictureBox8.Image = DeezKirb[8];
                pictureBox9.Image = DeezKirb[9];
                pictureBox10.Image = DeezKirb[10];
                pictureBox11.Image = DeezKirb[11];
                pictureBox12.Image = DeezKirb[12];
                pictureBox13.Image = DeezKirb[13];
                pictureBox14.Image = DeezKirb[14];
                pictureBox15.Image = DeezKirb[15];
                pictureBox16.Image = DeezKirb[16];
                pictureBox17.Image = DeezKirb[17];
                pictureBox18.Image = DeezKirb[18];
                pictureBox19.Image = DeezKirb[19];
                pictureBox20.Image = DeezKirb[20];
                pictureBox21.Image = DeezKirb[21];
                pictureBox22.Image = DeezKirb[22];
                pictureBox23.Image = DeezKirb[23];
                pictureBox24.Image = DeezKirb[24];
                pictureBox25.Image = DeezKirb[25];
                pictureBox26.Image = DeezKirb[26];
                pictureBox27.Image = DeezKirb[27];
                pictureBox28.Image = DeezKirb[28];
                pictureBox29.Image = DeezKirb[29];
                pictureBox30.Image = DeezKirb[30];
                pictureBox31.Image = DeezKirb[31];
                pictureBox32.Image = DeezKirb[32];
            }

            if (type == "MUSH")
            {
                picactivetile.Image = Mushie[1];
                ChangeActiveTile(Mushie[1]);

                pictureBox1.Image = Mushie[1];
                pictureBox2.Image = Mushie[2];
                pictureBox3.Image = Mushie[3];
                pictureBox4.Image = Mushie[4];
                pictureBox5.Image = Mushie[5];
                pictureBox6.Image = Mushie[6];
                pictureBox7.Image = Mushie[7];
                pictureBox8.Image = Mushie[8];
                pictureBox9.Image = Mushie[9];
                pictureBox10.Image = Mushie[10];
                pictureBox11.Image = Mushie[11];
                pictureBox12.Image = Mushie[12];
                pictureBox13.Image = Mushie[13];
                pictureBox14.Image = Mushie[14];
                pictureBox15.Image = Mushie[15];
                pictureBox16.Image = Mushie[16];
                pictureBox17.Image = Mushie[17];
                pictureBox18.Image = Mushie[18];
                pictureBox19.Image = Mushie[19];
                pictureBox20.Image = Mushie[20];
                pictureBox21.Image = Mushie[21];
                pictureBox22.Image = Mushie[22];
                pictureBox23.Image = Mushie[23];
                pictureBox24.Image = Mushie[24];
                pictureBox25.Image = Mushie[25];
                pictureBox26.Image = Mushie[26];
                pictureBox27.Image = Mushie[27];
                pictureBox28.Image = Mushie[28];
                pictureBox29.Image = Mushie[29];
                pictureBox30.Image = Mushie[30];
                pictureBox31.Image = Mushie[31];
                pictureBox32.Image = Mushie[32];
                pictureBox33.Image = Mushie[33];
                pictureBox34.Image = Mushie[34];
                pictureBox35.Image = Mushie[35];
                pictureBox36.Image = Mushie[36];
                pictureBox37.Image = Mushie[37];
                pictureBox38.Image = Mushie[38];
                pictureBox39.Image = Mushie[39];
                pictureBox40.Image = Mushie[40];
                pictureBox41.Image = Mushie[41];
                pictureBox42.Image = Mushie[42];
                pictureBox43.Image = Mushie[43];
            }

            if (type == "CHARA")
            {
                picactivetile.Image = Characters[1];
                ChangeActiveTile(Characters[1]);

                pictureBox1.Image = Characters[1];
                pictureBox2.Image = Characters[2];
                pictureBox3.Image = Characters[3];
                pictureBox4.Image = Characters[4];
                pictureBox5.Image = Characters[5];
                pictureBox6.Image = Characters[6];
                pictureBox7.Image = Characters[7];
                pictureBox8.Image = Characters[8];
                pictureBox9.Image = Characters[9];
                pictureBox10.Image = Characters[10];
                pictureBox11.Image = Characters[11];
                pictureBox12.Image = Characters[12];
                pictureBox13.Image = Characters[13];
                pictureBox14.Image = Other[1];
            }

            if (type == "ENM")
            {
                picactivetile.Image = Enemies[1];
                ChangeActiveTile(Enemies[1]);

                pictureBox1.Image = Enemies[1];
                pictureBox2.Image = Enemies[2];
                pictureBox3.Image = Enemies[3];
                pictureBox4.Image = Enemies[4];
                pictureBox5.Image = Enemies[5];
                pictureBox6.Image = Enemies[6];
                pictureBox7.Image = Enemies[7];
                pictureBox8.Image = Enemies[8];
                pictureBox9.Image = Enemies[9];
                pictureBox10.Image = Enemies[10];
                pictureBox11.Image = Enemies[11];
                pictureBox12.Image = Enemies[12];
                pictureBox13.Image = Enemies[13];
                pictureBox14.Image = Enemies[14];
                pictureBox15.Image = Enemies[15];
                pictureBox16.Image = Enemies[16];
                pictureBox17.Image = Enemies[17];
                pictureBox18.Image = Enemies[18];
                pictureBox19.Image = Enemies[19];
                pictureBox20.Image = Enemies[20];
                pictureBox21.Image = Enemies[21];
            }

            if (type == "SCROL")
            {
                picactivetile.Image = Scroller[1];
                ChangeActiveTile(Scroller[1]);

                pictureBox1.Image = Scroller[1];
                pictureBox2.Image = Scroller[2];
                pictureBox3.Image = Scroller[3];
                pictureBox4.Image = Scroller[4];
         //       pictureBox5.Image = Scroller[5];
           //     pictureBox6.Image = Scroller[6];
             //   pictureBox7.Image = Scroller[7];
             //   pictureBox8.Image = Scroller[8];
           //     pictureBox9.Image = Scroller[9];
            //    pictureBox10.Image = Scroller[10];
             //   pictureBox11.Image = Scroller[11];
              //  pictureBox12.Image = Scroller[12];
               // pictureBox13.Image = Scroller[13];
               // pictureBox14.Image = Scroller[14];
            }

        }


        private void LoadAllImages(int numberofimages)
        {
            int counter = 0;
            tiles = new Image[numberofimages + 1];

            BunnyMan = new Image[39];
            AuroraMan = new Image[21];
            CloverMan = new Image[34];
            GeoGirl = new Image[11];
            InfernoMan = new Image[13];
            LurkerMan = new Image[25];
            PosterMan = new Image[17];
            ToxicMan = new Image[12];
            CCM = new Image[20];
            Oli = new Image[26];
            DeezKirb = new Image[33];
            Mushie = new Image[44];
            Characters = new Image[14];
            Enemies = new Image[22];
            Scroller = new Image[15];
            Other = new Image[30];

            for (int i = 1; i <= 38; i++)
            {
                counter++;

                BunnyMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Bunny Man/BM" + i.ToString() + ".bmp");
                tiles[counter] = BunnyMan[i];


            }

            for (int i = 1; i <= 20; i++)
            {
                counter++;

                AuroraMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Aurora Man/AM" + i.ToString() + ".bmp");
                tiles[counter] = AuroraMan[i];


            }

            for (int i = 1; i <= 33; i++)
            {
                counter++;

                CloverMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Clover Man/CM" + i.ToString() + ".bmp");
                tiles[counter] = CloverMan[i];


            }

            for (int i = 1; i <= 10; i++)
            {
                counter++;

                GeoGirl[i] = Image.FromFile(Application.StartupPath + @"/tiles/Geo Girl/GG" + i.ToString() + ".bmp");
                tiles[counter] = GeoGirl[i];


            }

            for (int i = 1; i <= 12; i++)
            {
                counter++;

                InfernoMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Inferno Man/IM" + i.ToString() + ".bmp");
                tiles[counter] = InfernoMan[i];


            }

            for (int i = 1; i <= 24; i++)
            {
                counter++;

                LurkerMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Lurker Man/LM" + i.ToString() + ".bmp");
                tiles[counter] = LurkerMan[i];


            }

            for (int i = 1; i <= 16; i++)
            {
                counter++;

                PosterMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Poster Man/PM" + i.ToString() + ".bmp");
                tiles[counter] = PosterMan[i];

            }

            for (int i = 1; i <= 11; i++)
            {
                counter++;

                ToxicMan[i] = Image.FromFile(Application.StartupPath + @"/tiles/Toxic Man/TM" + i.ToString() + ".bmp");
                tiles[counter] = ToxicMan[i];


            }

            for (int i = 1; i <= 19; i++)
            {
                counter++;

                CCM[i] = Image.FromFile(Application.StartupPath + @"/tiles/CCM/C" + i.ToString() + ".bmp");
                tiles[counter] = CCM[i];


            }

            for (int i = 1; i <= 25; i++)
            {
                counter++;

                Oli[i] = Image.FromFile(Application.StartupPath + @"/tiles/Oli/o" + i.ToString() + ".bmp");
                tiles[counter] = Oli[i];


            }

            for (int i = 1; i <= 32; i++)
            {
                counter++;

                DeezKirb[i] = Image.FromFile(Application.StartupPath + @"/tiles/DeezKirb/DK" + i.ToString() + ".bmp");
                tiles[counter] = DeezKirb[i];

            }

            for (int i = 1; i <= 43; i++)
            {
                counter++;

                Mushie[i] = Image.FromFile(Application.StartupPath + @"/tiles/Mushie/M" + i.ToString() + ".bmp");
                tiles[counter] = Mushie[i];

            }

            for (int i = 1; i <= 13; i++)
            {
                counter++;

                Characters[i] = Image.FromFile(Application.StartupPath + @"/tiles/Things/" + i.ToString() + ".bmp");
                tiles[counter] = Characters[i];

            }

            for (int i = 1; i <= 21; i++)
            {
                counter++;

                Enemies[i] = Image.FromFile(Application.StartupPath + @"/tiles/Enemies/E" + i.ToString() + ".png");
                tiles[counter] = Enemies[i];

            }

            for (int i = 1; i <= 4; i++)
            {
                counter++;

                Scroller[i] = Image.FromFile(Application.StartupPath + @"/tiles/Scroller/" + i.ToString() + ".bmp");
                tiles[counter] = Scroller[i];

            }

            for (int i = 1; i <= 29; i++)
            {
                if (File.Exists(Application.StartupPath + @"/tiles/other/other" + i.ToString() + ".png"))
                {
                    counter++;

                    Other[i] = Image.FromFile(Application.StartupPath + @"/tiles/other/other" + i.ToString() + ".png");
                    tiles[counter] = Other[i];
                }

            }
        }


        private void pictureBox8_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox8.Image);
        }

        private void pictureBox27_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox27.Image);
        }

        private void pictureBox28_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox28.Image);
        }

        private void pictureBox32_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox32.Image);
        }

        private void pictureBox38_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox38.Image);
        }

        private void pictureBox40_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox40.Image);
        }

        private void pictureBox36_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox36.Image);
        }

        private void pictureBox42_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox42.Image);
        }

        private void pictureBox34_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox34.Image);
        }

        private void pictureBox31_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox31.Image);
        }

        private void pictureBox30_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox30.Image);
        }

        private void pictureBox47_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox47.Image);
        }

        private void pictureBox48_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox48.Image);
        }

        private void pictureBox29_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox29.Image);
        }

        private void pictureBox33_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox33.Image);
        }

        private void pictureBox35_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox35.Image);
        }

        private void pictureBox37_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox37.Image);
        }

        private void pictureBox39_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox39.Image);
        }

        private void pictureBox41_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox41.Image);
        }

        private void pictureBox43_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox43.Image);
        }

        private void pictureBox44_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox44.Image);
        }

        private void pictureBox45_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox45.Image);
        }

        private void pictureBox46_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox46.Image);
        }

        private void pictureBox25_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox25.Image);
        }

        private void pictureBox24_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox24.Image);
        }

        private void pictureBox23_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox23.Image);
        }

        private void pictureBox22_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox22.Image);
        }

        private void pictureBox21_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox21.Image);
        }

        private void pictureBox20_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox20.Image);
        }

        private void pictureBox19_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox19.Image);
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox18.Image);
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox17.Image);
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox16.Image);
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox15.Image);
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox14.Image);
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox13.Image);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox12.Image);
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox11.Image);
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox10.Image);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox9.Image);
        }

        private void pictureBox26_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox26.Image);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox7.Image);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox6.Image);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox5.Image);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox4.Image);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox3.Image);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox2.Image);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ChangeActiveTile(pictureBox1.Image);
        }

        private void UncheckAll()
        {
            bunnyManToolStripMenuItem.Checked = false;
            posterManToolStripMenuItem.Checked = false;
            auroraManToolStripMenuItem.Checked = false;
            toxicManToolStripMenuItem.Checked = false;
            cloverManToolStripMenuItem.Checked = false;
            infernoManToolStripMenuItem.Checked = false;
            geoGirlToolStripMenuItem.Checked = false;
            lurkerManToolStripMenuItem.Checked = false;
            cCMToolStripMenuItem.Checked = false;
            oliToolStripMenuItem.Checked = false;
            DEEZToolStripMenuItem.Checked = false;
            mushToolStripMenuItem1.Checked = false;
            rioManAndRobotMastersToolStripMenuItem.Checked = false;
            enemiesToolStripMenuItem.Checked = false;
            scrollToolStripMenuItem.Checked = false;
        }

        private void bunnyManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            bunnyManToolStripMenuItem.Checked = true;
            UpdateTileset("BM");
        }

        private void posterManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            posterManToolStripMenuItem.Checked = true;
            UpdateTileset("PM");
        }

        private void auroraManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            auroraManToolStripMenuItem.Checked = true;
            UpdateTileset("AM");
        }

        private void toxicManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            toxicManToolStripMenuItem.Checked = true;
            UpdateTileset("TM");
        }

        private void cloverManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            cloverManToolStripMenuItem.Checked = true;
            UpdateTileset("CM");
        }

        private void infernoManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            infernoManToolStripMenuItem.Checked = true;
            UpdateTileset("IM");
        }

        private void geoGirlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            geoGirlToolStripMenuItem.Checked = true;
            UpdateTileset("GG");
        }

        private void lurkerManToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            lurkerManToolStripMenuItem.Checked = true;
            UpdateTileset("LM");
        }

        private void cCMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            cCMToolStripMenuItem.Checked = true;
            UpdateTileset("CCM");
        }

        private void oliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            oliToolStripMenuItem.Checked = true;
            UpdateTileset("OLI");
        }

        private void DEEZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            DEEZToolStripMenuItem.Checked = true;
            UpdateTileset("DK");
        }

        private void mushToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            UncheckAll();
            mushToolStripMenuItem1.Checked = true;
            UpdateTileset("MUSH");
        }

        private void rioManAndRobotMastersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            rioManAndRobotMastersToolStripMenuItem.Checked = true;
            UpdateTileset("CHARA");
        }

        private void enemiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            enemiesToolStripMenuItem.Checked = true;
            UpdateTileset("ENM");
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            scrollToolStripMenuItem.Checked = true;
            UpdateTileset("SCROL");
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullRefreshLevel();
        }

        private void panel_MouseEnter(object sender, EventArgs e)
        {
            FullRefreshLevel();
        }
    }
}