
using FluentAssertions;
using System.Text;
using pfDataSource.Common;
using pfDataSource.Services;
using Moq;
using pfDataSource.Db;
using Microsoft.EntityFrameworkCore;
using pfDataSource.Db.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Cryptography;

namespace pfDataSource.Test
{
    public class EncryptionProviderTests
    {
        private string secret = "Some secret value here";
        private EncryptionProvider _encryptionProvider;

        public EncryptionProviderTests()
        {

            CryptoKeys keys = new CryptoKeys();

            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.GenerateKey();

                keys.Key = aes.Key;
                keys.InitialisationVector = aes.IV;
            }

            _encryptionProvider = new EncryptionProvider(keys);

        }


        [Fact]
        public void EncryptionProviderTests_Can_Encrypt_And_DecryptValue()
        {

            var encrypted = _encryptionProvider.Encrypt(secret);

            encrypted.Should().NotBeNull();

            var result = _encryptionProvider.Decrypt(encrypted);

            result.Should().NotBeNull();    
            result.Should().Be(secret);
        }

    }
}