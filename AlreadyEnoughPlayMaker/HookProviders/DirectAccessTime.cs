namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class DirectAccessTime : HookProvider {
	internal override IReadOnlyCollection<ILHook> CreateHooks() => new ILHook[] {
		new(
			Info.OfMethod<Fsm>(nameof(Fsm.Update)),
			OptimizeFsmUpdate,
			new() { ManualApply = true }
		),
		new(
			Info.OfPropertyGet(
				"PlayMaker",
				 "HutongGames.PlayMaker.FsmTime",
				 nameof(FsmTime.RealtimeSinceStartup)
			),
			OptimizeFsmTimeRealtimeSinceStartupGet,
			new() { ManualApply = true }
		)
	};

	private static void OptimizeFsmUpdate(ILContext il) =>
		new ILCursor(il).Goto(0).Remove();

	private static void OptimizeFsmTimeRealtimeSinceStartupGet(ILContext il) => il
		.RemoveAll()
		.Emit(
			OpCodes.Call,
			Info.OfPropertyGet(
				"UnityEngine.CoreModule",
				"UnityEngine.Time",
				nameof(Time.realtimeSinceStartup)
			)
		);
}
