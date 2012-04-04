﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPspEmu.Hle.Formats
{
	unsafe public partial class EncryptedPrx
	{
		static public readonly TAG_INFO2[] g_tagInfo2 = new TAG_INFO2[]
		{
			new TAG_INFO2() { tag = 0x380228F0, key = keys620_5v, code = 0x5A }, // -- PSPgo PSPgo 6.XX vshmain
			new TAG_INFO2() { tag = 0x4C942AF0, key = keys620_5k, code = 0x43 }, // PSPgo 6.XX
			new TAG_INFO2() { tag = 0x4C9428F0, key = keys620_5 , code = 0x43 }, // PSPgo
			new TAG_INFO2() { tag = 0x4C9429F0, key = keys570_5k, code = 0x43 }, // PSPgo 5.70
			new TAG_INFO2() { tag = 0x4C941DF0, key = keys620_1, code = 0x43 }, // -- 6.00-6.20
			new TAG_INFO2() { tag = 0x4C941CF0, key = keys620_0, code = 0x43 },
			new TAG_INFO2() { tag = 0x457B1EF0, key = keys620_3, code = 0x5B }, // pops_04g.prx
			new TAG_INFO2() { tag = 0x457B0BF0, key = keys600_u1_457B0BF0, code = 0x5B }, // -- 5.55 user modules
			new TAG_INFO2() { tag = 0x457B0CF0, key = keys600_u1_457B0CF0, code = 0x5B },
			new TAG_INFO2() { tag = 0x4C9419F0, key = keys500_1, code = 0x43 }, // -- 5.00 - 5.50
			new TAG_INFO2() { tag = 0x4C9418F0, key = keys500_0, code = 0x43 },
			new TAG_INFO2() { tag = 0x4C941FF0, key = keys500_2, code = 0x43 },
			new TAG_INFO2() { tag = 0x4C9417F0, key = keys500_1, code = 0x43 },
			new TAG_INFO2() { tag = 0x4C9416F0, key = keys500_0, code = 0x43 },
			new TAG_INFO2() { tag = 0x4C9414F0, key = keys390_0, code = 0x43 }, // -- 3.90 keys
			new TAG_INFO2() { tag = 0x4C9415F0, key = keys390_1, code = 0x43 },
			new TAG_INFO2() { tag = 0xD82310F0, key = keys02G_E, code = 0x51 }, // -- ipl and file tables
 			new TAG_INFO2() { tag = 0xD8231EF0, key = keys03G_E, code = 0x51 },
			new TAG_INFO2() { tag = 0xD82328F0, key = keys05G_E, code = 0x51 },
			new TAG_INFO2() { tag = 0x4C9412F0, key = keys370_0, code = 0x43 }, // -- 3.60-3.7X keys
			new TAG_INFO2() { tag = 0x4C9413F0, key = keys370_1, code = 0x43 },
			new TAG_INFO2() { tag = 0x457B10F0, key = keys370_2, code = 0x5B },
			new TAG_INFO2() { tag = 0x4C940DF0, key = keys360_0, code = 0x43 },
			new TAG_INFO2() { tag = 0x4C9410F0, key = keys360_1, code = 0x43 },
			new TAG_INFO2() { tag = 0x4C940BF0, key = keys330_0, code = 0x43 }, // -- 3.30-3.51
			new TAG_INFO2() { tag = 0x457B0AF0, key = keys330_1, code = 0x5B }, 
			new TAG_INFO2() { tag = 0x38020AF0, key = keys330_2, code = 0x5A }, 
			new TAG_INFO2() { tag = 0x4C940AF0, key = keys330_3, code = 0x43 }, 
			new TAG_INFO2() { tag = 0x4C940CF0, key = keys330_4, code = 0x43 }, 
			new TAG_INFO2() { tag = 0xcfef09f0, key = keys310_0, code = 0x62 }, // -- 3.10
			new TAG_INFO2() { tag = 0x457b08f0, key = keys310_1, code = 0x5B },
			new TAG_INFO2() { tag = 0x380208F0, key = keys310_2, code = 0x5A },
			new TAG_INFO2() { tag = 0xcfef08f0, key = keys310_3, code = 0x62 },
			new TAG_INFO2() { tag = 0xCFEF07F0, key = keys303_0, code = 0x62 }, // -- 2.60-3.03
			new TAG_INFO2() { tag = 0xCFEF06F0, key = keys300_0, code = 0x62 },
			new TAG_INFO2() { tag = 0x457B06F0, key = keys300_1, code = 0x5B },
			new TAG_INFO2() { tag = 0x380206F0, key = keys300_2, code = 0x5A },
			new TAG_INFO2() { tag = 0xCFEF05F0, key = keys280_0, code = 0x62 },
			new TAG_INFO2() { tag = 0x457B05F0, key = keys280_1, code = 0x5B },
			new TAG_INFO2() { tag = 0x380205F0, key = keys280_2, code = 0x5A },
			new TAG_INFO2() { tag = 0x16D59E03, key = keys260_0, code = 0x62 },
			new TAG_INFO2() { tag = 0x76202403, key = keys260_1, code = 0x5B },
			new TAG_INFO2() { tag = 0x0F037303, key = keys260_2, code = 0x5A },
			new TAG_INFO2() { tag = 0x457B28F0, key = keys620_e, code = 0x5B },	// -- misc ?
			new TAG_INFO2() { tag = 0xADF305F0, key = demokeys_280, code = 0x60 }, 	// 2.80 demos data.psp
			new TAG_INFO2() { tag = 0xADF306F0, key = demokeys_3XX_1, code = 0x60 },	// 3.XX demos 1
			new TAG_INFO2() { tag = 0xADF308F0, key = demokeys_3XX_2, code = 0x60 },	// 3.XX demos 2
			new TAG_INFO2() { tag = 0x8004FD03, key = ebootbin_271_new, code = 0x5D },	// 2.71 eboot.bin
			new TAG_INFO2() { tag = 0xD91605F0, key = ebootbin_280_new, code = 0x5D },	// 2.80 eboot.bin
			new TAG_INFO2() { tag = 0xD91606F0, key = ebootbin_300_new, code = 0x5D },	// 3.00 eboot.bin
			new TAG_INFO2() { tag = 0xD91608F0, key = ebootbin_310_new, code = 0x5D },	// 3.10 eboot.bin
			new TAG_INFO2() { tag = 0xD91609F0, key = key_D91609F0, code = 0x5D },	// 5.00 eboot.bin
			new TAG_INFO2() { tag = 0x2E5E10F0, key = key_2E5E10F0, code = 0x48 },	// 6.XX eboot.bin
			new TAG_INFO2() { tag = 0x2E5E12F0, key = key_2E5E12F0, code = 0x48 },	// 6.XX eboot.bin
			new TAG_INFO2() { tag = 0x2E5E12F0, key = key_2E5E13F0, code = 0x48 },	// 6.XX eboot.bin
			new TAG_INFO2() { tag = 0xD9160AF0, key = key_D9160AF0, code = 0x5D },
			new TAG_INFO2() { tag = 0xD9160BF0, key = key_D9160BF0, code = 0x5D },
			new TAG_INFO2() { tag = 0xD91611F0, key = key_D91611F0, code = 0x5D },	
			new TAG_INFO2() { tag = 0xD91612F0, key = key_D91612F0, code = 0x5D },	
			new TAG_INFO2() { tag = 0xD91613F0, key = key_D91613F0, code = 0x5D },
			new TAG_INFO2() { tag = 0x0A35EA03, key = gameshare_260_271, code = 0x5E }, // 2.60-2.71 gameshare
			new TAG_INFO2() { tag = 0x7B0505F0, key = gameshare_280, code = 0x5E },	 // 2.80 gameshare
			new TAG_INFO2() { tag = 0x7B0506F0, key = gameshare_300, code = 0x5E },	 // 3.00 gameshare
			new TAG_INFO2() { tag = 0x7B0508F0, key = gameshare_310, code = 0x5E },	 // 3.10+ gameshare
			new TAG_INFO2() { tag = 0x279D08F0, key = oneseg_310, code = 0x61 },	 // 3.10 1SEG
			new TAG_INFO2() { tag = 0x279D06F0, key = oneseg_300, code = 0x61 },	 // 3.00 1SEG
			new TAG_INFO2() { tag = 0x279D05F0, key = oneseg_280, code = 0x61 },	 // 2.80 1SEG
			new TAG_INFO2() { tag = 0xD66DF703, key = oneseg_260_271, code = 0x61 },	 // 2.60-2.71 1SEG
			new TAG_INFO2() { tag = 0x279D10F0, key = oneseg_slim, code = 0x61 },	 // 02g 1SEG
			new TAG_INFO2() { tag = 0x3C2A08F0, key = ms_app_main, code = 0x67 },	 // 1seg ms_application_main.prx
		};

		/* kernel modules 2.60-2.71 */
		static public readonly byte[] keys260_0 = new byte[0x10] {
			0xC3, 0x24, 0x89, 0xD3, 0x80, 0x87, 0xB2, 0x4E,
			0x4C, 0xD7, 0x49, 0xE4, 0x9D, 0x1D, 0x34, 0xD1

		};

		/* user modules 2.60-2.71 */
		static public readonly byte[] keys260_1 = new byte[0x10] {
			0xF3, 0xAC, 0x6E, 0x7C, 0x04, 0x0A, 0x23, 0xE7,
			0x0D, 0x33, 0xD8, 0x24, 0x73, 0x39, 0x2B, 0x4A
		};

		/* vshmain 2.60-2.71 */
		static public readonly byte[] keys260_2 = new byte[0x10] {
			0x72, 0xB4, 0x39, 0xFF, 0x34, 0x9B, 0xAE, 0x82,
			0x30, 0x34, 0x4A, 0x1D, 0xA2, 0xD8, 0xB4, 0x3C
		};

		/* kernel modules 2.80 */
		static public readonly byte[] keys280_0 = new byte[0x10] {
			0xCA, 0xFB, 0xBF, 0xC7, 0x50, 0xEA, 0xB4, 0x40,
			0x8E, 0x44, 0x5C, 0x63, 0x53, 0xCE, 0x80, 0xB1
		};

		/* user modules 2.80 */
		static public readonly byte[] keys280_1 = new byte[0x10] {
			0x40, 0x9B, 0xC6, 0x9B, 0xA9, 0xFB, 0x84, 0x7F,
			0x72, 0x21, 0xD2, 0x36, 0x96, 0x55, 0x09, 0x74
		};

		/* vshmain executable 2.80 */
		static public readonly byte[] keys280_2 = new byte[0x10] {
			0x03, 0xA7, 0xCC, 0x4A, 0x5B, 0x91, 0xC2, 0x07,
			0xFF, 0xFC, 0x26, 0x25, 0x1E, 0x42, 0x4B, 0xB5
		};

		/* kernel modules 3.00 */
		static public readonly byte[] keys300_0 = new byte[0x10] {
			0x9F, 0x67, 0x1A, 0x7A, 0x22, 0xF3, 0x59, 0x0B, 
			0xAA, 0x6D, 0xA4, 0xC6, 0x8B, 0xD0, 0x03, 0x77

		};

		/* user modules 3.00 */
		static public readonly byte[] keys300_1 = new byte[0x10] {
			0x15, 0x07, 0x63, 0x26, 0xDB, 0xE2, 0x69, 0x34, 
   			0x56, 0x08, 0x2A, 0x93, 0x4E, 0x4B, 0x8A, 0xB2

		};

		/* vshmain 3.00 */
		static public readonly byte[] keys300_2 = new byte[0x10] {
			0x56, 0x3B, 0x69, 0xF7, 0x29, 0x88, 0x2F, 0x4C, 
   			0xDB, 0xD5, 0xDE, 0x80, 0xC6, 0x5C, 0xC8, 0x73

		};

		/* kernel modules 3.00 */
		static public readonly byte[] keys303_0 = new byte[0x10] {
			0x7b, 0xa1, 0xe2, 0x5a, 0x91, 0xb9, 0xd3, 0x13,
			0x77, 0x65, 0x4a, 0xb7, 0xc2, 0x8a, 0x10, 0xaf
		};

		/* kernel modules 3.10 */
		static public readonly byte[] keys310_0 = new byte[0x10] {
			0xa2, 0x41, 0xe8, 0x39, 0x66, 0x5b, 0xfa, 0xbb,
			0x1b, 0x2d, 0x6e, 0x0e, 0x33, 0xe5, 0xd7, 0x3f
		};

		/* user modules 3.10 */
		static public readonly byte[] keys310_1 = new byte[0x10] {
			0xA4, 0x60, 0x8F, 0xAB, 0xAB, 0xDE, 0xA5, 0x65,
			0x5D, 0x43, 0x3A, 0xD1, 0x5E, 0xC3, 0xFF, 0xEA
		};

		/* vshmain 3.10 */
		static public readonly byte[] keys310_2 = new byte[0x10] {
			0xE7, 0x5C, 0x85, 0x7A, 0x59, 0xB4, 0xE3, 0x1D,
			0xD0, 0x9E, 0xCE, 0xC2, 0xD6, 0xD4, 0xBD, 0x2B
		};

		/* reboot.bin 3.10 */
		static public readonly byte[] keys310_3 = new byte[0x10] {
   			0x2E, 0x00, 0xF6, 0xF7, 0x52, 0xCF, 0x95, 0x5A,
   			0xA1, 0x26, 0xB4, 0x84, 0x9B, 0x58, 0x76, 0x2F
		};

		/* kernel modules 3.30 */ 
		static public readonly byte[] keys330_0 = new byte[0x10] { 
			0x3B, 0x9B, 0x1A, 0x56, 0x21, 0x80, 0x14, 0xED,
			0x8E, 0x8B, 0x08, 0x42, 0xFA, 0x2C, 0xDC, 0x3A
		};

		/* user modules 3.30 */ 
		static public readonly byte[] keys330_1 = new byte[0x10] { 
   			0xE8, 0xBE, 0x2F, 0x06, 0xB1, 0x05, 0x2A, 0xB9, 
   			0x18, 0x18, 0x03, 0xE3, 0xEB, 0x64, 0x7D, 0x26 
		}; 

		/* vshmain 3.30 */ 
		static public readonly byte[] keys330_2 = new byte[0x10] { 
   			0xAB, 0x82, 0x25, 0xD7, 0x43, 0x6F, 0x6C, 0xC1, 
   			0x95, 0xC5, 0xF7, 0xF0, 0x63, 0x73, 0x3F, 0xE7 
		}; 

		/* reboot.bin 3.30 */ 
		static public readonly byte[] keys330_3 = new byte[0x10] { 
   			0xA8, 0xB1, 0x47, 0x77, 0xDC, 0x49, 0x6A, 0x6F, 
   			0x38, 0x4C, 0x4D, 0x96, 0xBD, 0x49, 0xEC, 0x9B 
		}; 

		/* stdio.prx 3.30 */
		static public readonly byte[] keys330_4 = new byte[0x10] {
			0xEC, 0x3B, 0xD2, 0xC0, 0xFA, 0xC1, 0xEE, 0xB9,
			0x9A, 0xBC, 0xFF, 0xA3, 0x89, 0xF2, 0x60, 0x1F
		};

		/* demo data.psp 2.80 */
		static public readonly byte[] demokeys_280 = new byte[0x10] {
			0x12, 0x99, 0x70, 0x5E, 0x24, 0x07, 0x6C, 0xD0,
			0x2D, 0x06, 0xFE, 0x7E, 0xB3, 0x0C, 0x11, 0x26
		};

		/* demo data.psp 3.XX */
		static public readonly byte[] demokeys_3XX_1 = new byte[0x10] {
			0x47, 0x05, 0xD5, 0xE3, 0x56, 0x1E, 0x81, 0x9B,
			0x09, 0x2F, 0x06, 0xDB, 0x6B, 0x12, 0x92, 0xE0
		};

		/* demo data.psp 3.XX */
		static public readonly byte[] demokeys_3XX_2 = new byte[0x10] {
			0xF6, 0x62, 0x39, 0x6E, 0x26, 0x22, 0x4D, 0xCA,
			0x02, 0x64, 0x16, 0x99, 0x7B, 0x9A, 0xE7, 0xB8
		};

		/* new 2.7X eboot.bin */
		static public readonly byte[] ebootbin_271_new = new byte[0x10] {
			0xF4, 0xAE, 0xF4, 0xE1, 0x86, 0xDD, 0xD2, 0x9C,
			0x7C, 0xC5, 0x42, 0xA6, 0x95, 0xA0, 0x83, 0x88
		};

		/* new 2.8X eboot.bin */
		static public readonly byte[] ebootbin_280_new = new byte[0x10] {
			0xB8, 0x8C, 0x45, 0x8B, 0xB6, 0xE7, 0x6E, 0xB8,
			0x51, 0x59, 0xA6, 0x53, 0x7C, 0x5E, 0x86, 0x31
		};

		/* new 3.XX eboot.bin */
		static public readonly byte[] ebootbin_300_new = new byte[0x10] {
			0xED, 0x10, 0xE0, 0x36, 0xC4, 0xFE, 0x83, 0xF3,
			0x75, 0x70, 0x5E, 0xF6, 0xA4, 0x40, 0x05, 0xF7
		};

		/* new 3.XX eboot.bin */
		static public readonly byte[] ebootbin_310_new = new byte[0x10] {
			0x5C, 0x77, 0x0C, 0xBB, 0xB4, 0xC2, 0x4F, 0xA2,
			0x7E, 0x3B, 0x4E, 0xB4, 0xB4, 0xC8, 0x70, 0xAF
		};

		/* 2.60-2.71 gameshare */
		static public readonly byte[] gameshare_260_271 = new byte[0x10] {
			0xF9, 0x48, 0x38, 0x0C, 0x96, 0x88, 0xA7, 0x74,
			0x4F, 0x65, 0xA0, 0x54, 0xC2, 0x76, 0xD9, 0xB8
		};

		/* 2.80 gameshare */
		static public readonly byte[] gameshare_280 = new byte[0x10] {
			0x2D, 0x86, 0x77, 0x3A, 0x56, 0xA4, 0x4F, 0xDD,
			0x3C, 0x16, 0x71, 0x93, 0xAA, 0x8E, 0x11, 0x43
		};

		/* 3.00 gameshare */
		static public readonly byte[] gameshare_300 = new byte[0x10] {
			0x78, 0x1A, 0xD2, 0x87, 0x24, 0xBD, 0xA2, 0x96,
			0x18, 0x3F, 0x89, 0x36, 0x72, 0x90, 0x92, 0x85
		};

		/* 3.10 gameshare */
		static public readonly byte[] gameshare_310 = new byte[0x10] {
			0xC9, 0x7D, 0x3E, 0x0A, 0x54, 0x81, 0x6E, 0xC7,
			0x13, 0x74, 0x99, 0x74, 0x62, 0x18, 0xE7, 0xDD
		};

		/* 3.60 common kernel modules */
		static public readonly byte[] keys360_0 = new byte[0x10] {
			0x3C, 0x2B, 0x51, 0xD4, 0x2D, 0x85, 0x47, 0xDA, 
			0x2D, 0xCA, 0x18, 0xDF, 0xFE, 0x54, 0x09, 0xED
		};

		/* 3.60 specific slim kernel modules */
		static public readonly byte[] keys360_1 = new byte[0x10] {
			0x31, 0x1F, 0x98, 0xD5, 0x7B, 0x58, 0x95, 0x45, 
			0x32, 0xAB, 0x3A, 0xE3, 0x89, 0x32, 0x4B, 0x34
		};

		/* 3.70 common and fat kernel modules */
		static public readonly byte[] keys370_0 = new byte[0x10] {
			0x26, 0x38, 0x0A, 0xAC, 0xA5, 0xD8, 0x74, 0xD1, 
			0x32, 0xB7, 0x2A, 0xBF, 0x79, 0x9E, 0x6D, 0xDB
		};

		/* 3.70 slim specific kernel modules */
		static public readonly byte[] keys370_1 = new byte[0x10] {
			0x53, 0xE7, 0xAB, 0xB9, 0xC6, 0x4A, 0x4B, 0x77, 
			0x92, 0x17, 0xB5, 0x74, 0x0A, 0xDA, 0xA9, 0xEA
		};

		/* some 3.70 slim user modules */
		static public readonly byte[] keys370_2 = new byte[0x10] {
			0x71, 0x10, 0xF0, 0xA4, 0x16, 0x14, 0xD5, 0x93, 
			0x12, 0xFF, 0x74, 0x96, 0xDF, 0x1F, 0xDA, 0x89
		};

		/* 1SEG.PBP keys */
		static public readonly byte[] oneseg_310 = new byte[0x10] {
			0xC7, 0x27, 0x72, 0x85, 0xAB, 0xA7, 0xF7, 0xF0,
			0x4C, 0xC1, 0x86, 0xCC, 0xE3, 0x7F, 0x17, 0xCA
		};

		static public readonly byte[] oneseg_300 = new byte[0x10] {
			0x76, 0x40, 0x9E, 0x08, 0xDB, 0x9B, 0x3B, 0xA1,
			0x47, 0x8A, 0x96, 0x8E, 0xF3, 0xF7, 0x62, 0x92
		};
 
		static public readonly byte[] oneseg_280 = new byte[0x10] {
			0x23, 0xDC, 0x3B, 0xB5, 0xA9, 0x82, 0xD6, 0xEA,
			0x63, 0xA3, 0x6E, 0x2B, 0x2B, 0xE9, 0xE1, 0x54
		};
 
		static public readonly byte[] oneseg_260_271 = new byte[0x10] {
			0x22, 0x43, 0x57, 0x68, 0x2F, 0x41, 0xCE, 0x65,
			0x4C, 0xA3, 0x7C, 0xC6, 0xC4, 0xAC, 0xF3, 0x60
		};

		static public readonly byte[] oneseg_slim = new byte[0x10] {
			0x12, 0x57, 0x0D, 0x8A, 0x16, 0x6D, 0x87, 0x06,
			0x03, 0x7D, 0xC8, 0x8B, 0x62, 0xA3, 0x32, 0xA9
		};

		static public readonly byte[] ms_app_main = new byte[0x10] {
			0x1E, 0x2E, 0x38, 0x49, 0xDA, 0xD4, 0x16, 0x08,
			0x27, 0x2E, 0xF3, 0xBC, 0x37, 0x75, 0x80, 0x93
		};

		/* 3.90 kernel */
		static public readonly byte[] keys390_0 = new byte[0x10] {
			0x45, 0xEF, 0x5C, 0x5D, 0xED, 0x81, 0x99, 0x84,
			0x12, 0x94, 0x8F, 0xAB, 0xE8, 0x05, 0x6D, 0x7D
		};

		/* 3.90 slim */
		static public readonly byte[] keys390_1 = new byte[0x10] {
			0x70, 0x1B, 0x08, 0x25, 0x22, 0xA1, 0x4D, 0x3B, 
			0x69, 0x21, 0xF9, 0x71, 0x0A, 0xA8, 0x41, 0xA9
		};

		/* 5.00 kernel */
		static public readonly byte[] keys500_0 = new byte[0x10] {
			0xEB, 0x1B, 0x53, 0x0B, 0x62, 0x49, 0x32, 0x58, 
			0x1F, 0x83, 0x0A, 0xF4, 0x99, 0x3D, 0x75, 0xD0
		};

		/* 5.00 kernel 2000 specific */
		static public readonly byte[] keys500_1 = new byte[0x10] {
			0xBA, 0xE2, 0xA3, 0x12, 0x07, 0xFF, 0x04, 0x1B, 
			0x64, 0xA5, 0x11, 0x85, 0xF7, 0x2F, 0x99, 0x5B
		};

		/* 5.00 kernel 3000 specific */
		static public readonly byte[] keys500_2 = new byte[0x10] {
			0x2C, 0x8E, 0xAF, 0x1D, 0xFF, 0x79, 0x73, 0x1A, 
 			0xAD, 0x96, 0xAB, 0x09, 0xEA, 0x35, 0x59, 0x8B
		};

		static public readonly byte[] keys500_c = new byte[0x10] {
			0xA3, 0x5D, 0x51, 0xE6, 0x56, 0xC8, 0x01, 0xCA,
			0xE3, 0x77, 0xBF, 0xCD, 0xFF, 0x24, 0xDA, 0x4D
		};

		/* 5.05 kernel specific */
		static public readonly byte[] keys505_a = new byte[0x10] {
			0x7B, 0x94, 0x72, 0x27, 0x4C, 0xCC, 0x54, 0x3B,
			0xAE, 0xDF, 0x46, 0x37, 0xAC, 0x01, 0x4D, 0x87
		};

		static public readonly byte[] keys505_0 = new byte[0x10] {
			0x2E, 0x8E, 0x97, 0xA2, 0x85, 0x42, 0x70, 0x73,
			0x18, 0xDA, 0xA0, 0x8A, 0xF8, 0x62, 0xA2, 0xB0
		};

		static public readonly byte[] keys505_1 = new byte[0x10] {
			0x58, 0x2A, 0x4C, 0x69, 0x19, 0x7B, 0x83, 0x3D,
			0xD2, 0x61, 0x61, 0xFE, 0x14, 0xEE, 0xAA, 0x11
		};

		/* for psp 2000 file table and ipl pre-decryption */
		static public readonly byte[] keys02G_E = new byte[0x10] {
			0x9D, 0x09, 0xFD, 0x20, 0xF3, 0x8F, 0x10, 0x69, 
			0x0D, 0xB2, 0x6F, 0x00, 0xCC, 0xC5, 0x51, 0x2E
		};

		/* for psp 3000 file table and ipl pre-decryption */
		static public readonly byte[] keys03G_E = new byte[0x10] {
			0x4F, 0x44, 0x5C, 0x62, 0xB3, 0x53, 0xC4, 0x30, 
 			0xFC, 0x3A, 0xA4, 0x5B, 0xEC, 0xFE, 0x51, 0xEA
		};

		static public readonly byte[] key_D91609F0 = new byte[0x10] {
			0xD0, 0x36, 0x12, 0x75, 0x80, 0x56, 0x20, 0x43,
			0xC4, 0x30, 0x94, 0x3E, 0x1C, 0x75, 0xD1, 0xBF
		};
		static public readonly byte[] key_D9160AF0 = new byte[0x10] {
			0x10, 0xA9, 0xAC, 0x16, 0xAE, 0x19, 0xC0, 0x7E,
			0x3B, 0x60, 0x77, 0x86, 0x01, 0x6F, 0xF2, 0x63
		};
		static public readonly byte[] key_D9160BF0 = new byte[0x10] {
			0x83, 0x83, 0xF1, 0x37, 0x53, 0xD0, 0xBE, 0xFC,
			0x8D, 0xA7, 0x32, 0x52, 0x46, 0x0A, 0xC2, 0xC2
		};
		static public readonly byte[] key_D91611F0 = new byte[0x10] {
			0x61, 0xB0, 0xC0, 0x58, 0x71, 0x57, 0xD9, 0xFA,
			0x74, 0x67, 0x0E, 0x5C, 0x7E, 0x6E, 0x95, 0xB9
		};

		static public readonly byte[] key_D91612F0 = new byte[0x10] {/* UMD EBOOT.BIN (OPNSSMP.BIN) */
			0x9E, 0x20, 0xE1, 0xCD, 0xD7, 0x88, 0xDE, 0xC0,
			0x31, 0x9B, 0x10, 0xAF, 0xC5, 0xB8, 0x73, 0x23
		};

		static public readonly byte[] key_D91613F0 = new byte[0x10] {
			0xEB, 0xFF, 0x40, 0xD8, 0xB4, 0x1A, 0xE1, 0x66,
			0x91, 0x3B, 0x8F, 0x64, 0xB6, 0xFC, 0xB7, 0x12
		};

		static public readonly byte[] key_2E5E10F0 = new byte[0x10] {/* UMD EBOOT.BIN 2 (OPNSSMP.BIN) */
			0x9D, 0x5C, 0x5B, 0xAF, 0x8C, 0xD8, 0x69, 0x7E,
			0x51, 0x9F, 0x70, 0x96, 0xE6, 0xD5, 0xC4, 0xE8
		};

		static public readonly byte[] key_2E5E12F0 = new byte[0x10] {/* UMD EBOOT.BIN 3 (OPNSSMP.BIN) */
			0x8A, 0x7B, 0xC9, 0xD6, 0x52, 0x58, 0x88, 0xEA,
			0x51, 0x83, 0x60, 0xCA, 0x16, 0x79, 0xE2, 0x07
		};

		static public readonly byte[] key_2E5E13F0 = new byte[0x10] {
			0xFF, 0xA4, 0x68, 0xC3, 0x31, 0xCA, 0xB7, 0x4C,
			0xF1, 0x23, 0xFF, 0x01, 0x65, 0x3D, 0x26, 0x36
		};

		static public readonly byte[] keys600_u1_457B0BF0 = new byte[0x10] {
			0x7B, 0x94, 0x72, 0x27, 0x4C, 0xCC, 0x54, 0x3B,
			0xAE, 0xDF, 0x46, 0x37, 0xAC, 0x01, 0x4D, 0x87
		};

		static public readonly byte[] keys600_u1_457B0CF0 = new byte[0x10] {
			0xAC, 0x34, 0xBA, 0xB1, 0x97, 0x8D, 0xAE, 0x6F,
			0xBA, 0xE8, 0xB1, 0xD6, 0xDF, 0xDF, 0xF1, 0xA2
		};

		/* for psp go file table and ipl pre-decryption */
		static public readonly byte[] keys05G_E = new byte[0x10] {
			0x5D, 0xAA, 0x72, 0xF2, 0x26, 0x60, 0x4D, 0x1C,
			0xE7, 0x2D, 0xC8, 0xA3, 0x2F, 0x79, 0xC5, 0x54
		};

		/* 5.70 PSPgo kernel*/
		static public readonly byte[] keys570_5k = new byte[0x10] {
			0x6D, 0x72, 0xA4, 0xBA, 0x7F, 0xBF, 0xD1, 0xF1,
			0xA9, 0xF3, 0xBB, 0x07, 0x1B, 0xC0, 0xB3, 0x66
		};

		/* 6.00-6.20 kernel and phat */
		static public readonly byte[] keys620_0 = new byte[0x10] {
			0xD6, 0xBD, 0xCE, 0x1E, 0x12, 0xAF, 0x9A, 0xE6,
			0x69, 0x30, 0xDE, 0xDA, 0x88, 0xB8, 0xFF, 0xFB
		};

		/* 6.00-6.20 slim kernel */
		static public readonly byte[] keys620_1 = new byte[0x10] {
			0x1D, 0x13, 0xE9, 0x50, 0x04, 0x73, 0x3D, 0xD2,
			0xE1, 0xDA, 0xB9, 0xC1, 0xE6, 0x7B, 0x25, 0xA7
		};

		static public readonly byte[] keys620_3 = new byte[0x10] {
			0xA3, 0x5D, 0x51, 0xE6, 0x56, 0xC8, 0x01, 0xCA,
			0xE3, 0x77, 0xBF, 0xCD, 0xFF, 0x24, 0xDA, 0x4D
		};

		static public readonly byte[] keys620_e = new byte[0x10] {
			0xB1, 0xB3, 0x7F, 0x76, 0xC3, 0xFB, 0x88, 0xE6,
			0xF8, 0x60, 0xD3, 0x35, 0x3C, 0xA3, 0x4E, 0xF3
		};

		/* PSPgo internal */
		static public readonly byte[] keys620_5 = new byte[0x10] {
			0xF1, 0xBC, 0x17, 0x07, 0xAE, 0xB7, 0xC8, 0x30,
			0xD8, 0x34, 0x9D, 0x40, 0x6A, 0x8E, 0xDF, 0x4E
		};

		/* 6.XX PSPgo kernel */
		static public readonly byte[] keys620_5k = new byte[0x10] {
			0x41, 0x8A, 0x35, 0x4F, 0x69, 0x3A, 0xDF, 0x04,
			0xFD, 0x39, 0x46, 0xA2, 0x5C, 0x2D, 0xF2, 0x21
		};

		static public readonly byte[] keys620_5v = new byte[0x10] {
			0xF2, 0x8F, 0x75, 0xA7, 0x31, 0x91, 0xCE, 0x9E,
			0x75, 0xBD, 0x27, 0x26, 0xB4, 0xB4, 0x0C, 0x32
		};
	}
}
