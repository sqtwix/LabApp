using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LabApp.Infrastructure.Helpers;

public static class EncryptionHelper
{
    private static byte[]? _key;
    private static byte[]? _iv;

    public static void Initialize(string keyString, string ivString)
    {
        if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(ivString))
            throw new InvalidOperationException("Ключи шифрования не заданы");

        _key = Encoding.UTF8.GetBytes(keyString);
        _iv = Encoding.UTF8.GetBytes(ivString);
    }

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return plainText;
        if (_key == null || _iv == null)
            throw new InvalidOperationException("EncryptionHelper не инициализирован");

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(cipherBytes);
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;
        if (_key == null || _iv == null)
            throw new InvalidOperationException("EncryptionHelper не инициализирован");

        if (!IsBase64String(cipherText))
            return cipherText;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            var decryptor = aes.CreateDecryptor();
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException)
        {
            return cipherText;
        }
        catch (CryptographicException)
        {
            return cipherText;
        }
    }

    private static bool IsBase64String(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        s = s.Trim();
        return (s.Length % 4 == 0) &&
               Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,2}$");
    }
}