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
using Newtonsoft.Json;
using JsonExSerializer;

namespace Cudgel
{
    public partial class Form1 : Form, IMicroListener, IRecognitionListener
    {

#region FIELDS _________________________________
//                                                                                        \\

        private Micro micro;
        private VoiceRecognition voice;

//____________________________________________//
#endregion

        public Form1()
        {
            InitializeComponent();
            //
            micro = new Micro();
            micro.setMicroListener(this);
            voice = new VoiceRecognition();
            voice.setRecognitionListener(this);
        }

#region MicroListener ____________________________
//                                                                                        \\

        public virtual void getAudioLevel(int al)
        {
            lblmixervalue.Text = al + "";
        }
        public virtual void toRecognize(byte[] data)
        {
            voice.recognize(data);
        }

//____________________________________________//
#endregion

#region RecognitionListener ________________________
//                                                                                        \\

        public virtual void getResponse(StreamReader SR_Response)
        {
            string json = SR_Response.ReadToEnd();
            Serializer serializer = new Serializer(typeof(GoogleResponse));
            json = json.Split('\n')[1];
            if(json.Length == 0)
            {
                json = " - empty -";
            }
            else
            {
                GoogleResponse deserialized = (GoogleResponse)serializer.Deserialize(json);
                json = deserialized.result[0].alternative[0].transcript;
            }
            addToList(recognize, json);
            scrollToEnd(recognize);
        }

//____________________________________________//
#endregion


        void addToList(ListBox lb, string newText)
        {
            this.MaybeInvoke(() => lb.Items.Add(newText));
        }
        void scrollToEnd(ListBox lb)
        {
            this.MaybeInvoke(() => lb.SelectedIndex = lb.Items.Count - 1);
            this.MaybeInvoke(() => lb.SelectedIndex = - 1);
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