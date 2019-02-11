﻿namespace Sales.Helpers
{
    using System.IO;
    class FilesHelper
    {
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms); return ms.ToArray();
            }
        }
    }
}
