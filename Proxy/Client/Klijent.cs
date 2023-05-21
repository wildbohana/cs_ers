using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Klijent : IKlijent
    {
        private List<Merenje> merenja;

        [ExcludeFromCodeCoverage]
        public void RadKlijenta(IProksi kanal)
        {
            while (true)
            {
                switch (Meni())
                {
                    case 1: 
                        try { Opcija1(kanal, UnosId()); }
                        catch (FormatException fe) { Console.WriteLine(fe.Message); }
                        break; 
                    case 2:
                        try { Opcija2(kanal, UnosId()); }
                        catch (FormatException fe) { Console.WriteLine(fe.Message); }
                        break;
                    case 3: 
                        Opcija3(kanal); 
                        break; 
                    case 4: 
                        Opcija4(kanal); 
                        break; 
                    case 5: 
                        Opcija5(kanal); 
                        break;
                    case 0: 
                        return;
                    default: 
                        Console.WriteLine("Nepostojeća komanda..."); 
                        break;
                }
            }
        }

        #region MENI
        [ExcludeFromCodeCoverage]
        public int Meni()
        {
            Console.WriteLine("~  MENI  ~");
            Console.WriteLine("1. Dobavi sva merenja za odabrani ID uređaja");
            Console.WriteLine("2. Dobavi poslenje ažurirano merenje za odabrani ID uređaja");
            Console.WriteLine("3. Dobavi poslednje ažurirano merenje za sve uređaje");
            Console.WriteLine("4. Dobavi sva analogna merenja");
            Console.WriteLine("5. Dobavi sva digitalna merenja");
            Console.WriteLine("0. Ugasi klijenta");

            int i;
            Console.Write("\nIzbor operacije: > ");

            try
            {
                i = int.Parse(Console.ReadLine());
            }
            catch
            {
                //Console.WriteLine(e.Message);
                i = -1;
            }

            return i;
        }
        #endregion

        #region UPIS/ISPIS
        [ExcludeFromCodeCoverage]
        public int UnosId()
        {
            int id;
            Console.Write("Unesite ID uređaja: > ");

            try
            {
                id = int.Parse(Console.ReadLine());
            }
            catch
            {
                //Console.WriteLine(e.Message);
                return -1;
            }

            return id;
        }

        [ExcludeFromCodeCoverage]
        private void IspisRazultata(List<Merenje> merenja)
        {
            if (merenja != null && merenja.Count > 0)
                foreach (Merenje m in merenja)
                    Console.WriteLine("\t" + m.ToString());
            else
                Console.WriteLine("\tNema traženih merenja.");
            Console.WriteLine();
        }
        #endregion

        #region OPCIJE
        // Opcija 1 - sva merenja za odabrani ID
        public void Opcija1(IProksi kanal, int trazeni)
        {
            if (trazeni < 0)
                throw new FormatException("Neispravno unet ID!");

            try
            {
                merenja = kanal.DobaviPodatkeId(trazeni);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine($"Merenja za uređaj sa ID {trazeni}:");
            IspisRazultata(merenja);
        }

        // Opcija 2 - poslednje ažurirano merenje za odabrani ID 
        public void Opcija2(IProksi kanal, int trazeni)
        {
            if (trazeni < 0)
                throw new FormatException("Neispravno unet ID!");

            try
            {
                merenja = kanal.DobaviPoslednjiPodatakId(trazeni);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
                merenja = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine($"Poslednje merenja za uređaj sa ID {trazeni}:");
            IspisRazultata(merenja);
        }

        // Opcija 3 - poslednje ažurirano merenje za sve uređaje
        public void Opcija3(IProksi kanal)
        {
            List<Merenje> merenja = new List<Merenje>();

            try
            {
                merenja = kanal.DobaviPoslednjiPodatakSvi();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Merenja za sve uređaje:");
            IspisRazultata(merenja);
        }

        // Opcija 4 - sva analogna merenja
        public void Opcija4(IProksi kanal)
        {
            List<Merenje> merenja = new List<Merenje>();

            try
            {
                merenja = kanal.DobaviSveAnalogne();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Analogna merenja za sve uređaje:");
            IspisRazultata(merenja);
        }

        // Opcija 5 - sva digitalna merenja
        public void Opcija5(IProksi kanal)
        {
            List<Merenje> merenja = new List<Merenje>();

            try
            {
                merenja = kanal.DobaviSveDigitalne();
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Digitalna merenja za sve uređaje:");
            IspisRazultata(merenja);
        }
        #endregion
    }
}
