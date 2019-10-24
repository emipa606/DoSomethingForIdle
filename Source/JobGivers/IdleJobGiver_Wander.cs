﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace DSFI.JobGivers
{
    class IdleJobGiver_Wander : IdleJobGiver<IdleJobGiverDef>
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            return null;
        }
    }
}
