using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.PTZScenes
{
    public partial class EditTrigger : Form
    {
        PTZScene monitor;
        public EditTrigger(PTZScene id)
        {
            InitializeComponent();
            monitor = id;
            comboBox1.SelectedIndex = (int)monitor.Trigger;
            dateTimePicker1.Value = System.Convert.ToDateTime(monitor.DefaultTriggerData.Timer_Timeout.ToString());
            dateTimePicker2.Value = monitor.DefaultTriggerData.Date_Period < dateTimePicker2.MinDate ? DateTime.Now : monitor.DefaultTriggerData.Date_Period;
            comboBox2.Items.AddRange(Structures.Load().Select(tmp => tmp.IP).ToArray());
            comboBox2.SelectedIndex = (int)monitor.DefaultTriggerData.CameraTrigger;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public TriggerData ReturnData
        {
            get
            {
                monitor.DefaultTriggerData.Date_Period = dateTimePicker2.Value;
                monitor.DefaultTriggerData.Timer_Timeout = dateTimePicker1.Value;
                monitor.DefaultTriggerData.CameraTrigger = (uint)comboBox2.SelectedIndex;

                return monitor.DefaultTriggerData;
            }
        }
        public TriggerPTZ ReturnTrigger
        {
            get
            {
                return (TriggerPTZ)comboBox1.SelectedIndex;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
