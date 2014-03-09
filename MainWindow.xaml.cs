using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reader {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public MainWindow() {
            InitializeComponent();
            this.Text = System.IO.File.ReadAllText(@"..\..\Text.txt");
            this.Text = this.Text.Replace(' ', '\n');
            Task.Run(() => {
                var words = this.Text.Split('\n');
                try {
                    render(words);
                } catch (TaskCanceledException ex) {

                }
            });
        }

        private void render(string[] words) {
            foreach (var w in words) {
                if (pause) {
                    Thread.Sleep(20);
                    continue;
                }
                Dispatcher.Invoke((Action)(() => {
                    var s = this.MeasureString(w);
                    string OrpLetter;
                    var prefix = this.getPrefix(w, out OrpLetter);
                    var prefixSize = this.MeasureString(prefix);
                    this.LetterMargin = new Thickness(this.left , this.top, 0, 0);
                    this.LetterMarginBottom = new Thickness(this.left, this.top + 30, 0, 0);
                    this.textBlock.Margin = new Thickness(-prefixSize.Width + 40, 20, 0, 0);
                    this.LetterWidth = this.MeasureString(OrpLetter).Width;
                    this.left += s.Width + spaceWidth;
                    this.left = 40;
                    //28.39 is too slow
                    top += s.Height + 28.395;
                    this.gridRoot.Margin = new Thickness(0, -this.top + Height / 2, 0, 0);

                }));
                Thread.Sleep(sleep);
            }
        }

        private int sleep = 100;

        private string getPrefix(string word, out string letter) {

            switch (word.Length) {
                case 0:
                    letter = "";
                    return "";
                case 1:
                    letter = "";
                    return word;
                case 2:
                case 3:
                case 4:
                case 5:
                    letter = string.Concat(word.Skip(1).Take(1));
                    return string.Concat(word.Take(1));
                case 6:
                case 7:
                case 8:
                case 9:
                    letter = string.Concat(word.Skip(2).Take(1));
                    return string.Concat(word.Take(2));
                default:
                    letter = string.Concat(word.Skip(3).Take(1));
                    return string.Concat(word.Take(3));
            }
        }

        private readonly double spaceWidth = 5.5;
        private double left = 20;
        private double top = 20;

        private Thickness _LetterMargin;
        public Thickness LetterMargin {
            get { return _LetterMargin; }
            set {
                if (_LetterMargin != value) {
                    _LetterMargin = value;
                    OnPropertyChanged("LetterMargin");
                }
            }
        }

        private Thickness _LetterMarginBottom;
        public Thickness LetterMarginBottom {
            get { return _LetterMarginBottom; }
            set {
                if (_LetterMarginBottom != value) {
                    _LetterMarginBottom = value;
                    OnPropertyChanged("LetterMarginBottom");
                }
            }
        }

        private double _LetterWidth;
        public double LetterWidth {
            get { return _LetterWidth; }
            set {
                if (_LetterWidth != value) {
                    _LetterWidth = value;
                    OnPropertyChanged("LetterWidth");
                }
            }
        }

        private Size MeasureString(string candidate) {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(this.textBlock.FontFamily, this.textBlock.FontStyle, this.textBlock.FontWeight, this.textBlock.FontStretch),
                this.textBlock.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private string _Text;
        public string Text {
            get { return _Text; }
            set {
                if (_Text != value) {
                    _Text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            var eh = PropertyChanged;
            if (eh != null) {
                eh(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool pause = false;

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e) {

            switch (e.Key) {
                case Key.Space:
                    pause = !pause;
                    break;
                case Key.Up:
                    this.sleep += 5;
                    break;
                case Key.Down:
                    this.sleep -= 5;

                    break;
            }
        }
    }
}
