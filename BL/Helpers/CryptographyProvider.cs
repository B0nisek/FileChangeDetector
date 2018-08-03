using System.Security.Cryptography;

namespace BL.Helpers
{
    public static class CryptographyProvider
    {
        public static byte[] Encode(byte[] content)
        {
            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            {
                return hashAlgorithm.ComputeHash(content);
            }
        }

        public static bool HashesAreEqual(byte[] hash, byte[] hashToCompare)
        {
            bool bEqual = false;
            int i = 0;

            if (hash.Length == hashToCompare.Length)
            {
                while ((i < hash.Length) && (hash[i] == hashToCompare[i]))
                {
                    i++;
                }
                if (i == hash.Length)
                {
                    return true;
                }
            }

            return bEqual;
        }
    }
}
