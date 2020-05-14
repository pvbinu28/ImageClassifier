using Emgu.CV;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML.OnnxRuntime;


namespace LiveImageClassifier
{
    public partial class Form1 : Form
    {
        private const string PredictionUrl = "https://flower.cognitiveservices.azure.com/customvision/v3.0/Prediction/72a14e7b-aafc-43b2-9745-e3f776b45515/classify/iterations/Iteration1/image";
        private const string KeyHeaderName = "Prediction-Key";
        private const string KeyHeaderValue = "e5f87411288d46d2bf48cd66f1143603";
        private const string ContentType = "application/octet-stream";
        private Task requestTask;
        private static VideoCapture capture;

        public Form1()
        {
            InitializeComponent();
            Run();
        }

        void Run()
        {
           if(capture == null)
            {
                try
                {
                    capture = new VideoCapture();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

           if(capture != null)
            {
                Application.Idle += ProcessFrame;
            }
        }

        void ProcessFrame(object sender, EventArgs e)
        {
            var frame = capture.QueryFrame();
            imageBox1.Image = frame;
           

            if(requestTask == null)
            {
                requestTask = GetPrediction(frame);
            } 

            if(requestTask.IsCompleted)
            {
                requestTask = GetPrediction(frame);
            }                 
        }

        private async Task GetPrediction(Mat frame)
        {
            Bitmap bitmap = frame.ToBitmap();
            var imageArray = GetByteArray(bitmap);

            var request = WebRequest.Create(PredictionUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = ContentType;

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, ContentType);
                client.Headers.Add(KeyHeaderName, KeyHeaderValue);
                var result = await client.UploadDataTaskAsync(PredictionUrl, imageArray);
                var jsonString = System.Text.Encoding.Default.GetString(result);
                SetFlowerType(jsonString);
            }
        }

        public void SetFlowerType(string jsonString)
        {
            var jsonObj = JsonConvert.DeserializeObject<PredictionModel>(jsonString);
            var topPred = jsonObj.Predictions.OrderByDescending(item => item.Probability).FirstOrDefault();
            if(topPred.Probability * 100 >= 90)
            {
                label2.Text = topPred.TagName.ToUpper();
            }
            else
            {
                label2.Text = string.Empty;
            }
        }

        private byte[] GetByteArray(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
