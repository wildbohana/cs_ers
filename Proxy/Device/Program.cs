﻿using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        static void Main(string[] args)
        {
            Console.Title = "PROKSI - Uređaj";

            // Otvaranje kanala
            ChannelFactory<IServer> cf = new ChannelFactory<IServer>("Server");
            IServer kanal;

            try
            {
                kanal = cf.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n");
                Console.WriteLine("Uređaj se gasi. Pokušajte ponovo da ga pokrenete malo kasnije.");
                Console.ReadKey();
                return;
            }

            // Instanciranje uređaja i pokretanje njegovog rada
            Uredjaj u = new Uredjaj();
            Console.WriteLine($"ID uređaja je {u.IdUredjaja}");
            u.RadUredjaja(kanal);
        }
    }
}
