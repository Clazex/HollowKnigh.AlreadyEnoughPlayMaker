using Osmi.Utils.Tap;

namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class NoLoopCount : IHookProvider {
	public ICollection<ILHook> ApplyHooks() => new ILHook[] {
		new(
			typeof(Fsm).GetMethod(nameof(Fsm.UpdateStateChanges)),
			OptimizeFsmUpdateStateChanges
		),
		new(
			typeof(Fsm).GetMethod(
				   "EnterState",
				   BindingFlags.Instance | BindingFlags.NonPublic
			),
			OptimizeFsmEnterState
		),
		new(
			typeof(FsmState).GetMethod(nameof(FsmState.OnEnter)),
			OptimizeFsmStateOnEnter
		)
	};

	// Remove:
	//
	// for (int i = 0; i < this.States.Length; i++) {
	//	 this.States[i].ResetLoopCount();
	// }
	//
	private static void OptimizeFsmUpdateStateChanges(ILContext il) =>
		new ILCursor(il).Goto(0)
			.GotoNext(i => i.MatchLdcI4(out _))
			.RemoveUntil(i => i.MatchBlt(out _))
			.Remove()
			.MarkLabel(out ILLabel label)

			.Goto(0)
			.GotoNext(i => i.MatchBrfalse(out _))
			.Tap(cur => cur.Next.Operand = label);

	// Remove:
	//
	// if (state.loopCount >= this.MaxLoopCount) {
	//	 this.Owner.enabled = false;
	//	 this.MyLog.LogError("String here");
	//	 return;
	// }
	//
	private static void OptimizeFsmEnterState(ILContext il) =>
		new ILCursor(il).Goto(0)
			.GotoNext(
				i => i.MatchLdarg(1),
				i => i.MatchCallvirt(typeof(FsmState), "get_loopCount")
			)
			.RemoveUntil(i => i.MatchRet())
			.Remove()
			.MarkLabel(out ILLabel label)

			.Goto(0)
			.GotoNext(i => i.MatchBrfalse(out _))
			.Tap(cur => cur.Next.Operand = label);

	// Remove:
	//
	// int loopCount = this.loopCount;
	// this.loopCount = loopCount + 1;
	// if (this.loopCount > this.maxLoopCount) {
	//	 this.maxLoopCount = this.loopCount;
	// }
	//
	private static void OptimizeFsmStateOnEnter(ILContext il) =>
		new ILCursor(il).Goto(0)
			.RemoveUntil(i => i.MatchStfld(typeof(FsmState), "active"))
			.Emit(OpCodes.Ldarg_0)
			.Emit(OpCodes.Ldc_I4_1);
}
