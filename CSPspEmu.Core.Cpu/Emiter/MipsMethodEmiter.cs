﻿#define USE_DYNAMIC_METHOD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Reflection;
using CSPspEmu.Core.Memory;

namespace CSPspEmu.Core.Cpu.Emiter
{
	/// <summary>
	/// <see cref="http://www.mrc.uidaho.edu/mrc/people/jff/digital/MIPSir.html"/>
	/// </summary>
	unsafe public class MipsMethodEmiter
	{
#if USE_DYNAMIC_METHOD
		//protected DynamicMethod DynamicMethod;
		public DynamicMethod DynamicMethod;
#else
		public TypeBuilder TypeBuilder;
		protected MethodBuilder MethodBuilder;
#endif
		public ILGenerator ILGenerator;
		protected String MethodName;
		//public CpuThreadState CpuThreadState;
		public CpuProcessor Processor;
		
		//static protected FieldInfo Field_GPR_Ptr = typeof(Processor).GetField("GPR_Ptr");
		//static protected FieldInfo Field_FPR_Ptr = typeof(Processor).GetField("FPR_Ptr");
		static protected FieldInfo Field_BranchFlag = typeof(CpuThreadState).GetField("BranchFlag");
		static protected FieldInfo Field_PC = typeof(CpuThreadState).GetField("PC");
		static protected FieldInfo Field_LO = typeof(CpuThreadState).GetField("LO");
		static protected FieldInfo Field_HI = typeof(CpuThreadState).GetField("HI");
		static protected FieldInfo Field_StepInstructionCount = typeof(CpuThreadState).GetField("StepInstructionCount");
		static public MethodInfo Method_Syscall = ((Action<int>)(new CpuThreadState(null).Syscall)).Method;
		static private ulong UniqueCounter = 0;

		static protected bool InitializedOnce = false;
		static protected FieldInfo[] Field_GPRList;
		static protected FieldInfo[] Field_FPRList;

		readonly public Dictionary<string, uint> InstructionStats = new Dictionary<string, uint>();

		/*
		static public MipsMethodEmiter()
		{
		}
		*/

		public void _getmemptr(Action Action, bool Safe = false, String ErrorDescription = "", bool CanBeNull = true)
		{
			if (Safe)
			{
				ILGenerator.Emit(OpCodes.Ldarg_0);
				{
					Action();
				}
				//ILGenerator.Emit(OpCodes.Call, typeof(CpuThreadState).GetMethod("GetMemoryPtrSafe"));

				ILGenerator.Emit(OpCodes.Ldstr, ErrorDescription);
				ILGenerator.Emit(OpCodes.Ldc_I4, CanBeNull ? 1 : 0);
				ILGenerator.Emit(OpCodes.Call, typeof(CpuThreadState).GetMethod("GetMemoryPtrSafeWithError"));
			}
			else if (Processor.Memory is FastPspMemory)
			{
				ILGenerator.Emit(OpCodes.Ldc_I4, (int)((FastPspMemory)Processor.Memory).Base);
				{
					Action();
				}
				ILGenerator.Emit(OpCodes.Ldc_I4, (int)PspMemory.MemoryMask);
				ILGenerator.Emit(OpCodes.And);
				ILGenerator.Emit(OpCodes.Add);
			}
			else
			{
				ILGenerator.Emit(OpCodes.Ldarg_0);
				{
					Action();
				}
#if true
				//ILGenerator.Emit(OpCodes.Call, typeof(CpuThreadState).GetMethod("GetMemoryPtr"));
				ILGenerator.Emit(OpCodes.Call, typeof(CpuThreadState).GetMethod("GetMemoryPtrNotNull"));
#else
				ILGenerator.Emit(OpCodes.Ldstr, ErrorDescription);
				ILGenerator.Emit(OpCodes.Ldc_I4, CanBeNull ? 1 : 0);
				ILGenerator.Emit(OpCodes.Call, typeof(CpuThreadState).GetMethod("GetMemoryPtrSafeWithError"));
#endif
			}
		}

		public MipsMethodEmiter(MipsEmiter MipsEmiter, CpuProcessor Processor, uint PC)
		{
			this.Processor = Processor;

			if (!InitializedOnce)
			{
				Field_GPRList = new FieldInfo[32];
				Field_FPRList = new FieldInfo[32];
				for (int n = 0; n < 32; n++)
				{
					Field_GPRList[n] = typeof(CpuThreadState).GetField("GPR" + n);
					Field_FPRList[n] = typeof(CpuThreadState).GetField("FPR" + n);
				}

				InitializedOnce = true;
			}

			UniqueCounter++;

#if USE_DYNAMIC_METHOD
			DynamicMethod = new DynamicMethod(
				String.Format("DynamicMethod_0x{0:X}", PC),
				typeof(void),
				new Type[] { typeof(CpuThreadState) },
				Assembly.GetExecutingAssembly().ManifestModule
			);
			ILGenerator = DynamicMethod.GetILGenerator();
#else
			TypeBuilder = MipsEmiter.ModuleBuilder.DefineType("type" + UniqueCounter, TypeAttributes.Sealed | TypeAttributes.Public);
			MethodBuilder = TypeBuilder.DefineMethod(
				name: MethodName = "method" + UniqueCounter,
				attributes: MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.UnmanagedExport | MethodAttributes.Final,
				returnType: typeof(void),
				parameterTypes: new Type[] { typeof(CpuThreadState) }
			);
			ILGenerator = MethodBuilder.GetILGenerator();
#endif
		}

		public Action<CpuThreadState> CreateDelegate()
		{
			ILGenerator.Emit(OpCodes.Ret);

#if USE_DYNAMIC_METHOD
			return (Action<CpuThreadState>)DynamicMethod.CreateDelegate(typeof(Action<CpuThreadState>));
#else
			var Type = TypeBuilder.CreateType();
			return (Action<CpuThreadState>)Delegate.CreateDelegate(typeof(Action<CpuThreadState>), Type.GetMethod(MethodName));
#endif
		}

		MethodBody GetMethodBody()
		{
			return DynamicMethod.GetMethodBody();
		}

		public void LoadFieldPtr(FieldInfo FieldInfo)
		{
			ILGenerator.Emit(OpCodes.Ldarg_0);
			if (FieldInfo == null) throw (new InvalidCastException("FieldInfo == null"));
			if (FieldInfo.DeclaringType != typeof(CpuThreadState)) throw (new InvalidCastException("FieldInfo.DeclaringType != CpuThreadState"));
			ILGenerator.Emit(OpCodes.Ldflda, FieldInfo);
		}

		protected void LoadGPR_Ptr(int R) { LoadFieldPtr(Field_GPRList[R]); }
		protected void LoadFPR_Ptr(int R) { LoadFieldPtr(Field_FPRList[R]); }
		protected void LoadPC_Ptr() { LoadFieldPtr(Field_PC); }
		protected void LoadHI_Ptr() { LoadFieldPtr(Field_HI); }
		protected void LoadLO_Ptr() { LoadFieldPtr(Field_LO); }
		protected void LoadStepInstructionCountPtr() { ILGenerator.Emit(OpCodes.Ldarg_0); ILGenerator.Emit(OpCodes.Ldflda, Field_StepInstructionCount); }

		public void LoadStepInstructionCount()
		{
			LoadStepInstructionCountPtr();
			ILGenerator.Emit(OpCodes.Ldind_I4);
		}

		public void SaveStepInstructionCount(Action Action)
		{
			LoadStepInstructionCountPtr();
			Action();
			ILGenerator.Emit(OpCodes.Stind_I4);
		}

		public void LoadBranchFlag()
		{
			ILGenerator.Emit(OpCodes.Ldarg_0);
			ILGenerator.Emit(OpCodes.Ldfld, Field_BranchFlag);
		}

		public void StoreBranchFlag(Action Action)
		{
			ILGenerator.Emit(OpCodes.Ldarg_0);
			Action();
			ILGenerator.Emit(OpCodes.Stfld, Field_BranchFlag);
		}

		public void SaveFieldI4(FieldInfo FieldInfo, Action Action)
		{
			LoadFieldPtr(FieldInfo); Action(); ILGenerator.Emit(OpCodes.Stind_I4);
		}

		public void SaveFieldI8(FieldInfo FieldInfo, Action Action)
		{
			LoadFieldPtr(FieldInfo); Action(); ILGenerator.Emit(OpCodes.Stind_I8);
		}

		public void SaveFieldR4(FieldInfo FieldInfo, Action Action)
		{
			LoadFieldPtr(FieldInfo); Action(); ILGenerator.Emit(OpCodes.Stind_R4);
		}

		public void SavePC(Action Action) { SaveFieldI4(Field_PC, Action); }
		public void SavePC(uint PC) { SaveFieldI4(Field_PC, () => { ILGenerator.Emit(OpCodes.Ldc_I4, PC); }); }
		public void SaveLO(Action Action) { SaveFieldI4(Field_LO, Action); }
		public void SaveHI(Action Action) { SaveFieldI4(Field_HI, Action); }

		public void SaveHI_LO(Action Action) {
			LoadFieldPtr(Field_LO);
			Action();
			ILGenerator.Emit(OpCodes.Stind_I8);
		}

		public void LoadHI_LO()
		{
			LoadFieldPtr(Field_LO);
			ILGenerator.Emit(OpCodes.Ldind_I8);
		}

		public void SaveGPR_F(int R, Action Action) {
			if (R != 0) SaveFieldR4(Field_GPRList[R], Action);
		}
		public void SaveGPR(int R, Action Action) { if (R != 0) SaveFieldI4(Field_GPRList[R], Action); }
		public void SaveGPRLong(int R, Action Action) { if (R != 0) SaveFieldI8(Field_GPRList[R], Action); }
		public void SaveFPR(int R, Action Action)
		{
			LoadFPR_Ptr(R);
			{
				Action();
			}
			ILGenerator.Emit(OpCodes.Stind_R4);
		}
		public void SaveFPR_I(int R, Action Action)
		{
			LoadFPR_Ptr(R);
			{
				Action();
			}
			ILGenerator.Emit(OpCodes.Stind_I4);
		}

		static public bool _LoadFcr31CC(CpuThreadState CpuThreadState)
		{
			return CpuThreadState.Fcr31.CC;
		}

		public void LoadFCR31_CC()
		{
			ILGenerator.Emit(OpCodes.Ldarg_0);
			ILGenerator.Emit(OpCodes.Call, typeof(MipsMethodEmiter).GetMethod("_LoadFcr31CC"));
		}

		public void LoadGPR_Signed(int R)
		{
			if (R == 0)
			{
				ILGenerator.Emit(OpCodes.Ldc_I4_0);
			}
			else
			{
				LoadGPR_Ptr(R);
				ILGenerator.Emit(OpCodes.Ldind_I4);
			}
		}

		public void LoadGPR_Unsigned(int R)
		{
			if (R == 0)
			{
				ILGenerator.Emit(OpCodes.Ldc_I4_0);
				ILGenerator.Emit(OpCodes.Conv_U4);
			}
			else
			{
				LoadGPR_Ptr(R);
				ILGenerator.Emit(OpCodes.Ldind_U4);
			}
		}

		public void LoadGPRLong_Signed(int R)
		{
			LoadGPR_Ptr(R);
			ILGenerator.Emit(OpCodes.Ldind_I8);
		}


		public void LoadFPR(int R)
		{
			LoadFPR_Ptr(R);
			ILGenerator.Emit(OpCodes.Ldind_R4);
		}

		public void LoadFPR_I(int R)
		{
			LoadFPR_Ptr(R);
			ILGenerator.Emit(OpCodes.Ldind_U4);
		}

		public void LoadPC()
		{
			LoadPC_Ptr();
			ILGenerator.Emit(OpCodes.Ldind_U4);
		}

		public void LoadLO()
		{
			LoadLO_Ptr();
			ILGenerator.Emit(OpCodes.Ldind_U4);
		}

		public void LoadHI()
		{
			LoadHI_Ptr();
			ILGenerator.Emit(OpCodes.Ldind_U4);
		}

		public void OP_3REG_Unsigned(int RD, int RS, int RT, params OpCode[] OpCodes)
		{
			SaveGPR(RD, () =>
			{
				LoadGPR_Unsigned(RS);
				LoadGPR_Unsigned(RT);
				foreach (var OpCode in OpCodes)
				{
					ILGenerator.Emit(OpCode);
				}
			});
		}

		public void OP_3REG_Signed(int RD, int RS, int RT, params OpCode[] OpCodes)
		{
			SaveGPR(RD, () =>
			{
				LoadGPR_Signed(RS);
				LoadGPR_Signed(RT);
				foreach (var OpCode in OpCodes)
				{
					ILGenerator.Emit(OpCode);
				}
			});
		}

		public void OP_3REG_F(int FD, int FS, int FT, OpCode OpCode)
		{
			SaveFPR(FD, () =>
			{
				LoadFPR(FS);
				LoadFPR(FT);
				ILGenerator.Emit(OpCode);
			});
		}

		public void OP_2REG_IMM_F(int FD, int FS, float Immediate, OpCode OpCode)
		{
			SaveFPR(FD, () =>
			{
				LoadFPR(FS);
				ILGenerator.Emit(OpCodes.Ldc_R4, Immediate);
				ILGenerator.Emit(OpCode);
			});
		}

		public void OP_2REG_F(int FD, int FS, Action Action)
		{
			SaveFPR(FD, () =>
			{
				LoadFPR(FS);
				Action();
			});
		}

		public void OP_2REG_IMM_Signed(int RT, int RS, short Immediate, Action Action)
		{
			SaveGPR(RT, () =>
			{
				LoadGPR_Unsigned(RS);
				ILGenerator.Emit(OpCodes.Conv_I4);
				ILGenerator.Emit(OpCodes.Ldc_I4, (int)Immediate);
				ILGenerator.Emit(OpCodes.Conv_I4);
				Action();
			});
		}

		public void OP_2REG_IMM_Unsigned(int RT, int RS, uint Immediate, Action Action)
		{
			SaveGPR(RT, () =>
			{
				LoadGPR_Unsigned(RS);
				ILGenerator.Emit(OpCodes.Conv_U4);
				ILGenerator.Emit(OpCodes.Ldc_I4, (uint)Immediate);
				ILGenerator.Emit(OpCodes.Conv_U4);
				Action();
			});
		}

		public void OP_2REG_IMM_Signed(int RT, int RS, short Immediate, OpCode OpCode)
		{
			OP_2REG_IMM_Signed(RT, RS, Immediate, () =>
			{
				ILGenerator.Emit(OpCode);
			});
		}

		public void OP_2REG_IMM_Unsigned(int RT, int RS, uint Immediate, OpCode OpCode)
		{
			OP_2REG_IMM_Unsigned(RT, RS, Immediate, () =>
			{
				ILGenerator.Emit(OpCode);
			});
		}

		public void SET(int RT, uint Value)
		{
			SaveGPR(RT, () =>
			{
				ILGenerator.Emit(OpCodes.Ldc_I4, Value);
			});
		}

		public void SET_REG(int RT, int RS)
		{
			SaveGPR(RT, () => { LoadGPR_Unsigned(RS); });
		}

		public void CallMethod(MethodInfo MethodInfo)
		{
			ILGenerator.Emit(OpCodes.Call, MethodInfo);
		}

		public void CallMethod(Delegate Delegate)
		{
			CallMethod(Delegate.Method);
		}

		private void CallMethod(Type Class, String MethodName)
		{
			var MethodInfo = Class.GetMethod(MethodName);
			if (MethodInfo == null) throw (new KeyNotFoundException(String.Format("Can't find {0}::{1}", Class, MethodName)));
			CallMethod(MethodInfo);
		}

		public void CallMethodWithCpuThreadStateAsFirstArgument(Type Class, String MethodName)
		{
			ILGenerator.Emit(OpCodes.Ldarg_0);
			CallMethod(Class, MethodName);
		}

	}
}
