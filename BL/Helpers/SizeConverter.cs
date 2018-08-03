using ByteSizeLib;

namespace BL.Helpers
{
    public static class SizeConverter
    {
        public static double ConvertBytesToMegaBytes(long size)
        {
            return ByteSize.FromBytes(size).MegaBytes;
        }
    }
}