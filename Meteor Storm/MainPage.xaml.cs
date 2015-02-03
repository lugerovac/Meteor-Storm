using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Meteor_Storm
{
    public partial class MainPage : UserControl
    {
        private const int minBrzina = 100;
        private const int maxBrzina = 700;
        private Ship PlayerShip { get; set; }
        private GameLoop gameLoop;
        private KeyHandler keyHandler;

        private int amunicija;
        private int brojMeteora;
        private int brojStitova;
        private int killZonePresent;
        private int pauseCooldown;
        private int pokupljeniGrumeni;
        private bool repeat = false;
        private double stanjeStita;
        private int sviGrumeni;
        private int torpedoCooldown;
        private int udarci;
        private int unisteniMeteoriti;
        public double zdravlje;

        List<eksplozija> Eksplozije;
        List<eksplozija> EksplozijeRemove;
        List<Grumen> Grumenje;
        List<Grumen> GrumenjeRemove;
        List<Informacije> informacije;
        List<Informacije> informacijeRemove;
        List<Meteorite> Meteoriti;
        List<Meteorite> MeteoritiRemove;
        List<Sonda> Sonde;
        List<Sonda> SondeRemove;
        List<Torpedo> Torpedi;
        List<Torpedo> TorpediRemove;
        KillZone killzone;

        enum GameState
        {
            pauza = 0,
            spremno = 1,
            pokrenuto = 2
        }
        GameState status = GameState.spremno;

        public MainPage()
        {
            InitializeComponent();

            keyHandler = new KeyHandler(this);
            gameLoop = new GameLoop(this);
            gameLoop.Update += new GameLoop.UpdateHandler(gameLoop_Update);
        }  //MainPage

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            cnvInfo.Visibility = Visibility.Collapsed;  //sakrij pravila igre

            if (status == GameState.spremno)
            {              
                gameRoot.Children.Remove(Rezultati);  //ukloni rezultate
                status = GameState.pokrenuto;

                /*Inicijalizacija igračeva broda*/
                PlayerShip = new Ship(33, 40, new Point(gameRoot.Width / 2, gameRoot.Height / 2), minBrzina, 0, 0);
                PlayerShip.MaxX = gameRoot.Width - 30;
                PlayerShip.MaxY = gameRoot.Height - 30;
                PlayerShip.MinX = 10;
                PlayerShip.MinY = 10;
                gameRoot.Children.Add(PlayerShip.SpriteCanvas);

                if (repeat)  //ukoliko se igra ponavlja, očisti ekran
                {
                    foreach (Meteorite meteorit in Meteoriti)
                        gameRoot.Children.Remove(meteorit.SpriteCanvas);
                    foreach (Torpedo pucanj in Torpedi)
                        gameRoot.Children.Remove(pucanj.SpriteCanvas);
                    foreach (eksplozija explosion in Eksplozije)
                        gameRoot.Children.Remove(explosion.SpriteCanvas);
                    foreach (Grumen grumen in Grumenje)
                        gameRoot.Children.Remove(grumen.SpriteCanvas);
                    foreach (Informacije info in informacije)
                        gameRoot.Children.Remove(info);
                    foreach (Sonda sonda in Sonde)
                        gameRoot.Children.Remove(sonda.SpriteCanvas);
                    if (killZonePresent > 0)
                        gameRoot.Children.Remove(killzone.SpriteCanvas);
                }

                /*inicijalizacija varijabli na početne vrijednosti*/
                brojMeteora = torpedoCooldown = pauseCooldown = unisteniMeteoriti = udarci = sviGrumeni = pokupljeniGrumeni = 0;
                killZonePresent = 0;
                stanjeStita = 0;
                brojStitova = 1;
                zdravlje = 100;
                amunicija = 20;

                /*deklaracija listi*/
                Meteoriti = new List<Meteorite>();
                MeteoritiRemove = new List<Meteorite>();
                Torpedi = new List<Torpedo>();
                TorpediRemove = new List<Torpedo>();
                Eksplozije = new List<eksplozija>();
                EksplozijeRemove = new List<eksplozija>();
                Grumenje = new List<Grumen>();
                GrumenjeRemove = new List<Grumen>();
                informacije = new List<Informacije>();
                informacijeRemove = new List<Informacije>();
                Sonde = new List<Sonda>();
                SondeRemove = new List<Sonda>();

                gameRoot.Children.Remove(btnStart);
                gameLoop.Start();
            } //if (status == GameState.spremno)
            else
            {
                odpauziraj();
            }
        }  //btnStart_Click

        /*funkcija koja vrti glavni dio programa*/
        void gameLoop_Update(TimeSpan elapsed)
        {
            if (torpedoCooldown > 0) torpedoCooldown--;
            if (pauseCooldown > 0) pauseCooldown--;

            /*detekcija korisnikova unosa*/
            if (status == GameState.pokrenuto)
            {
                PlayerShip.Velocity = new Vector(0, 0);
                if (keyHandler.IsKeyPressed(Key.Left) || keyHandler.IsKeyPressed(Key.A))
                {
                    if (PlayerShip.kut == 0) PlayerShip.kut = 355;
                    else PlayerShip.kut -= 5;
                }
                if (keyHandler.IsKeyPressed(Key.Right) || keyHandler.IsKeyPressed(Key.D))
                {
                    if (PlayerShip.kut == 355) PlayerShip.kut = 0;
                    else PlayerShip.kut += 5;
                }
                if ((keyHandler.IsKeyPressed(Key.Up) || keyHandler.IsKeyPressed(Key.W)) && PlayerShip.brzina < maxBrzina)
                {
                    PlayerShip.brzina += 5;
                }
                if ((keyHandler.IsKeyPressed(Key.Down) || keyHandler.IsKeyPressed(Key.S)) && PlayerShip.brzina > minBrzina)
                {
                    if (PlayerShip.brzina > minBrzina) PlayerShip.brzina -= 5;
                }
                if ((keyHandler.IsKeyPressed(Key.Space) ||keyHandler.IsKeyPressed(Key.NumPad0)) && amunicija > 0 && torpedoCooldown == 0)
                {
                    torpedoCooldown = 10;
                    Torpedo pucanj = PlayerShip.Fire();
                    pucanj.MinX = pucanj.MinY = 0;
                    pucanj.MaxX = gameRoot.Width;
                    pucanj.MaxY = gameRoot.Height;
                    Torpedi.Add(pucanj);
                    gameRoot.Children.Add(pucanj.SpriteCanvas);
                    Torpedo_Zvuk(0.5);

                    RotateTransform rotate = new RotateTransform();
                    rotate.Angle = PlayerShip.kut;
                    rotate.CenterX = pucanj.CentarX;
                    rotate.CenterY = pucanj.CentarY;
                    pucanj.SpriteCanvas.RenderTransform = rotate;

                    amunicija--;
                }
                if ((keyHandler.IsKeyPressed(Key.Delete) || keyHandler.IsKeyPressed(Key.Y)) && pauseCooldown == 0)
                {
                    status = GameState.pauza;
                    gameRoot.Visibility = Visibility.Collapsed;
                    cnvInfo.Visibility = Visibility.Visible;
                    pauseCooldown = 10;
                    
                    Style stil = gameRoot.Resources["btnStyle3"] as Style;
                    btnStart.Style = stil;
                    //Canvas.Left="269" Canvas.Top="446"
                    LayoutRoot.Children.Add(btnStart);
                }
                if ((keyHandler.IsKeyPressed(Key.NumPad2) || keyHandler.IsKeyPressed(Key.Ctrl)) && brojStitova > 0 && stanjeStita == 0)
                {
                    brojStitova--;
                    stanjeStita = 100;

                    Point pozicija = new Point();
                    pozicija = PlayerShip.Position;
                    int brzina = PlayerShip.brzina;
                    int kut = PlayerShip.kut;
                    gameRoot.Children.Remove(PlayerShip.SpriteCanvas);
                    PlayerShip = new Ship(30, 20, pozicija, brzina, kut, 1);
                    PlayerShip.MaxX = gameRoot.Width - 30;
                    PlayerShip.MaxY = gameRoot.Height - 30;
                    PlayerShip.MinX = 10;
                    PlayerShip.MinY = 10;
                    gameRoot.Children.Add(PlayerShip.SpriteCanvas);            
                }
                if ((keyHandler.IsKeyPressed(Key.X) || keyHandler.IsKeyPressed(Key.Enter)) && amunicija >= 10 && torpedoCooldown == 0 && killZonePresent == 0)
                {
                    torpedoCooldown = 10;
                    amunicija -= 10;
                    killzone = new KillZone(300, 300, PlayerShip.Position, 0, 0);
                    killZonePresent = 200;
                    gameRoot.Children.Add(killzone.SpriteCanvas);
                    Torpedo_Zvuk(1);

                    foreach(Meteorite meteor in Meteoriti)
                    {
                        if(Sprite.Collides(killzone, meteor))
                        {
                            eksplozija explosion = meteor.unisten();
                            Eksplozije.Add(explosion);
                            gameRoot.Children.Add(explosion.SpriteCanvas);
                            MeteoritiRemove.Add(meteor);
                            unisteniMeteoriti++;

                            Eksplozija_Zvuk(0.3);
                        }
                    }

                    foreach(Sonda sonda in Sonde)
                    {
                        if(Sprite.Collides(killzone, sonda))
                        {
                            eksplozija explosion = sonda.unistena();
                            Eksplozije.Add(explosion);
                            gameRoot.Children.Add(explosion.SpriteCanvas);
                            SondeRemove.Add(sonda);

                            Eksplozija_Zvuk(0.3);
                        }
                    }

                    foreach(Grumen grumen in Grumenje)
                    {
                        if (Sprite.Collides(killzone, grumen))
                        {
                            eksplozija explosion = grumen.unisten();
                            Eksplozije.Add(explosion);
                            gameRoot.Children.Add(explosion.SpriteCanvas);
                            GrumenjeRemove.Add(grumen);

                            Eksplozija_Zvuk(0.3);
                        }
                    }
                } 

                if (zdravlje < 100) zdravlje += 0.0001 * PlayerShip.brzina;
                else zdravlje = 100;

                if (stanjeStita > 0) stanjeStita -= 0.1;
                else stanjeStita = 0;

                if (stanjeStita == 0)
                {
                    Point pozicija = new Point();
                    pozicija = PlayerShip.Position;
                    gameRoot.Children.Remove(PlayerShip.SpriteCanvas);
                    int brzina = PlayerShip.brzina;
                    int kut = PlayerShip.kut;
                    PlayerShip = new Ship(30, 20, pozicija, brzina, kut, 0);
                    PlayerShip.MaxX = gameRoot.Width - 30;
                    PlayerShip.MaxY = gameRoot.Height - 30;
                    PlayerShip.MinX = 10;
                    PlayerShip.MinY = 10;
                    gameRoot.Children.Add(PlayerShip.SpriteCanvas);
                }

                RotateTransform rotacija = new RotateTransform();
                rotacija.Angle = PlayerShip.kut;
                rotacija.CenterX = 16.5;
                rotacija.CenterY = 25;
                PlayerShip.SpriteCanvas.RenderTransform = rotacija;
                PlayerShip.Velocity = Vector.CreateVectorFromAngle(PlayerShip.kut, PlayerShip.brzina);
                PlayerShip.Update(elapsed); 

                
                Meteorite_Loop(elapsed);
                Torpedo_Loop(elapsed);
                ExplosionLoop(elapsed);
                InformacijeLoop(elapsed);
                GrumenjeLoop(elapsed);
                Sonde_Loop(elapsed);
                if (killZonePresent > 0) KillZoneLoop(elapsed);

                /*ažuriranje podataka na korisničkom sučelju*/
                MP.MeteoriScore = brojMeteora;
                TA.TorpediAmunicija = amunicija;
                UM.MeteoriUnisteni = unisteniMeteoriti;
                zdravljeBroda.Health = zdravlje;
                KZ.Update(killZonePresent, amunicija);
                GS.Bodovi = pokupljeniGrumeni;
                SS.BrojStitova = brojStitova;
                SS.StanjeStita = stanjeStita;  
            }
            else 
            {
                if ((keyHandler.IsKeyPressed(Key.Delete) || keyHandler.IsKeyPressed(Key.Y)) && pauseCooldown == 0)
                {
                    odpauziraj();
                }
            }
        }  //gameLoop_Update

        private void odpauziraj()
        {
            status = GameState.pokrenuto;
            LayoutRoot.Children.Remove(btnStart);
            gameRoot.Visibility = Visibility.Visible;
            cnvInfo.Visibility = Visibility.Collapsed;
            pauseCooldown = 10;
        }

        private void KillZoneLoop(TimeSpan elapsed)
        {
            killzone.SpriteCanvas.Opacity -= .01;
            killZonePresent--;
            if (killZonePresent == 0)
            gameRoot.Children.Remove(killzone.SpriteCanvas);
        }

        private void InformacijeLoop(TimeSpan elapsed)
        {
            foreach(Informacije info in informacije)
            {
                info.prorijedi();
                if (info.nestalo) informacijeRemove.Add(info);
            }

            foreach(Informacije info in informacijeRemove)
            {
                gameRoot.Children.Remove(info);
                informacije.Remove(info);
            }
            informacijeRemove.Clear();
        }  //InformacijeLoop

        private void ExplosionLoop(TimeSpan elapsed)
        {
            foreach(eksplozija explosion in Eksplozije)
            {
                if(--explosion.vijekTrajanja == 0)
                    EksplozijeRemove.Add(explosion);
                explosion.SpriteCanvas.Opacity -= .04;
            }

            foreach(eksplozija explosion in EksplozijeRemove)
            {
                gameRoot.Children.Remove(explosion.SpriteCanvas);
                Eksplozije.Remove(explosion);
            }
            EksplozijeRemove.Clear();
        }  //ExplosionLoop

        void Torpedo_Loop(TimeSpan elapsed)
        {
            foreach(Torpedo pucanj in Torpedi)
            {
                pucanj.Update(elapsed);
                if (pucanj.checkLocation()) TorpediRemove.Add(pucanj);

                foreach(Meteorite meteor in Meteoriti)
                {
                    if(Sprite.Collides(pucanj, meteor))
                    {
                        TorpediRemove.Add(pucanj);
                        eksplozija explosion = meteor.unisten();
                        Eksplozije.Add(explosion);
                        gameRoot.Children.Add(explosion.SpriteCanvas);
                        Eksplozija_Zvuk(0.75);

                        if (meteor.Width > 25)  //ako je meteor veći, podjeliti ćemo ga u dva manja
                        {
                            Meteorite noviMeteor1, noviMeteor2;
                            noviMeteor1 = new Meteorite(25, 25, meteor.Position, meteor.brzina * 2, pucanj.kut - 60);
                            noviMeteor1.MinY = 0;
                            noviMeteor1.MinX = 0;
                            noviMeteor1.MaxX = gameRoot.Width;
                            noviMeteor1.MaxY = gameRoot.Height;
                            if (noviMeteor1.kut < 0) noviMeteor1.kut += 360;
                            noviMeteor1.Velocity = Vector.CreateVectorFromAngle(noviMeteor1.kut, noviMeteor1.brzina);
                            Meteoriti.Add(noviMeteor1);
                            gameRoot.Children.Add(noviMeteor1.SpriteCanvas);

                            noviMeteor2 = new Meteorite(25, 25, meteor.Position, meteor.brzina * 2, pucanj.kut + 60);
                            noviMeteor2.MinY = 0;
                            noviMeteor2.MinX = 0;
                            noviMeteor2.MaxX = gameRoot.Width;
                            noviMeteor2.MaxY = gameRoot.Height;
                            if (noviMeteor2.kut > 360) noviMeteor2.kut -= 360;
                            noviMeteor2.Velocity = Vector.CreateVectorFromAngle(noviMeteor2.kut, noviMeteor2.brzina);
                            Meteoriti.Add(noviMeteor2);
                            gameRoot.Children.Add(noviMeteor2.SpriteCanvas);
                        }
                        zdravlje += meteor.brzina / 25;

                        MeteoritiRemove.Add(meteor);
                        unisteniMeteoriti++;
                        break;
                    }  //if(Sprite.Collides(pucanj, meteor))
                }  //foreach(Meteorite meteor in Meteoriti)

                foreach(Sonda sonda in Sonde)
                {
                    if(Sprite.Collides(pucanj, sonda))
                    {
                        TorpediRemove.Add(pucanj);
                        eksplozija explosion = sonda.unistena();
                        Eksplozije.Add(explosion);
                        gameRoot.Children.Add(explosion.SpriteCanvas);
                        SondeRemove.Add(sonda);
                        Eksplozija_Zvuk(0.75);
                        break;
                    }
                }

                foreach(Grumen grumen in Grumenje)
                {
                    if(Sprite.Collides(pucanj, grumen))
                    {
                        TorpediRemove.Add(pucanj);
                        eksplozija explosion = grumen.unisten();
                        Eksplozije.Add(explosion);
                        gameRoot.Children.Add(explosion.SpriteCanvas);
                        GrumenjeRemove.Add(grumen);
                        Eksplozija_Zvuk(0.75);
                        break;
                    }
                }
            }  //foreach(Torpedo pucanj in Torpedi)

            foreach(Torpedo pucanj in TorpediRemove)
            {
                Torpedi.Remove(pucanj);
                gameRoot.Children.Remove(pucanj.SpriteCanvas);
            }
            TorpediRemove.Clear();
        }  //torpedo_Loop

        void GrumenjeLoop(TimeSpan elapsed)
        {
            int slb = GetRandInt(1, 350);
            if (slb < 10)  //dodaj novi grumen
            {
                sviGrumeni++;
                Grumen noviGrumen;
                int slb1;
                slb = GetRandInt(1, 5);

                int dodajBrzinu = 100 + brojMeteora * 10;
                if (dodajBrzinu > 400) dodajBrzinu = 400;

                switch(slb)
                {
                    case 1:  //slijeva
                        slb1 = GetRandInt(0, 595);
                        noviGrumen = new Grumen(50, 50, new Point(5, slb1), GetRandInt(100, dodajBrzinu), GetRandInt(30, 150));
                        noviGrumen.MinY = 5;
                        noviGrumen.MinX = 5;
                        noviGrumen.MaxX = gameRoot.Width - 5;
                        noviGrumen.MaxY = gameRoot.Height - 5;
                        noviGrumen.Velocity = Vector.CreateVectorFromAngle(noviGrumen.kut, noviGrumen.brzina);
                        Grumenje.Add(noviGrumen);
                        gameRoot.Children.Add(noviGrumen.SpriteCanvas);
                        break;

                    case 2: //sdesna
                        slb1 = GetRandInt(0, 595);
                        noviGrumen = new Grumen(50, 50, new Point(gameRoot.Width - 5, slb1), GetRandInt(100, dodajBrzinu), GetRandInt(210, 330));
                        noviGrumen.MinY = 5;
                        noviGrumen.MinX = 5;
                        noviGrumen.MaxX = gameRoot.Width - 5;
                        noviGrumen.MaxY = gameRoot.Height - 5;
                        noviGrumen.Velocity = Vector.CreateVectorFromAngle(noviGrumen.kut, noviGrumen.brzina);
                        Grumenje.Add(noviGrumen);
                        gameRoot.Children.Add(noviGrumen.SpriteCanvas);
                        break;

                    case 3: //od gore
                        slb1 = GetRandInt(0, 745);  //početak putanje
                        noviGrumen = new Grumen(50, 50, new Point(slb1, 5), GetRandInt(100, dodajBrzinu), GetRandInt(120, 150));
                        noviGrumen.MinY = 5;
                        noviGrumen.MinX = 5;
                        noviGrumen.MaxX = gameRoot.Width - 5;
                        noviGrumen.MaxY = gameRoot.Height - 5;
                        noviGrumen.Velocity = Vector.CreateVectorFromAngle(noviGrumen.kut, noviGrumen.brzina);
                        Grumenje.Add(noviGrumen);
                        gameRoot.Children.Add(noviGrumen.SpriteCanvas);
                        break;

                    case 4: //od dolje
                        slb1 = GetRandInt(0, 745);  //početak putanje
                        noviGrumen = new Grumen(50, 50, new Point(slb1, gameRoot.Height - 5), GetRandInt(100, dodajBrzinu), GetRandInt(300, 420));
                        noviGrumen.MinY = 5;
                        noviGrumen.MinX = 5;
                        noviGrumen.MaxX = gameRoot.Width - 5;
                        noviGrumen.MaxY = gameRoot.Height - 5;
                        if (noviGrumen.kut > 360) noviGrumen.kut -= 360;
                        noviGrumen.Velocity = Vector.CreateVectorFromAngle(noviGrumen.kut, noviGrumen.brzina);
                        Grumenje.Add(noviGrumen);
                        gameRoot.Children.Add(noviGrumen.SpriteCanvas);
                        break;
                }  //switch(slb)
            }  //if (slb < 20)

            foreach(Grumen grumen in Grumenje)
            {
                if(Sprite.Collides(PlayerShip, grumen))
                {
                    pokupljeniGrumeni++;
                    if ((pokupljeniGrumeni % 5) == 0) amunicija++;
                    GrumenjeRemove.Add(grumen);
                    zdravlje += 3;
                    Grumen_Zvuk();
                }

                RotateTransform rotacija = new RotateTransform();
                double rot = grumen.rotacija + 1.5;
                if (rot > 360) rot -= 360;
                grumen.rotacija = rot;
                rotacija.Angle = grumen.rotacija;
                rotacija.CenterX = grumen.CentarX;
                rotacija.CenterY = grumen.CentarY;
                grumen.SpriteCanvas.RenderTransform = rotacija;
                grumen.Update(elapsed);

                if (grumen.checkLocation()) GrumenjeRemove.Add(grumen);
            }

            foreach(Grumen grumen in GrumenjeRemove)
            {
                Grumenje.Remove(grumen);
                gameRoot.Children.Remove(grumen.SpriteCanvas);
            }
            GrumenjeRemove.Clear();
        }  //GrumenjeLoop

        void Sonde_Loop(TimeSpan elapsed)
        {
            bool DodajSondu = false;
            int slb = GetRandInt(1, 450);
            if (slb < 5) DodajSondu = true;

            if(DodajSondu)
            {
                Sonda novaSonda;
                int slb1;

                switch (slb)
                {
                    case 1:
                        slb1 = GetRandInt(0, 595);
                        novaSonda = new Sonda(50, 50, new Point(5, slb1), GetRandInt(100, 200), GetRandInt(30, 150));
                        novaSonda.MinY = 5;
                        novaSonda.MinX = 5;
                        novaSonda.MaxX = gameRoot.Width - 5;
                        novaSonda.MaxY = gameRoot.Height - 5;
                        novaSonda.Velocity = Vector.CreateVectorFromAngle(novaSonda.kut, novaSonda.brzina);
                        Sonde.Add(novaSonda);
                        gameRoot.Children.Add(novaSonda.SpriteCanvas);
                        break;

                    case 2:
                        slb1 = GetRandInt(0, 595);
                        novaSonda = new Sonda(50, 50, new Point(gameRoot.Width - 5, slb1), GetRandInt(100, 200), GetRandInt(210, 330));
                        novaSonda.MinY = 5;
                        novaSonda.MinX = 5;
                        novaSonda.MaxX = gameRoot.Width - 5;
                        novaSonda.MaxY = gameRoot.Height - 5;
                        novaSonda.Velocity = Vector.CreateVectorFromAngle(novaSonda.kut, novaSonda.brzina);
                        Sonde.Add(novaSonda);
                        gameRoot.Children.Add(novaSonda.SpriteCanvas);
                        break;

                    case 3:
                        slb1 = GetRandInt(0, 745);
                        novaSonda = new Sonda(50, 50, new Point(slb1, 5), GetRandInt(100, 200), GetRandInt(120, 150));
                        novaSonda.MinY = 5;
                        novaSonda.MinX = 5;
                        novaSonda.MaxX = gameRoot.Width - 5;
                        novaSonda.MaxY = gameRoot.Height - 5;
                        novaSonda.Velocity = Vector.CreateVectorFromAngle(novaSonda.kut, novaSonda.brzina);
                        Sonde.Add(novaSonda);
                        gameRoot.Children.Add(novaSonda.SpriteCanvas);
                        break;

                    case 4:
                        slb1 = GetRandInt(0, 745);
                        novaSonda = new Sonda(50, 50, new Point(slb1, gameRoot.Height - 5), GetRandInt(100, 200), GetRandInt(300, 420));
                        novaSonda.MinY = 5;
                        novaSonda.MinX = 5;
                        novaSonda.MaxX = gameRoot.Width - 5;
                        novaSonda.MaxY = gameRoot.Height - 5;
                        if (novaSonda.kut > 360) novaSonda.kut -= 360;
                        novaSonda.Velocity = Vector.CreateVectorFromAngle(novaSonda.kut, novaSonda.brzina);
                        Sonde.Add(novaSonda);
                        gameRoot.Children.Add(novaSonda.SpriteCanvas);
                        break;
                }  //switch (slb)
            }  //if(DodajSondu)

            foreach (Sonda sonda in Sonde)
            {
                if (Sprite.Collides(PlayerShip, sonda))
                {
                    SondeRemove.Add(sonda);
                    Grumen_Zvuk();
                    bonus();
                }

                sonda.Update(elapsed);
                if (sonda.checkLocation()) SondeRemove.Add(sonda);
            }

            foreach (Sonda sonda in SondeRemove)
            {
                Sonde.Remove(sonda);
                gameRoot.Children.Remove(sonda.SpriteCanvas);
            }
            SondeRemove.Clear();
        }  //Sonde_Loop

        void Meteorite_Loop(TimeSpan elapsed)
        {
            bool DodajMeteor = false;
            if (Meteoriti.Count == 0) DodajMeteor = true;

            int dm = 550 - brojMeteora / 5;
            if (dm < 350) dm = 350;
            int slb = GetRandInt(1, dm);
            if (slb < 25) DodajMeteor = true;

            if (DodajMeteor)
            {
                brojMeteora++;
                if (brojMeteora % 10 == 0) amunicija++;
                Meteorite noviMeteor;
                int slb1, velicina;
                slb = GetRandInt(1, 5);
                velicina = GetRandInt(1, 5);

                int dodajBrzinu = 100 + brojMeteora * 2;
                if (dodajBrzinu > 400) dodajBrzinu = 400;

                switch (slb)
                {
                    case 1: //meteor dolazi s slijeva
                        slb1 = GetRandInt(0, 600);  //početak putanje
                        if (velicina != 1) noviMeteor = new Meteorite(50, 50, new Point(5, slb1), GetRandInt(100, dodajBrzinu), GetRandInt(30, 150));
                        else noviMeteor = new Meteorite(25, 25, new Point(5, slb1), GetRandInt(100, dodajBrzinu), GetRandInt(30, 150));
                        noviMeteor.MinY = 5;
                        noviMeteor.MinX = 5;
                        noviMeteor.MaxX = gameRoot.Width - 5;
                        noviMeteor.MaxY = gameRoot.Height - 5;
                        noviMeteor.Velocity = Vector.CreateVectorFromAngle(noviMeteor.kut, noviMeteor.brzina);
                        Meteoriti.Add(noviMeteor);
                        gameRoot.Children.Add(noviMeteor.SpriteCanvas);
                        break;

                    case 2: //sdesna
                        slb1 = GetRandInt(0, 595);  //početak putanje
                        if (velicina != 1) noviMeteor = new Meteorite(50, 50, new Point(gameRoot.Width - 5, slb1), GetRandInt(100, dodajBrzinu), GetRandInt(210, 330));
                        else noviMeteor = new Meteorite(25, 20, new Point(gameRoot.Width - 5, slb1), GetRandInt(100, dodajBrzinu), GetRandInt(210, 330));
                        noviMeteor.MinY = 5;
                        noviMeteor.MinX = 5;
                        noviMeteor.MaxX = gameRoot.Width - 5;
                        noviMeteor.MaxY = gameRoot.Height - 5;
                        noviMeteor.Velocity = Vector.CreateVectorFromAngle(noviMeteor.kut, noviMeteor.brzina);
                        Meteoriti.Add(noviMeteor);
                        gameRoot.Children.Add(noviMeteor.SpriteCanvas);
                        break;

                    case 3: //od gore
                        slb1 = GetRandInt(0, 745);  //početak putanje
                        if (velicina != 1) noviMeteor = new Meteorite(50, 50, new Point(slb1, 5), GetRandInt(100, dodajBrzinu), GetRandInt(120, 150));
                        else noviMeteor = new Meteorite(20, 20, new Point(slb1, 5), GetRandInt(100, dodajBrzinu), GetRandInt(120, 150));
                        noviMeteor.MinY = 5;
                        noviMeteor.MinX = 5;
                        noviMeteor.MaxX = gameRoot.Width - 5;
                        noviMeteor.MaxY = gameRoot.Height - 5;
                        noviMeteor.Velocity = Vector.CreateVectorFromAngle(noviMeteor.kut, noviMeteor.brzina);
                        Meteoriti.Add(noviMeteor);
                        gameRoot.Children.Add(noviMeteor.SpriteCanvas);
                        break;

                    case 4: //od dolje
                        slb1 = GetRandInt(0, 745);  //početak putanje
                        if (velicina != 1) noviMeteor = new Meteorite(50, 50, new Point(slb1, gameRoot.Height - 5), GetRandInt(100, dodajBrzinu), GetRandInt(300, 420));
                        noviMeteor = new Meteorite(25, 25, new Point(slb1, gameRoot.Height - 5), GetRandInt(100, dodajBrzinu), GetRandInt(300, 420));
                        noviMeteor.MinY = 5;
                        noviMeteor.MinX = 5;
                        noviMeteor.MaxX = gameRoot.Width - 5;
                        noviMeteor.MaxY = gameRoot.Height - 5;
                        if (noviMeteor.kut > 360) noviMeteor.kut -= 360;
                        noviMeteor.Velocity = Vector.CreateVectorFromAngle(noviMeteor.kut, noviMeteor.brzina);
                        Meteoriti.Add(noviMeteor);
                        gameRoot.Children.Add(noviMeteor.SpriteCanvas);
                        break;
                }  //switch(slb)
            }  //if(DodajMeteor)

            foreach(Meteorite meteorit in Meteoriti)
            {
                if (Sprite.Collides(PlayerShip, meteorit))
                {
                    int ukupnaSteta = (meteorit.brzina / 2 + PlayerShip.brzina) / 10;
                    if (meteorit.Width < 50) ukupnaSteta /= 2;

                    if (stanjeStita > 0)
                    {
                        stanjeStita -= ukupnaSteta;
                    }else
                    {
                        udarci++;
                        zdravlje -= ukupnaSteta;
                    }

                    eksplozija explosion = meteorit.unisten();
                    Eksplozije.Add(explosion);
                    Eksplozija_Zvuk(1);
                    gameRoot.Children.Add(explosion.SpriteCanvas);
                    MeteoritiRemove.Add(meteorit);

                    if (zdravlje <= 0)
                    {
                        GameOver();
                        return;
                    }
                }

                RotateTransform rotacija = new RotateTransform();
                double rot = meteorit.rotacija + 1.5;
                if (rot >= 360) meteorit.rotacija -= 360;
                meteorit.rotacija = rot;
                rotacija.Angle = meteorit.rotacija;
                rotacija.CenterX = meteorit.CentarX;
                rotacija.CenterY = meteorit.CentarY;
                meteorit.SpriteCanvas.RenderTransform = rotacija;
                meteorit.Update(elapsed);

                if (meteorit.checkLocation()) MeteoritiRemove.Add(meteorit);
            }  //foreach(Meteorite meteorit in Meteoriti)

            foreach (Meteorite meteorit in MeteoritiRemove)
            {
                Meteoriti.Remove(meteorit);
                gameRoot.Children.Remove(meteorit.SpriteCanvas);
            }
            MeteoritiRemove.Clear();

        }  //Meteorite_Loop

        private void Eksplozija_Zvuk(double volume)
        {
            MediaElement zvuk = new MediaElement();
            zvuk.Source = new Uri("Resources/Explosion.mp3", UriKind.RelativeOrAbsolute);
            zvuk.Position = new TimeSpan(0, 0, 2);
            zvuk.Volume = volume;
            gameRoot.Children.Add(zvuk);
            zvuk.MediaEnded += new RoutedEventHandler((sender, e) => zvuk_ukloni(sender, e, zvuk));
            zvuk.Play();
        }  //Eksplozija_Zvuk

        private void Grumen_Zvuk()
        {
            MediaElement zvuk = new MediaElement();
            zvuk.Source = new Uri("Resources/Grumen.mp3", UriKind.RelativeOrAbsolute);
            zvuk.Volume = 1;
            gameRoot.Children.Add(zvuk);
            zvuk.MediaEnded += new RoutedEventHandler((sender, e) => zvuk_ukloni(sender, e, zvuk));
            zvuk.Play();
        }  //Eksplozija_Zvuk

        private void Torpedo_Zvuk(double volume)
        {
            MediaElement zvuk = new MediaElement();
            zvuk.Source = new Uri("Resources/Torpedo.mp3", UriKind.RelativeOrAbsolute);
            zvuk.Volume = volume;
            gameRoot.Children.Add(zvuk);
            zvuk.MediaEnded += new RoutedEventHandler((sender, e) => zvuk_ukloni(sender, e, zvuk));
            zvuk.Play();
        } //Torpedo_Zvuk

        private void zvuk_ukloni(object sender, RoutedEventArgs e, MediaElement zvuk)
        {
            zvuk.Stop();
            gameRoot.Children.Remove(zvuk);
        } //zvuk_ukloni

        /*nagrađivanje korisnika koji je pokupio sondu*/
        public void bonus()
        {
            Informacije novaInformacija = new Informacije();
            int slb = GetRandInt(1, 14);
            switch(slb)
            {
                case 1:
                    if (zdravlje < 100)
                    {
                        zdravlje = 100;
                        novaInformacija.Bonus = "Popravak broda";
                    }
                    else if (stanjeStita > 0 && stanjeStita < 90)
                    {
                        stanjeStita = 100;
                        novaInformacija.Bonus = "Popravak štitova";
                    }else
                    {
                        amunicija += 15;
                        novaInformacija.Bonus = "Ammo +15";
                    }
                    break;
                case 2:
                case 3:
                case 4:
                    amunicija += 5;
                    novaInformacija.Bonus = "Ammo +5";
                    break;
                case 5:
                case 6:
                    amunicija += 10;
                    novaInformacija.Bonus = "Ammo +10";
                    break;
                case 7:
                    amunicija += 15;
                    novaInformacija.Bonus = "Ammo +15";
                    break;
                case 8:
                case 9:
                    if (brojStitova < 5)
                    {
                        brojStitova++;
                        novaInformacija.Bonus = "Štit";
                    }else
                    {
                        amunicija += 5;
                        novaInformacija.Bonus = "Ammo +5";
                        break;
                    }
                    break;
                case 10:
                    if (brojStitova > 2)
                    {
                        brojStitova++;
                        novaInformacija.Bonus = "Štit";
                    }
                    else if (brojStitova < 4)
                    {
                        brojStitova += 2;
                        novaInformacija.Bonus = "Štit + 2";
                    }
                    else
                    {
                        amunicija += 10;
                        novaInformacija.Bonus = "Ammo +10";
                    }
                    break;
                case 11:
                case 12:
                    pokupljeniGrumeni += 3;
                    novaInformacija.Bonus = "3 grumenja";
                    zdravlje += 9;
                    Grumen_Zvuk();
                    break;
                case 13:
                    pokupljeniGrumeni += 5;
                    novaInformacija.Bonus = "5 grumenja";
                    zdravlje += 15;
                    Grumen_Zvuk();
                    break;
            }

            novaInformacija.SetValue(Canvas.LeftProperty, PlayerShip.Position.X);
            novaInformacija.SetValue(Canvas.TopProperty, PlayerShip.Position.Y);
            gameRoot.Children.Add(novaInformacija);
            informacije.Add(novaInformacija);
        }  //bonus

        public int GetRandInt(int p1, int p2)
        {
            Byte[] rndBytes = new Byte[10];
            RNGCryptoServiceProvider rndC = new RNGCryptoServiceProvider();
            rndC.GetBytes(rndBytes);
            int seed = BitConverter.ToInt32(rndBytes, 0);
            Random rand = new Random(seed);
            return rand.Next(p1, p2);
        }

        public void GameOver()
        {
            repeat = true;
            gameLoop.Stop();
            status = GameState.spremno;

            gameRoot.Children.Remove(PlayerShip.SpriteCanvas);
            gameRoot.Children.Add(btnStart);
            Style stil = gameRoot.Resources["btnStyle1"] as Style;
            btnStart.Style = stil;

            gameRoot.Children.Add(Rezultati);
            Rezultati.Visibility = Visibility.Visible;
            Rezultati.prikazi(brojMeteora, unisteniMeteoriti, udarci, pokupljeniGrumeni);
        }

        private void btnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            if (status != GameState.pauza)
            {
                Style stil = gameRoot.Resources["btnStyle2"] as Style;
                btnStart.Style = stil;
            }
        }

        private void btnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            if (status != GameState.pauza)
            {
                Style stil = gameRoot.Resources["btnStyle1"] as Style;
                btnStart.Style = stil;
            }
        }

        private void meGlazba_MediaEnded(object sender, RoutedEventArgs e)
        {
            meGlazba.Stop();
            meGlazba.Play();
        }
    }
}
