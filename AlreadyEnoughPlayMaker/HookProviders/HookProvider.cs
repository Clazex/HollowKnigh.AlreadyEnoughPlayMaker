namespace AlreadyEnoughPlayMaker.HookProviders;

internal abstract class HookProvider {
	private bool enabled = false;

	internal IReadOnlyCollection<ILHook> Hooks { get; private protected init; }

	internal HookProvider() {
		Hooks = CreateHooks();
		Enabled = DefaultEnabled;
	}

	internal abstract IReadOnlyCollection<ILHook> CreateHooks();

	internal bool Enabled {
		get => enabled;
		set {
			if (enabled != value) {
				enabled = value;

				if (value) {
					foreach (IDetour detour in Hooks) {
						detour.Apply();
					}
				} else {
					foreach (IDetour detour in Hooks) {
						detour.Undo();
					}
				}
			}
		}
	}

	internal virtual bool DefaultEnabled => true;
}
