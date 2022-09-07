using Dalamud.Plugin;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;

using OnlyFandaniels.Interface;

namespace OnlyFandaniels {
	public sealed class OnlyFandaniels : IDalamudPlugin {
		public string Name => "OnlyFandaniels";
		public string CommandName = "/fandaniel";

		public bool Active = false;

		public byte[] Fandaniel = { 1, 0, 1, 0, 1, 215, 152, 0, 253, 6, 6, 7, 0, 0, 1, 6, 0, 0, 0, 128, 255, 50, 22, 50, 3, 255 };

		public FandanielUi Ui { get; set; }

		internal DalamudPluginInterface PluginInterface { get; init; }
		internal CommandManager CommandManager { get; init; }
		internal ClientState ClientState { get; init; }
		internal ObjectTable ObjectTable { get; init; }

		public OnlyFandaniels(
			DalamudPluginInterface pluginInterface,
			CommandManager cmdManager,
			ClientState clientState,
			ObjectTable objTable
		) {
			PluginInterface = pluginInterface;
			CommandManager = cmdManager;
			ClientState = clientState;
			ObjectTable = objTable;

			// Interface

			Ui = new FandanielUi(this);

			PluginInterface.UiBuilder.DisableGposeUiHide = true;
			PluginInterface.UiBuilder.Draw += Ui.Draw;

			// Register command

			CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = "Fandaniel."
			});
		}

		public void Dispose() {
			CommandManager.RemoveHandler(CommandName);
		}

		private void OnCommand(string command, string arguments) {
			Ui.Toggle();
		}

		// yes

		public unsafe void Apply() {
			for (var i = 0; i < ObjectTable.Length; i++) {
				var actor = ObjectTable[i];
				if (actor == null)
					continue;

				if (actor.ObjectKind == ObjectKind.Player || actor.ObjectKind == ObjectKind.BattleNpc) {
					var fandan = (Actor*)actor.Address;
					for (var x = 0; x < 26; x++)
						fandan->Customize[x] = Fandaniel[x];
					fandan->Redraw();
				}
			}
		}
	}
}
