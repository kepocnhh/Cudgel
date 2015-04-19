using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cudgel
{
    public partial class Form1 : Form , IMicroListener
    {

#region FIELDS
//__________FIELDS____________________________\\

        private Micro micro;

//____________________________________________//
#endregion

        public Form1()
        {
            InitializeComponent();
            //
            micro = new Micro();
            micro.setMicroListener(this);
        }
        public virtual void getAudioLevel(int al)
        {
            lblmixervalue.Text = al + "";
        }
        public virtual void toRecognize(byte[] data)
        {
        }

        private void bengage_Click(object sender, EventArgs e)
        {
            micro.StarTrek();
        }

        private void bstop_Click(object sender, EventArgs e)
        {
            micro.StopRecording();
        }

        private void baccept_Click(object sender, EventArgs e)
        {

        }
    }
}