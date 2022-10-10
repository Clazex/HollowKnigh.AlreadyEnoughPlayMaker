namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class DirectAccessTime : IHookProvider {
	public ICollection<ILHook> ApplyHooks() => new ILHook[] {
		new(typeof(Fsm).GetMethod(nameof(Fsm.Update)), OptimizeFsmUpdate),
		new(
			typeof(FsmTime).GetProperty(nameof(FsmTime.RealtimeSinceStartup))!
				.GetMethod,
			OptimizeFsmTimeRealtimeSinceStartupGet
		)
	};

	private static void OptimizeFsmUpdate(ILContext il) =>
		new ILCursor(il).Goto(0).Remove();

	private static void OptimizeFsmTimeRealtimeSinceStartupGet(ILContext il) => il
		.RemoveAll()
		.Emit(
			OpCodes.Call,
			typeof(Time).GetProperty(nameof(Time.realtimeSinceStartup))!
				.GetMethod
		);
}
