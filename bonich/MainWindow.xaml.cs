using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace bonich
{
    public partial class MainWindow : Window
    {
        public class Report
        {
            public string kw;
            public int count;
            public Report(string k, int c)
            {
                kw = k;
                count = c;
            }
        }
        public class URLs
        {
            public int ID { get; set; }
            public string Keyword { get; set; }
            public string URL { get; set; }
        }
        bool testing = false;

        readonly List<URLs> surls = new List<URLs>();
        public IWebDriver driver;
        int wait_time = 10,loop_time=30;
        readonly List<Report> report = new List<Report>();
        readonly List<string> kws = new List<string> { "pla 1.75", "filamento stampante 3d", "filamento pla", "stampa 3d", "filamento per penna 3d", "pla 3d pen", "filamenti pla", "3d pen", "pla colori assortiti", "pla colori misti" };
        readonly List<string> urls = new List<string> {
                "https://www.amazon.it/s?k=pla+1.75&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=filamento+stampante+3d&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=filamento+pla&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=stampa+3d&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=filamento+per+penna+3d&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=pla+3d+pen&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=filamenti+pla&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=3d+pen&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=pla+colori+assortiti&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss",
                "https://www.amazon.it/s?k=pla+colori+misti&me=A3ISHOTR86A43U&__mk_it_IT=%C3%85M%C3%85%C5%BD%C3%95%C3%91&ref=nb_sb_noss"
            };
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 10; i++)
                surls.Add(new URLs()
                {
                    ID = i + 1,
                    Keyword = kws[i],
                    URL = urls[i]
                });

            dataGrid.ItemsSource = surls;
            foreach (string kw in kws)
            {
                report.Add(new Report(kw, 0));
            }
        }
        private void Start(object sender, RoutedEventArgs e)
        {
            try
            {
                new Thread(start).Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter valid time value!", "Error");
            }

        }
        void start()
        {
            TimeSpan start = TimeSpan.Parse("23:30");
            TimeSpan end = TimeSpan.Parse("06:00");
            while (true)
            {

                if (DateTime.Now.TimeOfDay < start && DateTime.Now.TimeOfDay > end)
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        wait_time = int.Parse(time.Text);
                        loop_time = int.Parse(loop.Text);
                        btn_start.IsEnabled = time.IsEnabled = dataGrid.IsEnabled = false;
                        btn_start.Foreground = new SolidColorBrush(Colors.Black);
                    }));
                    try
                    {
                        driver = getChromeDriver();
                    }
                    catch (Exception)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            MessageBox.Show("Make sure Chrome is updated to latest version and latest chromedriver.exe is in the same directory. Download chromedriver from https://chromedriver.chromium.org/downloads", "Chromedriver error");
                        }));
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            dataGrid.SelectedIndex = i;
                            status.Text = $"Working on keyword {kws[i]} ({i + 1})";
                            pb.Value = (i + 1) * 10;
                        }));
                        if(! testing)
                            driver.Navigate().GoToUrl(urls[i]);
                        report[i].count++;
                        string res = "Keyword,Count";
                        foreach (Report r in report)
                        {
                            res += $"\n{r.kw},{r.count}";
                        }
                        File.WriteAllText("Report.csv", res);
                        Thread.Sleep(wait_time * 1000);
                    }
                    Dispatcher.Invoke((Action)(() =>
                    {
                        status.Text = "All done!";
                        btn_start.IsEnabled = time.IsEnabled = dataGrid.IsEnabled = true;
                        btn_start.Foreground = new SolidColorBrush(Colors.White);
                    }));
                    driver.Close();
                    driver.Quit();
                    //MessageBox.Show("Done");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        status.Text = $"Waiting for {loop_time} min(s)...";
                    }));
                    Thread.Sleep(1000 * 1);
                    for (int i = loop_time; i > 0; i--)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            status.Text = $"{i} minute(s) left...";
                        }));
                        if(testing)
                            Thread.Sleep(1000 * 1);
                        else
                            Thread.Sleep(1000 * 60);
                    }
                }
                else
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        status.Text = "Time is between 6:00am to 11:30pm so bot is paused.";
                    }));
                    while (DateTime.Now.TimeOfDay > start && DateTime.Now.TimeOfDay < end)
                        Thread.Sleep(1000);
                }
            }
        }
        IWebDriver getChromeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            //if (debug)
            //{
            //    print("Connecting existing Chrome for debugging...");
            //    options.DebuggerAddress = "127.0.0.1:9222";
            //}
            //if (!images)
            //{
            //    print("Turning off images to save bandwidth");
            //    options.AddArgument("--blink-settings=imagesEnabled=false");
            //}
            //if (headless)
            //{
            //    print("Going headless");
            //    options.AddArgument("--headless");
            //    options.AddArgument("--window-size=1920x1080");
            //}
            //if (maximize)
            //{
            //    print("Maximizing Chrome");
            //    options.AddArgument("--start-maximized");
            //}
            //if (incognito)
            //{
            //    print("Going incognito");
            //    options.AddArgument("--incognito");
            //}
            //if (proxy)
            //{
            //    string PROXY = $"{prxy[0]}:{prxy[1]}";
            //    options.AddArgument($"--proxy-server={PROXY}");
            //}
            options.AddArgument("--start-maximized");
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            return new ChromeDriver(driverService, options);
        }
    }
}
