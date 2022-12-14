using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS.Code.Commands.Security
{
    internal class LoginManager
    {
        public static void ChangePassword()
        {
            var unames = File.ReadAllLines("0:\\SETTINGS\\users");
            var pswds = File.ReadAllLines("0:\\SETTINGS\\login");

            Console.Write("Enter username >> ");
            var Username = Console.ReadLine();

            if (!unames.Contains(Username))
            {
                Console.WriteLine("Invalid username!\n");
                return;
            }

            Console.Write("Enter current password for {0} >> ", Username);
            var Password = Console.ReadLine();

            int UnameIndex = 0;
            int count = 0;
            var passdata = string.Empty;
            foreach (var un in unames)
            {
                if (un == Username)
                {
                    if (pswds[count] == Password)
                    {
                        Console.Write("Enter new password for {0} >> ", Username);
                        var NewPassword = Console.ReadLine();
                        passdata += NewPassword + "\n";
                    }
                }
                else
                {
                    if (pswds[count] != string.Empty)
                    {
                        passdata += pswds[count] + "\n";
                    }
                }

                count++;
            }

            File.WriteAllText("0:\\SETTINGS\\login", "\n" + passdata);
        }

        public static void RemoveUser()
        {
            Console.Write("Enter username >> ");
            var Username = Console.ReadLine();

            var unames = File.ReadAllLines("0:\\SETTINGS\\users");
            var pswds = File.ReadAllLines("0:\\SETTINGS\\login");

            if (!unames.Contains(Username))
            {
                Console.WriteLine("User doesn't exist!\n");
                return;
            }

            foreach (var u in unames)
            {
                if (u == Username)
                {
                    int UnameIndex = 0;
                    int count = 0;
                    var passdata = string.Empty;
                    var namedata = string.Empty;
                    foreach (var un in unames)
                    {
                        if (un == Username)
                        {
                            UnameIndex = count;
                        }
                        else
                        {
                            if (un != string.Empty)
                            {
                                namedata += un + "\n";
                                passdata += pswds[count] + "\n";
                            }
                        }

                        count++;
                    }

                    File.WriteAllText("0:\\SETTINGS\\users", namedata);
                    File.WriteAllText("0:\\SETTINGS\\login", passdata);
                    break;
                }
            }
        }

        public static void AddUser()
        {
            var unames = File.ReadAllText("0:\\SETTINGS\\users");
            var pswds = File.ReadAllText("0:\\SETTINGS\\login");

            Console.Write("Enter a new username >> ");
            var Username = Console.ReadLine();

            if (unames.Contains(Username))
            {
                Console.WriteLine("User already exists!\n");
                return;
            }

            Console.Write("Enter a password >> ");
            var Password = Console.ReadLine();

            unames += "\n" + Username;
            pswds += "\n" + Password;

            File.WriteAllText("0:\\SETTINGS\\users", unames);
            File.WriteAllText("0:\\SETTINGS\\login", pswds);
        }
    }
}
