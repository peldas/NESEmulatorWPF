using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NESEmulatorWPF
{
    internal static class BitOperations
    {
        public static void ModifyNthBit(ref byte num, byte mask, bool val) => num ^= (byte)((-Convert.ToByte(val) ^ num) & mask);
    }
}
