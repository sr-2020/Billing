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
            {"who-needs-it", ActiveAbility.WhoNeed },
            { "how-much-is-rent", ActiveAbility.PayAndCry },
            { "let-him-pay", ActiveAbility.LetHim},
            { "let-me-pay", ActiveAbility.Letme},

        };


    }

    public enum ActiveAbility
    {
        HowMuch,
        WhoNeed,
        PayAndCry,
        LetHim,
        Letme
    }

}
