using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SammyFunApp
{
    public sealed class SpeechHelper
    {
        private static readonly Lazy<SpeechHelper> lazy =
            new Lazy<SpeechHelper>(() => new SpeechHelper());

        public static SpeechHelper Instance { get { return lazy.Value; } }


        private WaveOutEvent _wavPlayer;
        private WaveFormat _wavFormat = new WaveFormat(22050, 16, 1);

        private SpeechHelper()
        {

        }

        public void SpeakAsync(Action onComplete = null, params string[] messages)
        {
            PlayAudio(onComplete, messages);
        }

        private void PlayAudio(Action onPlayStop, params string[] messages)
        {
            //get audio
            Stream[] audioStreams = new Stream[messages.Length];
            Stream combinedStream = null;


            for (int i = 0; i < messages.Length; i++)
            {
                audioStreams[i] = Utils.ResourceHelper.GetResourceData($"SammyFunApp.Audio.{messages[i].CreateFileName()}");
            }


            if (audioStreams.Length == 1)
                combinedStream = audioStreams[0];
            else
            {
                combinedStream = new MemoryStream(CombineStreams(audioStreams));
            }


            if (combinedStream == null) return;

            if (combinedStream.CanSeek) combinedStream.Seek(0, SeekOrigin.Begin);

            combinedStream.Position = 0;

            _wavPlayer = new WaveOutEvent();
            var audioReader = new WaveFileReader(combinedStream);
            _wavPlayer.Init(audioReader);
            _wavPlayer.Play();

            _wavPlayer.PlaybackStopped += (o, e) =>
            {
                onPlayStop?.Invoke();
                _wavPlayer.Dispose();
            };
        }

        private byte[] CombineStreams(Stream[] audioStreams)
        {
            MemoryStream tempStream = new MemoryStream();

            return CombineStreamsCrossFade(audioStreams, tempStream);
        }

        private static byte[] CombineStreamsCrossFade(Stream[] audioStreams, MemoryStream tempStream)
        {
            WaveFormat waveformat = null;
            WaveFileWriter output = null;
            float volume;
            float volumemod;
            float[] sample;
            try
            {
                // Alrighty.  Let's march over them.  We'll index them (rather than
                // foreach'ing) so that we can monitor first/last file.
                for (int index = 0; index < audioStreams.Length; ++index)
                {

                    // Grab the file and use an 'audiofilereader' to load it.
                    Stream audioStream = audioStreams[index];

                    if (audioStream == null) continue;

                    using (WaveFileReader reader = new WaveFileReader(audioStream))
                    {
                        // Get our first/last flags.
                        bool firstfile = (index == 0);
                        bool lastfile = (index == audioStreams.Length - 1);

                        // If it's the first...
                        if (firstfile)
                        {
                            // Initialize the writer.
                            waveformat = reader.WaveFormat;
                            output = new WaveFileWriter(tempStream, waveformat);
                        }
                        else
                        {
                            // All files must have a matching format.
                            if (!reader.WaveFormat.Equals(waveformat))
                            {
                                throw new InvalidOperationException("Different formats");
                            }
                        }


                        long fadeinsamples = 0;
                        if (!firstfile)
                        {
                            // Assume 1 second of fade in, but set it to total size
                            // if the file is less than one second.
                            fadeinsamples = waveformat.SampleRate / 25;
                            if (fadeinsamples > reader.SampleCount) fadeinsamples = reader.SampleCount;

                        }

                        // Initialize volume and read from the start of the file to
                        // the 'fadeinsamples' count (which may be 0, if it's the first
                        // file).
                        volume = 0.0f;
                        volumemod = 1.0f / (float)fadeinsamples;
                        int sampleix = 0;
                        while (sampleix < (long)fadeinsamples)
                        {
                            sample = reader.ReadNextSampleFrame();
                            for (int floatix = 0; floatix < waveformat.Channels; ++floatix)
                            {
                                sample[floatix] = sample[floatix] * volume;
                            }

                            // Add modifier to volume.  We'll make sure it isn't over
                            // 1.0!
                            if ((volume = (volume + volumemod)) > 1.0f) volume = 1.0f;

                            // Write them to the output and bump the index.
                            output.WriteSamples(sample, 0, sample.Length);
                            ++sampleix;
                        }

                        // Now for the time between fade-in and fade-out.
                        // Determine when to start.
                        long fadeoutstartsample = reader.SampleCount;
                        //if( !lastfile )
                        {
                            // We fade out every file except the last.  Move the
                            // sample counter back by one second.
                            fadeoutstartsample -= waveformat.SampleRate;
                            if (fadeoutstartsample < sampleix)
                            {
                                // We've actually crossed over into our fade-in
                                // timeframe.  We'll have to adjust the actual
                                // fade-out time accordingly.
                                fadeoutstartsample = reader.SampleCount - sampleix;
                            }
                        }

                        // Ok, now copy everything between fade-in and fade-out.
                        // We don't mess with the volume here.
                        while (sampleix < (int)fadeoutstartsample)
                        {
                            sample = reader.ReadNextSampleFrame();
                            output.WriteSamples(sample, 0, sample.Length);
                            ++sampleix;
                        }

                        // Fade out is next.  Initialize the volume.  Note that
                        // we use a bit-shorter of a time frame just to make sure
                        // we hit 0.0f as our ending volume.
                        long samplesleft = reader.SampleCount - fadeoutstartsample;
                        volume = 1.0f;
                        volumemod = 1.0f / ((float)samplesleft * 0.95f);

                        // And loop over the reamaining samples
                        while (sampleix < (int)reader.SampleCount)
                        {
                            // Grab a sample (one float per channel) and adjust by
                            // volume.
                            sample = reader.ReadNextSampleFrame();
                            for (int floatix = 0; floatix < waveformat.Channels; ++floatix)
                            {
                                sample[floatix] = sample[floatix] * volume;
                            }

                            // Subtract modifier from volume.  We'll make sure it doesn't
                            // accidentally go below 0.
                            if ((volume = (volume - volumemod)) < 0.0f) volume = 0.0f;

                            // Write them to the output and bump the index.
                            output.WriteSamples(sample, 0, sample.Length);
                            ++sampleix;
                        }
                    }
                }

                output.Flush();

                return tempStream.ToArray();
            }
            finally
            {
                output.Dispose();
            }
        }

        private static byte[] CombineStreamsInternal(Stream[] audioStreams, MemoryStream tempStream)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;

            try
            {
                bool firstFile = false;
                bool lastFile = false;
                //foreach (var stream in audioStreams)
                for (int i = 0; i < audioStreams.Length; i++)
                {
                    // Get our first/last flags.
                    firstFile = (i == 0);
                    lastFile = (i == audioStreams.Length - 1);

                    Stream stream = audioStreams[i];

                    if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
                    stream.Position = 0;

                    //using (RawSourceWaveStream reader = new RawSourceWaveStream(stream, _wavFormat))
                    using (WaveFileReader reader = new WaveFileReader(stream))
                    {
                        if (waveFileWriter == null)
                        {
                            // first time in create new Writer
                            waveFileWriter = new WaveFileWriter(tempStream, reader.WaveFormat);
                        }
                        else
                        {
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                            {
                                throw new InvalidOperationException("Can't concatenate WAV Files that don't share the same format");
                            }
                        }


                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteData(buffer, 0, read);
                        }
                    }
                }

                waveFileWriter.Flush();

                return tempStream.ToArray();
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    waveFileWriter.Dispose();
                }
            }
        }

        public void Speak(params string[] text)
        {
            PlayAudio(null, text);
        }
    }


    public sealed class SpeechHelperMp3
    {
        private static readonly Lazy<SpeechHelperMp3> lazy =
            new Lazy<SpeechHelperMp3>(() => new SpeechHelperMp3());

        public static SpeechHelperMp3 Instance { get { return lazy.Value; } }


        private WaveOutEvent _wavPlayer;


        private SpeechHelperMp3()
        {

        }

        public void SpeakAsync(Action onComplete = null, params string[] messages)
        {
            PlayAudio(onComplete, messages);
        }

        private void PlayAudio(Action onPlayStop, params string[] messages)
        {
            //get audio
            Stream[] audioStreams = new Stream[messages.Length];
            Stream combinedStream = null;


            for (int i = 0; i < messages.Length; i++)
            {
                audioStreams[i] = Utils.ResourceHelper.GetResourceData($"SammyFunApp.Audio.{messages[i].CreateFileNameMp3()}");
            }


            if (audioStreams.Length == 1)
                combinedStream = audioStreams[0];
            else
            {
                combinedStream = CombineStreams(audioStreams);
            }


            if (combinedStream == null) return;

            if (combinedStream.CanSeek) combinedStream.Seek(0, SeekOrigin.Begin);

            combinedStream.Position = 0;

            _wavPlayer = new WaveOutEvent();
            //var audioReader = new RawSourceWaveStream(combinedStream, _wavFormat);
            var audioReader = new Mp3FileReader(combinedStream);
            _wavPlayer.Init(audioReader);
            _wavPlayer.Play();

            _wavPlayer.PlaybackStopped += (o, e) =>
            {
                onPlayStop?.Invoke();
                _wavPlayer.Dispose();
            };
        }

        private Stream CombineStreams(Stream[] audioStreams)
        {

            Stream outputStream = new MemoryStream();

            byte[] buffer = new byte[1024];

            try
            {
                bool firstFile = false;
                bool lastFile = false;
                //foreach (var stream in audioStreams)
                for (int i = 0; i < audioStreams.Length; i++)
                {
                    // Get our first/last flags.
                    firstFile = (i == 0);
                    lastFile = (i == audioStreams.Length - 1);

                    Stream stream = audioStreams[i];

                    if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
                    stream.Position = 0;

                    //using (RawSourceWaveStream reader = new RawSourceWaveStream(stream, _wavFormat))
                    using (Mp3FileReader reader = new Mp3FileReader(stream))
                    {
                        if ((outputStream.Position == 0) && (reader.Id3v2Tag != null))
                        {
                            outputStream.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
                        }

                        Mp3Frame frame;

                        while ((frame = reader.ReadNextFrame()) != null)
                        {
                            outputStream.Write(frame.RawData, 0, frame.RawData.Length);
                        }

                    }
                }

                return outputStream;

            }
            finally
            {
                //if (waveFileWriter != null)
                //{
                //    waveFileWriter.Dispose();
                //}
            }
        }

        public void Speak(params string[] text)
        {
            PlayAudio(null, text);
        }
    }
}
