using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace PracTest3 //Singles
{
    public partial class Form1 : Form
    {
        //Name :
        //ID No:

        public Form1()
        {
            InitializeComponent();
        }

        //the width and height of an icon
        const int ICON_SIZE = 14;

        const string FILTER = "CSV files|*.csv|All Files|*.*";

        string[] csvArray;
        
        List<int> thisWeekList = new List<int>();
        List<int> lastWeekList = new List<int>();
        List<int> weeksOnList = new List<int>();
        List<string> titleList = new List<string>();
        List<string> artistList = new List<string>();
        List<string> certificationList = new List<string>();
        List<string> labelList = new List<string>();

        /// <summary>
        /// Draws an icon representing rising on the Graphics object specified.
        /// It fills an up triangle at position (x,y) in green with a black outline.
        /// Uses the ICON_SIZE constant for the size of the triangle.
        /// </summary>
        /// <param name="paper">The Graphics object to draw on.</param>
        /// <param name="x">The x position of the top left corner of the triangle.</param>
        /// <param name="y">The y position of the top left corner of the triangle.</param>
        private void Rising(Graphics paper, int x, int y)
        {
            //define a triangle pointing upwards
            Point[] triangle = new Point[3] { new Point(x + ICON_SIZE / 2, y), new Point(x, y + ICON_SIZE), new Point(x + ICON_SIZE, y + ICON_SIZE) };
            //fill triangle with green 
            paper.FillPolygon(Brushes.Chartreuse, triangle);
            //draw outline in black
            paper.DrawPolygon(Pens.Black, triangle);
        }

        /// <summary>
        /// Draws an icon representing falling on the Graphics object specified.
        /// It fills a down triangle at position (x,y) in red with a black outline.
        /// Uses the ICON_SIZE constant for the size of the triangle.
        /// </summary>
        /// <param name="paper">The Graphics object to draw on.</param>
        /// <param name="x">The x position of the top left corner of the triangle.</param>
        /// <param name="y">The y position of the top left corner of the triangle.</param>
        private void Falling(Graphics paper, int x, int y)
        {
            //define a triangle pointing down
            Point[] triangle = new Point[3] { new Point(x, y), new Point(x+ ICON_SIZE, y ), new Point(x + ICON_SIZE/2, y + ICON_SIZE) };
            //fill triangle with red 
            paper.FillPolygon(Brushes.Red, triangle);
            //draw outline in black
            paper.DrawPolygon(Pens.Black, triangle);
        }

        /// <summary>
        /// Draws an icon representing static on the Graphics object specified.
        /// It fills a square at position (x,y) in blue with a black outline.
        /// Uses the ICON_SIZE constant for the size of the square.
        /// </summary>
        /// <param name="paper">The Graphics object to draw on.</param>
        /// <param name="x">The x position of the top left corner of the square.</param>
        /// <param name="y">The y position of the top left corner of the square.</param>
        private void Stable(Graphics paper, int x, int y)
        {
            //define a square
            Rectangle square = new Rectangle(x, y, ICON_SIZE, ICON_SIZE);
            //fill square with green 
            paper.FillRectangle(Brushes.CornflowerBlue, square);
            //draw outline in black
            paper.DrawRectangle(Pens.Black, square);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private int ComparePositions(int thisWeek, int lastWeek)
        {
            if (thisWeek<lastWeek) //If it has risen
            {
                return 1;
            } 
            else if (thisWeek > lastWeek) //If it has fallen
            {
                return -1;
            }
            else if (lastWeek == 0) //If it wasnt in the charts
            {
                return 1;
            }
            else //If it is at the same position
            {
                return 0;
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamReader reader;
            string line = "";
            int thisWeek, lastWeek, weeksOn;
            string title, artist, certification, label;

            //OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.Filter = FILTER

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                reader = File.OpenText(openFileDialog1.FileName);
                int x = 0;
                int y = 0;
                while (!reader.EndOfStream)
                {
                    try
                    {
                        Graphics paper = pictureBoxDisplay.CreateGraphics();
                        
                        line = reader.ReadLine();
                        csvArray = line.Split(',');
                        
                        if (csvArray.Length == 7)
                        {

                            thisWeek = int.Parse(csvArray[0]);
                            lastWeek = int.Parse(csvArray[1]);
                            weeksOn = int.Parse(csvArray[2]);
                            title = csvArray[3];
                            artist = csvArray[4];
                            certification = csvArray[5];
                            label = csvArray[6];

                            listBoxOutput.Items.Add(thisWeek.ToString().PadRight(3) + lastWeek.ToString().PadRight(3) + weeksOn.ToString().PadRight(3) + title.PadRight(30) + artist.PadRight(40)
                                +  certification.PadRight(15) + label);

                            int change = ComparePositions(thisWeek, lastWeek);

                            if (change == 1)
                            {
                                Rising(paper, x, y);
                            }
                            else if (change == 0)
                            {
                                Stable(paper, x, y);
                            }
                            else if (change == -1)
                            {
                                Falling(paper, x, y);
                            }
                            y += ICON_SIZE;

                            thisWeekList.Add(thisWeek);
                            lastWeekList.Add(lastWeek);
                            weeksOnList.Add(weeksOn);
                            titleList.Add(title);
                            artistList.Add(artist);
                            certificationList.Add(certification);
                            labelList.Add(label);
                        }
                        else
                        {
                            MessageBox.Show("Error: " + line);
                        }

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                reader.Close();
            }
        }

        private void biggestMoverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int moves = 0;
            int biggestMover = 0;

            string currentBiggestMover = "";
            string actualBiggestMover;

            for (int i = 0; i<titleList.Count; i++)
            {
                if (lastWeekList[i] == 0)
                {
                    lastWeekList[i] = 21;
                }
                if (thisWeekList[i] < lastWeekList[i]) // if a single has moved up the rankings
                {

                    moves = lastWeekList[i] - thisWeekList[i];
                }
                else if (lastWeekList[i] < thisWeekList[i])
                {
                    moves = thisWeekList[i] - lastWeekList[i];
                }
                if (moves> biggestMover)
                {
                    biggestMover = moves;
                    currentBiggestMover = titleList[i];
                }
            }
            MessageBox.Show(currentBiggestMover);
        }
    }
}
