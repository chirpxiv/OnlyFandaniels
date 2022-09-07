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

			if (ImGui.Begin("Only Fandaniels", ref Visible)) {
				ImGui.BeginGroup();
				ImGui.AlignTextToFramePadding();

				if (ImGui.Button("Activate Fandaniel")) {
					Plugin.Apply();
				}
			}

			ImGui.PopStyleVar(1);
			ImGui.End();
		}
	}
}
