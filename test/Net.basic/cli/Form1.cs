using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nirge.Core;
using System.Net;
using System.Net.Sockets;

namespace cli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Cli1 _cli1;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _cli1 = new Cli1();
            _cli1.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));

            timerExec.Tick += TimerExec_Tick;
        }
        private void TimerExec_Tick(object sender, EventArgs e)
        {
            _cli1.Exec();
        }
    }
}
