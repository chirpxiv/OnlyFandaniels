﻿using System;
using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Objects.Enums;

namespace OnlyFandaniels {
	[StructLayout(LayoutKind.Explicit, Size = 0x84A)]
	public unsafe struct Actor {
		[FieldOffset(0x08C)] public byte Kind;
		[FieldOffset(0x840)] public fixed byte Customize[0x1A];

		// https://github.com/ktisis-tools/Ktisis/blob/main/Ktisis/Structs/Actor/Actor.cs

		public static void DisableDraw(IntPtr addr)
		=> ((delegate* unmanaged<IntPtr, void>**)addr)[0][17](addr);

		public static void EnableDraw(IntPtr addr)
		=> ((delegate* unmanaged<IntPtr, void>**)addr)[0][16](addr);

		public unsafe void Redraw() {
			var isPlayer = Kind == (byte)ObjectKind.Player;
			fixed (Actor* self = &this) {
				var ptr = (IntPtr)self;
				if (isPlayer) Kind = (byte)ObjectKind.BattleNpc;
				DisableDraw(ptr);
				EnableDraw(ptr);
				if (isPlayer) Kind = (byte)ObjectKind.Player;
			}
		}
	}
}