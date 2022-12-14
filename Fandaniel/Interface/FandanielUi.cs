using System.Numerics;

using ImGuiNET;

namespace OnlyFandaniels.Interface {
	public class FandanielUi {
		private OnlyFandaniels Plugin;

		public bool Visible = false;

		// Constructor

		public FandanielUi(OnlyFandaniels plogon) {
			Plugin = plogon;
		}

		// Toggle

		public void Toggle() {
			Visible = !Visible;
		}

		// Draw window

		public void Draw() {
			if (!Visible)
				return;

			var size = new Vector2(-1, -1);
			ImGui.SetNextWindowSize(size, ImGuiCond.Always);
			ImGui.SetNextWindowSizeConstraints(size, size);

			ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));

			if (ImGui.Begin("OnlyFandaniels", ref Visible)) {
				ImGui.BeginGroup();
				ImGui.AlignTextToFramePadding();

				var active = Plugin.Active;
				if (ImGui.Checkbox("Activate Fandaniel", ref active)) {
					Plugin.Active = active;
					Plugin.RefreshAll();
				}

				ImGui.Separator();

				var cutscene = Plugin.Configuration.InCutscene;
				if (ImGui.Checkbox("Active in Cutscene", ref cutscene))
					Plugin.Configuration.InCutscene = cutscene;

				var startup = Plugin.Configuration.OnStartup;
				if (ImGui.Checkbox("Active on Startup", ref startup))
					Plugin.Configuration.OnStartup = startup;
			}

			ImGui.PopStyleVar(1);
			ImGui.End();
		}
	}
}