using AlreadyEnoughPlayMaker.HookProviders;

namespace AlreadyEnoughPlayMaker;

public sealed partial class AlreadyEnoughPlayMaker : IMenuMod {
	public bool ToggleButtonInsideMenu => true;

	public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) {
		string[] options = new[] {
			Lang.Get("MOH_OFF", "MainMenu"),
			Lang.Get("MOH_ON", "MainMenu")
		};

		return new() {
			new(
				$"Providers/{nameof(BetterDelayedEventUpdate)}".Localize(),
				options,
				$"Providers/{nameof(BetterDelayedEventUpdate)}/Desc".Localize(),
				(i) => providers[nameof(BetterDelayedEventUpdate)].Enabled = i != 0,
				() => providers[nameof(BetterDelayedEventUpdate)].Enabled ? 1 : 0
			),
			new(
				$"Providers/{nameof(DirectAccessTime)}".Localize(),
				options,
				$"Providers/{nameof(DirectAccessTime)}/Desc".Localize(),
				(i) => providers[nameof(DirectAccessTime)].Enabled = i != 0,
				() => providers[nameof(DirectAccessTime)].Enabled ? 1 : 0
			),
			new(
				$"Providers/{nameof(InitActionsOnce)}".Localize(),
				options,
				$"Providers/{nameof(InitActionsOnce)}/Desc".Localize(),
				(i) => providers[nameof(InitActionsOnce)].Enabled = i != 0,
				() => providers[nameof(InitActionsOnce)].Enabled ? 1 : 0
			),
			new(
				$"Providers/{nameof(NoLoopCount)}".Localize(),
				options,
				$"Providers/{nameof(NoLoopCount)}/Desc".Localize(),
				(i) => providers[nameof(NoLoopCount)].Enabled = i != 0,
				() => providers[nameof(NoLoopCount)].Enabled ? 1 : 0
			),
			new(
				$"Providers/{nameof(ReplaceForeach)}".Localize(),
				options,
				$"Providers/{nameof(ReplaceForeach)}/Desc".Localize(),
				(i) => providers[nameof(ReplaceForeach)].Enabled = i != 0,
				() => providers[nameof(ReplaceForeach)].Enabled ? 1 : 0
			)
		};
	}
}
