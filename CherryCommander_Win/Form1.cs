using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excimer;
using CherryCommander.Core;

namespace CherryCommander_Win
{
    public partial class Form1 : Form
    {
        private CherryCommanderService cherryCommander = new CherryCommanderService();

        public Form1()
        {
            InitializeComponent();
        }
    }
}
