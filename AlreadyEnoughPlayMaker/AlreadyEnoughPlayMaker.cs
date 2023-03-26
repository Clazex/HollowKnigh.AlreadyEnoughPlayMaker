using AlreadyEnoughPlayMaker.HookProviders;

namespace AlreadyEnoughPlayMaker;

[PublicAPI]
public sealed partial class AlreadyEnoughPlayMaker : Mod, IGlobalSettings<GlobalSettings> {
	public static AlreadyEnoughPlayMaker Instance { get; private set; } = null!;

	public static Lazy<string> version = AssemblyUtil
#if DEBUG
		.GetMyDefaultVersionWithHash();
#else
		.GetMyDefaultVersion();
#endif

	public override string GetVersion() => version.Value;

	public override string GetMenuButtonText() =>
		"ModName".Localize() + ' ' + Lang.Get("MAIN_OPTIONS", "MainMenu");

	internal Dictionary<string, HookProvider> providers;

	public AlreadyEnoughPlayMaker() {
		Instance = this;

		float startTime = Time.realtimeSinceStartup;

		providers = new() {
			{ nameof(BetterDelayedEventUpdate), new BetterDelayedEventUpdate() },
			{ nameof(DirectAccessTime), new DirectAccessTime() },
			{ nameof(InitActionsOnce), new InitActionsOnce() },
			{ nameof(NoLoopCount), new NoLoopCount() },
			{ nameof(ReplaceForeach), new ReplaceForeach() },
		};

		int count = providers.Values.Map(provider => provider.Hooks.Count).Sum();

		Log($"Successfully initialized {count} hooks in {Time.realtimeSinceStartup - startTime}s");
	}

	public GlobalSettings GlobalSettings { get; private set; } = new();
	public void OnLoadGlobal(GlobalSettings s) => GlobalSettings = s;
	public GlobalSettings OnSaveGlobal() => GlobalSettings;
}
