using AlreadyEnoughPlayMaker.HookProviders;

namespace AlreadyEnoughPlayMaker;

[PublicAPI]
public sealed class AlreadyEnoughPlayMaker : Mod {
	public static Lazy<string> version = AssemblyUtil
#if DEBUG
		.GetMyDefaultVersionWithHash();
#else
		.GetMyDefaultVersion();
#endif

	public override string GetVersion() => version.Value;

	public AlreadyEnoughPlayMaker() {
		float startTime = Time.realtimeSinceStartup;

		int count = new IHookProvider[] {
			new BetterDelayedEventUpdate(),
			new DirectAccessTime(),
			new InitActionsOnce(),
			new NoLoopCount(),
			new ReplaceForeach()
		}.Map(provider => provider.ApplyHooks().Count).Sum();

		Log($"Successfully applied {count} hooks in {Time.realtimeSinceStartup - startTime}s");
	}
}
