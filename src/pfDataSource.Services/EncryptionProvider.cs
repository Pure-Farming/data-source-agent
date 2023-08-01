using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pfDataSource.Db;
using pfDataSource.Db.Models;
using pfDataSource.Common;
using System;

namespace pfDataSource.Services
{
    public interface IEncryptionProvider
    {
        string Encrypt(string input);
        string Decrypt(string input);
    }

    public class EncryptionProvider : IEncryptionProvider
    {
        private readonly CryptoKeys keys;

        public EncryptionProvider(ApplicationDbContext context)
        {
            var t = GetOrCreateKeys(context);
            t.Wait();
            this.keys = t.Result;
        }

        public string Decrypt(string input)
        {
            var bytes = Convert.FromBase64String(input);

            using var ms = new MemoryStream(bytes);
            using (var aes = Aes.Create())
            {
                aes.Key = keys.Key;
                aes.IV = keys.InitialisationVector;

                using var dc = aes.CreateDecryptor(keys.Key, keys.InitialisationVector);
                using var cs = new CryptoStream(ms, dc, CryptoStreamMode.Read);
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public string Encrypt(string input)
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

            using (var aes = Aes.Create())
            {
                aes.Key = keys.Key;
                aes.IV = keys.InitialisationVector;

                using (var ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private async Task<CryptoKeys> GetOrCreateKeys(ApplicationDbContext context)
        {
            var found = await context.CryptoKeys.FirstOrDefaultAsync();
            if (found != null) return found;

            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.GenerateKey();

                found = new CryptoKeys()
                {
                    Key = aes.Key,
                    InitialisationVector = aes.IV
                };
            }

            context.CryptoKeys.Add(found);
            await context.SaveChangesAsync();
            return found;
        }
    }
}

