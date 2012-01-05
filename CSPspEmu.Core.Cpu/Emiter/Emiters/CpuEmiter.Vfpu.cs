﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using CSharpUtils;
using System.Linq.Expressions;

namespace CSPspEmu.Core.Cpu.Emiter
{
	unsafe sealed public partial class CpuEmiter
	{
		// Move From/to Vfpu (C?)_
		public void mfv() {
			MipsMethodEmiter.SaveGPR_F(RT, () =>
			{
				Load_VD(0, 1);
			});
		}
		public void mfvc() { throw (new NotImplementedException("mfvc")); }

		/// <summary>
		/// ID("mtv",         VM("010010:00:111:rt:0:0000000:0:vd"), "%t, %zs", ADDR_TYPE_NONE, INSTR_TYPE_PSP),
		/// </summary>
		public void mtv()
		{
			/*
			Console.Error.WriteLine("MTV:{0}", Instruction.VD);
			_VfpuLoadVectorWithIndexPointer(Instruction.VD, 0, 1);
			MipsMethodEmiter.LoadGPR_Signed(Instruction.RT);
			MipsMethodEmiter.CallMethod(typeof(MathFloat), "ReinterpretIntAsFloat");
			MipsMethodEmiter.ILGenerator.Emit(OpCodes.Stind_R4);
			*/

			VectorOperationSaveVd(1, (Index, Load) =>
			{
				MipsMethodEmiter.LoadGPR_Signed(RT);
				MipsMethodEmiter.CallMethod(typeof(MathFloat), "ReinterpretIntAsFloat");
			});
		}
		public void mtvc() { throw (new NotImplementedException("mtvc")); }

		// Load/Store Vfpu (Left/Right)_
		public void lv_s()
		{
			//return;
			uint VT = Instruction.VT5 | (Instruction.VT2 << 5);
			uint Column = (VT >> 5) & 3;
			uint Matrix = (VT >> 2) & 7;
			uint Row    = (VT >> 0) & 3;

			SaveVprField(CalcVprRegisterIndex(Matrix, Column, Row), () =>
			{
				_load_memory_imm14_index(0);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldind_R4);
			});
		}

		// ID("lv.q",        VM("110110:rs:vt5:imm14:0:vt1"), "%Xq, %Y", ADDR_TYPE_NONE, INSTR_TYPE_PSP),
		public void lv_q()
		{
			uint Register = Instruction.VT5_1;

			for (uint Index = 0; Index < 4; Index++)
			{
				_VfpuLoadVectorWithIndexPointer(Register, Index, 4);

				_load_memory_imm14_index(Index);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldind_R4);

				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Stind_R4);
			}
		}

		static public void _lvl_svl_q(CpuThreadState CpuThreadState, uint m, uint i, uint address, bool dir, bool save)
		{
			uint k = 3 - ((address >> 2) & 3);
			address &= unchecked((uint)~0xF);

			for (uint j = k; j < 4; ++j) {
				fixed (float* VFPR = &CpuThreadState.VFR0)
				{
					float* ptr;
					if (dir)
					{
						ptr = &VFPR[m * 16 + i * 4 + j];
					}
					else
					{
						ptr = &VFPR[m * 16 + j * 4 + i];
					}
					if (save) {
						*(float*)CpuThreadState.GetMemoryPtr(address) = *ptr;
					} else {
						*ptr = *(float*)CpuThreadState.GetMemoryPtr(address);
					}
				}
				address += 4;
			}
		}

		static public void _lvr_svr_q(CpuThreadState CpuThreadState, uint m, uint i, uint address, bool dir, bool save)
		{
			uint k = 4 - ((address >> 2) & 3);

			for (uint j = 0; j < k; ++j)
			{
				fixed (float* VFPR = &CpuThreadState.VFR0)
				{
					float* ptr;
					if (dir)
					{
						ptr = &VFPR[m * 16 + i * 4 + j];
					}
					else
					{
						ptr = &VFPR[m * 16 + j * 4 + i];
					}
					if (save)
					{
						*(float*)CpuThreadState.GetMemoryPtr(address) = *ptr;
					}
					else
					{
						*ptr = *(float*)CpuThreadState.GetMemoryPtr(address);
					}
				}
				address += 4;
			}
		}

		private void lv_sv_l_r_q(bool left, bool save)
		{
			var vt = Instruction.VT5 | (Instruction.VT1 << 5);
			var m = (vt >> 2) & 7;
			var i = (vt >> 0) & 3;
			var dir = (vt & 32) != 0;

			{
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldarg_0); // CpuThreadState
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_I4, m);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_I4, i);
				MipsMethodEmiter.LoadGPR_Unsigned(RS);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_I4, Instruction.IMM14 * 4);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Add);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_I4, dir ? 1 : 0);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_I4, save ? 1 : 0);
			}

			if (left)
			{
				MipsMethodEmiter.CallMethod(this.GetType(), "_lvl_svl_q");
			}
			else
			{
				MipsMethodEmiter.CallMethod(this.GetType(), "_lvr_svr_q");
			}
		}

		public void lvl_q()
		{
			lv_sv_l_r_q(left: true, save: false);
		}

		public void lvr_q()
		{
			lv_sv_l_r_q(left: false, save: false);
		}

		/// <summary>
		/// ID("sv.q",        VM("111110:rs:vt5:imm14:0:vt1"), "%Xq, %Y", ADDR_TYPE_NONE, INSTR_TYPE_PSP),
		/// </summary>
		public void sv_q()
		{
			//loadVt(4, instruction.VT5_1);

			uint Register = Instruction.VT5_1;

			uint VectorSize = 4;

			foreach (var Index in XRange(VectorSize))
			{
				_load_memory_imm14_index((uint)Index);
				{
					//Load_VT(
					//VfpuLoad_Register(Register, Index, VectorSize, PrefixTarget);
					VfpuLoad_Register(Register, Index, VectorSize, ref PrefixNone);
					//_VfpuLoadVectorWithIndexPointer(Register, (uint)Index, 4);
					//MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldind_R4);
				}
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Stind_R4);
			}
		}

		public void svl_q()
		{
			lv_sv_l_r_q(left: true, save: true);
		}

		public void svr_q()
		{
			lv_sv_l_r_q(left: false, save: true);
		}

		// Vfpu DOT product
		// Vfpu SCaLe/ROTate
		/// <summary>
		/// ID("vdot",        VM("011001:001:vt:two:vs:one:vd"), "%zs, %yp, %xp", ADDR_TYPE_NONE, INSTR_TYPE_PSP),
		/// </summary>
		public void vdot()
		{
			uint VectorSize = Instruction.ONE_TWO;
			if (VectorSize == 1)
			{
				throw (new NotImplementedException(""));
			}
			_VfpuLoadVectorWithIndexPointer(Instruction.VD, 0, 1);
			{
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 0.0f);
				for (uint Index = 0; Index < VectorSize; Index++)
				{
					_VfpuLoadVectorWithIndexPointer(Instruction.VS, Index, VectorSize);
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldind_R4);

					_VfpuLoadVectorWithIndexPointer(Instruction.VT, Index, VectorSize);
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldind_R4);

					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Add);
				}
			}
			MipsMethodEmiter.ILGenerator.Emit(OpCodes.Stind_R4);

			/*
			loadVs(vsize);
			loadVt(vsize);
			{
				VD[0] = 0.0;
				foreach (n; 0..vsize) VD[0] += VS[n] * VT[n];
			}
			saveVd(1);
			*/
		}
		public void vscl() {
			uint VectorSize = Instruction.ONE_TWO;
			if (VectorSize == 1)
			{
				throw (new NotImplementedException(""));
			}

			foreach (var Index in XRange(VectorSize))
			{
				Save_VD(Index, VectorSize, () =>
				{
					Load_VS(Index, VectorSize);
					Load_VT(0, 1);
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
				});
			}
		}
		public void vslt() { throw (new NotImplementedException("")); }
		public void vsge() { throw (new NotImplementedException("")); }

		// ROTate
		public void vrot()
		{
			var VectorSize = Instruction.ONE_TWO;
			uint imm5 = Instruction.IMM5;
			if (VectorSize == 1) throw(new NotImplementedException());

			//uint imm5 = instruction.IMM5;
			int SinIndex = (int)((imm5 >> 2) & 3);
			int CosIndex = (int)((imm5 >> 0) & 3);
			bool NegateSin = ((imm5 & 16) != 0);

			foreach (var Index in XRange(VectorSize))
			{
				Save_VD(Index, VectorSize, () =>
				{
					if (SinIndex == CosIndex)
					{
						// Angle [-1, +1]
						Load_VS(0, 1); // Angle
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)(Math.PI / 2.0f));
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
						MipsMethodEmiter.CallMethod(typeof(MathFloat), "Sin");
						if (NegateSin)
						{
							MipsMethodEmiter.ILGenerator.Emit(OpCodes.Neg);
						}
					}
					else
					{
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)(0.0f));
					}
				});
			}

			if (SinIndex != CosIndex)
			{
				Save_VD(SinIndex, VectorSize, () =>
				{
					// Angle [-1, +1]
					Load_VS(0, 1); // Angle
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)(Math.PI / 2.0f));
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
					MipsMethodEmiter.CallMethod(typeof(MathFloat), "Sin");
					if (NegateSin)
					{
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Neg);
					}
				});
			}

			Save_VD(CosIndex, VectorSize, () =>
			{
				// Angle [-1, +1]
				Load_VS(0, 1); // Angle
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)(Math.PI / 2.0f));
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
				MipsMethodEmiter.CallMethod(typeof(MathFloat), "Cos");
			});
		}

		// Vfpu ZERO/ONE
		public void vzero()
		{
			VectorOperationSaveVd((Index, Load) => { MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 0.0f); });
		}
		public void vone()
		{
			VectorOperationSaveVd((Index, Load) => { MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 1.0f); });
		}

		// Vfpu MOVe/SiGN/Reverse SQuare root/COSine/Arc SINe/LOG2
		// @CHECK
		public void vmov()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(1);
			});
		}
		public void vabs() { throw (new NotImplementedException("")); }
		public void vneg() { throw (new NotImplementedException("")); }
		public void vocp() {
			VectorOperationSaveVd((Index, Load) => {
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 1.0f);
				Load(1);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Sub);
			});
		}
		public void vsgn() { throw (new NotImplementedException("")); }
		public void vrcp()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 1.0f);
				Load(1);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Div);
			});
		}

		// OP_V_INTERNAL_IN_N!(1, "1.0f / sqrt(v)");
		public void vrsq()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 1.0f);
				Load(1);
				MipsMethodEmiter.CallMethod(typeof(MathFloat), "Sqrt");
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Div);
			});
		}
		public void vsin() {
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(1);
				MipsMethodEmiter.CallMethod(typeof(MathFloat), "SinV1");
			});
		}
		public void vcos() {
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(1);
				MipsMethodEmiter.CallMethod(typeof(MathFloat), "CosV1");
			});
		}
		public void vexp2() { throw (new NotImplementedException("")); }
		public void vlog2() { throw (new NotImplementedException("")); }
		public void vsqrt() {
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(1);
				MipsMethodEmiter.CallMethod(typeof(MathFloat), "Sqrt");
			});
		}
		public void vasin() { throw (new NotImplementedException("")); }
		public void vnrcp() { throw (new NotImplementedException("")); }
		public void vnsin() { throw (new NotImplementedException("")); }
		public void vrexp2() { throw (new NotImplementedException("")); }

		public void vsat0() { throw (new NotImplementedException("")); }
		public void vsat1() { throw (new NotImplementedException("")); }

		// Vfpu ConSTant
		public void vcst()
		{
			var VectorSize = Instruction.ONE_TWO;
			float FloatConstant = (Instruction.IMM5 >= 0 && Instruction.IMM5 < VfpuConstants.Length) ? VfpuConstants[Instruction.IMM5] : 0.0f;

			foreach (var Index in XRange(VectorSize))
			{
				//Console.Error.WriteLine("{0}/{1}", Index, VectorSize);
				Save_VD(Index, VectorSize, () =>
				{
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)FloatConstant);
				}
				//, Debug: true
				);
			}
			//_call_debug_vfpu();
		}

		// Vfpu Matrix MULtiplication
		// @FIX!!!
		public void vmmul()
		{
			var VectorSize = Instruction.ONE_TWO;
			//var MatrixRank = VectorSize;

			foreach (var IndexI in XRange(VectorSize))
			{
				foreach (var IndexJ in XRange(VectorSize))
				{
					// VD[j] += VS[k] * VT[k];
					//
					//VfpuSave_Register((uint)(Instruction.VD + IndexI), IndexJ, VectorSize, PrefixDestination, () =>
					Save_VD(IndexJ, VectorSize, IndexI, () =>
					{
						MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, 0.0f);
						foreach (var IndexK in XRange(VectorSize))
						{
							Load_VT(IndexK, VectorSize, IndexI);
							Load_VS(IndexK, VectorSize, IndexJ);
							MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
							MipsMethodEmiter.ILGenerator.Emit(OpCodes.Add);
						}
					});
				}
			}

			/*
			foreach (i; 0..vsize) {
				loadVt(vsize, instruction.VT + i);
				foreach (j; 0..vsize) {
					loadVs(vsize, instruction.VS + j);
					VD[j] = 0.0f; foreach (k; 0..vsize) VD[j] += VS[k] * VT[k];
				}
				saveVd(vsize, instruction.VD + i);
			}
			*/
			//throw (new NotImplementedException(""));
		}

		// -
		public void vhdp() { throw (new NotImplementedException("")); }
		public void vcrs_t() { throw (new NotImplementedException("")); }
		public void vcrsp_t() { throw (new NotImplementedException("")); }

		// Vfpu Integer to(2) Color
		public void vi2c() { throw (new NotImplementedException("")); }
		public void vi2uc() { throw (new NotImplementedException("")); }

		// -
		public void vtfm2() { throw (new NotImplementedException("")); }
		public void vtfm3() { throw (new NotImplementedException("")); }
		public void vtfm4() { throw (new NotImplementedException("")); }

		public void vhtfm2() { throw (new NotImplementedException("")); }
		public void vhtfm3() { throw (new NotImplementedException("")); }
		public void vhtfm4() { throw (new NotImplementedException("")); }

		public void vsrt3() { throw (new NotImplementedException("")); }

		public void vfad() { throw (new NotImplementedException("")); }

		// Vfpu MINimum/MAXium/ADD/SUB/DIV/MUL
		public void vmin() { throw (new NotImplementedException("")); }
		public void vmax() { throw (new NotImplementedException("")); }

		/// <summary>
		/// 	+----------------------+--------------+----+--------------+---+--------------+
		///     |31                 23 | 22        16 | 15 | 14         8 | 7 | 6         0  |
		///     +----------------------+--------------+----+--------------+---+--------------+
		///     |  opcode 0x60000000   | vfpu_rt[6-0] |    | vfpu_rs[6-0] |   | vfpu_rd[6-0] |
		///     +----------------------+--------------+----+--------------+---+--------------+
		///     
		///     VectorAdd.Single/Pair/Triple/Quad
		///     
		///     vadd.s %vfpu_rd, %vfpu_rs, %vfpu_rt   ; Add Single
		///     vadd.p %vfpu_rd, %vfpu_rs, %vfpu_rt   ; Add Pair
		///     vadd.t %vfpu_rd, %vfpu_rs, %vfpu_rt   ; Add Triple
		///     vadd.q %vfpu_rd, %vfpu_rs, %vfpu_rt   ; Add Quad
		///     
		///     %vfpu_rt:	VFPU Vector Source Register ([s|p|t|q]reg 0..127)
		///     %vfpu_rs:	VFPU Vector Source Register ([s|p|t|q]reg 0..127)
		///     %vfpu_rd:	VFPU Vector Destination Register ([s|p|t|q]reg 0..127)
		///     
		///     vfpu_regs[%vfpu_rd] <- vfpu_regs[%vfpu_rs] + vfpu_regs[%vfpu_rt]
		/// </summary>
		public void vadd()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(2);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Add);
			});
		}
		public void vsub()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(2);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Sub);
			});
		}
		public void vdiv()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(2);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Div);
			});
		}

		/*
		static public float _vmul(float a, float b)
		{
			Console.Error.WriteLine("{0} * {1} = {2}", a, b, a * b);
			return a * b;
		}
		*/

		public void vmul()
		{
			VectorOperationSaveVd((Index, Load) =>
			{
				Load(2);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
				//MipsMethodEmiter.CallMethod(this.GetType(), "_vmul");
			});
			/*
			_VectorOperation2Registers(Index =>
			{
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Mul);
			});
			*/
		}

		void _vidt_x(uint VectorSize, uint Register)
		{
			uint IndexOne = BitUtils.Extract(Register, 0, 2);
			foreach (var Index in XRange(VectorSize))
			{
				VfpuSave_Register(Register, Index, VectorSize, PrefixNone, () =>
				{
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (Index == IndexOne) ? 1.0f : 0.0f);
				}
				//, Debug: true
				);
			}
		}

		void _vzero_x(uint VectorSize, uint Register)
		{
			uint IndexOne = BitUtils.Extract(Register, 0, 2);
			foreach (var Index in XRange(VectorSize))
			{
				VfpuSave_Register(Register, Index, VectorSize, PrefixNone, () =>
				{
					MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4,  0.0f);
				}
					//, Debug: true
				);
			}
		}

		// Vfpu (Matrix) IDenTity
		public void vidt()
		{
			var MatrixSize = Instruction.ONE_TWO;
			_vidt_x(MatrixSize, (uint)(Instruction.VD));
			//throw (new NotImplementedException(""));
		}
		public void vmidt()
		{
			var MatrixSize = Instruction.ONE_TWO;

			foreach (var Index in XRange(MatrixSize))
			{
				_vidt_x(MatrixSize, (uint)(Instruction.VD + Index));
			}
		}

		// Vfpu load Integer IMmediate
		public void viim()
		{
			Save_VT(0, 1, () =>
			{
				//MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)Instruction.IMM);
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, (float)Instruction.IMMU);
			});
		}

		public void vmmov()
		{
			var VectorSize = Instruction.ONE_TWO;

			foreach (var y in XRange(VectorSize))
			{
				foreach (var x in XRange(VectorSize))
				{
					Save_VD(x, VectorSize, y, () =>
					{
						Load_VS(x, VectorSize, y);
					});
				}
			}
		}
		public void vmzero()
		{
			var MatrixSize = Instruction.ONE_TWO;

			foreach (var Index in XRange(MatrixSize))
			{
				_vzero_x(MatrixSize, (uint)(Instruction.VD + Index));
			}

			//throw (new NotImplementedException(""));
		}
		public void vmone() { throw (new NotImplementedException("")); }

		public void vnop() {
			//throw (new NotImplementedException(""));
		}
		public void vsync() { throw (new NotImplementedException("")); }
		public void vflush() { throw (new NotImplementedException("")); }

		public void vpfxd()
		{
			PrefixDestination.EnableAndSetValueAndPc(Instruction.Value, PC);
		}

		public void vpfxs()
		{
			PrefixSource.EnableAndSetValueAndPc(Instruction.Value, PC);
		}

		public void vpfxt()
		{
			PrefixTarget.EnableAndSetValueAndPc(Instruction.Value, PC);
		}

		public void vdet() { throw (new NotImplementedException("")); }

		public void vrnds() { throw (new NotImplementedException("")); }
		public void vrndi() { throw (new NotImplementedException("")); }
		public void vrndf1() { throw (new NotImplementedException("")); }
		public void vrndf2() { throw (new NotImplementedException("")); }

		public void vcmp() { throw (new NotImplementedException("")); }

		public void vcmovf() { throw (new NotImplementedException("")); }
		public void vcmovt() { throw (new NotImplementedException("")); }

		////////////////////////////
		/// Not implemented yet!
		////////////////////////////

		public void vavg() { throw (new NotImplementedException("")); }
		public void vf2id() { throw (new NotImplementedException("")); }
		public void vf2in() { throw (new NotImplementedException("")); }
		public void vf2iu() { throw (new NotImplementedException("")); }
		public void vf2iz() { throw (new NotImplementedException("")); }
		public void vi2f() { throw (new NotImplementedException("")); }

		public void vscmp() { throw (new NotImplementedException("")); }
		public void vmscl() { throw (new NotImplementedException("")); }

		public void vt4444_q() { throw (new NotImplementedException("")); }
		public void vt5551_q() { throw (new NotImplementedException("")); }
		public void vt5650_q() { throw (new NotImplementedException("")); }

		public void vmfvc() { throw (new NotImplementedException("")); }
		public void vmtvc() { throw (new NotImplementedException("")); }

		public void mfvme() { throw (new NotImplementedException("")); }
		public void mtvme() { throw (new NotImplementedException("")); }

		public void sv_s()
		{
			uint VT = Instruction.VT5 | (Instruction.VT2 << 5);
			uint Column = (VT >> 5) & 3;
			uint Matrix = (VT >> 2) & 7;
			uint Row = (VT >> 0) & 3;

			_load_memory_imm14_index(0);
			{
				LoadVprFieldPtr(CalcVprRegisterIndex(Matrix, Column, Row));
				MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldind_R4);
			}
			MipsMethodEmiter.ILGenerator.Emit(OpCodes.Stind_R4);
		}

		/// <summary>
		/// ID("vfim",        VM("110111:11:1:vt:imm16"), "%xs, %vh",      ADDR_TYPE_NONE, INSTR_TYPE_PSP),
		/// </summary>
		public void vfim()
		{
			_VfpuLoadVectorWithIndexPointer(Instruction.VT, 0, 1);
			MipsMethodEmiter.ILGenerator.Emit(OpCodes.Ldc_R4, Instruction.IMM_HF);
			MipsMethodEmiter.ILGenerator.Emit(OpCodes.Stind_R4);

			//_call_debug_vfpu();
		}

		public void vbfy1() { throw (new NotImplementedException("")); }
		public void vbfy2() { throw (new NotImplementedException("")); }

		public void vf2h() { throw (new NotImplementedException("")); }
		public void vh2f() { throw (new NotImplementedException("")); }

		public void vi2s() { throw (new NotImplementedException("")); }
		public void vi2us() { throw (new NotImplementedException("")); }

		public void vlgb() { throw (new NotImplementedException("")); }
		public void vqmul() { throw (new NotImplementedException("")); }
		public void vs2i() { throw (new NotImplementedException("")); }
		public void vsbn() { throw (new NotImplementedException("")); }

		public void vsbz() { throw (new NotImplementedException("")); }
		public void vsocp() { throw (new NotImplementedException("")); }
		public void vsrt1() { throw (new NotImplementedException("")); }
		public void vsrt2() { throw (new NotImplementedException("")); }
		public void vsrt4() { throw (new NotImplementedException("")); }
		public void vus2i() { throw (new NotImplementedException("")); }

		public void vwbn() { throw (new NotImplementedException("")); }
		//public void vwb_q() { throw(new NotImplementedException()); }
	}
	/*
	union {
		struct { VfpuPrefix vfpu_prefix_s, vfpu_prefix_t, vfpu_prefix_d; }
		struct { VfpuPrefix[3] vfpu_prefixes; }
	}
	*/
}