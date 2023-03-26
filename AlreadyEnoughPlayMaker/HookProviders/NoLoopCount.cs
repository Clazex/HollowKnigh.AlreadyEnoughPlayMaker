using Osmi.Utils.Tap;

namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class NoLoopCount : HookProvider {
	internal override IReadOnlyCollection<ILHook> CreateHooks() => new ILHook[] {
		new(
			Info.OfMethod<Fsm>(nameof(Fsm.UpdateStateChanges)),
			OptimizeFsmUpdateStateChanges,
			new() { ManualApply = true }
		),
		new(
			Info.OfMethod<Fsm>("EnterState"),
			OptimizeFsmEnterState,
			new() { ManualApply = true }
		),
		new(
			Info.OfMethod<FsmState>(nameof(FsmState.OnEnter)),
			OptimizeFsmStateOnEnter,
			new() { ManualApply = true }
		)
	};

	// Remove:
	//
	// for (int i = 0; i < this.States.Length; i++) {
	//	 this.States[i].ResetLoopCount();
	// }
	//
	private static void OptimizeFsmUpdateStateChanges(ILContext il) => new ILCursor(il)
		.Goto(0)
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
	private static void OptimizeFsmEnterState(ILContext il) => new ILCursor(il)
		.Goto(0)
		.GotoNext(
			i => i.MatchLdarg(1),
			i => i.MatchCallvirt(Info.OfPropertyGet<FsmState>(nameof(FsmState.loopCount)))
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
	private static void OptimizeFsmStateOnEnter(ILContext il) => new ILCursor(il)
		.Goto(0)
		.RemoveUntil(i => i.MatchStfld(Info.OfField<FsmState>("active")))
		.Emit(OpCodes.Ldarg_0)
		.Emit(OpCodes.Ldc_I4_1);
}
