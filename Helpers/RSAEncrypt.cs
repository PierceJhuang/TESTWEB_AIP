using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace AIPWeb.Helpers
{
    /// <summary>
    /// RSA
    /// 可逆非对称加密
    /// 非对称加密算法的优点是密钥管理很方便，缺点是速度慢。
    /// </summary>
    public class RSAEncrypt
    {
        public static string strPublicKey = "<RSAKeyValue><Modulus>vLKAF1u5TmadpwKq8MAUbV/vQCimABmLQMZ2GUk55prTrjKw7LXahAdIJG39sEGXbKDdz/EmS2AEC/YvWqc90lWMVtjqtXsjVT9Ljx9Ba3yusCQpRwyMIQh+EzlUhg9z9iPxp6qyeZnswFUGL86ZqCRTILlUxj/37MrySdY27Dk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public static string strPrivateKey = "<RSAKeyValue><Modulus>vLKAF1u5TmadpwKq8MAUbV/vQCimABmLQMZ2GUk55prTrjKw7LXahAdIJG39sEGXbKDdz/EmS2AEC/YvWqc90lWMVtjqtXsjVT9Ljx9Ba3yusCQpRwyMIQh+EzlUhg9z9iPxp6qyeZnswFUGL86ZqCRTILlUxj/37MrySdY27Dk=</Modulus><Exponent>AQAB</Exponent><P>zmuaEnmu1WggrJkD5MoNY5y1ZLR6d13LiDzOKPA59mvWzN+kr65pO54jl9kyXwnyvmK0L9q2bClDgANNgf+hiw==</P><Q>6gUkKVXeJHADMYsEFt47dAgGWXT3O8b648FjOSTGOSzYdlmZhM+fm3woKesOz58hJmbBycAftLHPBarXHhnZyw==</Q><DP>hmzOZk+2CqYH+T6gqYLnm0I3YsChrFB5tlwaydA/fvmVZdAS2JHVWAzRExdP1VKOMWvn+e4NtHxex+PeutT1pw==</DP><DQ>P7tWtcskzFeVBxEM9A1hs5/b2tD349e0zAIGu7FAKqID8XdFT94bYYTWi1PwnKqBpKunawJfoB4seyajnxb7MQ==</DQ><InverseQ>DbQM5xR0l7OrtiqBMuiAq4icsS28EU8YRtlWu7sWtj86VRScXXpyBZEgKPtN3hghhUowtbT1TFWSh0EUqS9Ubg==</InverseQ><D>BRCJbZcRpzAL5TKmIeYV5kWf50nwAqqs2SU6gD0955maOt+zbfcHkWmcnxWwrG1mQg0jRvSDAZznHucRqE4604GT/wmgc/rcaMvhwrfK4WoqlTuznSdUJdNzfz1GwDIc8Y8QhAwfJjOkcGqPZ3c1ftUhvcp9lBtRq+4WASWtrp0=</D></RSAKeyValue>";

            /// <summary>
            /// 获取加密/解密对
            /// 给你一个，是无法推算出另外一个的
            ///
            /// Encrypt   Decrypt
            /// Encrypt   Decrypt
            /// </summary>
            /// <returns>Encrypt   Decrypt</returns>
            public static KeyValuePair<string, string> GetKeyPair()
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                string publicKey = RSA.ToXmlString(false);
                string privateKey = RSA.ToXmlString(true);
                return new KeyValuePair<string, string>(publicKey, privateKey);
            }
            /// <summary>
            /// 加密：内容+加密key
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static string Encrypt(string content)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(strPublicKey);
                UnicodeEncoding ByteConverter = new UnicodeEncoding();
                byte[] DataToEncrypt = ByteConverter.GetBytes(content);
                byte[] resultBytes = rsa.Encrypt(DataToEncrypt, false);
                return Convert.ToBase64String(resultBytes);
            }
            /// <summary>
            /// 解密  内容+解密key
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public static string Decrypt(string content)
            {
                byte[] dataToDecrypt = Convert.FromBase64String(content);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(strPrivateKey);
                byte[] resultBytes = RSA.Decrypt(dataToDecrypt, false);
                UnicodeEncoding ByteConverter = new UnicodeEncoding();
                return ByteConverter.GetString(resultBytes);
            }

            /// <summary>
            /// 可以合并在一起的，，每次产生一组新的密钥
            /// </summary>
            /// <param name="content"></param>
            /// <param name="encryptKey">加密key</param>
            /// <param name="decryptKey">解密key</param>
            /// <returns>加密后结果</returns>
            private static string Encrypt(string content, out string publicKey, out string privateKey)
            {
                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
                publicKey = rsaProvider.ToXmlString(false);
                privateKey = rsaProvider.ToXmlString(true);
                UnicodeEncoding ByteConverter = new UnicodeEncoding();
                byte[] DataToEncrypt = ByteConverter.GetBytes(content);
                byte[] resultBytes = rsaProvider.Encrypt(DataToEncrypt, false);
                return Convert.ToBase64String(resultBytes);
            }
        }
}
