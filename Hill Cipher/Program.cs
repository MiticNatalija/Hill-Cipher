using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Hill_Cipher
{
    class Program
    {
        public const int N = 30;
        enum Znaci
        {
            a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z, _, tacka, zarez, upitnik

        }

      
        static void popuniMatricuProba(int[,] k)  //default matrica kljuca
        {
            k[0, 0] = 13;
            k[0, 1] = 2;
            k[0, 2] = 5;
            k[1, 0] = 8;
            k[1, 1] = 7;
            k[1, 2] = -4;
            k[2, 0] = 6;
            k[2, 1] = 10;
            k[2, 2] = 11;

        }
        static void stampajMatricu(int[,] k, int len) //stampanje matrice
        {
            for (int i = 0; i < len; i++)
            {

                for (int j = 0; j < len; j++)
                {
                    Console.Write(k[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        static void obradiUlazniTekst(string t, out int[] ot) //ulazni string konvertuje u odgovarajuci niz integera
        {
            ot = new int[t.Length];
            char[] niz = new char[t.Length];
            int i = 0;
            foreach (char c in t)
            {
                object z;
                if (c == '.')
                    z = Znaci.tacka;
                else if (c == ',')
                    z = Znaci.zarez;
                else if (c == '?')
                    z = Znaci.upitnik;
                else
                {
                    niz[i] = c;
                    string s = c.ToString();
                    //  ot[i] = niz[i] - 'a';
                    z = Enum.Parse(typeof(Znaci), s);
                }
                ot[i] = (int)z;

                i++;
            }
        }
        static int[] sifrirajHill(int[] p, int[,] k, int len) //sifriranje unetog teksta
        {
            int[] c = new int[p.Length];
            int i = 0, l = 0;
            while (i < p.Length)
            {
                c[i] = 0;
                for (int j = 0; j < len; j++)
                {
                    c[i] += p[l + j] * k[i % len, j];
                }
                //if (c[i] > 0)
                //    c[i] = c[i] % N;
                //else
                c[i] = negativniModuo(c[i], N);
                if (i % len == len - 1)
                    l += len;
                i++;


            }


            return c;
        }
        static int nadjiDeterminantu(int[,] k, int len) //determinanta matrice
        {
            int ret = 0;
            if (len == 2)
            {
                ret = k[0, 0] * k[1, 1] - k[0, 1] * k[1, 0];
                return ret;
            }
            for (int i = 0; i < 3; i++)
                ret = ret + (k[0, i] * (k[1, (i + 1) % 3] * k[2, (i + 2) % 3] - k[1, (i + 2) % 3] * k[2, (i + 1) % 3]));

            return ret;
        }
        static int[,] transponuj(int[,] k, int len) //transponovana matrica
        {
            int[,] tran = new int[len, len];
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                    tran[i, j] = k[j, i];
            }

            return tran;
        }
        static int[,] adjungovanaMatrica(int[,] k, int len) //adjungovana matrica
        {
            int[,] adj = new int[len, len];
            int[,] pom = new int[len - 1, len - 1];
            int pi = 0, pj = 0;

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    for (int m = 0; m < len; m++)
                    {
                        for (int n = 0; n < len; n++)
                        {
                            if (m != i && n != j)
                            {
                                pom[pi, pj] = k[m, n];
                                if (pj == len - 2)
                                {
                                    pi++;
                                    pj = 0;
                                }
                                else pj++;
                            }
                        }
                    }
                    pi = 0;
                    pj = 0;
                    adj[i, j] = (int)Math.Pow(-1, i + j) * nadjiDeterminantu(pom, 2);
                }
            }
            adj = transponuj(adj, 3);


            return adj;
        }
        static int moduoRazlomka(int b, int n) //multiplikativna inverza pomocu prosirenog Euklidovog algoritma
        {
            int r1, r2, r, t1, t2, t, q, z = 0;
            r1 = n;
            r2 = b;
            t1 = 0;
            t2 = 1;
            while (r2 > 0)
            {
                q = r1 / r2;
                r = r1 - q * r2;
                r1 = r2;
                r2 = r;
                t = t1 - q * t2;
                t1 = t2;
                t2 = t;

            }
            if (r1 == 1)
            {
                if (t1 > 0)
                {
                    z = t1;
                }
                else
                {
                    t1 = n + t1;
                    z = t1 % n;
                }
            }
            return z;
        }
        static int[,] nadjiInverznuMatricu(int[,] k, int len) //inverzna matrica
        {
            int[,] inv = new int[len, len];
            inv = adjungovanaMatrica(k, 3);

            int det = nadjiDeterminantu(k, 3);
            //if (det > 0)
            //{
            //    det = det % N;
            //}
            //else
            //{
            det = negativniModuo(det, N);
            // }
          //  Console.WriteLine("determinanta: " + det);
            det = moduoRazlomka(det, N);
           // Console.WriteLine("multipl.inv: " + det);
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    inv[i, j] *= det;
                    //if (inv[i, j] > 0)
                    //    inv[i, j] %= N;
                    //else
                    inv[i, j] = negativniModuo(inv[i, j], N);
                }
            }

            return inv;
        }
        static int negativniModuo(int b, int n) //racuna moduo i ako je broj negativan
        {
            if (b > 0)
                return b % n;
            int pom;
            pom = -b;
            pom = (pom / n + 1);
            pom *= n;
            pom += b;
            b = pom % n;
            return b;

        }
        static int[] desifrirajHill(int[] p, int[,] k, int len) //desifriranje teksta
        {
            int[] c = new int[p.Length];
            int i = 0, l = 0;
            while (i < p.Length)
            {
                c[i] = 0;
                for (int j = 0; j < len; j++)
                {
                    c[i] += p[l + j] * k[i % len, j];
                }
                //if(c[i]>0)
                //c[i] = c[i] % N;             
                //else
                //{
                c[i] = negativniModuo(c[i], N);
                // }
                if (i % len == len - 1)
                    l += len;
                i++;


            }


            return c;
        }
        static int NZD(int x, int y) //NZD pomocu Euklidovog algoritma
        {
            int m, n, r;
            if (x > y)
            {
                m = x;
                n = y;
            }
            else
            {
                m = y;
                n = x;
            }
            r = m % n;
            while (r != 0)
            {
                m = n;
                n = r;
                r = m % n;
            }
            return n;

        }
        static bool validnostKljuca(int[,] k, int len) //provera da li je kljuc validan
        {
            int det = nadjiDeterminantu(k, len);
            if (det == 0)
                return false;
            //if (det > 0)
            //{
            //    det = det % N;
            //}
            //else
            //{
            det = negativniModuo(det, N);
            // }
            if (det == 0)
                return false;
            if (NZD(det, N) != 1)
                return false;

            return true;
        }
        static string dopuniTekst(string text, int klength) // ako duzina teksta nije deljiva sa 3, dopunjuje se znakom 'a' odredjeni broj puta
        {
            int pom = text.Length % klength;
            pom = klength - pom;

            for (int i = 0; i < pom; i++)
                text += 'a';

            return text;
        }

        static bool validnostTeksta(string text)
        {
            foreach (char c in text)
            {
                if (c < 'a' || c > 'z')

                    if (c != '.' && c != ',' && c != '?' && c != '_')
                        return false;

            }
            return true;
        }
        static void Main(string[] args) //glavni program
        {

            int[,] k, kinv;
            int[] p, c;
            int i, j, klength, textlength;
            string text;
            bool ok = false;
            bool exit = false;

            klength = 3;
            k = new int[klength, klength];
            kinv = new int[klength, klength];

            popuniMatricuProba(k);

        kljuclab:
            ok = false;
            while (!ok)
            {
                Console.WriteLine("Da li zelite da sami unesete kljuc? d/n");
                text = Console.ReadLine();
                if (text == "d")
                {
                    Console.WriteLine("Unesite kljuc:");
                    for (i = 0; i < klength; i++)
                    {
                        for (j = 0; j < klength; j++)
                        {
                            Int32.TryParse(Console.ReadLine(), out k[i, j]);

                        }
                    }
                }
                ok = validnostKljuca(k, 3);

                if (!ok)
                {
                    Console.WriteLine("Kljuc nije validan! Unesite kljuc ponovo!");
                }

            }
            Console.WriteLine("Kljuc je:");



            stampajMatricu(k, klength);
            // Console.WriteLine(nadjiDeterminantu(k, 3));
            kinv = nadjiInverznuMatricu(k, 3);
           // Console.WriteLine("Iverzni kljuc je:");
           // stampajMatricu(kinv, klength);
            while (!exit)
            {
                Console.WriteLine("Hilov algoritam: \n 1) Za sifriranje unesite 1 \n 2) Za desifriranje unesite 2 \n" +
                    " 3) Za ceo postupak unesite 3 \n 4) Za unos novog kljuca unesite 4 \n 5) Za izlaz iz programa unesite 5");

                int read = 0;
                Int32.TryParse(Console.ReadLine(), out read);
                if (read == 0)
                    continue;
                if (read == 5)
                    break;
                if (read == 4)
                    goto kljuclab;
                if (read == 1)
                {
                    Console.WriteLine("Unesite tekst za sifriranje!");

                    text = Console.ReadLine();
                    if (!validnostTeksta(text))
                    {
                        Console.WriteLine("Nevalidan tekst!");
                        continue;
                    }
                    if (text.Length % klength != 0)
                        text = dopuniTekst(text, klength);

                    obradiUlazniTekst(text, out p);

                    c = new int[text.Length];
                    c = sifrirajHill(p, k, klength);

                    Console.WriteLine("Sifrirani tekst:");
                    foreach (int el in c)
                    {

                        Znaci z = (Znaci)el;
                        string x = z.ToString();

                        if (x == "tacka")
                            x = ".";
                        else if (x == "zarez")
                            x = ",";
                        else if (x == "upitnik")
                            x = "?";
                        Console.Write(x);
                    }
                    Console.WriteLine();
                }
                else if (read == 2)
                {
                    Console.WriteLine("Unesite tekst za desifriranje!");

                    text = Console.ReadLine();
                    if (!validnostTeksta(text))
                    {
                        Console.WriteLine("Nevalidan tekst!");
                        continue;
                    }
                    if (text.Length % klength != 0)
                        text = dopuniTekst(text, klength);
                    obradiUlazniTekst(text, out p);


                    c = desifrirajHill(p, kinv, klength);
                    Console.WriteLine("Desifrirani tekst:");
                    foreach (int el in c)
                    {

                        Znaci z = (Znaci)el;
                        string x = z.ToString();

                        if (x == "tacka")
                            x = ".";
                        else if (x == "zarez")
                            x = ",";
                        else if (x == "upitnik")
                            x = "?";
                        Console.Write(x);
                    }
                    Console.WriteLine();
                }
                else if (read == 3)
                {
                    Console.WriteLine("Unesite tekst za sifriranje!");

                    text = Console.ReadLine();
                    if (!validnostTeksta(text))
                    {
                        Console.WriteLine("Nevalidan tekst!");
                        continue;
                    }
                    textlength = text.Length;
                    if (text.Length % klength != 0)
                    {
                        text = dopuniTekst(text, klength);
                        Console.WriteLine("Prosireni tekst:");
                        Console.WriteLine(text);
                    }
                    obradiUlazniTekst(text, out p);


                    c = sifrirajHill(p, k, klength);

                    Console.WriteLine("Sifrirani tekst:");
                    foreach (int el in c)
                    {
                        Znaci z = (Znaci)el;

                        string x = z.ToString();
                        if (x == "tacka")
                            x = ".";
                        else if (x == "zarez")
                            x = ",";
                        else if (x == "upitnik")
                            x = "?";

                        Console.Write(x);
                    }
                    Console.WriteLine();


                    c = desifrirajHill(c, kinv, klength);
                    Console.WriteLine("Desifrirani tekst:");
                    for (i = 0; i < textlength; i++)
                    {
                        // char x = (char)('a' + c[i]);
                        Znaci z = (Znaci)c[i];
                        string x = z.ToString();

                        if (x == "tacka")
                            x = ".";
                        else if (x == "zarez")
                            x = ",";
                        else if (x == "upitnik")
                            x = "?";
                        Console.Write(x);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
