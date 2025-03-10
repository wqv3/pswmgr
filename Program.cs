using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

class manager
{
    private static string key;
    private static string profileproccessed;
    private static string filepath;

    static void Main()
    {
        while (true)
        {
            Console.Write("enter encryption key: ");
            string headkey = Console.ReadLine();
            key = processkey(headkey);

            Console.Write("enter profile name: ");
            profileproccessed = Console.ReadLine();

            string originx = Path.Combine(Directory.GetCurrentDirectory(), "data");
            if (!Directory.Exists(originx))
            {
                Directory.CreateDirectory(originx);
            }
            filepath = Path.Combine(originx, $"{profileproccessed}_encrypted.sec");

            uixd();

            break;
        }
    }

    private static void uixd()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. create password");
            Console.WriteLine("2. view saved passwords");
            Console.WriteLine("3. switch profile");
            Console.WriteLine("0. exit");
            Console.Write("select an option: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    castpsw();
                    break;
                case "2":
                    listpsw();
                    break;
                case "3":
                    switchbetweenx();
                    break;
                case "0":
                    Console.WriteLine("exiting...");
                    return;
                default:
                    Console.WriteLine("invalid input. please try again.");
                    break;
            }
        }
    }

    private static void switchbetweenx()
    {
        Console.Write("enter new encryption key: ");
        string headkey = Console.ReadLine();
        key = processkey(headkey);

        Console.Write("enter new profile name: ");
        profileproccessed = Console.ReadLine();

        string originx = Path.Combine(Directory.GetCurrentDirectory(), "data");
        filepath = Path.Combine(originx, $"{profileproccessed}_encrypted.sec");

        uixd();
    }

    private static string processkey(string headkey)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] cryptorhash = sha256.ComputeHash(Encoding.UTF8.GetBytes(headkey));
            return Convert.ToBase64String(cryptorhash);
        }
    }

    private static void castpsw()
    {
        Console.Clear();
        Console.Write("enter a note for the password: ");
        string note = Console.ReadLine();
        string password = generaterandompassword(16);
        string encryptedpassword = encrypt(password);

        var entry = new passwordentry { note = note, password = encryptedpassword };
        savepassword(entry);

        Console.WriteLine($"password created: {password}");
        Console.WriteLine("press any key to return to menu...");
        Console.ReadKey();
    }

    private static string generaterandompassword(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        StringBuilder result = new StringBuilder(length);
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] buffer = new byte[length];
            rng.GetBytes(buffer);
            foreach (var b in buffer)
                result.Append(chars[b % chars.Length]);
        }
        return result.ToString();
    }

    private static void savepassword(passwordentry entry)
    {
        List<passwordentry> passwords = new List<passwordentry>();
        if (File.Exists(filepath))
        {
            string json = File.ReadAllText(filepath);
            passwords = JsonConvert.DeserializeObject<List<passwordentry>>(json) ?? new List<passwordentry>();
        }
        passwords.Add(entry);
        File.WriteAllText(filepath, JsonConvert.SerializeObject(passwords, Formatting.Indented));
    }

    private static void listpsw()
    {
        Console.Clear();
        if (!File.Exists(filepath))
        {
            Console.WriteLine("password file is missing.");
            Console.WriteLine("press any key to return to menu...");
            Console.ReadKey();
            return;
        }
        string json = File.ReadAllText(filepath);
        List<passwordentry> passwords = JsonConvert.DeserializeObject<List<passwordentry>>(json);
        if (passwords == null || passwords.Count == 0)
        {
            Console.WriteLine("no passwords saved yet.");
            Console.WriteLine("press any key to return to menu...");
            Console.ReadKey();
            return;
        }
        foreach (var entry in passwords)
        {
            string decryptedpassword = decrypt(entry.password);
            Console.WriteLine($"note: {entry.note} | password: {decryptedpassword}");
        }
        Console.WriteLine("press any key to return to menu...");
        Console.ReadKey();
    }

    private static string encrypt(string plaintext)
    {
        using (Aes ahhhyes = Aes.Create())
        {
            ahhhyes.Key = Convert.FromBase64String(key);
            ahhhyes.GenerateIV();

            using (var encryptor = ahhhyes.CreateEncryptor(ahhhyes.Key, ahhhyes.IV))
            using (var encrypteddata = new MemoryStream())
            {
                using (var cs = new CryptoStream(encrypteddata, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plaintext);
                }

                byte[] cat = ahhhyes.IV;
                byte[] encrypted = encrypteddata.ToArray();

                byte[] result = new byte[cat.Length + encrypted.Length];
                Buffer.BlockCopy(cat, 0, result, 0, cat.Length);
                Buffer.BlockCopy(encrypted, 0, result, cat.Length, encrypted.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    private static string decrypt(string encryptedtext)
    {
        byte[] completed = Convert.FromBase64String(encryptedtext);
        if (completed.Length < 16)
        {
            throw new CryptographicException("encrypted data is too short.");
        }

        byte[] cat = new byte[16];
        byte[] Locker = new byte[completed.Length - cat.Length];

        Buffer.BlockCopy(completed, 0, cat, 0, cat.Length);
        Buffer.BlockCopy(completed, cat.Length, Locker, 0, Locker.Length);

        using (Aes ahhhyes = Aes.Create())
        {
            ahhhyes.Key = Convert.FromBase64String(key);
            ahhhyes.IV = cat;

            try
            {
                using (var decryptor = ahhhyes.CreateDecryptor(ahhhyes.Key, ahhhyes.IV))
                using (var decrypteddata = new MemoryStream())
                {
                    using (var cs = new CryptoStream(decrypteddata, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(Locker, 0, Locker.Length);
                    }

                    return Encoding.UTF8.GetString(decrypteddata.ToArray());
                }
            }
            catch (CryptographicException)
            {
                return new string('?', Locker.Length);
            }
        }
    }
}

class passwordentry
{
    public string note { get; set; }
    public string password { get; set; }
}
