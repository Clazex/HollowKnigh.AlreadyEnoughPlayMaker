using static AlreadyEnoughPlayMaker.AlreadyEnoughPlayMaker;

namespace AlreadyEnoughPlayMaker;

public sealed class GlobalSettings {
	public bool BetterDelayedEventUpdate {
		get => Instance.providers[nameof(BetterDelayedEventUpdate)].Enabled;
		set => Instance.providers[nameof(BetterDelayedEventUpdate)].Enabled = value;
	}

	public bool DirectAccessTime {
		get => Instance.providers[nameof(DirectAccessTime)].Enabled;
		set => Instance.providers[nameof(DirectAccessTime)].Enabled = value;
	}

	public bool InitActionsOnce {
		get => Instance.providers[nameof(InitActionsOnce)].Enabled;
		set => Instance.providers[nameof(InitActionsOnce)].Enabled = value;
	}

	public bool NoLoopCount {
		get => Instance.providers[nameof(NoLoopCount)].Enabled;
		set => Instance.providers[nameof(NoLoopCount)].Enabled = value;
	}

	public bool ReplaceForeach {
		get => Instance.providers[nameof(ReplaceForeach)].Enabled;
		set => Instance.providers[nameof(ReplaceForeach)].Enabled = value;
	}
}
