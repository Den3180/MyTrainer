using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace MyTrainer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Максимальное число символов в строке.
        int stringMaxLenght;
        //Счетчик ошибок.
        int errorCounter;
        //Счетчик времени.
        int countTimer;
        //Счетчик правильных символов.
        int countCorrectChar;
        //Индекс элементов в коллекции клавиш.
        int indexKey;
        //Ссылки на кисти для промежуточного храниния сведений о кистях клавиш.
        Brush br,brushSpace;
        //Коллекция символов(количество зависит от выбранной сложности).
        List<char> characters;
        //Коллекция клавиш.
        List<Border> myKey;
        //Переменная таймера.
        DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            //Первый запуск.
            FirstStart();
            //Заполнение коллекции клавиш.
            FillingList();
        }
        //Обобщенный метод поиска клавиш на форме.
            public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
            {
                if (depObj != null)
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                        if (child != null && child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindVisualChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        //Внесение клавиш в коллекцию.
        private void FillingList()
        {
            myKey = new List<Border>();
            foreach (Border tb in FindVisualChildren<Border>(mainGrid))
            {
                myKey.Add(tb);
            }
        }
     //Заполнение формы при первом запуске.
        private void FirstStart()
        {
            textDifficulty.Text = (slider.Value).ToString();
            stopButton.IsEnabled = false;
            stringMaxLenght = 68;
            errorCounter = 0;
            countTimer = 0;
            countCorrectChar = 0;
            indexKey = -1;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += delegate { ++countTimer; };
        }     
        //Генерация строки 1,2,3,4 уровня сложности.
        private void Slider_1_2_3_4()
        {
            Random rand = new Random();
            int tempLower = 0;
            int temp;
            //Заполнение коллекции символов происходит до момента, пока количество элементов меньше чем сложность*3
            //3 - это коэффициент, что б слишком просто не было.
            while (characters.Count < slider.Value * 3)
            {
                tempLower = rand.Next(97, 122);
                //Проверка что бы не было повторов в коллекции.
                if (!characters.Contains((char)tempLower))
                {
                    characters.Add((char)tempLower);
                }
            }
            //Генерация строки.
            for (int i = 0; i < stringMaxLenght; i++)
            {
                //Вставка пробелов.
                temp = rand.Next(5);
                if (temp == 0 && textUpStringBlue.Text != "" && textUpStringBlue.Text[textUpStringBlue.Text.Length - 1] != (char)32)
                {
                    textUpStringBlue.Text += " ";
                }
                //Если чекбокс заглавных букв отключен.
                else if (checkbox.IsChecked == false)
                {
                    int r = rand.Next(characters.Count);
                    textUpStringBlue.Text += characters[r].ToString();
                }
                //Если чекбокс заглавных букв включен.
                else if (checkbox.IsChecked == true)
                {
                    int r = rand.Next(characters.Count);
                    if (rand.Next(5) == 0)
                    {
                        textUpStringBlue.Text += (Char.ToUpper(characters[r])).ToString();
                    }
                    else
                    {
                        textUpStringBlue.Text += characters[r].ToString();
                    }
                }
            }
        }
        //Генерация строки 5 уровня сложности.
        private void Slider_5()
        {
            Random rand = new Random();
            int temp;
            for (int i = 0; i < stringMaxLenght; i++)
            {
                //Вставка пробелов.
                temp = rand.Next(5);
                if (temp == 0 && textUpStringBlue.Text != "" && textUpStringBlue.Text[textUpStringBlue.Text.Length - 1] != (char)32)
                {
                    textUpStringBlue.Text += " ";
                }
                //Вставка всех символов клавиатуры.
                else
                {
                    int r = rand.Next(32, 126);
                    textUpStringBlue.Text += ((char)r).ToString();
                }
            }
        }
        //Генератор текста.
        private void GenerateText()
        {
            characters = new List<char>();

            switch (slider.Value)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    Slider_1_2_3_4();
                    break;
                case 5:
                    Slider_5();
                    break;
            }
        }
        //Нажата кнопка старт.
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            stopButton.IsEnabled = true;
            startButton.IsEnabled = false;
            textDownStringGreen.Focus();
            slider.IsEnabled = false;
            checkbox.IsEnabled = false;
            GenerateText();
        }
        //Отображение уровня выбранной сложности.
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textDifficulty.Text = (slider.Value).ToString(); 
        }
        //Нажата кнопка стоп.
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            textUpStringBlue.Text = "";
            textUpStringGreen.Text = "";
            textDownStringGreen.Text = "";
            mainGrid.Background = null;
            characters.Clear();
            slider.IsEnabled = true;
            checkbox.IsEnabled = true;
            errorCounter = 0;
            errorCount.Text = "0";
            countCorrectChar = 0;
            countTimer = 0;
            speed.Text = "0";
            if (indexKey != -1)
            {
            myKey[indexKey].Background = br;
            }

        }
        //Щтображение текста в пользовательской строке.
        private void TextDownStringGreen_TextInput(object sender, TextCompositionEventArgs e)
        {
            //Условие печати символов.
            if (Char.IsLetterOrDigit(e.Text, 0) || e.Text == " " || Char.IsPunctuation(e.Text, 0) || Char.IsSymbol(e.Text, 0))
            {     
                //Запуск таймера.
                if (countTimer == 0)
                {
                    _timer.Start();
                }
                //Если введен верный символ.
                if (e.Text == textUpStringBlue.Text[0].ToString())
                {
                    //Увеличиваем счетчик правильных символов.
                    countCorrectChar++;
                    //Записываем символ в зеленую строку.
                    textUpStringGreen.Text += textUpStringBlue.Text[0].ToString();
                    //Убираем символ из синей строки.
                    textUpStringBlue.Text = textUpStringBlue.Text.Remove(0, 1);
                    //Записываем символ в строку пользователя.
                    textDownStringGreen.Text += e.Text;
                }
                //Если пользователь ошибся.
                else
                {
                    //Увеличиваем счетчик ошибок.
                    errorCount.Text = (++errorCounter).ToString();
                    //Включаем вспышку формы при ошибке.
                    DispatcherTimer dt = new DispatcherTimer();
                    dt.Interval = TimeSpan.FromMilliseconds(300);
                    mainGrid.Background = Brushes.Pink;
                    dt.Tick += delegate
                    {
                        mainGrid.Background = Brushes.Transparent;
                        dt.Stop();
                    };
                    dt.Start();
                    textDownStringGreen.Focus();
                }
                //Считаем скорость печати.
                if (countCorrectChar%5==0)
                {
                    int _speed = (int)((double)countCorrectChar / ((double)countTimer / 60));
                    speed.Text = _speed.ToString();
                }
                //Если достигнута граница строки, генерируем новую.
                if (textDownStringGreen.Text.Length == stringMaxLenght)
                {
                    textDownStringGreen.Text = "";
                    textUpStringGreen.Text = "";
                    textUpStringBlue.Text = "";
                    GenerateText();
                }
                //Подсветка нажатых клавиш.
                if (indexKey != -1)
                {
                    myKey[indexKey].Background = br;
                }

                foreach (var item in myKey)
                {
                    TextBlock childItem = (TextBlock)item.Child;
                    if (childItem.Text == e.Text)
                    {
                        indexKey = myKey.IndexOf(item);
                        br = item.Background.Clone();
                        item.Background = Brushes.White;
                    }
                  
                }
            }
        }
        //Действия при нажатии клавиш на клавиатуре.
        private void TextDownStringGreen_KeyDown(object sender, KeyEventArgs e)
        {   
            //Нажат Shift
            if (e.Key.ToString() == "LeftShift" || e.Key.ToString() == "RightShift")
            {
                //Смена панелей клавиш на форме.
                panel_1.Visibility = Visibility.Collapsed;
                panel_2.Visibility = Visibility.Visible;
            }
            //Если нажат пробел.
            if (e.Key == Key.Space)
            {
                //Посветка пробела.
                brushSpace = spaceUp.Background.Clone();
                spaceUp.Background = Brushes.White;
                spaceDown.Background = Brushes.White;
            }
        }
        //Действия, если клавиши отпущены.
        private void TextDownStringGreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "LeftShift" || e.Key.ToString() == "RightShift")
            {
                //Смена панелей клавиш на форме.
                panel_1.Visibility = Visibility.Visible;
                panel_2.Visibility = Visibility.Collapsed;
            }
            if (e.Key == Key.Space)
            {
                //Возврат к первоначальному цвету.
                spaceUp.Background = brushSpace;
                spaceDown.Background =brushSpace;
            }
        }
    }
}
