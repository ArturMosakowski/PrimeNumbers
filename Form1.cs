using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
namespace PrimeNumbers
{
    public partial class Form1 : Form
    {
        private BackgroundWorker m_oBackgroundWorker = null;
        List<Cycle> cycle = new List<Cycle>();
        Stopwatch timer = new Stopwatch();
        int loopCounter;
        public int cycleCount;
        public bool startStop;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("Uruchomiono cykl 1." + Environment.NewLine);
            textBox1.AppendText("Trwa ustalanie" + Environment.NewLine);
            textBox1.AppendText("liczb pierwszych." + Environment.NewLine);
            timer.Start();
            loopCounter = 2;
            startStop = true;
            cycle.Add(new Cycle());


            if (null == m_oBackgroundWorker)
            {
                m_oBackgroundWorker = new BackgroundWorker();
                m_oBackgroundWorker.DoWork += new DoWorkEventHandler(m_oBackgroundWorker_DoWork);
                m_oBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oBackgroundWorker_RunWorkerCompleted);
                m_oBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(m_oBackgroundWorker_ProgressChanged);
                m_oBackgroundWorker.WorkerReportsProgress = true;
                m_oBackgroundWorker.WorkerSupportsCancellation = true;
            }
            m_oBackgroundWorker.RunWorkerAsync();
            
        }

        private void m_oBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBox1.Clear();
            if(startStop == false)
            {
                for (int i = 0; i <= cycle[cycleCount-1].primeNumber.Count-1; i++)
                {
                    textBox1.AppendText("nr " + i.ToString() + ":    liczba " + cycle[cycleCount-1].primeNumber[i].ToString() + Environment.NewLine);
                }
            }
            if(startStop == true)
            {
                textBox1.AppendText("Koniec przerwy." + Environment.NewLine);
                textBox1.AppendText("Trwa ustalanie kolejnych" + Environment.NewLine);
                textBox1.AppendText("liczb pierwszych." + Environment.NewLine);
            }
            
        }

        private void m_oBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                textBox1.AppendText("Stop.");                
            }
            else
            {
                textBox1.AppendText("Stop.");
            }
        }

        private void m_oBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            cycleCount = 0;
            //cycle.Add(new Cycle());
            for (; ;)
            {                
                this.Invoke((MethodInvoker)delegate
                {
                    if (startStop == true)
                    {
                        label2.Text = "Cykl: " + (cycleCount + 1).ToString();
                    }
                    if (startStop == false)
                    {
                        label2.Text = "Przerwa";
                    }
                    label1.Text =timer.Elapsed.Minutes + " min " + timer.Elapsed.Seconds.ToString() + " sek";
                });

                if(timer.Elapsed.Minutes==5 && timer.Elapsed.Seconds>=0 && startStop==true)                
                {
                    cycle.Add(new Cycle());
                    m_oBackgroundWorker.ReportProgress(0);
                    int tmp = cycle[cycleCount].primeNumber.Count - 1;                    
                    cycle[cycleCount].cycleTimeMin = timer.Elapsed.Minutes;
                    cycle[cycleCount].cycleTimeSec = timer.Elapsed.Seconds;

                    if (cycle[cycleCount].primeNumber[tmp] == cycle[cycleCount].lastPrimeNumber)
                    {
                        if ((null != m_oBackgroundWorker) && m_oBackgroundWorker.IsBusy)
                        {
                            m_oBackgroundWorker.CancelAsync();
                        }
                        timer.Stop();
                        this.Invoke((MethodInvoker)delegate
                        {
                            label3.Text = "Nastąpiło zatrzymanie programu ponieważ nie ustalono kolejnej liczny pierwszej.";
                        });
                    }                    

                    cycle[cycleCount].lastPrimeNumber = cycle[cycleCount].primeNumber[tmp];
                    loopCounter = cycle[cycleCount].lastPrimeNumber;        
                    cycle[cycleCount].cycleNumber = cycleCount;
                    timer.Restart();
                    cycleCount++;
                    
                    startStop = false;
                    
                }
                if(timer.Elapsed.Minutes == 1 && timer.Elapsed.Seconds >= 0 && startStop==false)
                {
                    m_oBackgroundWorker.ReportProgress(0);
                    timer.Restart();
                    startStop = true;
                }
                if (m_oBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                if(cycle[cycleCount].IfPrime(loopCounter)==true)
                {
                    cycle[cycleCount].primeNumber.Add(loopCounter);
                    cycle[cycleCount].lastFoundMin = timer.Elapsed.Minutes;
                    cycle[cycleCount].lastFoundSec = timer.Elapsed.Seconds;
                }
                
                Thread.Sleep(10);
                loopCounter++;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if ((null != m_oBackgroundWorker) && m_oBackgroundWorker.IsBusy)
            {
                m_oBackgroundWorker.CancelAsync();
            }
            timer.Stop();

            XDocument xml = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Cykle ustalajace liczby pierwsze"),
                new XElement("Cykle",
                from Cycle in cycle
                select new XElement("najwieksza_wartosc_w_cyklu", Cycle.lastPrimeNumber,
                    new XElement("cykl", Cycle.cycleNumber + 1),
                    new XElement("czas_trwania_cyklu", Cycle.cycleTimeMin + " min " + Cycle.cycleTimeSec + " sek "),
                    new XElement("czas_znalezienia_ostatniej_liczby", Cycle.lastFoundMin + " min " + Cycle.lastFoundSec + " sek ")
                    )
                )
                );
            xml.Save("LiczbyPierwszeXML.xml");         
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
