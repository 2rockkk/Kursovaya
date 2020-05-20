using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace saper
{
    public partial class Form1 : Form
    {
        int width = 10;
        int height = 10;
        int offset = 30;
        int bombPercent = 15;
        bool isFirstCLick = true;
        FieldButton[,] field;
        int cellsOpened = 0;
        int bombs = 0;
        string path1 = @"c:/Сапер v2.1.2/wins.txt";
        string path2 = @"c:/Сапер v2.1.2/loses.txt";




        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void GenerateField()
        {
            Random random = new Random();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    FieldButton newButton = new FieldButton();
                    newButton.Location = new Point(x * offset, (y + 1) * offset);
                    newButton.Size = new Size(offset, offset);
                    newButton.isClickable = true;
                    newButton.TabStop = false;



                    if (random.Next(0, 100) <= bombPercent)
                    {
                        newButton.isBomb = true;
                        bombs++;
                    }
                    newButton.xCoord = x;
                    newButton.yCoord = y;

                    Controls.Add(newButton);
                    newButton.MouseUp += new MouseEventHandler(FieldButtonClick);
                    field[x, y] = newButton;
                }
            }
        }

        void FieldButtonClick(object sender, MouseEventArgs e)
        {

            FieldButton clickedButton = (FieldButton)sender;
            if (e.Button == MouseButtons.Left && clickedButton.isClickable)
            {
                clickedButton.BackColor = Color.LightSlateGray;
                if (clickedButton.isBomb)
                {
                    if (isFirstCLick)
                    {
                        clickedButton.isBomb = false;
                        isFirstCLick = false;
                        bombs--;
                        OpenRegion(clickedButton.xCoord, clickedButton.yCoord, clickedButton);
                    }
                    else
                    {
                        Explode();
                    }

                }
                else
                {
                    EmptyFieldButtonClick(clickedButton);
                }
                isFirstCLick = false;
            }
            if (e.Button == MouseButtons.Right)
            {

                clickedButton.isClickable = !clickedButton.isClickable;
                if (!clickedButton.isClickable)
                {
                    playFlag();
                    clickedButton.Text = "B";
                }
                else
                {
                    playFlag();
                    clickedButton.Text = "";

                }
            }
            CheckWin();
        }

        void Explode()
        {

            foreach (FieldButton button in field)
            {
                if (button.isBomb)
                {
                    button.Text = "*";
                }
            }
            playExplode();
            MessageBox.Show("¯/_(ツ)_/¯ u lose...");
            CountLoses();
            Application.Restart();
        }
        void EmptyFieldButtonClick(FieldButton clickedButton)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (field[x, y] == clickedButton)
                    {

                        OpenRegion(x, y, clickedButton);
                    }
                }
            }


        }
        void OpenRegion(int xCoord, int yCoord, FieldButton clickedButton)
        {
            Queue<FieldButton> queue = new Queue<FieldButton>();
            queue.Enqueue(clickedButton);
            clickedButton.wasAdded = true;
            while (queue.Count > 0)
            {
                FieldButton currentCell = queue.Dequeue();
                OpenCell(currentCell.xCoord, currentCell.yCoord, currentCell);
                cellsOpened++;
                if (CountBombsAround(currentCell.xCoord, currentCell.yCoord) == 0)
                {
                    for (int y = currentCell.yCoord - 1; y <= currentCell.yCoord + 1; y++)
                    {
                        for (int x = currentCell.xCoord - 1; x <= currentCell.xCoord + 1; x++)
                        {
                            if (x == currentCell.xCoord && y == currentCell.yCoord)
                            {
                                continue;
                            }
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                if (!field[x, y].wasAdded)
                                {
                                    queue.Enqueue(field[x, y]);
                                    field[x, y].wasAdded = true;
                                    field[x, y].BackColor = Color.LightSlateGray;
                                }
                            }
                        }
                    }
                }
            }

        }
        void OpenCell(int x, int y, FieldButton clickedButton)
        {
            int bombsAround = CountBombsAround(x, y);
            if (bombsAround == 0)
            {

            }
            else
            {
                clickedButton.Text = "" + bombsAround;
            }
            clickedButton.Enabled = false;
        }

        int CountBombsAround(int xCoord, int yCoord)
        {
            int bombsAround = 0;
            for (int x = xCoord - 1; x <= xCoord + 1; x++)
            {
                for (int y = yCoord - 1; y <= yCoord + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (field[x, y].isBomb == true)
                        {
                            bombsAround++;
                        }
                    }
                }
            }
            return bombsAround;
        }
        void CheckWin()
        {
            int cells = width * height;
            int emptyCells = cells - bombs;
            if (cellsOpened == emptyCells)
            {
                CountWins();
                playWin();
                MessageBox.Show("(╯°□°）╯︵ ┻━┻ u win! ");
                Application.Restart();

            }



        }
        void CountWins()
        {
            StreamReader sr = new StreamReader(path1);
            int i = Convert.ToInt32(sr.ReadToEnd());
            sr.Close();
            int x = i + 1;
            StreamWriter sr1 = new StreamWriter(path1, false, Encoding.Default);
            sr1.WriteLine(x);
            sr1.Close();
        }
        void CountLoses()
        {
            StreamReader sr = new StreamReader(path2);
            int i = Convert.ToInt32(sr.ReadToEnd());
            sr.Close();
            int x = i + 1;
            StreamWriter sr1 = new StreamWriter(path1, false, Encoding.Default);
            sr1.WriteLine(x);
            sr1.Close();
        }

        private void X10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playMenu();
            width = 10;
            height = 10;
            field = new FieldButton[width, height];
            GenerateField();
            menuStrip1.Enabled = false;

        }

        private void X5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playMenu();
            width = 5;
            height = 5;
            field = new FieldButton[width, height];
            GenerateField();
            menuStrip1.Enabled = false;
            this.Size = new System.Drawing.Size(167, 220);



        }
        private void playMenu()
        {
            SoundPlayer menuClick = new SoundPlayer(@"c:\Сапер v2.1\door_open.wav");
            menuClick.Play();
        }
        private void playExplode()
        {
            SoundPlayer menuClick = new SoundPlayer(@"c:\Сапер v2.1\explode1.wav");
            menuClick.Play();
        }
        private void playWin()
        {
            SoundPlayer menuClick = new SoundPlayer(@"c:\Сапер v2.1\successful_hit.wav");
            menuClick.Play();
        }
        private void playFlag()
        {
            SoundPlayer menuClick = new SoundPlayer(@"c:\Сапер v2.1\pop.wav");
            menuClick.Play();
        }


        private void LevelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void GameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playMenu();
        }

        private void SizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playMenu();
        }

        private void MenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ContactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playMenu();
            MessageBox.Show("Turok_SV_18@mf.grsu.by");

        }

        private void AttemptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playMenu();
            StreamReader sr = new StreamReader(path1);
            string s = sr.ReadToEnd();
            sr.Close();
            StreamReader sg = new StreamReader(path2);
            string g = sg.ReadToEnd();
            sg.Close();
            MessageBox.Show("Общее количество побед: " + s +"." + "Общее количество поравжений:"+ g );

        }
    }



    public class FieldButton : Button
    {
        public bool isBomb;
        public bool isClickable;
        public bool wasAdded;
        public int xCoord;
        public int yCoord;
    }
}

