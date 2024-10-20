using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static NESEmulatorWPF.Operations;

namespace NESEmulatorWPF
{
    internal class Engine
    {
        System.Timers.Timer aTimer;

        // CPU Registers
        private byte A = 0, P = 0, X = 0, Y = 0;

        // Stack
        private ReturnPoint previousReturnPoint = new(null);

        private ReturnPoint PreviousReturnPoint
        {
            get => previousReturnPoint;
            set => previousReturnPoint = value;
        }

        // Counters
        private int PC = CPUOperations.ResetVectorCPUAddress;

        // Bit Constants
        private const byte BIT_7 = (1 << 7);
        private const byte BIT_6 = (1 << 6);
        private const byte BIT_5 = (1 << 5);
        private const byte BIT_4 = (1 << 4);
        private const byte BIT_3 = (1 << 3);
        private const byte BIT_2 = (1 << 2);
        private const byte BIT_1 = (1 << 1);
        private const byte BIT_0 = (1 << 0);

        // Processor Status Flags
        private byte statusFlags = 0;

        private byte StatusFlags
        {
            set => statusFlags = value;
        }

        private bool CarryFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_0);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_0, value);
        }

        private bool ZeroFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_1);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_1, value);
        }

        private bool InterruptDisableFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_2);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_2, value);
        }

        private bool DecimalModeFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_3);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_3, value);
        }

        private bool BFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_4);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_4, value);
        }

        private bool OverflowFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_6);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_6, value);
        }

        private bool NegativeFlag
        {
            get => Convert.ToBoolean(statusFlags & BIT_7);
            set => BitOperations.ModifyNthBit(ref statusFlags, BIT_7, value);
        }

        private class ReturnPoint
        {
            private readonly ReturnPoint? previousNode;
            private int address;

            public int Address
            {
                get => address;
                set => address = value;
            }

            public ReturnPoint? PreviousNode
            {
                get => previousNode;
            }

            public ReturnPoint(ReturnPoint? previousNode, int address = 0)
            {
                this.previousNode = previousNode;
                this.address = address;
            }
        }

        public void Initialise(Canvas canvas)
        {
            var text = new TextBlock()
            {
                Text = RetrieveNESFileContents(),
                Width = 256,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(5)
            };
            canvas.Children.Add(text);
        }

        private string RetrieveNESFileContents()
        {
            var path = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\Documents\\Emulation\\ROMs\\Donkey Kong (World) (Rev 1).nes");
            CPUOperations.Memory = File.ReadAllBytes(path);
            var header = RetrieveFileHeader(CPUOperations.Memory);

            var resetVectorLocation = CPUOperations.GetIntFrom16BitAddress(CPUOperations.ResetVectorRAMAddress);
            PC = resetVectorLocation;

            bool newFrame = false;
            aTimer = new(1000 / 60); // metronome for NTSC frame timing
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler((_, _) => { newFrame = true; });
            aTimer.Start();

            string history = "";
            for (int i = 0; i < 600; i++)
            {
                var programCounterROMAddress = CPUOperations.GetROMAddress(PC);
                var opcode = CPUOperations.Memory[programCounterROMAddress];
                string opcodeName = Enum.GetName(typeof(Instruction), OpcodeMapping.Mapping[opcode].Item1) ?? "N/A";
                history += $"Line {i} 0x0{programCounterROMAddress:X}: {opcodeName} ${CPUOperations.Memory[programCounterROMAddress + 1]:X}\n";
                PerformOperation(opcode, programCounterROMAddress + 1);
                PC++;
                while (!newFrame) { /* do nothing */ }
                newFrame = false;
            }

            //return $"Reset vector location = 0x{resetVectorLocation:X}";
            return history;
        }

        private string RetrieveFileHeader(byte[] bytes)
        {
            CPUOperations.Header = bytes[..16];
            if (CPUOperations.Header == null || CPUOperations.Header.Length < 16) return "Invalid header!";
            var constant = Encoding.Default.GetString(CPUOperations.Header[..4]);
            CPUOperations.PRGRomSize = CPUOperations.Header[4] * 16 * 1024;
            var CHRROMSize = CPUOperations.Header[5] * 8;
            var CHRRAMString = CHRROMSize == 0 ? " (uses CHRRAM)" : "";
            var flags6 = CPUOperations.ConvertToBitString(CPUOperations.Header[6]); //Mapper, mirroring, battery, trainer
            var flags7 = CPUOperations.ConvertToBitString(CPUOperations.Header[7]); //Mapper, VS/Playchoice, NES 2.0
            var flags8 = CPUOperations.Header[8] == 0 ? 8 : CPUOperations.Header[8] * 8; //PRG-RAM size
            var flags9 = CPUOperations.Header[9] == 0 ? "NTSC" : "PAL";
            var flags10 = CPUOperations.ConvertToBitString(CPUOperations.Header[10]);
            var flagsFinal = "";
            Array.ForEach(CPUOperations.Header[11..], c => flagsFinal += CPUOperations.ConvertToBitString(c) + " ");
            return $"Constant: {constant}\nPRG ROM size: {CPUOperations.PRGRomSize / 1024} KB\nCHR ROM size: {CHRROMSize} KB{CHRRAMString}\n" +
                $"Flags 6 - Mapper, mirroring, battery, trainer: {flags6}\nFlags 7 - Mapper, VS/Playchoice, NES 2.0: {flags7}\n" +
                $"PRG RAM size: {flags8} KB\nTV system: {flags9}\nFlags 10 - Unofficial extension: {flags10}\nUnused padding: {flagsFinal}";
        }

        private static bool WillSumOverflow(byte val1, byte val2)
        {
            byte sum = (byte)(val1 + val2);
            return ((val1 ^ sum) & (val2 ^ sum) & BIT_7) > 0;
        }

        public void PerformOperation(byte opcode, int arg1Address)
        {
            byte immediate(int address)
            {
                PC++;
                return CPUOperations.Memory[address];
            }

            byte zeroPage(int address, out int computedAddress, byte offset = 0)
            {
                computedAddress = immediate(address);
                return CPUOperations.GetValueFromCPUAddress(computedAddress);
            }

            byte zeroPageX(int address, out int computedAddress) => zeroPage(address, out computedAddress, X);

            byte zeroPageY(int address, out int computedAddress) => zeroPage(address, out computedAddress, Y);

            byte relative(int address) => immediate(address);

            byte absolute(int address, out int computedAddress, byte offset = 0)
            {
                PC += 2;
                computedAddress = CPUOperations.GetIntFrom16BitAddress(address);
                return CPUOperations.GetValueFromCPUAddress(computedAddress);
            }

            byte absoluteX(int address, out int computedAddress) => absolute(address, out computedAddress, X);

            byte absoluteY(int address, out int computedAddress) => absolute(address, out computedAddress, Y);

            byte indirect(int address, out int computedAddress, byte offset = 0)
            {
                computedAddress = CPUOperations.GetIntFrom16BitAddress(address);
                return CPUOperations.GetValueFromCPUAddress(computedAddress);
            }

            byte indirectX(int address, out int computedAddress) => indirect(address, out computedAddress, X);

            byte indirectY(int address, out int computedAddress) => indirect(address, out computedAddress, Y);

            var instruction = OpcodeMapping.Mapping[opcode].Item1;
            var addressingMode = OpcodeMapping.Mapping[opcode].Item2;

            byte value = 0; // computed value, contents of PC + 1 if immediate otherwise contents of address contained in PC + 1
            int computedAddress = -1; // address referred to by PC + 1, -1 if constant
            switch (addressingMode)
            {
                case AddressingMode.Accumulator:
                case AddressingMode.Implied:
                    break;
                case AddressingMode.Immediate:
                    value = immediate(arg1Address);
                    break;
                case AddressingMode.ZeroPage:
                    value = zeroPage(arg1Address, out computedAddress);
                    break;
                case AddressingMode.ZeroPageX:
                    value = zeroPageX(arg1Address, out computedAddress);
                    break;
                case AddressingMode.ZeroPageY:
                    value = zeroPageY(arg1Address, out computedAddress);
                    break;
                case AddressingMode.Relative:
                    value = relative(arg1Address);
                    break;
                case AddressingMode.Absolute:
                    value = absolute(arg1Address, out computedAddress);
                    break;
                case AddressingMode.AbsoluteX:
                    value = absoluteX(arg1Address, out computedAddress);
                    break;
                case AddressingMode.AbsoluteY:
                    value = absoluteY(arg1Address, out computedAddress);
                    break;
                case AddressingMode.Indirect:
                    value = indirect(arg1Address, out computedAddress);
                    break;
                case AddressingMode.IndirectX:
                    value = indirectX(arg1Address, out computedAddress);
                    break;
                case AddressingMode.IndirectY:
                    value = indirectY(arg1Address, out computedAddress);
                    break;
            }

            switch (instruction)
            {
                case Instruction.ADC:
                    ADC(value);
                    break;
                case Instruction.AND:
                    AND(value);
                    break;
                case Instruction.ASL:
                    ASL(computedAddress);
                    break;
                case Instruction.BCC:
                    BCC(value);
                    break;
                case Instruction.BCS:
                    BCS(value);
                    break;
                case Instruction.BEQ:
                    BEQ((sbyte)value);
                    break;
                case Instruction.BIT:
                    BIT(value);
                    break;
                case Instruction.BMI:
                    BMI(value);
                    break;
                case Instruction.BNE:
                    BNE(value);
                    break;
                case Instruction.BPL:
                    BPL(value);
                    break;
                case Instruction.BRK:
                    BRK();
                    break;
                case Instruction.BVC:
                    BVC(value);
                    break;
                case Instruction.BVS:
                    BVS(value);
                    break;
                case Instruction.CLC:
                    CLC();
                    break;
                case Instruction.CLD:
                    CLD();
                    break;
                case Instruction.CLI:
                    CLI();
                    break;
                case Instruction.CLV:
                    CLV();
                    break;
                case Instruction.CMP:
                    CMP(value);
                    break;
                case Instruction.CPX:
                    CPX(value);
                    break;
                case Instruction.CPY:
                    CPY(value);
                    break;
                case Instruction.DEC:
                    DEC(computedAddress);
                    break;
                case Instruction.DEX:
                    DEX();
                    break;
                case Instruction.DEY:
                    DEY();
                    break;
                case Instruction.EOR:
                    EOR(value);
                    break;
                case Instruction.INC:
                    INC(computedAddress);
                    break;
                case Instruction.INX:
                    INX();
                    break;
                case Instruction.INY:
                    INY();
                    break;
                case Instruction.JMP:
                    JMP(computedAddress);
                    break;
                case Instruction.JSR:
                    JSR(computedAddress);
                    break;
                case Instruction.LDA:
                    LDA(value);
                    break;
                case Instruction.LDX:
                    LDX(value);
                    break;
                case Instruction.LDY:
                    LDY(value);
                    break;
                case Instruction.LSR:
                    LSR(value);
                    break;
                case Instruction.NOP:
                    NOP();
                    break;
                case Instruction.ORA:
                    ORA(value);
                    break;
                case Instruction.PHA:
                    PHA();
                    break;
                case Instruction.PHP:
                    PHP();
                    break;
                case Instruction.PLA:
                    PLA();
                    break;
                case Instruction.PLP:
                    PLP();
                    break;
                case Instruction.ROL:
                    ROL(computedAddress);
                    break;
                case Instruction.ROR:
                    ROR(computedAddress);
                    break;
                case Instruction.RTI:
                    RTI();
                    break;
                case Instruction.RTS:
                    RTS();
                    break;
                case Instruction.SBC:
                    SBC(value);
                    break;
                case Instruction.SEC:
                    SEC();
                    break;
                case Instruction.SED:
                    SED();
                    break;
                case Instruction.SEI:
                    SEI();
                    break;
                case Instruction.STA:
                    STA(computedAddress);
                    break;
                case Instruction.STX:
                    STX(computedAddress);
                    break;
                case Instruction.STY:
                    STY(computedAddress);
                    break;
                case Instruction.TAX:
                    TAX();
                    break;
                case Instruction.TAY:
                    TAY();
                    break;
                case Instruction.TSX:
                    TSX();
                    break;
                case Instruction.TXA:
                    TXA();
                    break;
                case Instruction.TXS:
                    TXS();
                    break;
                case Instruction.TYA:
                    TYA();
                    break;
            }
        }

        public void ADC(byte val)
        {
            OverflowFlag = CarryFlag = WillSumOverflow(A, val);
            A += val;
            TransferProcessing(A);
        }

        public void AND(byte val) => A = TransferProcessing((byte)(A & val));

        public void ASL(int address)
        {
            if (address == -1)
            {
                ASLCalculation(ref A);
                return;
            }
            byte val = CPUOperations.GetValueFromCPUAddress(address);
            ASLCalculation(ref val);
            CPUOperations.StoreValueAtCPUAddress(address, val);
        }

        private void ASLCalculation(ref byte val)
        {
            CarryFlag = IsNegative(val);
            val = TransferProcessing((byte)(val << 1));
        }

        public void BCC(byte val)
        {
            if (!CarryFlag) PC += val;
        }

        public void BCS(byte val)
        {
            if (CarryFlag) PC += val;
        }

        public void BEQ(sbyte val)
        {
            if (ZeroFlag) PC += val;
        }

        public void BIT(byte val)
        {
            ZeroFlag = (A & val) == 0;
            OverflowFlag = Convert.ToBoolean(val & BIT_6);
            NegativeFlag = IsNegative(val);
        }

        public void BMI(byte val)
        {
            if (NegativeFlag) PC += val;
        }

        public void BNE(byte val)
        {
            if (!ZeroFlag) PC += val;
        }

        public void BPL(byte val)
        {
            if (!NegativeFlag) PC += val;
        }

        public void BRK()
        {
            // TODO: figure out what "processor status are pushed on the stack" means
            previousReturnPoint = new ReturnPoint(previousReturnPoint, PC);
            //TODO: only uncomment below code if reset vector can be changed - probably not?
            //int resetVector = CPUOperations.GetIntFrom16BitAddress(0xFFFE);
            JMP(CPUOperations.GetIntFrom16BitAddress(CPUOperations.ResetVectorRAMAddress));
            BFlag = true;
        }

        public void BVC(byte val)
        {
            if (!OverflowFlag) PC += val;
        }

        public void BVS(byte val)
        {
            if (OverflowFlag) PC += val;
        }

        public void CLC() => CarryFlag = false;

        public void CLD() => DecimalModeFlag = false;

        public void CLI() => InterruptDisableFlag = false;

        public void CLV() => OverflowFlag = false;

        public void CMP(byte val) => CMPCalculation(val, ref A);
        public void CPX(byte val) => CMPCalculation(val, ref X);
        public void CPY(byte val) => CMPCalculation(val, ref Y);

        private void CMPCalculation(byte val, ref byte comp)
        {
            CarryFlag = comp >= val;
            ZeroFlag = comp == val;
            NegativeFlag = IsNegative((byte)(comp - val));
        }

        public void DEC(int address) // TODO: replace with ref if necessary
        {
            byte val = CPUOperations.GetValueFromCPUAddress(address);
            DECCalculation(ref val);
            CPUOperations.StoreValueAtCPUAddress(address, val);

        }

        public void DEX() => DECCalculation(ref X);
        public void DEY() => DECCalculation(ref Y);

        private void DECCalculation(ref byte reg)
        {
            reg--;
            TransferProcessing(reg);
        }

        public void EOR(byte val)
        {
            A ^= val;
            TransferProcessing(A);
        }

        public void INC(int address)
        {
            byte val = CPUOperations.GetValueFromCPUAddress(address);
            INCCalculation(ref val);
            CPUOperations.StoreValueAtCPUAddress(address, val);
        }

        public void INX() => INCCalculation(ref X);
        public void INY() => INCCalculation(ref Y);

        private void INCCalculation(ref byte reg)
        {
            reg++;
            TransferProcessing(reg);
        }

        public void JMP(int address) => PC = address - 1;

        public void JSR(int address)
        {
            previousReturnPoint = new ReturnPoint(previousReturnPoint, PC - 1);
            JMP(address);
        }

        public void LDA(byte val) => LDACalculation(ref A, val);
        public void LDX(byte val) => LDACalculation(ref X, val);
        public void LDY(byte val) => LDACalculation(ref Y, val);

        private void LDACalculation(ref byte reg, byte val) => reg = TransferProcessing(val);

        public void LSR(int? address = null) //TODO: consider changing to ref
        {
            byte val = address == null ? A : CPUOperations.GetValueFromCPUAddress((int)address);
            CarryFlag = Convert.ToBoolean(val & BIT_0); // get last bit
            var result = TransferProcessing((byte)(val >> 1));

            if (address == null)
            {
                A = val;
                return;
            }

            CPUOperations.StoreValueAtCPUAddress((int)address, val);
        }

        private Action NOP = () => { }; // do nothing

        public void ORA(byte val)
        {
            A |= val;
            TransferProcessing(A);
        }

        public void PHA() => previousReturnPoint = new ReturnPoint(previousReturnPoint, A);

        public void PHP() => previousReturnPoint = new ReturnPoint(previousReturnPoint, statusFlags);

        public void PLA() => A = GetByteFromStack();

        public void PLP() => StatusFlags = GetByteFromStack();

        private byte GetByteFromStack() => BitConverter.GetBytes(PreviousReturnPoint.Address)[Convert.ToByte(!BitConverter.IsLittleEndian)]; //TODO: check if this actually pulls the expected 8-bit value

        public void ROL(int address)
        {
            if (address == -1)
            {
                ROLCalculation(ref A);
                return;
            }

            var val = CPUOperations.GetValueFromCPUAddress(address);
            ROLCalculation(ref val);
            CPUOperations.StoreValueAtCPUAddress(address, val);
        }

        private void ROLCalculation(ref byte val)
        {
            var oldCarryFlag = CarryFlag;
            CarryFlag = Convert.ToBoolean(val & BIT_7);
            val <<= 1;
            val += Convert.ToByte(oldCarryFlag);
            TransferProcessing(val);
        }

        public void ROR(int address)
        {
            if (address == -1)
            {
                RORCalculation(ref A);
                return;
            }

            var val = CPUOperations.GetValueFromCPUAddress(address);
            RORCalculation(ref val);
            CPUOperations.StoreValueAtCPUAddress(address, val);
        }

        private void RORCalculation(ref byte val)
        {
            var oldCarryFlag = CarryFlag;
            CarryFlag = Convert.ToBoolean(val & BIT_0);
            val >>= 1;
            val += (byte)(Convert.ToByte(oldCarryFlag) << BIT_7);
            TransferProcessing(val);
        }

        public void RTI() => PLP();

        public void RTS()
        {
            PC = previousReturnPoint.Address;
            previousReturnPoint = previousReturnPoint.PreviousNode; // TODO: figure this out
        }

        public void SBC(byte val)
        {
            if (WillSumOverflow(A, val)) OverflowFlag = CarryFlag = true;
            A -= val;

            var carry = Convert.ToByte(!CarryFlag);
            if (WillSumOverflow(A, carry)) OverflowFlag = CarryFlag = true;
            A -= carry;

            TransferProcessing(A);
        }

        public void SEC() => CarryFlag = true;

        public void SED() => DecimalModeFlag = true;

        public void SEI() => InterruptDisableFlag = true;

        public void STA(int address) => STCalculation(address, ref A);
        public void STX(int address) => STCalculation(address, ref X);
        public void STY(int address) => STCalculation(address, ref Y);

        private void STCalculation(int address, ref byte reg) => CPUOperations.StoreValueAtCPUAddress(address, reg);

        public void TAX() => X = TransferProcessing(A);
        public void TAY() => Y = TransferProcessing(A);
        public void TSX() => X = TransferProcessing(GetByteFromStack());
        public void TXA() => A = TransferProcessing(X);
        public void TXS() => PreviousReturnPoint.Address = TransferProcessing(X);
        public void TYA() => A = TransferProcessing(Y);

        private byte TransferProcessing(byte source)
        {
            ZeroFlag = source == 0;
            NegativeFlag = IsNegative(source);
            return source;
        }

        private bool IsNegative(byte input) => (input & BIT_7) > 0; // check first bit set


    }
}