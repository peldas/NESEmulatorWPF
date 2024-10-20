namespace NESEmulatorWPF
{
    internal static class CPUOperations
    {
        // PRG ROM size
        private static int PRG_ROM_SIZE = 0;

        public static int PRGRomSize
        {
            get => PRG_ROM_SIZE;
            set
            {
                PRG_ROM_SIZE = value;
                RESET_VECTOR_ROM_ADDRESS = GetROMAddress(RESET_VECTOR_CPU_ADDRESS);
            }
        }

        // Zero Page memory
        private static byte[] zeroPageMemory = new byte[256];

        // ROM
        private static byte[] header = [];
        private static byte[] memory = [];

        public static byte[] Header
        {
            get => header;
            set => header = value;
        }

        public static byte[] Memory
        {
            get => memory;
            set => memory = value;
        }

        // CPU addresses
        private const int RESET_VECTOR_CPU_ADDRESS = 0xFFFC;

        public static int ResetVectorCPUAddress
        {
            get => RESET_VECTOR_CPU_ADDRESS;
        }

        // ROM addresses
        private const int PRG_ROM_OFFSET = 0x0010;
        private static int RESET_VECTOR_ROM_ADDRESS;

        public static int ResetVectorRAMAddress
        {
            get => RESET_VECTOR_ROM_ADDRESS;
        }

        // PPU registers
        public enum PPURegister
        {
            PPUCTRL = 0x2000,
            PPUMASK = 0x2001,
            PPUSTATUS = 0x2002,
            OAMDDR = 0x2003,
            OAMDATA = 0x2004,
            PPUSCROLL = 0x2005,
            PPUADDR = 0x2006,
            PPUDATA = 0x2007,
            OAMDMA = 0x4014
        }

        // CPU memory ranges
        private const int CPU_INTERNAL_RAM_LO = 0x0000;
        private const int CPU_INTERNAL_RAM_HI = 0x07FF;
        private const int ZERO_PAGE_HI = 0x00FF;
        private const int STACK_PAGE_LO = 0x0100;
        private const int STACK_PAGE_HI = 0x01FF;
        private const int CPU_INTERNAL_RAM_MIRROR_HI = 0x1FFF;
        private const int PPU_REGISTER_LO = (int)PPURegister.PPUCTRL;
        private const int PPU_REGISTER_HI = (int)PPURegister.PPUDATA;
        private const int PPU_REGISTER_MIRROR_LO = 0x2008;
        private const int PPU_REGISTER_MIRROR_HI = 0x3FFF;
        private const int APU_IO_REGISTER_LO = 0x4000;
        private const int APU_IO_REGISTER_HI = 0x4017;
        private const int APU_IO_DISABLED_REGISTER_LO = 0x4018;
        private const int APU_IO_DISABLED_REGISTER_HI = 0x401F;
        private const int CARTRIDGE_RAM_LO = 0x6000;
        private const int CARTRIDGE_RAM_HI = 0x7FFF;
        private const int CARTRIDGE_ROM_LO = 0x8000;
        private const int CARTRIDGE_ROM_HI = 0xFFFF;

        private enum MemoryRange
        {
            ZERO_PAGE,
            STACK_PAGE,
            GENERIC_INTERNAL_RAM,
            PPU_REGISTER,
            APU_IO_REGISTER,
            APU_IO_DISABLED_REGISTER,
            CARTRIDGE_RAM,
            CARTRIDGE_ROM
        }


        public static Dictionary<PPURegister, byte> ppuRegisters = [];

        private static int UnmirrorCPUAddress(int cpuAddress)
        {
            switch (cpuAddress)
            {
                case var _ when cpuAddress <= CPU_INTERNAL_RAM_MIRROR_HI:
                    return cpuAddress % CPU_INTERNAL_RAM_HI;
                case var _ when cpuAddress >= PPU_REGISTER_MIRROR_LO && cpuAddress <= PPU_REGISTER_MIRROR_HI:
                    return (cpuAddress % 8) + PPU_REGISTER_LO;
                case var _ when cpuAddress >= CARTRIDGE_ROM_LO + PRGRomSize:
                    return ((cpuAddress - CARTRIDGE_ROM_LO) % PRGRomSize) + CARTRIDGE_ROM_LO;
                default:
                    return cpuAddress;
            }
        }

        private static MemoryRange GetMemoryRangeFromCPUAddress(int cpuAddress)
        {
            if (cpuAddress <= ZERO_PAGE_HI) return MemoryRange.ZERO_PAGE;

            if (cpuAddress <= STACK_PAGE_HI) return MemoryRange.STACK_PAGE;

            if (cpuAddress <= CPU_INTERNAL_RAM_HI) return MemoryRange.GENERIC_INTERNAL_RAM;

            if (Enum.IsDefined(typeof(PPURegister), cpuAddress)) return MemoryRange.PPU_REGISTER ;

            if (cpuAddress <= APU_IO_REGISTER_HI) return MemoryRange.APU_IO_REGISTER;

            if (cpuAddress <= APU_IO_DISABLED_REGISTER_HI) return MemoryRange.APU_IO_DISABLED_REGISTER;

            if (cpuAddress <= CARTRIDGE_RAM_HI) return MemoryRange.CARTRIDGE_RAM;

            return MemoryRange.CARTRIDGE_ROM;
        }



        public static byte GetValueFromCPUAddress(int cpuAddress)
        {
            //cpuAddress = UnmirrorCPUAddress(cpuAddress);

            switch (GetMemoryRangeFromCPUAddress(cpuAddress))
            {
                case MemoryRange.ZERO_PAGE:
                    return zeroPageMemory[cpuAddress];
                case MemoryRange.STACK_PAGE:
                    throw new NotImplementedException();
                case MemoryRange.GENERIC_INTERNAL_RAM:
                    throw new NotImplementedException();
                case MemoryRange.PPU_REGISTER:
                    return ppuRegisters.GetValueOrDefault((PPURegister)cpuAddress);
                case MemoryRange.APU_IO_REGISTER:
                    throw new NotImplementedException();
                case MemoryRange.APU_IO_DISABLED_REGISTER:
                    throw new NotImplementedException();
                case MemoryRange.CARTRIDGE_RAM:
                    throw new NotImplementedException();
                default:
                    return memory[GetROMAddress(cpuAddress)];
            }
        }

        public static void StoreValueAtCPUAddress(int cpuAddress, byte value)
        {
            //cpuAddress = UnmirrorCPUAddress(cpuAddress);

            switch (GetMemoryRangeFromCPUAddress(cpuAddress))
            {
                case MemoryRange.ZERO_PAGE:
                    zeroPageMemory[cpuAddress] = value;
                    break;
                case MemoryRange.STACK_PAGE:
                    throw new NotImplementedException();
                case MemoryRange.GENERIC_INTERNAL_RAM:
                    throw new NotImplementedException();
                case MemoryRange.PPU_REGISTER:
                    ppuRegisters[(PPURegister)cpuAddress] = value;
                    break;
                case MemoryRange.APU_IO_REGISTER:
                    throw new NotImplementedException();
                case MemoryRange.APU_IO_DISABLED_REGISTER:
                    throw new NotImplementedException();
                case MemoryRange.CARTRIDGE_RAM:
                    throw new NotImplementedException();
                default:
                    memory[GetROMAddress(cpuAddress)] = value;
                    break;
            }
        }

        public static byte GetZeroPageValueFromCPUAddress(int address, int offset = 0)
        {
            var zeroPageAddress = GetValueFromCPUAddress(address) + offset;
            return GetValueFromCPUAddress(zeroPageAddress);
        }

        public static int GetROMAddress(int cpuAddress)
        {
            cpuAddress = UnmirrorCPUAddress(cpuAddress);
            return PRG_ROM_OFFSET + cpuAddress - CARTRIDGE_ROM_LO;
        }

        public static string ConvertToBitString(byte input)
        {
            return Convert.ToString(input, 2).PadLeft(8, '0');
        }

        public static ushort GetIntFrom16BitAddress(int address)
        {
            return BitConverter.ToUInt16(Memory[address..(address + 2)], 0);
        }
    }
}
