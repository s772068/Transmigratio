using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intervention : StaticInstance<Intervention>
{
    [SerializeField] private int _interventionPoints = 100;

    public int InterventionPoints => _interventionPoints;

    public Func<int, bool> UseIntervention;

    private void OnEnable()
    {
        UseIntervention += OnUseIntervention;
    }

    private void OnDisable()
    {
        UseIntervention -= OnUseIntervention;
    }

    private bool OnUseIntervention(int points)
    {
        if (points > _interventionPoints)
            return false;

        _interventionPoints -= points;
        return true;
    }
}
