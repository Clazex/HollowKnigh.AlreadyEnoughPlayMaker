namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class ReplaceForeach : HookProvider {
	internal override IReadOnlyCollection<ILHook> CreateHooks() => new ILHook[] {
		new(
			Info.OfMethod<Fsm>(nameof(Fsm.BroadcastEvent), "FsmEvent, Boolean"),
			OptimizeBroadcastEvent,
			new() { ManualApply = true }
		),
		new(
			Info.OfMethod<Fsm>(
				nameof(Fsm.BroadcastEventToGameObject),
				"GameObject, FsmEvent, FsmEventData, Boolean, Boolean"
			),
			OptimizeBroadcastEventToGameObject,
			new() { ManualApply = true }
		)
	};

	private static void OptimizeBroadcastEvent(ILContext il) => il
		.ClearExceptionHandlers()
		.RemoveAll()
		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Ldarg_1) // fsmEvent
		.Emit(OpCodes.Ldarg_2) // excludeSelf
		.Emit(
			OpCodes.Call,
			Info.OfMethod<Fsm>("GetEventDataSentByInfo")
		)
		.Emit(OpCodes.Call, Info.OfMethod(
			"AlreadyEnoughPlayMaker",
			"AlreadyEnoughPlayMaker.HookProviders.ReplaceForeach",
			nameof(BroadcastEvent)
		));

	private static void OptimizeBroadcastEventToGameObject(ILContext il) => il
		.RemoveAll()
		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Ldarg_1) // go
		.Emit(OpCodes.Ldarg_2) // fsmEvent
		.Emit(OpCodes.Ldarg_3) // eventData
		.Emit(OpCodes.Ldarg, 4) // sendToChildren
		.Emit(OpCodes.Ldarg, 5) // excludeSelf
		.Emit(OpCodes.Call, Info.OfMethod(
			"AlreadyEnoughPlayMaker",
			"AlreadyEnoughPlayMaker.HookProviders.ReplaceForeach",
			nameof(BroadcastEventToGameObject)
		));


	private static void BroadcastEvent(Fsm self, FsmEvent fsmEvent, bool excludeSelf, FsmEventData eventDataSentByInfo) {
		foreach (PlayMakerFSM pmfsm in PlayMakerFSM.FsmList.ToArray()) {
			Fsm fsm = pmfsm.Fsm;

			if (fsm != null && (!excludeSelf || fsm != self) && pmfsm != null) {
				fsm.ProcessEvent(fsmEvent, eventDataSentByInfo);
			}
		}
	}

	private static void BroadcastEventToGameObject(Fsm self, GameObject go, FsmEvent fsmEvent, FsmEventData eventData, bool sendToChildren, bool excludeSelf) {
		if (go == null) {
			return;
		}

		foreach (PlayMakerFSM pmfsm in go.GetComponents<PlayMakerFSM>()) {
			Fsm fsm = pmfsm.Fsm;

			if ((!excludeSelf || fsm != self) && pmfsm != null && pmfsm.gameObject == go) {
				fsm.ProcessEvent(fsmEvent, eventData);
			}
		}

		if (sendToChildren) {
			// Transform implements the non-generic IEnumerator
			// so it won't matter using foreach here
			foreach (Transform child in go.transform) {
				BroadcastEventToGameObject(
					self,
					child.gameObject,
					fsmEvent,
					eventData,
					true,
					excludeSelf
				);
			}
		}
	}
}
