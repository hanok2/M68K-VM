﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace M68K {
	public enum OpSize {
		BYTE = 1,
		WORD = 2,
		LONG = 4,

		_8 = BYTE,
		_16 = WORD,
		_32 = LONG,
	}

	public partial class CPU {
		public const ulong MODE_REGISTER_D = 0;
		public const ulong MODE_REGISTER_A = 1 << 8;
		public const ulong MODE_IMMEDIATE = (7 << 8) | 4;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static OpSize Decode_Size(ulong Val) {
			if (Val == 0)
				return OpSize.BYTE;
			return (OpSize)(Val * 2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ushort Decode_DestEAddr(ulong Value) {
			return Decode_EAddr(Value.GetBits(2, 0), Value.GetBits(5, 3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ushort Decode_SrcEAddr(ulong Value) {
			return Decode_EAddr(Value.GetBits(5, 3), Value.GetBits(2, 0));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ushort Decode_EAddr(ulong Value) {
			return Decode_SrcEAddr(Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ushort Decode_EAddr(ulong Mode, ulong Reg) {
			return (ushort)(((Mode & 0xFF) << 8) | (Reg & 0xFF));
		}

		public virtual void Move(OpSize Size, ulong Instruction) {
			ulong SrcData = GetData(Size, Decode_SrcEAddr(Instruction.GetBits(5, 0)));
			SetData(Size, Decode_DestEAddr(Instruction.GetBits(11, 6)), SrcData);

			CCR_N_Sign = Size.IsNegative(SrcData);
			CCR_Z_Zero = SrcData == 0;
			CCR_V_Overflow = CCR_C_Carry = false;
		}

		public virtual void Trap(int Num) {
			TrapQueue.Enqueue(Num);
		}
		
	}
}
