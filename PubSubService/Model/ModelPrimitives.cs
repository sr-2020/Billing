using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService.Model
{
    public class ModelPrimitives
    {
        public static Dictionary<string, ActiveAbility> Abilities = new Dictionary<string, Model.ActiveAbility>
        {
            { "how-much-it-costs", ActiveAbility.HowMuch   },
            { "who-needs-it", ActiveAbility.WhoNeed },
            { "how-much-is-rent", ActiveAbility.PayAndCry },
            { "let-him-pay", ActiveAbility.LetHim},
            { "let-me-pay", ActiveAbility.Letme},
            { "re-rent", ActiveAbility.Rerent},
            { "marauder-2", ActiveAbility.Marauder},
        };
        public static Dictionary<string, HealthState> HealthStates = new Dictionary<string, Model.HealthState>
        {
            { "healthy", HealthState.Healthy   },
            { "wounded", HealthState.Wounded },
            { "clinically_dead", HealthState.ClinicallyDead },
            { "biologically_dead", HealthState.BiologicallyDead },
        };

    }

    public enum HealthState
    { 
        Healthy,
        Wounded,
        ClinicallyDead,
        BiologicallyDead
    }


    public enum ActiveAbility
    {
        HowMuch,
        WhoNeed,
        PayAndCry,
        LetHim,
        Letme,
        Rerent,
        Marauder

    }

}
