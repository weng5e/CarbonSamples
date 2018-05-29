using Microsoft.CognitiveServices.Speech;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Core20Speech
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunAsync().Wait();

            Console.Read();
        }

        public static async Task RunAsync()
        {
            var factory = SpeechFactory.FromEndPoint(new Uri("wss://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?cid=ehhhjfa"), "");

            while (true)
            {
                var speechRecognizer = factory.CreateSpeechRecognizerWithStream(new AudioStreamReader(new BinaryReader(File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "b0017.wav")))));

                speechRecognizer.FinalResultReceived += OnResult;

                try
                {
                    await speechRecognizer.StartContinuousRecognitionAsync();
                    await Task.Delay(TimeSpan.FromMinutes(0.5));
                }
                catch { }
                finally
                {
                    await speechRecognizer.StopContinuousRecognitionAsync();
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }

        private static void OnResult(object sender, SpeechRecognitionResultEventArgs e)
        {
            Console.WriteLine("[RecognizedText] " + e.Result.RecognizedText);
        }

    }

    public class AudioStreamReader : AudioInputStream
    {
        private static readonly AudioInputStreamFormat DefaultFormat = new AudioInputStreamFormat
        {
            FormatTag = 1,
            Channels = 1,
            SamplesPerSec = 16000,
            AvgBytesPerSec = 16000 * 16 / 8,
            BlockAlign = 1 * 16 / 8,
            BitsPerSample = 16,
        };

        private BinaryReader _reader;

        public AudioStreamReader(BinaryReader reader) => _reader = reader;

        public override AudioInputStreamFormat GetFormat() => DefaultFormat;

        public override int Read(byte[] dataBuffer, int size) => _reader.Read(dataBuffer, 0, size);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reader.Dispose();
            }
        }
    }
}
