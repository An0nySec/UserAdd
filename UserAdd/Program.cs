using System;
using System.DirectoryServices;


// Reference:https://docs.microsoft.com/zh-cn/troubleshoot/dotnet/csharp/add-user-local-system

namespace UserAdd
{
    class Class1
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: UserAdd.exe <username> <password>");
                //Console.WriteLine("       UserAdd.exe guest password (Guest enabled)");
            }
            else
            {
                string username = args[0];
                string password = args[1];
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
                    Console.WriteLine("Account Created Successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    }
}
