using Osmi.Utils.Tap;

namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class BetterDelayedEventUpdate : IHookProvider {
	public ICollection<ILHook> ApplyHooks() => new ILHook[] {
		new(
			typeof(Fsm).GetMethod(nameof(Fsm.UpdateDelayedEvents)),
			OptimizeUpdateDelayedEvents
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
	private static void OptimizeUpdateDelayedEvents(ILContext il) {
		FieldInfo fldDelayedEvents = typeof(Fsm).GetField(
			 "delayedEvents",
			BindingFlags.Instance | BindingFlags.NonPublic
		)!;
		FieldInfo fldUpdateEvents = typeof(Fsm).GetField(
			 "updateEvents",
			 BindingFlags.Instance | BindingFlags.NonPublic
		)!;
		ConstructorInfo ctorList = typeof(List<DelayedEvent>)
			.GetConstructor(Type.EmptyTypes)!;

		new ILCursor(il).Goto(0)
			.RemoveUntil(i => i.MatchLdcI4(out _))

			.Emit(OpCodes.Ldarg_0) // this
			.Emit(OpCodes.Ldarg_0) // this
			.Emit(OpCodes.Ldfld, fldDelayedEvents)
			.Emit(OpCodes.Stfld, fldUpdateEvents)

			.Emit(OpCodes.Ldarg_0) // this
			.Emit(OpCodes.Newobj, ctorList)
			.Emit(OpCodes.Stfld, fldDelayedEvents)

			.GotoNext(i => i.MatchBrfalse(out _))
			.Tap(cur => cur.Next.OpCode = OpCodes.Brtrue_S)

			.GotoNext(i => i.MatchLdfld(out _))
			.Tap(cur => cur.Next.Operand = fldDelayedEvents)

			.GotoNext(MoveType.After, i => i.MatchBlt(out _))
			.RemoveUntilEnd()

			.Emit(OpCodes.Ldarg_0) // this
			.Emit(OpCodes.Newobj, ctorList)
			.Emit(OpCodes.Stfld, fldUpdateEvents)
			.Discard();
	}
}
