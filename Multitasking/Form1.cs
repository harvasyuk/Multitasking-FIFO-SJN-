using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace FIFO
{
    public partial class Form1 : Form
    {
        FIFOTaskManager fifoTaskManager;
        SJNTaskManager sjnTaskManager;
        List<string> multiProcList = new List<string>(); 

        public Form1()
        {
            InitializeComponent();
            AcceptButton = button1;
            this.Text = "Диспетчеризація задач";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearData();
            int input = 0;

            try
            {
                input = Int32.Parse(inputTextBox.Text);
                FillForm(input);
            }
            catch
            {
                MessageBox.Show("Будь ласка, перевірте вхідні дані", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } 
        }

        private void FillForm(int input)
        {
            if (fifoRadioButton.Checked)
            {
                fifoTaskManager = new FIFOTaskManager(input);
                FillGrid1(fifoTaskManager);
                FillGrid2(fifoTaskManager);
                FillStatisticGrid(fifoTaskManager);
                DrawSeries(fifoTaskManager);
                FillExecuteGrid(fifoTaskManager);
                DrawStatisticChart(fifoTaskManager);
                wTextBox.Text = fifoTaskManager.averageW.ToString();
                mTextBox.Text = fifoTaskManager.GetMultiprocState().Max().ToString() + FindMultiProcRange(fifoTaskManager.GetMultiprocState());

                multiProcList.Add(fifoTaskManager.averageW.ToString());
                multiProcList.Add(fifoTaskManager.GetMultiprocState().Max().ToString());
                multiProcList.Add(FindMultiProcRange(fifoTaskManager.GetMultiprocState()));
                label2.Text = "Таблиця, яка характеризує виконання завдань по FIFO";
            }
            else if (sjnRadioButton.Checked)
            {
                sjnTaskManager = new SJNTaskManager(input);
                FillGrid1(sjnTaskManager);
                FillGrid2(sjnTaskManager);
                FillStatisticGrid(sjnTaskManager);
                DrawSeries(sjnTaskManager);
                FillExecuteGrid(sjnTaskManager);
                DrawStatisticChart(sjnTaskManager);
                wTextBox.Text = sjnTaskManager.averageW.ToString();
                mTextBox.Text = sjnTaskManager.GetMultiprocState().Max().ToString() + FindMultiProcRange(sjnTaskManager.GetMultiprocState());

                multiProcList.Add(sjnTaskManager.averageW.ToString());
                multiProcList.Add(sjnTaskManager.GetMultiprocState().Max().ToString());
                multiProcList.Add(FindMultiProcRange(sjnTaskManager.GetMultiprocState()));
                label2.Text = "Таблиця, яка характеризує виконання завдань по SJN";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (multiProcList.Count < 6)
            {
                multiProcList.Clear();
                MessageBox.Show("Розрахуйте дані спочатку", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                CompareForm compareForm = new CompareForm(multiProcList);
                compareForm.ShowDialog();
            }
        }

        private void ClearData() 
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            executeGridView.Rows.Clear();
            statGridView.Rows.Clear();
            foreach (var series in timeDiagram.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in statisticChart.Series)
            {
                series.Points.Clear();
            }
        }

        private string FindMultiProcRange(List<int> multiList)
        {
            string multiString = "";
            int start = 0;
            int end = 0;
            int max = multiList[0];

            max = multiList.Max();
            start = multiList.IndexOf(max);
            bool flag = false;

            for (int i = start; i < multiList.Count; i++)
            {
                if (!flag)
                {
                    if (multiList[i] == max)
                    {
                        start = i;
                        flag = true;
                    }
                }

                if (multiList[i] < max && multiList[i - 1] == max)
                {
                    end = i;
                    multiString += " (" + start.ToString() + " - " + end.ToString() + ") ";
                    flag = false;
                }
            }
            return multiString;
        }

        private void DrawStatisticChart(ITaskManager taskManager)
        {
            statisticChart.ChartAreas["ChartArea1"].AxisX.Interval = 50;
            statisticChart.ChartAreas["ChartArea1"].AxisX.IntervalOffset = 0;
            statisticChart.ChartAreas["ChartArea1"].AxisX.Minimum = 0;

            statisticChart.ChartAreas["ChartArea1"].AxisY.Interval = 2;
            statisticChart.ChartAreas["ChartArea1"].AxisY.Maximum = 16;

            statisticChart.Series["Series1"].Color = ColorTranslator.FromHtml("#0336FF");
            statisticChart.Series["Series2"].Color = ColorTranslator.FromHtml("#FFDE03");
            statisticChart.Series["Series3"].Color = ColorTranslator.FromHtml("#FF0266");

            int[] V = taskManager.GetVHistory().ToArray();
            int[] H = taskManager.GetHHistory().ToArray();
            int[] M = taskManager.GetMultiprocState().ToArray();

            for (int t = 0; t < V.Length; t++)
            {
                statisticChart.Series["Series1"].Points.AddXY(t, V[t]);
                statisticChart.Series["Series1"].LegendText = "V";
            }

            for (int t = 0; t < H.Length; t++)
            {
                statisticChart.Series["Series2"].Points.AddXY(t, H[t]);
                statisticChart.Series["Series2"].LegendText = "H";
            }

            for (int t = 0; t < M.Length; t++)
            {
                statisticChart.Series["Series3"].Points.AddXY(t, M[t]);
                statisticChart.Series["Series3"].LegendText = "Коеф. мульт.";
            }
        }

        private void DrawSeries(ITaskManager taskManager)
        {
            timeDiagram.ChartAreas["ChartArea1"].AxisX.Interval = 20;
            timeDiagram.ChartAreas["ChartArea1"].AxisX.IntervalOffset = 0;
            timeDiagram.ChartAreas["ChartArea1"].AxisX.Minimum = 0;

            timeDiagram.ChartAreas["ChartArea1"].AxisY.Interval = 1;
            timeDiagram.ChartAreas["ChartArea1"].AxisY.Maximum = 10;

            //set color for series
            for (int i = 1; i <= 10; i++)
            {
                timeDiagram.Series["Series" + (i).ToString()].Color = ColorTranslator.FromHtml("#FF0266");
                timeDiagram.Series["Series" + (i + 10).ToString()].Color = ColorTranslator.FromHtml("#FFDE03");
                timeDiagram.Series["Series" + (i + 20).ToString()].Color = ColorTranslator.FromHtml("#0336FF");
            }

            for (int i = 1; i <= 10; i++)
            {
                string[] series = taskManager.GetDictionary()[i - 1].ToArray();
                for (int t = 0; t < series.Length; t++)
                {
                    timeDiagram.Series["Series" + (i).ToString()].LegendText = "Виконання";
                    timeDiagram.Series["Series" + (i + 10).ToString()].LegendText = "Завантаження";
                    timeDiagram.Series["Series" + (i + 20).ToString()].LegendText = "Черга";

                    if (series[t].Contains("R"))
                    {
                        timeDiagram.Series["Series" + (i).ToString()].Points.AddXY(t, i);
                    } 
                    else if (series[t].Contains("L"))
                    {
                        timeDiagram.Series["Series" + (i + 10).ToString()].Points.AddXY(t, i);
                    }
                    else if (series[t].Contains("W"))
                    {
                        timeDiagram.Series["Series" + (i + 20).ToString()].Points.AddXY(t, i);
                    }
                }
            }
        }

        private void FillStatisticGrid(ITaskManager taskManager)
        {
            int[] V = taskManager.GetVHistory().ToArray();
            int[] H = taskManager.GetHHistory().ToArray();

            string[] vString = V.Select(x => x.ToString()).ToArray();
            string[] hString = H.Select(x => x.ToString()).ToArray();
            string[] processState;

            statGridView.ColumnCount = taskManager.GetHHistory().Count;

            for (int i = 0; i < taskManager.GetHHistory().Count; i++)
            {
                statGridView.Columns[i].Name = i.ToString();
            }

            statGridView.Rows.Add(vString);
            statGridView.Rows[0].HeaderCell.Value = "V";
            statGridView.Rows.Add(hString);
            statGridView.Rows[1].HeaderCell.Value = "H";

            for (int i = 0; i < 10; i++)
            {
                processState = taskManager.GetDictionary()[i].ToArray();
                statGridView.Rows.Add(processState);

                for (int j = 0; j < processState.Length; j++)
                {
                    if (processState[j].Contains("W"))
                    {
                        statGridView.Rows[i + 2].Cells[j].Style.BackColor = ColorTranslator.FromHtml("#B2C1FF");
                    }
                    else if (processState[j].Contains("L"))
                    {
                        statGridView.Rows[i + 2].Cells[j].Style.BackColor = ColorTranslator.FromHtml("#FFF4B2");
                    }
                    else if (processState[j].Contains("R"))
                    {
                        statGridView.Rows[i + 2].Cells[j].Style.BackColor = ColorTranslator.FromHtml("#FFB2D1");
                    }
                }

                statGridView.Rows[i + 2].HeaderCell.Value = "П" + (i + 1).ToString();
            }

            statGridView.ClearSelection();
        }

        private void FillExecuteGrid(ITaskManager taskManager)
        {
            int[] Tp = taskManager.GetTp();
            int[] realTp = taskManager.GetRealTp();
            int[] realStartT = taskManager.GetRealStartT();
            int[] realEndT = taskManager.GetRealEndT();
            int[] activeT = taskManager.GetActiveT();
            double[] Wi = taskManager.GetWi();

            string[] tpString = Tp.Select(x => x.ToString()).ToArray();
            string[] realTpString = realTp.Select(x => x.ToString()).ToArray();
            string[] realStartTString = realStartT.Select(x => x.ToString()).ToArray();
            string[] realEndTString = realEndT.Select(x => x.ToString()).ToArray();
            string[] activeTString = activeT.Select(x => x.ToString()).ToArray();
            string[] WiString = Wi.Select(x => x.ToString()).ToArray();

            executeGridView.Rows.Add(WiString);
            executeGridView.Rows[0].HeaderCell.Value = "Wi";
            
            executeGridView.Rows.Add(tpString);
            executeGridView.Rows[1].HeaderCell.Value = "tп";
            executeGridView.Rows[1].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#B2C1FF");

            executeGridView.Rows.Add(realTpString);
            executeGridView.Rows[2].HeaderCell.Value = "Поч. введ.";
            executeGridView.Rows[2].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFF4B2");

            executeGridView.Rows.Add(realStartTString);
            executeGridView.Rows[3].HeaderCell.Value = "Поч. вик.";
            executeGridView.Rows[3].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFB2D1");

            executeGridView.Rows.Add(realEndTString);
            executeGridView.Rows[4].HeaderCell.Value = "Кін. вик.";
            executeGridView.Rows[4].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFB2D1");

            executeGridView.Rows.Add(activeTString);
            executeGridView.Rows[5].HeaderCell.Value = "Час на пр.";

            executeGridView.ClearSelection();
        }

        private void FillGrid1(ITaskManager taskManager)
        {
            int[] X = taskManager.GetX();
            int[] K = taskManager.GetK();
            int[] Tp = taskManager.GetTp();

            string[] xString = X.Select(x => x.ToString()).ToArray();
            string[] kString = K.Select(x => x.ToString()).ToArray();
            string[] tpString = Tp.Select(x => x.ToString()).ToArray();

            dataGridView1.Rows.Add(xString);
            dataGridView1.Rows[0].HeaderCell.Value = "X";
            dataGridView1.Rows.Add(kString);
            dataGridView1.Rows[1].HeaderCell.Value = "K";
            dataGridView1.Rows.Add(tpString);
            dataGridView1.Rows[2].HeaderCell.Value = "tп";

            dataGridView1.ClearSelection();
        }

        private void FillGrid2(ITaskManager taskManager)
        {
            int[] V = taskManager.GetV();
            int[] H = taskManager.GetH();
            int[] T = taskManager.GetT();
            int[] Tp = taskManager.GetTp();
            int[] Tz = taskManager.GetTz();

            string[] VString = V.Select(x => x.ToString()).ToArray();
            string[] HString = H.Select(x => x.ToString()).ToArray();
            string[] tString = T.Select(x => x.ToString()).ToArray();
            string[] tpString = Tp.Select(x => x.ToString()).ToArray();
            string[] tzString = Tz.Select(x => x.ToString()).ToArray();


            dataGridView2.Rows.Add(VString);
            dataGridView2.Rows[0].HeaderCell.Value = "V";

            dataGridView2.Rows.Add(HString);
            dataGridView2.Rows[1].HeaderCell.Value = "H";

            dataGridView2.Rows.Add(tString);
            dataGridView2.Rows[2].HeaderCell.Value = "t";
            dataGridView2.Rows[2].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFB2D1");

            dataGridView2.Rows.Add(tpString);
            dataGridView2.Rows[3].HeaderCell.Value = "tп";
            dataGridView2.Rows[3].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#B2C1FF");

            dataGridView2.Rows.Add(tzString);
            dataGridView2.Rows[4].HeaderCell.Value = "tз";
            dataGridView2.Rows[4].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFF4B2");

            dataGridView2.ClearSelection();
        }

        private void statGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.Width = 50;
        }

       
    }
}
