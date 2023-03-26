using AlreadyEnoughPlayMaker.HookProviders;

namespace AlreadyEnoughPlayMaker;

[PublicAPI]
public sealed class AlreadyEnoughPlayMaker : Mod {
	public static AlreadyEnoughPlayMaker Instance { get; private set; } = null!;

	public static Lazy<string> version = AssemblyUtil
#if DEBUG
		.GetMyDefaultVersionWithHash();
#else
		.GetMyDefaultVersion();
#endif

	public override string GetVersion() => version.Value;

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
}
