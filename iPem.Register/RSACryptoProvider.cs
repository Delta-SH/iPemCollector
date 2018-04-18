using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace iPem.Register {
    /// <summary>
    /// 密钥结构体
    /// </summary>
    public struct RSA_Key {
        /// <summary>
        /// 公钥
        /// </summary>
        public string Public { get; set; }

        /// <summary>
        /// 私钥
        /// </summary>
        public string Private { get; set; }
    }

    public static class RSACryptoProvider {
        /// <summary>
        /// RSA生成公钥和私钥
        /// </summary>
        /// <returns></returns>
        public static RSA_Key GenerateKey() {
            var rsa = new RSACryptoServiceProvider();
            return new RSA_Key {
                Private = rsa.ToXmlString(true),
                Public = rsa.ToXmlString(false)
            };
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="base64code">需要进行解密的密文字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>解密后的明文</returns>
        public static string Decrypt(string base64code, string privateKey) {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);

            var encryptedData = Convert.FromBase64String(base64code);
            var decryptedData = RSA.Decrypt(encryptedData, false);
            return Encoding.UTF8.GetString(decryptedData);
        }

        /// <summary>
        /// RSA分段解密;用于对超长字符串解密
        /// </summary>
        /// <param name="base64code">需要进行解密的字符串</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>解密后的明文</returns>
        public static string SectionDecrypt(string base64code, string privateKey) {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);

            var encryptedData = Convert.FromBase64String(base64code);
            var maxBlockSize = RSA.KeySize / 8;
            if (encryptedData.Length <= maxBlockSize) {
                var decryptedData = RSA.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }

            using (var encryptedStream = new MemoryStream(encryptedData)) {
                using (var totalStream = new MemoryStream()) {
                    var buffer = new byte[maxBlockSize];
                    var blockSize = encryptedStream.Read(buffer, 0, maxBlockSize);
                    while (blockSize > 0) {
                        var toDecrypt = new byte[blockSize];
                        Array.Copy(buffer, 0, toDecrypt, 0, blockSize);

                        var section = RSA.Decrypt(toDecrypt, false);
                        totalStream.Write(section, 0, section.Length);

                        blockSize = encryptedStream.Read(buffer, 0, maxBlockSize);
                    }

                    return Encoding.UTF8.GetString(totalStream.ToArray());
                }
            }
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="toEncryptString">需要进行加密的字符串</param>
        /// <param name="publicKey">公钥</param>
        /// <returns>加密后的密文</returns>
        public static string Encrypt(string toEncryptString, string publicKey) {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(publicKey);

            var dataToEncrypt = Encoding.UTF8.GetBytes(toEncryptString);
            var encrytedData = RSA.Encrypt(dataToEncrypt, false);
            return Convert.ToBase64String(encrytedData);
        }

        /// <summary>
        /// RSA分段加密;用于对超长字符串加密
        /// </summary>
        /// <param name="toEncryptString">需要进行加密的字符串</param>
        /// <param name="publickKey">公钥</param>
        /// <returns>加密后的密文</returns>
        public static string SectionEncrypt(string toEncryptString, string publickKey) {
            var RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(publickKey);

            var dataToEncrypt = Encoding.UTF8.GetBytes(toEncryptString);
            var maxBlockSize = RSA.KeySize / 8 - 11;
            if (dataToEncrypt.Length <= maxBlockSize) {
                var encrytedData = RSA.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encrytedData);
            }

            using (var encryptedStream = new MemoryStream(dataToEncrypt)) {
                using (var totalStream = new MemoryStream()) {
                    var buffer = new byte[maxBlockSize];
                    var blockSize = encryptedStream.Read(buffer, 0, maxBlockSize);
                    while (blockSize > 0) {
                        var toEncrypt = new byte[blockSize];
                        Array.Copy(buffer, 0, toEncrypt, 0, blockSize);

                        var cryptograph = RSA.Encrypt(toEncrypt, false);
                        totalStream.Write(cryptograph, 0, cryptograph.Length);

                        blockSize = encryptedStream.Read(buffer, 0, maxBlockSize);
                    }

                    return Convert.ToBase64String(totalStream.ToArray());
                }
            }
        }
    }
}
