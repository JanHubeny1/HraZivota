using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp2_hraZivota
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Casovac;
        Rectangle Bunka;
        Random rnd;
        int[,] pole = new int[36, 64];
        int[,] poleZmeny = new int[36, 64];
        int opakovani = 0;
        int pNGB;
        int msIC;
        bool podminkaOptimalizace;

        /// <summary>
        /// Inicializuje aplikaci, vytváří časovač
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Casovac = new DispatcherTimer();
            Casovac.Interval = TimeSpan.FromMilliseconds(500);
            Casovac.Tick += Casovac_Tick;
        }
        /// <summary>
        /// Obsahuje počet náhodných buněk od uživatele
        /// </summary>
        public int pocetNahodGenBunek
        {
            get
            {
                return pNGB;
            }
            set
            {
                if (value < 2304 && value >= 1)
                {
                    pNGB = value;
                }
                else
                {
                    pNGB = 500;
                    TBPocGenBunek.Text = Convert.ToString(500);
                }
            }
        }
        /// <summary>
        /// Obsahuje počet milisekund pro časovač od uživatele
        /// </summary>
        public int msIntervaluCasovace
        {
            get
            {
                return msIC;
            }
            set
            {
                if (value < 10000 && value >= 10)
                {
                    msIC = value;
                }
                else
                {
                    msIC = 500;
                    TBIntervalCasovac.Text = Convert.ToString(500);
                }
            }
        }
        /// <summary>
        /// Počítá, kolikrát proběhl celý proces hry
        /// </summary>
        public void PocitaniOpakovani()
        {
            opakovani++;
            LBOpakovani.Content = opakovani;
        }
        /// <summary>
        /// Vynuluje počítání opakování procesů hry
        /// </summary>
        public void VynulovaniPocitaniOpakovani()
        {
            opakovani = 0;
            LBOpakovani.Content = opakovani;
        }
        /// <summary>
        /// Celý algoritmus hry života, který určuje, jak se mají buňky chovat
        /// </summary>
        public void AlgoritmusHry()
        {

            int okoli1 = -1;
            int okoli2 = 0;
            for (int sloupec = 0; sloupec < pole.GetLength(0); sloupec++)
            {
                for (int radek = 0; radek < pole.GetLength(1); radek++)
                {
                    for (int i = sloupec - 1; i <= sloupec + 1; i++)
                    {
                        for (int j = radek - 1; j <= radek + 1; j++)
                        {
                            //živé
                            if (pole[sloupec, radek] == 1)
                            {
                                if (j > pole.GetLength(1) - 1 || i > pole.GetLength(0) - 1 || j < 0 || i < 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (pole[i, j] == 1)
                                    {
                                        okoli1++;
                                    }
                                }
                            }
                            //mrtvé
                            if (pole[sloupec, radek] == 2)
                            {
                                if (j > pole.GetLength(1) - 1 || i > pole.GetLength(0) - 1 || j < 0 || i < 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (pole[i, j] == 1)
                                    {
                                        okoli2++;
                                    }
                                }
                            }
                        }
                    }
                    //pro živé
                    if (pole[sloupec, radek] == 1)
                    {
                        if (okoli1 < 2)
                        {
                            poleZmeny[sloupec, radek] = 2;
                        }
                        else if (okoli1 == 2 || okoli1 == 3)
                        {
                            poleZmeny[sloupec, radek] = 1;
                        }
                        else
                        {
                            poleZmeny[sloupec, radek] = 2;
                        }
                    }
                    //pro mrtvé
                    if (pole[sloupec, radek] == 2)
                    {
                        if (okoli2 == 3)
                        {
                            poleZmeny[sloupec, radek] = 1;
                        }
                        else
                        {
                            poleZmeny[sloupec, radek] = 2;
                        }
                    }
                    okoli1 = -1;
                    okoli2 = 0;
                }
            }
            pole = poleZmeny;
            //poleZmeny = new int[36, 64];
        }
        /// <summary>
        /// Vykresluje a obnovuje stav rectanglů na Gridu GRHraciPole
        /// </summary>
        public void VykresleniHracihoPole()
        {
            int index = 0;
            for (int i = 0; i < pole.GetLength(0); i++)
            {
                for (int j = 0; j < pole.GetLength(1); j++)
                {
                    if (pole[i, j] == 1)
                    {
                        if (GRHraciPole.Children[index] is Rectangle rect)
                        {
                            rect.Tag = 1;
                            rect.Fill = Brushes.Green;
                        }
                    }
                    else
                    {
                        if (GRHraciPole.Children[index] is Rectangle rect)
                        {
                            rect.Tag = 2;
                            rect.Fill = Brushes.White;
                        }
                    }
                    index++;
                }
            }
        }
        /// <summary>
        /// Vypisuje informace o buňkach do konzole z dvourozměrného pole "pole" (1 nebo 2)
        /// </summary>
        /*
        public void VypisConsole()
        {
            Console.Clear();
            for (int i = 0; i < pole.GetLength(0); i++)
            {
                for (int j = 0; j < pole.GetLength(1); j++)
                {

                    Console.Write("{0} ", pole[i, j]);
                }
                Console.WriteLine();
            }
        }
        */
        /// <summary>
        /// Zastavuje časovač a zobrazuje možnosti nastavení
        /// </summary>
        public void CasovacZastaven()
        {
            Casovac.Stop();
            BTStart.Visibility = Visibility.Visible;
            BTStop.Visibility = Visibility.Hidden;
            RECSkrytiNast.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Spouští časovač a skrývá možnosti nastavení
        /// </summary>
        public void CasovacSpusten()
        {
            Casovac.Start();
            BTStart.Visibility = Visibility.Hidden;
            BTStop.Visibility = Visibility.Visible;
            RECSkrytiNast.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Vymaže všechny buňky z dvourozměrného pole "pole" (přepis na '2', nemění tagy rectanglů!)
        /// </summary>
        public void VymazaniVsechBunek()
        {
            for (int i = 0; i < pole.GetLength(0); i++)
            {
                for (int j = 0; j < pole.GetLength(1); j++)
                {
                    pole[i, j] = 2;
                }
            }
        }
        /// <summary>
        /// Ukončuje časovač, když se na poli nic neděje (případ: všude mrtvé buňky)
        /// </summary>
        public void OptimalizaceNecinnehoPole()
        {
            int pom = 0;
            for (int i = 0; i < pole.GetLength(0); i++)
            {
                for (int j = 1; j < pole.GetLength(1); j++)
                {
                    if (pole[i, j - 1] != pole[i, j])
                    {
                        pom++;
                        break;
                    }
                }
            }


            if (pom == 0)
            {
                CasovacZastaven();
                podminkaOptimalizace = true;
            }


        }
        /// <summary>
        /// Obsahuje metody k obnovování GRHraciPole
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Casovac_Tick(object sender, EventArgs e)
        {

            PocitaniOpakovani();
            AlgoritmusHry();
            VykresleniHracihoPole();
            OptimalizaceNecinnehoPole();
            //VypisConsole();
        }
        /// <summary>
        /// Spouští celou hru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTStart_Click(object sender, RoutedEventArgs e)
        {
            if (podminkaOptimalizace)
            {
                VynulovaniPocitaniOpakovani();
                podminkaOptimalizace = false;
            }

            int index = 0;
            for (int i = 0; i < pole.GetLength(0); i++)
            {
                for (int j = 0; j < pole.GetLength(1); j++)
                {
                    if (GRHraciPole.Children[index] is Rectangle rect)
                    {
                        if (rect.Tag is 1)
                        {
                            pole[i, j] = 1;
                        }
                        else pole[i, j] = 2;
                    }
                    index++;
                }
            }
            CasovacSpusten();
        }
        /// <summary>
        /// Zastavuje celou hru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTStop_Click(object sender, RoutedEventArgs e)
        {
            CasovacZastaven();
        }
        /// <summary>
        /// Otevře dialogové okno pro obnovení uloženého stavu hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTNahrat_Click(object sender, RoutedEventArgs e)
        {
            CasovacZastaven();
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "data files (*.txt)|*.txt";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //"zamkne soubor"
                using (System.IO.Stream soub = new System.IO.FileStream(openFileDialog.FileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite)) //filename - cesta k souboru, append - přidá se xxx na konec souboru
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(soub, Encoding.Default))
                    {
                        string radek;
                        int index = 0;
                        while ((radek = sr.ReadLine()) != null) //se stringem - kvůli odentrování
                        {
                            if (radek == "1")
                            {
                                if (GRHraciPole.Children[index] is Rectangle rect)
                                {
                                    rect.Tag = 1;
                                    rect.Fill = Brushes.Green;
                                }
                            }
                            else
                            {
                                if (GRHraciPole.Children[index] is Rectangle rect)
                                {
                                    rect.Tag = 2;
                                    rect.Fill = Brushes.White;
                                }
                            }
                            if (index < GRHraciPole.Children.Count - 1)
                            {
                                index++;

                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Otevře dialogové okno pro uložení stavu hry pro pozdější obnovení
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTUlozit_Click(object sender, RoutedEventArgs e)
        {
            CasovacZastaven();
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "data files (*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //"zamkne soubor"
                using (System.IO.Stream soub = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write)) //filename - cesta k souboru, append - přidá se xxx na konec souboru
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(soub, Encoding.Default))
                    {
                        for (int i = 0; i < pole.Length; i++)
                        {
                            if (GRHraciPole.Children[i] is Rectangle rect)
                            {
                                if (rect.Tag is 1)
                                {
                                    sw.WriteLine(1);
                                }
                                else sw.WriteLine(2);
                            }
                        }

                    }
                }
            }
        }
        /// <summary>
        /// Generuje buňky do gridu GRHraciPole
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GRHraciPole_Loaded(object sender, RoutedEventArgs e)
        {
            double sirka = GRHraciPole.ActualWidth;
            double vyska = GRHraciPole.ActualHeight;
            for (double i = 0; i < vyska; i += vyska / 36)
            {
                for (double j = 0; j < sirka; j += sirka / 64)
                {
                    Bunka = new Rectangle()
                    {
                        Width = sirka / 64,
                        Height = vyska / 36,
                        Margin = new Thickness(j, i, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5,
                        Tag = 2, //mrtvá buňka

                    };
                    Bunka.MouseDown += Bunka_MouseDown;
                    GRHraciPole.Children.Add(Bunka);
                }

            }
            //VypisConsole();

        }
        /// <summary>
        /// Mění buňku na živou (1) nebo mrtvou (2)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bunka_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CasovacZastaven();
            if (sender is Rectangle rect)
            {
                if (rect.Tag is 2)
                {
                    rect.Tag = 1;
                    rect.Fill = Brushes.Green;

                }
                else
                {
                    rect.Tag = 2;
                    rect.Fill = Brushes.White;
                }
            }


        }
        /// <summary>
        /// Generuje zadaný počet buněk na náhodná místa na gridu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTNahodnaGen_Click(object sender, RoutedEventArgs e)
        {
            rnd = new Random();
            pocetNahodGenBunek = Convert.ToInt32(TBPocGenBunek.Text);
            VymazaniVsechBunek();

            for (int i = 0; i < pocetNahodGenBunek; i++)
            {
                int a = rnd.Next(0, 36);
                int b = rnd.Next(0, 64);

                if (pole[a, b] == 2)
                {
                    pole[a, b] = 1;
                }
                else
                {
                    i--;
                }
            }
            VykresleniHracihoPole();
            VynulovaniPocitaniOpakovani();
        }
        /// <summary>
        /// Vynuluje počítání opakování
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTVynulovatOpakovani_Click(object sender, RoutedEventArgs e)
        {
            VynulovaniPocitaniOpakovani();
        }
        /// <summary>
        /// Vymaže všechny buňky na gridu (nastaví je na mrtvé)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTVycistitPole_Click(object sender, RoutedEventArgs e)
        {
            VymazaniVsechBunek();
            VykresleniHracihoPole();
            VynulovaniPocitaniOpakovani();
        }
        /// <summary>
        /// Ukončí aplikaci hry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTCloseApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Nastavuje interval podle vstupu uživatele
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BTNastavInterval_Click(object sender, RoutedEventArgs e)
        {
            msIntervaluCasovace = Convert.ToInt32(TBIntervalCasovac.Text);
            Casovac.Interval = new TimeSpan(0, 0, 0, 0, msIntervaluCasovace);
            LBAktualniInterval.Content = msIntervaluCasovace;
        }
    }
}
