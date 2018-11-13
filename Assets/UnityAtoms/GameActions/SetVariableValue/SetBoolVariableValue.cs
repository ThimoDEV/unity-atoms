﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityAtoms
{
    [CreateAssetMenu(menuName = "Unity Atoms/Game Actions/Set Variable Value/Bool")]
    public class SetBoolVariableValue : SetVariableValue<bool, BoolVariable, BoolReference, BoolEvent, BoolBoolEvent> { }
}