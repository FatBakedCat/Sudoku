using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            createCells();

            startNewGame();
        }
        private void startNewGame()
        {
            loadValues();

            var count= 0;

            if (comboBox1.SelectedItem == "Łatwy")
                count = 40;
            else if (comboBox1.SelectedItem == "Średni")
                count = 30;
            else if (comboBox1.SelectedItem == "Trudny")
                count = 20;

            showRandomValuesHints(count);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] difficulties = new string[3] { "Łatwy", "Średni", "Trudny" };
            comboBox1.Items.AddRange(difficulties);
            comboBox1.SelectedItem = "Łatwy";
        }

        Cell[,] cells = new Cell[9, 9];

        private void createCells()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = new Cell();
                    cells[i, j].Font = new Font(SystemFonts.DefaultFont.FontFamily, 20);
                    cells[i, j].Size = new Size(40, 40);
                    cells[i, j].ForeColor = SystemColors.ControlDarkDark;
                    cells[i, j].Location = new Point(i * 40, j * 40);
                    if((i/3 + j/3)%2 == 0)
                        cells[i, j].BackColor = Color.LightGray;
                    cells[i, j].FlatStyle = FlatStyle.Flat;
                    cells[i, j].FlatAppearance.BorderColor = Color.Black;
                    cells[i, j].X = i;
                    cells[i, j].Y = j;

                    cells[i, j].KeyPress += cell_keyPressed;

                    panel1.Controls.Add(cells[i, j]);
                }
            }
        }

        private void cell_keyPressed(object sender, KeyPressEventArgs e)
        {
            var cell = sender as Cell;


            if (cell.IsLocked)
                return;

            int value;

            if (int.TryParse(e.KeyChar.ToString(), out value))
            {
                if (value == 0)
                    cell.Clear();
                else
                    cell.Text = value.ToString();

                cell.ForeColor = SystemColors.ControlDarkDark;
            }
            var wrongCells = new List<Cell>();


            foreach (var cell1 in cells)
            {
                if (!string.Equals(cell1.Value.ToString(), cell1.Text))
                {
                    wrongCells.Add(cell1);
                }
            }
            if (wrongCells.Any())
            {
                wrongCells.ForEach(x => x.ForeColor = Color.Red);
            }
            else
            {
                MessageBox.Show("Wygrałeś!!!");
            }
        }

        private void loadValues()
        {
            foreach (var cell in cells)
            {
                cell.Value = 0;
                cell.Clear();
            }
            findValueForNextCell(0, -1);
        }

        Random random = new Random();

        private bool findValueForNextCell(int i, int j)
        {
            if (++j > 8)
            {
                j = 0;

                if (++i > 8)
                    return true;
            }

            var value = 0;
            var numsLeft = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            do
            {
                if (numsLeft.Count < 1)
                {
                    cells[i, j].Value = 0;
                    return false;
                }

                value = numsLeft[random.Next(0, numsLeft.Count)];
                cells[i, j].Value = value;

                numsLeft.Remove(value);
            }
            while (!isValidNumber(value, i, j) || !findValueForNextCell(i, j));

            return true;
        }

        private bool isValidNumber(int value, int x, int y)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i != y && cells[x, i].Value == value)
                    return false;

                if (i != x && cells[i, y].Value == value)
                    return false;
            }

            for (int i = x - (x % 3); i < x - (x % 3) + 3; i++)
            {
                for (int j = y - (y % 3); j < y - (y % 3) + 3; j++)
                {
                    if (i != x && j != y && cells[i, j].Value == value)
                        return false;
                }
            }

            return true;
        }
        Random rand = new Random();

        private void showRandomValuesHints(int hintsCount)
        {
            for (int i = 0; i < hintsCount; i++)
            {
                var rX = rand.Next(9);
                var rY = rand.Next(9);

                cells[rX, rY].Text = cells[rX, rY].Value.ToString();
                cells[rX, rY].ForeColor = Color.Black;
                cells[rX, rY].IsLocked = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            startNewGame();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var cell in cells)
            {
                if (cell.IsLocked == false)
                    cell.Clear();
            }
        }

    }
}
