using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PracTest2
{
    public partial class Form1 : Form
    {
        //Name: 
        //ID:

        //The smallest Easting value on the NZMG260 S14 (Hamilton) map
        const int MIN_EASTING = 2690000;
        //The largest Easting value on the NZMG260 S14 (Hamilton) map
        const int MAX_EASTING = 2730000;
        //The smallest Northing value on the NZMG260 S14 (Hamilton) map
        const int MIN_NORTHING = 6370000;
        //The largest Northing value on the NZMG260 S14 (Hamilton) map
        const int MAX_NORTHING = 6400000;

        //Filter for csv files and all files
        const string FILTER = "CSV files|*.csv|All Files|*.*";

        //Create csvArray to store line
        string[] csvArray;

        //Create lists
        List<string> locationList = new List<string>();
        List<int> eastingList = new List<int>();
        List<int> northingList = new List<int>();
        List<double> powerList = new List<double>();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draws a cell tower centered at the given x and y position
        /// in the colour specified.
        /// </summary>
        /// <param name="paper">Where to draw the tower</param>
        /// <param name="x">X position of the centre of the tower</param>
        /// <param name="y">Y position of the centre of the tower</param>
        /// <param name="power">The range of the tower, i.e. the radius of the circle</param>
        /// <param name="towerColour">Colour to draw the tower in</param>
        private void DrawTower(Graphics paper, int x, int y, double power, Color towerColour)
        {
            //The size of a side of the rectangle to represent a tower
            const int TOWER_SIZE = 6;
            //Brush and pen to draw the tower in the given colour
            SolidBrush br = new SolidBrush(towerColour);
            Pen pen1 = new Pen(towerColour, 2);
            //Caluclate the radius of the circle to represent the power as an integer
            int radius = (int) power;

            //Draw the tower centered around the given x and y point
            paper.FillRectangle(br, x - TOWER_SIZE / 2, y - TOWER_SIZE / 2, TOWER_SIZE, TOWER_SIZE);
            //Draw the circle to represent the range cenetred around the given x and y point
            paper.DrawEllipse(pen1, x - radius, y - radius, radius * 2, radius * 2);
        }

        /// <summary>
        /// This method will calculate the correct x coordinate value of the cell tower
        /// based on the given easting value.
        /// </summary>
        /// <param name="easting">The easting value of the cell tower</param>
        /// <returns>The x coordinate of the cell tower in the picturebox.</returns>
        private int CalculateX(int easting)
        {
            //calculate x position of easting value, must cast one of the values to a double
            //otherwise will perform integer division
            double ratio = (double) (easting - MIN_EASTING) / (MAX_EASTING - MIN_EASTING);
            int x = (int)(ratio * pictureBoxMap.Width);
            return x;
        }

        private int CalculateY(int northing)
        {
            double percentUP = (double)(northing - MIN_NORTHING)/(MAX_NORTHING - MIN_NORTHING);

            int y = pictureBoxMap.Height-(int)(pictureBoxMap.Height * percentUP);

            return y;
        }

        private int CountTowers(double power)
        {
            int count = 0;
            for (int i = 0; i<powerList.Count; i++)
            {
                if (powerList[i] <= power)
                {
                    count++;
                }
            }
            return count;
        }

        private void oToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics paper = pictureBoxMap.CreateGraphics();


            StreamReader reader;
            int i = 0;

            openFileDialog1.Filter = FILTER;
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    reader = File.OpenText(openFileDialog1.FileName);

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        i++;
                        
                        csvArray = line.Split(',');

                        if (csvArray.Length == 5)
                        {
                            string licensee = csvArray[0];
                            string location = csvArray[1];
                            int easting = int.Parse(csvArray[2]);
                            int northing = int.Parse(csvArray[3]);
                            double power = double.Parse(csvArray[4]);

                            listBoxData.Items.Add(csvArray[0].PadRight(20) + csvArray[1].PadRight(50) + csvArray[2].PadRight(10) + csvArray[3].PadRight(10) +csvArray[4]);

                            locationList.Add(location);
                            eastingList.Add(easting);
                            northingList.Add(northing);
                            powerList.Add(power);

                            DrawTower(paper, CalculateX(easting), CalculateY(northing), power, Color.Blue);
                        }
                        else
                        {
                            
                            MessageBox.Show("Invalid number of elements in line " + i + " :\n" + line);
                        }
                    }
                    reader.Close();
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void countTowersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                double powerValue = double.Parse(textBoxPower.Text);
                MessageBox.Show(CountTowers(powerValue).ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
