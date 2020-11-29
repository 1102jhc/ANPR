using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

using System.Net.Http;
using System.IO;
/*using System.Windows.Media.Imaging;*/

namespace Recognize_VehicleLicensePlate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRecognize_Click(object sender, EventArgs e)
        {
            Bitmap img = new Bitmap(pictureBox1.Image);
            var ocr = new TesseractEngine("./tessdata", "kor", EngineMode.TesseractAndLstm);
            var texts = ocr.Process(img);
            MessageBox.Show(texts.GetText());
        }

        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> ProcessImage(string image_path)
        {
            string SECRET_KEY = "키설정을 해주세요.";

            Byte[] bytes = File.ReadAllBytes(image_path);
            string imagebase64 = Convert.ToBase64String(bytes);

            var content = new StringContent(imagebase64);

            var response = await client.PostAsync("https://api.openalpr.com/v3/recognize_bytes?recognize_vehicle=1&country=kr&secret_key=" + SECRET_KEY, content).ConfigureAwait(false);

            var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var byteArray = buffer.ToArray();
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            return responseString;
        }

        private void btnSector_Click(object sender, EventArgs e)
        {
            Task<string> recognizeTask = Task.Run(() => ProcessImage(@"C:\qqq.jpg"));
            recognizeTask.Wait();
            string task_result = recognizeTask.Result;

            string result = task_result.Split(':')[11].Split(',')[0];

            result = result.Replace("\"", "");

            result = result.Trim();  // 결과 값을 적절히 파싱

            textBox1.AppendText(result);

           /* System.Console.WriteLine(task_result);
            textBox1.Text = task_result;
           */
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
