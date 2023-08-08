using Microsoft.EntityFrameworkCore;
using Moq;
using pfDataSource.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace pfDataSource.Test
{
    public class CommonTestUtils
    {

        public static CryptoKeys GetCryptoKeys()
        {
            CryptoKeys keys = new CryptoKeys();

            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.GenerateKey();

                keys.Key = aes.Key;
                keys.InitialisationVector = aes.IV;
            }

            return keys;
        }

    }
}
