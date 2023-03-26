using Osmi.Utils.Tap;

namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class BetterDelayedEventUpdate : HookProvider {
	internal override IReadOnlyCollection<ILHook> CreateHooks() => new ILHook[] {
		new(
			Info.OfMethod<Fsm>(nameof(Fsm.UpdateDelayedEvents)),
			OptimizeUpdateDelayedEvents,
			new() { ManualApply = true }
		)
	};

	//
	// this.updateEvents = this.delayedEvents;
	// this.delayedEvents = new List<DelayedEvent>();
	//
	// for (int i = 0; i < this.updateEvents.Count; i++)
	//	 DelayedEvent delayedEvent = this.updateEvents[i];
	//	 delayedEvent.Update();
	//
	//	 if (!delayedEvent.Finished) {
	//		 this.delayedEvents.Add(delayedEvent);
	//	 }
	// }
	//
	private static void OptimizeUpdateDelayedEvents(ILContext il) => new ILCursor(il)
		.Goto(0)
		.RemoveUntil(i => i.MatchLdcI4(out _))

		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Ldfld, Info.OfField<Fsm>("delayedEvents"))
		.Emit(OpCodes.Stfld, Info.OfField<Fsm>("updateEvents"))

		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Newobj, Info.OfConstructor(
			"mscorlib",
			"System.Collections.Generic.List`1<PlayMaker|HutongGames.PlayMaker.DelayedEvent>"
		))
		.Emit(OpCodes.Stfld, Info.OfField<Fsm>("delayedEvents"))

		.GotoNext(i => i.MatchBrfalse(out _))
		.Tap(cur => cur.Next.OpCode = OpCodes.Brtrue_S)

		.GotoNext(i => i.MatchLdfld(out _))
		.Tap(cur => cur.Next.Operand = Info.OfField<Fsm>("delayedEvents"))

		.GotoNext(MoveType.After, i => i.MatchBlt(out _))
		.RemoveUntilEnd()

		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Newobj, Info.OfConstructor(
			"mscorlib",
			"System.Collections.Generic.List`1<PlayMaker|HutongGames.PlayMaker.DelayedEvent>"
		))
		.Emit(OpCodes.Stfld, Info.OfField<Fsm>("updateEvents"));
}
