namespace AlreadyEnoughPlayMaker.HookProviders;

internal sealed class InitActionsOnce : IHookProvider {
	public ICollection<ILHook> ApplyHooks() {
		List<ILHook> hooks = new();

		MethodInfo methodInit = typeof(FsmStateAction).GetMethod(
			nameof(FsmStateAction.Init)
		)!;

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
			}));
		}

		hooks.Add(new(
			typeof(ActionData).GetMethod(nameof(ActionData.LoadActions)),
			EnsureActionsInit
		));

		return hooks;
	}

	private static void EnsureActionsInit(ILContext il) => new ILCursor(il)
		.GotoEnd()
		.Emit(OpCodes.Ldarg_1) // state
		.EmitStaticMethodCall(InitActions);

	private static FsmStateAction[] InitActions(FsmStateAction[] actions, FsmState state) {
		foreach (FsmStateAction action in actions) {
			action.Init(state);
		}

		return actions;
	}
}
