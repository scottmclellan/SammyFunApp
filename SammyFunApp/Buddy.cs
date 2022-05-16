using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SammyFunApp
{
    public class Buddy : Image
    {
        public enum BuddyState
        {
            Winking,
            RunningAway,
            Caught,
            Peeking
        }

        private readonly string _character;
        private BuddyState _state;

        public Buddy(string character)
        {
            _character = character;
            _state = BuddyState.Peeking;
            SetImage();
        }

        private void SetImage()
        {
            Stream image = null;

            switch (_state)
            {
                case BuddyState.Winking:
                    image =Images.ResourceManager.GetStream($"{_character}_wink");
                    break;
                case BuddyState.RunningAway:
                    image = Images.ResourceManager.GetStream($"{_character}");
                    break;
                case BuddyState.Peeking:
                    image = Images.ResourceManager.GetStream($"{_character}");
                    break;
                case BuddyState.Caught:
                    image = Images.ResourceManager.GetStream($"{_character}_scared");
                    break;
                default:
                    break;
            }

            BitmapImage bmp = new BitmapImage();

            bmp.BeginInit();

            bmp.StreamSource = image;

            bmp.EndInit();

            this.Source = bmp;
            this.RenderSize = new Size(bmp.Width, bmp.Height);
        }


        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Caught();
            base.OnMouseEnter(e);
        }

        public void Caught()
        {
            _state = BuddyState.Caught;
            SetImage();
        }

        public void Wink()
        {
            _state = BuddyState.Winking;
            SetImage();
        }
    }
}
