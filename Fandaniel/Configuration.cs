using System;

using Dalamud;
using Dalamud.Configuration;

namespace OnlyFandaniels {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public int Version { get; set; } = 0;

		public bool OnStartup { get; set; } = false;
		public bool InCutscene { get; set; } = false;

		public void Save(OnlyFandaniels plugin) {
			plugin.PluginInterface.SavePluginConfig(this);
		}
	}
}