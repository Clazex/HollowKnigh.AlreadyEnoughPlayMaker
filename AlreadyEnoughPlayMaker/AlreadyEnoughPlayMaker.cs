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
}
