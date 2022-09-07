using System;

using Dalamud.Plugin;
using Dalamud.Hooking;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Conditions;

using OnlyFandaniels.Interface;

namespace OnlyFandaniels {
	public sealed class OnlyFandaniels : IDalamudPlugin {
		public static byte[] Fandaniel = { 1, 0, 1, 0, 1, 215, 152, 0, 253, 6, 6, 7, 0, 0, 1, 6, 0, 0, 0, 128, 255, 50, 22, 50, 3, 255 };

		public string Name => "OnlyFandaniels";
		public string CommandName = "/fandaniel";

		public bool Active = false;

		public Configuration Configuration { get; init; }

		public FandanielUi Ui { get; init; }

		internal DalamudPluginInterface PluginInterface { get; init; }
		internal CommandManager CommandManager { get; init; }
		internal Condition Condition { get; init; }
		internal ClientState ClientState { get; init; }
		internal ObjectTable ObjectTable { get; init; }

		internal delegate IntPtr CharInit(IntPtr actorPtr, IntPtr customize);
		internal Hook<CharInit> CharInitHook;

		public OnlyFandaniels(
			DalamudPluginInterface pluginInterface,
			CommandManager cmdManager,
			Condition condition,
			ClientState clientState,
			ObjectTable objTable,
			SigScanner sigScanner
		) {
			PluginInterface = pluginInterface;
			CommandManager = cmdManager;
			Condition = condition;
			ClientState = clientState;
			ObjectTable = objTable;

			// Config

			Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			Active = Configuration.OnStartup;

			// https://github.com/avafloww/OopsAllLalafells/blob/main/Plugin.cs

			var charInitAddr = sigScanner.ScanText("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 48 8B F9 48 8B EA 48 81 C1 ?? ?? ?? ?? E8 ?? ?? ?? ??");
			CharInitHook ??= new Hook<CharInit>(charInitAddr, CharInitDetour);
			CharInitHook.Enable();

			// Check for cutscene exit

			Condition.ConditionChange += CheckCutsceneExit;

			// Interface

			Ui = new FandanielUi(this);

			PluginInterface.UiBuilder.DisableCutsceneUiHide = true;
			PluginInterface.UiBuilder.DisableGposeUiHide = true;
			PluginInterface.UiBuilder.Draw += Ui.Draw;
			PluginInterface.UiBuilder.OpenConfigUi += OnConfig;

			// Register command

			CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = "Fandaniel."
			});
		}

		public void Dispose() {
			PluginInterface.UiBuilder.Draw -= Ui.Draw;
			PluginInterface.UiBuilder.OpenConfigUi -= OnConfig;

			CharInitHook.Disable();
			CharInitHook.Dispose();

			CommandManager.RemoveHandler(CommandName);

			Configuration.Save(this);

			RefreshAll();
		}

		// Toggle UI on chat command

		private void OnCommand(string command, string arguments) {
			Ui.Toggle();
		}

		private void OnConfig() {
			Ui.Toggle();
		}

		// Refresh all characters

		public unsafe void RefreshAll() {
			if (ClientState.LocalPlayer == null)
				return;

			for (var i = 0; i < ObjectTable.Length; i++) {
				var obj = ObjectTable[i];
				if (obj == null)
					continue;

				var actor = (Actor*)obj.Address;
				actor->Redraw();
			}
		}

		// Hook character init

		private unsafe IntPtr CharInitDetour(IntPtr actorPtr, IntPtr customize) {
			if (Active) {
				var inCut = IsInCutscene();
				if (!inCut || (inCut && Configuration.InCutscene)) {
					for (var i = 0; i < Fandaniel.Length; i++)
						((byte*)customize)[i] = Fandaniel[i];
				}
			}
			return CharInitHook.Original(actorPtr, customize);
		}

		// Cutscene checks

		public bool IsInCutscene() {
			return Condition[ConditionFlag.OccupiedInCutSceneEvent];
		}

		public void CheckCutsceneExit(ConditionFlag flag, bool value) {
			if (!Active)
				return;

			if (flag == ConditionFlag.OccupiedInCutSceneEvent && value == false)
				RefreshAll();
		}
	}
}