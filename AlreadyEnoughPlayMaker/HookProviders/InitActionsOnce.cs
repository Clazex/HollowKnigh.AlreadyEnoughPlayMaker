namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class InitActionsOnce : HookProvider {
	internal override IReadOnlyCollection<ILHook> CreateHooks() {
		List<ILHook> hooks = new();

		MethodInfo methodInit = Info.OfMethod<FsmStateAction>(nameof(FsmStateAction.Init));

		foreach (
			MethodInfo mi in typeof(FsmState).GetMethods(
				BindingFlags.DeclaredOnly
				| BindingFlags.Instance
				| BindingFlags.Public
				| BindingFlags.NonPublic
			)
		) {
			using (DynamicMethodDefinition dmd = new(mi)) {
				if (
					!dmd.Definition.Body.Instructions
						.Any(i => i.MatchCallOrCallvirt(methodInit))
				) {
					continue;
				}
			}

			hooks.Add(new(mi, (il) => {
				ILCursor cursor = new ILCursor(il).Goto(0);

				while (
					cursor.TryGotoNext(i => i.MatchCallOrCallvirt(methodInit))
				) {
					_ = cursor.GotoPrev().GotoPrev().Remove().Remove().Remove();
				}
			}, new() { ManualApply = true }));
		}

		hooks.Add(new(
			Info.OfMethod<ActionData>(nameof(ActionData.LoadActions)),
			EnsureActionsInit,
			new() { ManualApply = true }
		));

		return hooks;
	}

	internal override bool DefaultEnabled => false;

	private static void EnsureActionsInit(ILContext il) => new ILCursor(il)
		.GotoEnd()
		.Emit(OpCodes.Ldarg_1) // state
		.Emit(OpCodes.Call, Info.OfMethod(
			nameof(AlreadyEnoughPlayMaker),
			"AlreadyEnoughPlayMaker.HookProviders.InitActionsOnce",
			nameof(InitActions)
		));

	private static FsmStateAction[] InitActions(FsmStateAction[] actions, FsmState state) {
		foreach (FsmStateAction action in actions) {
			action.Init(state);
		}

		return actions;
	}
}
