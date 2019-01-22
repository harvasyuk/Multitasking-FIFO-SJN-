using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FIFO
{
    public partial class CompareForm : Form
    {

        List<string> multiProcList = new List<string>();

        public CompareForm(List<string> multiProcList)
        {
            InitializeComponent();
            this.multiProcList = multiProcList;

            wLabel.Text = multiProcList[0];
            mLabel.Text = multiProcList[1];
            rLabel.Text = multiProcList[2];

            wLabel1.Text = multiProcList[3];
            mLabel2.Text = multiProcList[4];
            rLabel2.Text = multiProcList[5];

        }


    }
}
