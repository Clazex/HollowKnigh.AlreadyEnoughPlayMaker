namespace AlreadyEnoughPlayMaker.HookProviders;

internal interface IHookProvider {
	public ICollection<ILHook> ApplyHooks();
}
