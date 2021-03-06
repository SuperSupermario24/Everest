﻿using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Celeste.Mod.Helpers {
    public static class CoroutineDelayHackfixHelper {

        public static HashSet<string> Hooks = new HashSet<string>();

        public static readonly Type Type = typeof(CoroutineDelayHackfixHelper).GetMethod("Wrap").GetStateMachineTarget().DeclaringType;

        public static void HandleDetour(MethodBase _from, MethodBase _to) {
            if (_from is not MethodInfo from || _to is not MethodInfo to)
                return;

            if (from.ReturnType == typeof(IEnumerator))
                Hooks.Add(to.GetID().Replace('+', '/'));
        }

        public static IEnumerator Wrap(IEnumerator inner, string context, bool oldWithMoveNext) {
            yield return new Action<patch_Coroutine>(cb => {
                if (oldWithMoveNext || !Hooks.Contains(context)) {
                    cb.ForceDelayedSwap = true;
                }
            });

            while (inner.MoveNext())
                yield return inner.Current;
        }

    }
}
