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

        private Dictionary<string, byte[]> _audioCache = new Dictionary<string, byte[]>();
        private void PlayAudio(Action onPlayStop, params string[] messages)
        {
            //get audio
            byte[][] audioStreams = new byte[messages.Length][];
            Stream combinedStream = null;

            string audioFileName;

            for (int i = 0; i < messages.Length; i++)
            {
                audioFileName = $"SammyFunApp.Audio.{messages[i].CreateFileName()}";

                if (!_audioCache.ContainsKey(audioFileName))
                {

                    var tmp = Utils.ResourceHelper.GetResourceData(audioFileName);

                    if (tmp == null) continue;

                    _audioCache.Add(audioFileName, tmp.ToArray());
                }

                audioStreams[i] = _audioCache[audioFileName];

            }

            if (audioStreams.Count(x => x != null) == 0) return;//nothing to play

            if (audioStreams.Length == 1)
                combinedStream = new MemoryStream(audioStreams[0]);
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
            return CombineStreamsCrossFade(audioStreams);
        }

        private byte[] CombineStreams(byte[][] audioStreams)
        {
            return CombineStreamsCrossFade(audioStreams);
        }


        private static byte[] CombineStreamsCrossFade(byte[][] audioStreams)
        {
            return CombineStreamsCrossFade(audioStreams.Where(x => x != null).Select(x => new MemoryStream(x)).ToArray());
        }


        private static byte[] CombineStreamsCrossFade(Stream[] audioStreams)
        {
            WaveFormat waveformat = null;
            WaveFileWriter output = null;
            float volume;
            float volumemod;
            float[] sample;

            MemoryStream tempStream = new MemoryStream();

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


        public void Speak(params string[] text)
        {
            PlayAudio(null, text);
        }
    }
}
