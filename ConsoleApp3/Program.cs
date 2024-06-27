using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace Allatok
{
    class Versenyzok
    {

        private string nev;
        private int szuletesEv;

        private int szepseg, viselkedes;
        private int pont;

        private static int Ev;
        private static int korHatar;

        private int rajtSzam;
        private int oltasiSzam;




        public Versenyzok(string nev, int szuletesEv, int rajtSzam, int oltasiSzam)
        {
            this.nev = nev;
            this.szuletesEv = szuletesEv;
            this.rajtSzam = rajtSzam;
            this.oltasiSzam = oltasiSzam;

        }

        public int Kor()
        {
            return Ev - szuletesEv;
        }
        public void Pontozas(int szepseg, int viskelkedes)
        {
            this.szepseg = szepseg;
            this.viselkedes = viskelkedes;

        }
        public virtual int PontSzam()
        {
            if (Kor() <= korHatar)
            {
                return viselkedes * Kor() + szepseg * (korHatar - Kor());
            }
            return 0;
        }
        public override string ToString()
        {
            if (PontSzam() == 0)
            {
                return $" {rajtSzam}  .   {nev,-10}   nevű   {this.GetType().Name.ToLower(),-6}   Nincs pontszám";
            }
            else
            {
                return $" {rajtSzam}  .   {nev,-10}   nevű   {this.GetType().Name.ToLower(),-6}  Pontszáma: {PontSzam()} pont";

            }

        }



        public string Neve
        {
            get { return nev; }
        }

        public int SzuletesEve
        {
            get { return szuletesEv; }
        }

        public int SzepsegPontja
        {
            get { return szepseg; }
        }

        public int ViselkedesPontja
        {
            get { return viselkedes; }
        }

        public int Pontszama
        {
            get { return PontSzam(); }
        }


        public static int AktualisEv
        {
            get { return Ev; }
            set { Ev = value; }
        }

        public static int KorHatar
        {
            get { return korHatar; }
            set { korHatar = value; }
        }

        public static int rajtszama
        {
            get { return rajtszama; }
            set { rajtszama = value; }
        }


    }
    class Kutya : Versenyzok
    {
        public int GazdaViszonyPont
        {
            get; private set;
        }
        public bool KapottViszonyPontot
        {
            get; private set;
        }
        public Kutya(string nev, int rajtSzam, int Ev, int oltasiSzam) : base(nev, rajtSzam, Ev, oltasiSzam) { }

        public void ViszonyPontozas(int pont)
        {
            this.GazdaViszonyPont = pont;
            KapottViszonyPontot = true;
        }
        public override int PontSzam()
        {
            int pont = 0;
            if (KapottViszonyPontot)
            {
                pont = base.PontSzam() + GazdaViszonyPont;
            }
            return pont;
        }




    }
    class Cica : Versenyzok
    {
        public bool VanMacskaSzallitoDoboz { get; set; }
        public Cica(string nev, int rajtSzam, int Ev, int oltasiSzam, bool VanMacskaSzallitoDoboz) : base(nev, rajtSzam, Ev, oltasiSzam)
        {
            this.VanMacskaSzallitoDoboz = VanMacskaSzallitoDoboz;
        }
        public override int PontSzam()
        {
            if (VanMacskaSzallitoDoboz)
            {
                return base.PontSzam();
            }
            return 0;

        }


    }
    class Vezerles
    {
        private List<Versenyzok> allatok = new List<Versenyzok>();

        public void Start()
        {
            Versenyzok.AktualisEv = 2024;
            Versenyzok.KorHatar = 10;



            Regisztracio();
            Kiiratas("\na regiszált versenyzők\n");
            Verseny();
            Kiiratas("\na verseny eredménye\n");
            int osszes = 0;
            int osszpont = 0;

            for (int i = 0; i < allatok.Count; i++)
            {
                osszes++;
                osszpont += allatok[i].PontSzam();
            }
            int nyertes = 0;

            int atlag = osszpont / osszes;
            for (int i = 0; i < allatok.Count; i++)
            {
                if (allatok[i].Pontszama >= allatok[nyertes].Pontszama)
                {
                    nyertes = i;


                }

            }
            Console.WriteLine($"\nÖsszes versenyző száma:{osszes}, Áltag Pontok: {atlag} \nNyertes: {allatok[nyertes]}");

        }
        private void Regisztracio()
        {
            StreamReader sr = new StreamReader("versenyzok.txt");

            string fajta, nev;
            int rajtSzam = 1, Ev, oltasiSzam;
            bool vanDoboz;

            while (!sr.EndOfStream)
            {
                string sor;
                string[] s;
                sor = sr.ReadLine();
                s = sor.Split(';');
                fajta = s[0];
                nev = s[1];
                Ev = int.Parse(s[2]);
                oltasiSzam = int.Parse(s[3]);

                if (fajta == "kutya")
                {
                    allatok.Add(new Kutya(nev, Ev, rajtSzam, oltasiSzam));
                }
                else
                {
                    vanDoboz = bool.Parse(s[4]);
                    allatok.Add(new Cica(nev, Ev, rajtSzam, oltasiSzam, vanDoboz));
                }
                rajtSzam++;

            }
            sr.Close();
        }
        private void Verseny()
        {
            Random rnd = new Random();
            int hatar = 10;
            foreach (Versenyzok i in allatok)
            {
                if (i is Kutya)
                {
                    (i as Kutya).ViszonyPontozas(rnd.Next(hatar));
                }
                i.Pontozas(rnd.Next(hatar), rnd.Next(hatar));

            }


        }
        private void Kiiratas(string sor)
        {

            Console.WriteLine(sor);
            foreach (Versenyzok db in allatok)
            {
                Console.WriteLine("+---+------------+----------------+------------------+");
                Console.WriteLine(db);
            }
            Console.WriteLine("+---+------------+----------------+------------------+");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Vezerles vezerles = new Vezerles();
            vezerles.Start();

            Console.ReadKey();
        }
    }
}