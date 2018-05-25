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
            var s = SpeechFactory.FromEndPoint(new Uri("wss://westus.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?cid=064bf7a0-ddc1-4b8e-ae58-e07e62318849"), "{key}");

            var r2 = s.CreateSpeechRecognizerWithStream(new AudioStreamReader(new BinaryReader(File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "b0017.wav")))));

            r2.FinalResultReceived += (sender, e) =>
            {
                Console.WriteLine(e.Result.RecognizedText);
            };

            await r2.StartContinuousRecognitionAsync();
            await Task.Delay(TimeSpan.FromMinutes(1));
        }

    }

    public class AudioStreamReader : AudioInputStream
    {
        private static readonly AudioInputStreamFormat DefaultFormat = new AudioInputStreamFormat
        {
            FormatTag = 1,
            Channels = 1,
            SamplesPerSec = 16000,
            AvgBytesPerSec = 16000 * 1 * 16 / 8,
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
