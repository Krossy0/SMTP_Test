using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

namespace SMTP_Test
{
    internal class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public User(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
    }

    internal class Program
    {
        static string fromMail = "[GMAIL]";
        static string appPassword = "[APP PASSWORD]";

        static void Main(string[] args)
        {
            Console.Write("Adinizi daxil edin: ");
            string name = Console.ReadLine();

            Console.Write("Email daxil edin: ");
            string email = Console.ReadLine();

            Console.Write("Sifre daxil edin: ");
            string password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Butun xanalari doldurun!");
                return;
            }

            User user = new User(name, email, password);

            string otp = RandomNumberGenerator
                .GetInt32(100000, 1000000).ToString();

            try
            {
                SendEmail(user, otp);

                Console.WriteLine("\nTesdiq kodu email unvaniniza gonderildi.");

                DateTime istifademuddeti = DateTime.Now.AddMinutes(3);//indiki vaxtda 3 deq elave edir

                for (int i = 0; i < 3; i++)
                {
                    Console.Write("6 reqemli tesdiq kodunu daxil edin: ");
                    string daxiledilmisotp = Console.ReadLine() ?? "";

                    if (DateTime.Now > istifademuddeti) //indiki vaxtlla 3 deq elave ettiyi vaxti muqaise eliyir eger booyukduse demeli oootpnin vaxti kecib
                    {
                        Console.WriteLine("Kodun etibarliliq muddeti bitib");
                        return;
                    }

                    if (daxiledilmisotp == otp)
                    {
                        Console.WriteLine("\nQeydiyyat ugurla tamamlandi!");
                        Console.WriteLine($"Xos geldiniz, {user.Name}!");
                        return;
                    }

                    Console.WriteLine("Tesdiq kodu yanlisdir!");

                    if (i < 2)
                    {
                        Console.WriteLine($"Qalan cehd: {2 - i}");
                    }
                }

                Console.WriteLine("Cehd limiti bitdi. Qeydiyyat tamamlanmadi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email gonderilmesi zamani xeta bas verdi!");
                Console.WriteLine(ex.Message);
            }
        }

        static void SendEmail(User user, string otp)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(fromMail);
            mail.To.Add(user.Email);

            mail.Subject = "Email tesdiqi";

            mail.Body = $@"
                Salam, {(user.Name)}!
                Qeydiyyatinizi tamamlamaq ucun tesdiq kodunuz:
                    {otp}
                Kod 3 deqiqe erzinde etibarlidir.";

            using SmtpClient smtp = new SmtpClient("smtp.gmail.com");

            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials =
                new NetworkCredential(fromMail, appPassword);

            smtp.Send(mail);
        }
    }
}
