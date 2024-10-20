using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NESEmulatorWPF.Operations;

namespace NESEmulatorWPF
{
    // TODO: make class a static singleton
    internal class OpcodeMapping
    {
/*        public Tuple<Instruction, AddressingMode> this[int key]
        {
            get
            {
                return mapping[key];
            }
        }*/
        public static Dictionary<int, Tuple<Instruction, AddressingMode>> Mapping { get; }

        static OpcodeMapping()
        {
            Mapping = new()
            {
                { 0x69, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.Immediate) },
                { 0x65, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.ZeroPage) },
                { 0x75, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.ZeroPageX) },
                { 0x6D, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.Absolute) },
                { 0x7D, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.AbsoluteX) },
                { 0x79, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.AbsoluteY) },
                { 0x61, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.IndirectX) },
                { 0x71, new Tuple<Instruction, AddressingMode>(Instruction.ADC, AddressingMode.IndirectY) },

                { 0x29, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.Immediate) },
                { 0x25, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.ZeroPage) },
                { 0x35, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.ZeroPageX) },
                { 0x2D, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.Absolute) },
                { 0x3D, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.AbsoluteX) },
                { 0x39, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.AbsoluteY) },
                { 0x21, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.IndirectX) },
                { 0x31, new Tuple<Instruction, AddressingMode>(Instruction.AND, AddressingMode.IndirectY) },

                { 0x0A, new Tuple<Instruction, AddressingMode>(Instruction.ASL, AddressingMode.Accumulator) },
                { 0x06, new Tuple<Instruction, AddressingMode>(Instruction.ASL, AddressingMode.ZeroPage) },
                { 0x16, new Tuple<Instruction, AddressingMode>(Instruction.ASL, AddressingMode.ZeroPageX) },
                { 0x0E, new Tuple<Instruction, AddressingMode>(Instruction.ASL, AddressingMode.Absolute) },
                { 0x1E, new Tuple<Instruction, AddressingMode>(Instruction.ASL, AddressingMode.AbsoluteX) },

                { 0x90, new Tuple<Instruction, AddressingMode>(Instruction.BCC, AddressingMode.Relative) },

                { 0xB0, new Tuple<Instruction, AddressingMode>(Instruction.BCS, AddressingMode.Relative) },

                { 0xF0, new Tuple<Instruction, AddressingMode>(Instruction.BEQ, AddressingMode.Relative) },

                { 0x24, new Tuple<Instruction, AddressingMode>(Instruction.BIT, AddressingMode.ZeroPage) },
                { 0x2C, new Tuple<Instruction, AddressingMode>(Instruction.BIT, AddressingMode.Absolute) },

                { 0x30, new Tuple<Instruction, AddressingMode>(Instruction.BMI, AddressingMode.Relative) },

                { 0xD0, new Tuple<Instruction, AddressingMode>(Instruction.BNE, AddressingMode.Relative) },

                { 0x10, new Tuple<Instruction, AddressingMode>(Instruction.BPL, AddressingMode.Relative) },

                { 0x00, new Tuple<Instruction, AddressingMode>(Instruction.BRK, AddressingMode.Implied) },

                { 0x50, new Tuple<Instruction, AddressingMode>(Instruction.BVC, AddressingMode.Relative) },

                { 0x70, new Tuple<Instruction, AddressingMode>(Instruction.BVS, AddressingMode.Relative) },

                { 0x18, new Tuple<Instruction, AddressingMode>(Instruction.CLC, AddressingMode.Implied) },

                { 0xD8, new Tuple<Instruction, AddressingMode>(Instruction.CLD, AddressingMode.Implied) },

                { 0x58, new Tuple<Instruction, AddressingMode>(Instruction.CLI, AddressingMode.Implied) },

                { 0xB8, new Tuple<Instruction, AddressingMode>(Instruction.CLV, AddressingMode.Implied) },

                { 0xC9, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.Immediate) },
                { 0xC5, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.ZeroPage) },
                { 0xD5, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.ZeroPageX) },
                { 0xCD, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.Absolute) },
                { 0xDD, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.AbsoluteX) },
                { 0xD9, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.AbsoluteY) },
                { 0xC1, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.IndirectX) },
                { 0xD1, new Tuple<Instruction, AddressingMode>(Instruction.CMP, AddressingMode.IndirectY) },

                { 0xE0, new Tuple<Instruction, AddressingMode>(Instruction.CPX, AddressingMode.Immediate) },
                { 0xE4, new Tuple<Instruction, AddressingMode>(Instruction.CPX, AddressingMode.ZeroPage) },
                { 0xEC, new Tuple<Instruction, AddressingMode>(Instruction.CPX, AddressingMode.Absolute) },

                { 0xC0, new Tuple<Instruction, AddressingMode>(Instruction.CPY, AddressingMode.Immediate) },
                { 0xC4, new Tuple<Instruction, AddressingMode>(Instruction.CPY, AddressingMode.ZeroPage) },
                { 0xCC, new Tuple<Instruction, AddressingMode>(Instruction.CPY, AddressingMode.Absolute) },

                { 0xC6, new Tuple<Instruction, AddressingMode>(Instruction.DEC, AddressingMode.ZeroPage) },
                { 0xD6, new Tuple<Instruction, AddressingMode>(Instruction.DEC, AddressingMode.ZeroPageX) },
                { 0xCE, new Tuple<Instruction, AddressingMode>(Instruction.DEC, AddressingMode.Absolute) },
                { 0xDE, new Tuple<Instruction, AddressingMode>(Instruction.DEC, AddressingMode.AbsoluteX) },

                { 0xCA, new Tuple<Instruction, AddressingMode>(Instruction.DEX, AddressingMode.Implied) },

                { 0x88, new Tuple<Instruction, AddressingMode>(Instruction.DEY, AddressingMode.Implied) },

                { 0x49, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.Immediate) },
                { 0x45, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.ZeroPage) },
                { 0x55, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.ZeroPageX) },
                { 0x4D, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.Absolute) },
                { 0x5D, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.AbsoluteX) },
                { 0x59, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.AbsoluteY) },
                { 0x41, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.IndirectX) },
                { 0x51, new Tuple<Instruction, AddressingMode>(Instruction.EOR, AddressingMode.IndirectY) },

                { 0xE6, new Tuple<Instruction, AddressingMode>(Instruction.INC, AddressingMode.ZeroPage) },
                { 0xF6, new Tuple<Instruction, AddressingMode>(Instruction.INC, AddressingMode.ZeroPageX) },
                { 0xEE, new Tuple<Instruction, AddressingMode>(Instruction.INC, AddressingMode.Absolute) },
                { 0xFE, new Tuple<Instruction, AddressingMode>(Instruction.INC, AddressingMode.AbsoluteX) },

                { 0xE8, new Tuple<Instruction, AddressingMode>(Instruction.INX, AddressingMode.Implied) },

                { 0xC8, new Tuple<Instruction, AddressingMode>(Instruction.INY, AddressingMode.Implied) },

                { 0x4C, new Tuple<Instruction, AddressingMode>(Instruction.JMP, AddressingMode.Absolute) },
                { 0x6C, new Tuple<Instruction, AddressingMode>(Instruction.JMP, AddressingMode.Indirect) },

                { 0x20, new Tuple<Instruction, AddressingMode>(Instruction.JSR, AddressingMode.Absolute) },

                { 0xA9, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.Immediate) },
                { 0xA5, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.ZeroPage) },
                { 0xB5, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.ZeroPageX) },
                { 0xAD, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.Absolute) },
                { 0xBD, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.AbsoluteX) },
                { 0xB9, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.AbsoluteY) },
                { 0xA1, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.IndirectX) },
                { 0xB1, new Tuple<Instruction, AddressingMode>(Instruction.LDA, AddressingMode.IndirectY) },

                { 0xA2, new Tuple<Instruction, AddressingMode>(Instruction.LDX, AddressingMode.Immediate) },
                { 0xA6, new Tuple<Instruction, AddressingMode>(Instruction.LDX, AddressingMode.ZeroPage) },
                { 0xB6, new Tuple<Instruction, AddressingMode>(Instruction.LDX, AddressingMode.ZeroPageY) },
                { 0xAE, new Tuple<Instruction, AddressingMode>(Instruction.LDX, AddressingMode.Absolute) },
                { 0xBE, new Tuple<Instruction, AddressingMode>(Instruction.LDX, AddressingMode.AbsoluteY) },

                { 0xA0, new Tuple<Instruction, AddressingMode>(Instruction.LDY, AddressingMode.Immediate) },
                { 0xA4, new Tuple<Instruction, AddressingMode>(Instruction.LDY, AddressingMode.ZeroPage) },
                { 0xB4, new Tuple<Instruction, AddressingMode>(Instruction.LDY, AddressingMode.ZeroPageX) },
                { 0xAC, new Tuple<Instruction, AddressingMode>(Instruction.LDY, AddressingMode.Absolute) },
                { 0xBC, new Tuple<Instruction, AddressingMode>(Instruction.LDY, AddressingMode.AbsoluteX) },

                { 0x4A, new Tuple<Instruction, AddressingMode>(Instruction.LSR, AddressingMode.Accumulator) },
                { 0x46, new Tuple<Instruction, AddressingMode>(Instruction.LSR, AddressingMode.ZeroPage) },
                { 0x56, new Tuple<Instruction, AddressingMode>(Instruction.LSR, AddressingMode.ZeroPageX) },
                { 0x4E, new Tuple<Instruction, AddressingMode>(Instruction.LSR, AddressingMode.Absolute) },
                { 0x5E, new Tuple<Instruction, AddressingMode>(Instruction.LSR, AddressingMode.AbsoluteX) },

                { 0xEA, new Tuple<Instruction, AddressingMode>(Instruction.NOP, AddressingMode.Implied) },

                { 0x09, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.Immediate) },
                { 0x05, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.ZeroPage) },
                { 0x15, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.ZeroPageX) },
                { 0x0D, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.Absolute) },
                { 0x1D, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.AbsoluteX) },
                { 0x19, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.AbsoluteY) },
                { 0x01, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.IndirectX) },
                { 0x11, new Tuple<Instruction, AddressingMode>(Instruction.ORA, AddressingMode.IndirectY) },

                { 0x48, new Tuple<Instruction, AddressingMode>(Instruction.PHA, AddressingMode.Implied) },

                { 0x08, new Tuple<Instruction, AddressingMode>(Instruction.PHP, AddressingMode.Implied) },

                { 0x68, new Tuple<Instruction, AddressingMode>(Instruction.PLA, AddressingMode.Implied) },

                { 0x28, new Tuple<Instruction, AddressingMode>(Instruction.PLP, AddressingMode.Implied) },

                { 0x2A, new Tuple<Instruction, AddressingMode>(Instruction.ROL, AddressingMode.Accumulator) },
                { 0x26, new Tuple<Instruction, AddressingMode>(Instruction.ROL, AddressingMode.ZeroPage) },
                { 0x36, new Tuple<Instruction, AddressingMode>(Instruction.ROL, AddressingMode.ZeroPageX) },
                { 0x2E, new Tuple<Instruction, AddressingMode>(Instruction.ROL, AddressingMode.Absolute) },
                { 0x3E, new Tuple<Instruction, AddressingMode>(Instruction.ROL, AddressingMode.AbsoluteX) },

                { 0x6A, new Tuple<Instruction, AddressingMode>(Instruction.ROR, AddressingMode.Accumulator) },
                { 0x66, new Tuple<Instruction, AddressingMode>(Instruction.ROR, AddressingMode.ZeroPage) },
                { 0x76, new Tuple<Instruction, AddressingMode>(Instruction.ROR, AddressingMode.ZeroPageX) },
                { 0x6E, new Tuple<Instruction, AddressingMode>(Instruction.ROR, AddressingMode.Absolute) },
                { 0x7E, new Tuple<Instruction, AddressingMode>(Instruction.ROR, AddressingMode.AbsoluteX) },

                { 0x40, new Tuple<Instruction, AddressingMode>(Instruction.RTI, AddressingMode.Implied) },

                { 0x60, new Tuple<Instruction, AddressingMode>(Instruction.RTS, AddressingMode.Implied) },

                { 0xE9, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.Immediate) },
                { 0xE5, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.ZeroPage)  },
                { 0xF5, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.ZeroPageX) },
                { 0xED, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.Absolute)  },
                { 0xFD, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.AbsoluteX) },
                { 0xF9, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.AbsoluteY) },
                { 0xE1, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.IndirectX) },
                { 0xF1, new Tuple<Instruction, AddressingMode>(Instruction.SBC, AddressingMode.IndirectY) },

                { 0x38, new Tuple<Instruction, AddressingMode>(Instruction.SEC, AddressingMode.Implied) },

                { 0xF8, new Tuple<Instruction, AddressingMode>(Instruction.SED, AddressingMode.Implied) },

                { 0x78, new Tuple<Instruction, AddressingMode>(Instruction.SEI, AddressingMode.Implied) },

                { 0x85, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.ZeroPage) },
                { 0x95, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.ZeroPageX) },
                { 0x8D, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.Absolute) },
                { 0x9D, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.AbsoluteX) },
                { 0x99, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.AbsoluteY) },
                { 0x81, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.IndirectX) },
                { 0x91, new Tuple<Instruction, AddressingMode>(Instruction.STA, AddressingMode.IndirectY) },

                { 0x86, new Tuple<Instruction, AddressingMode>(Instruction.STX, AddressingMode.ZeroPage) },
                { 0x96, new Tuple<Instruction, AddressingMode>(Instruction.STX, AddressingMode.ZeroPageY) },
                { 0x8E, new Tuple<Instruction, AddressingMode>(Instruction.STX, AddressingMode.Absolute) },

                { 0x84, new Tuple<Instruction, AddressingMode>(Instruction.STY, AddressingMode.ZeroPage) },
                { 0x94, new Tuple<Instruction, AddressingMode>(Instruction.STY, AddressingMode.ZeroPageX) },
                { 0x8C, new Tuple<Instruction, AddressingMode>(Instruction.STY, AddressingMode.Absolute) },

                { 0xAA, new Tuple<Instruction, AddressingMode>(Instruction.TAX, AddressingMode.Implied) },

                { 0xA8, new Tuple<Instruction, AddressingMode>(Instruction.TAY, AddressingMode.Implied) },

                { 0xBA, new Tuple<Instruction, AddressingMode>(Instruction.TSX, AddressingMode.Implied) },

                { 0x8A, new Tuple<Instruction, AddressingMode>(Instruction.TXA, AddressingMode.Implied) },

                { 0x9A, new Tuple<Instruction, AddressingMode>(Instruction.TXS, AddressingMode.Implied) },

                { 0x98, new Tuple<Instruction, AddressingMode>(Instruction.TYA, AddressingMode.Implied) },

            };
        }
    }
}
