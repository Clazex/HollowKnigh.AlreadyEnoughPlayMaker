namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class ReplaceForeach : IHookProvider {
	public ICollection<ILHook> ApplyHooks() => new ILHook[] {
		new(
			typeof(Fsm).GetMethod(
				nameof(Fsm.BroadcastEvent),
				new[] { typeof(FsmEvent), typeof(bool) }
			),
			OptimizeBroadcastEvent
		),
		new(
			typeof(Fsm).GetMethod(
				nameof(Fsm.BroadcastEventToGameObject),
				new[] {
					typeof(GameObject),
					typeof(FsmEvent),
					typeof(FsmEventData),
					typeof(bool),
					typeof(bool)
				}
			),
			OptimizeBroadcastEventToGameObject
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
			  typeof(Fsm).GetMethod(
				"GetEventDataSentByInfo",
				BindingFlags.Static | BindingFlags.NonPublic
			)
		)
		.EmitStaticMethodCall(BroadcastEvent);

	private static void OptimizeBroadcastEventToGameObject(ILContext il) => il
		.RemoveAll()
		.Emit(OpCodes.Ldarg_0) // this
		.Emit(OpCodes.Ldarg_1) // go
		.Emit(OpCodes.Ldarg_2) // fsmEvent
		.Emit(OpCodes.Ldarg_3) // eventData
		.Emit(OpCodes.Ldarg, 4) // sendToChildren
		.Emit(OpCodes.Ldarg, 5) // excludeSelf
		.EmitStaticMethodCall(BroadcastEventToGameObject);


	private static void BroadcastEvent(Fsm self, FsmEvent fsmEvent, bool excludeSelf, FsmEventData eventDataSentByInfo) {
		List<PlayMakerFSM> list = PlayMakerFSM.FsmList;
		int count = list.Count;

		for (int i = 0; i < count; i++) {
			PlayMakerFSM pmfsm = list[i];
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

		List<PlayMakerFSM> fsmList = PlayMakerFSM.FsmList;
		int count = fsmList.Count;

		for (int i = 0; i < count; i++) {
			PlayMakerFSM pmfsm = fsmList[i];
			Fsm fsm = pmfsm.Fsm;

			if ((!excludeSelf || fsm != self) && pmfsm != null && pmfsm.gameObject == go) {
				fsm.ProcessEvent(fsmEvent, eventData);
			}
		}

		if (sendToChildren) {
			// Transform implements the non-generic IEnumerator
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
